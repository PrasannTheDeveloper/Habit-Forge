using HabitForge_MudBlazor.Data;
using HabitForge_MudBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace HabitForge_MudBlazor.Services
{
    public class HabitService
    {
        private readonly AppDbContext _context;

        public HabitService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Habit>> GetAllHabitAsync()
        {
            try
            {
                var habitIds = await _context.Habits.Select(h => h.HabitId).ToListAsync();

                foreach (var id in habitIds)
                {
                    try
                    {
                        _context.ChangeTracker.Clear();
                        await UpdateStreakAsync(id);
                    }
                    catch
                    {
                        // skip streak update failures per-habit
                    }
                }

                _context.ChangeTracker.Clear();

                return await _context.Habits
                    .AsNoTracking()
                    .Include(h => h.HabitEntries)
                        .ThenInclude(e => e.ListCompletions)
                    .Include(h => h.HabitListTemplates)
                    .ToListAsync();
            }
            catch
            {
                return new List<Habit>();
            }
        }

        public async Task<Habit?> GetHabitbyIdAsync(int habitid)
        {
            try
            {
                return await _context.Habits
                    .Include(h => h.HabitEntries)
                    .Include(h => h.HabitListTemplates)
                    .Include(h => h.Category)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(h => h.HabitId == habitid);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CreateHabitAsync(Habit habit)
        {
            if (habit == null) return false;
            try
            {
                _context.Habits.Add(habit);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateHabitAsync(Habit habit)
        {
            if (habit == null) return false;

            var existing = await _context.Habits
                .Include(h => h.HabitListTemplates)
                .Include(h=>h.Category)
                .FirstOrDefaultAsync(h => h.HabitId == habit.HabitId);

            if (existing == null) return false;

            // --- 1. Update all scalar/raw fields manually ---
            existing.Name = habit.Name;
            existing.Description = habit.Description;
            existing.HabitType = habit.HabitType;
            existing.GoalValue = habit.GoalValue;
            existing.IsGoodHabit = habit.IsGoodHabit;
            existing.IsActive = habit.IsActive;
            existing.CategoryId = habit.CategoryId;
            existing.CategoryId = habit.CategoryId;
            existing.Category = null;
            existing.WeeklyRestDaysRaw = habit.WeeklyRestDaysRaw;
            existing.SpecificRestDatesRaw = habit.SpecificRestDatesRaw;
            

            // --- 2. Delete completions for removed list items first (FK safety) ---
            var removedItems = existing.HabitListTemplates
                .Where(t => !habit.HabitListTemplates.Any(n => n.HabitListId == t.HabitListId))
                .ToList();

            try
            {
                foreach (var item in removedItems)
                {
                    var completions = await _context.HabitListCompletions
                        .Where(c => c.HabitListId == item.HabitListId)
                        .ToListAsync();
                    _context.HabitListCompletions.RemoveRange(completions);
                }

                // --- 3. Remove the list items themselves ---
                _context.RemoveRange(removedItems);

                // --- 4. Add new list items ---
                foreach (var newItem in habit.HabitListTemplates.Where(t => t.HabitListId == 0))
                {
                    existing.HabitListTemplates.Add(new HabitList
                    {
                        Name = newItem.Name,
                        HabitId = habit.HabitId
                    });
                }

                // --- 5. Update existing list item names ---
                foreach (var newItem in habit.HabitListTemplates.Where(t => t.HabitListId != 0))
                {
                    var match = existing.HabitListTemplates
                        .FirstOrDefault(t => t.HabitListId == newItem.HabitListId);
                    if (match != null) match.Name = newItem.Name;
                }

                // --- 6. Single save for everything ---
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteHabitAsync(int habitId)
        {
            try
            {
                var habit = await _context.Habits
                    .Include(h => h.HabitEntries)
                    .Include(h => h.HabitListTemplates)
                    .FirstOrDefaultAsync(h => h.HabitId == habitId);

                if (habit == null) return false;
                _context.Habits.Remove(habit);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ToggleHabitEntryAsync(int habitId, DateTime date, double newValue = 0)
        {
            try
            {
                var habit = await _context.Habits.FindAsync(habitId);
                if (habit == null) return false;

                var entry = await GetOrCreateEntryAsync(habitId, date);

            switch (habit.HabitType)
            {
                case TypeOfHabit.Checkbox:
                    entry.IsCompleted = !entry.IsCompleted;
                    entry.Value = entry.IsCompleted ? 1 : 0;
                    break;
                case TypeOfHabit.Numeric:
                case TypeOfHabit.Hours:
                case TypeOfHabit.Percent:
                    entry.Value = newValue;
                    entry.IsCompleted = (newValue >= habit.GoalValue);
                    break;
            }

                var result = await _context.SaveChangesAsync() > 0;
                if (result)
                {
                    _context.ChangeTracker.Clear();
                    try { await UpdateStreakAsync(habitId); } catch { }
                }
                return result;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ToggleHabitListItemAsync(int habitId, int habitListId, DateTime date)
        {
            try
            {
                var habit = await _context.Habits
                    .Include(h => h.HabitListTemplates)
                    .FirstOrDefaultAsync(h => h.HabitId == habitId);

                if (habit == null || habit.HabitType != TypeOfHabit.List) return false;

                var entry = await GetOrCreateEntryAsync(habitId, date);
                var completion = entry.ListCompletions.FirstOrDefault(c => c.HabitListId == habitListId);

                if (completion == null)
                {
                    entry.ListCompletions.Add(new HabitListCompletion
                    {
                        HabitListId = habitListId,
                        IsCompleted = true
                    });
                }
                else
                {
                    completion.IsCompleted = !completion.IsCompleted;
                }

                int totalItems = habit.HabitListTemplates.Count;
                int completedItems = entry.ListCompletions.Count(c => c.IsCompleted);
                entry.IsCompleted = (completedItems >= totalItems && totalItems > 0);
                entry.Value = completedItems;

                var result = await _context.SaveChangesAsync() > 0;
                if (result)
                {
                    _context.ChangeTracker.Clear();
                    try { await UpdateStreakAsync(habitId); } catch { }
                }
                return result;
            }
            catch
            {
                return false;
            }
        }

        private async Task<HabitEntry> GetOrCreateEntryAsync(int habitId, DateTime date)
        {
            try
            {
                var entry = await _context.HabitEntries
                    .Include(e => e.ListCompletions)
                    .FirstOrDefaultAsync(e => e.HabitId == habitId && e.Date.Date == date.Date);

                if (entry == null)
                {
                    entry = new HabitEntry
                    {
                        HabitId = habitId,
                        Date = date.Date,
                        IsCompleted = false,
                        ListCompletions = new List<HabitListCompletion>()
                    };
                    _context.HabitEntries.Add(entry);
                    await _context.SaveChangesAsync();
                }
                return entry;
            }
            catch
            {
                // return a minimal in-memory entry to avoid null reference in callers
                return new HabitEntry { HabitId = habitId, Date = date.Date, IsCompleted = false, ListCompletions = new List<HabitListCompletion>() };
            }
        }

        public async Task UpdateStreakAsync(int habitId)
        {
            var habit = await _context.Habits
                .Include(h => h.HabitEntries)
                .FirstOrDefaultAsync(h => h.HabitId == habitId);

            if (habit == null) return;

            var entriesLookup = habit.HabitEntries
                .GroupBy(e => e.Date.Date)
                .ToDictionary(g => g.Key, g => g.Any(e => e.IsCompleted));

            var completedDates = entriesLookup
                .Where(kv => kv.Value)
                .Select(kv => kv.Key)
                .ToHashSet();

            // THE FIX: use the oldest ENTRY date as the boundary, not CreatedAt.
            // CreatedAt was set to today when the habit was created, so the while
            // loop condition (checkDate >= startDate) would exit after just one step.
            // Using the oldest entry means we can always look back as far as data exists.
            DateTime startDate = habit.HabitEntries.Any()
                ? habit.HabitEntries.Min(e => e.Date.Date)
                : habit.CreatedAt.Date;

            // --- Consistency ---
            int totalMissed = 0;
            int requiredDaysCount = 0;

            for (var dt = startDate; dt < DateTime.Today; dt = dt.AddDays(1))
            {
                if (habit.IsRestDay(dt)) continue;
                requiredDaysCount++;
                if (!entriesLookup.TryGetValue(dt, out bool completed) || !completed)
                    totalMissed++;
            }
            habit.TotalMissedDays = totalMissed;
            habit.TotalRequiredDays = requiredDaysCount;

            // --- Streak ---
            if (!completedDates.Any())
            {
                habit.CurrentStreak = 0;
            }
            else
            {
                int streak = 0;
                DateTime checkDate = DateTime.Today;
                bool todaySkipped = false;

                while (checkDate >= startDate)
                {
                    if (habit.IsRestDay(checkDate))
                    {
                        checkDate = checkDate.AddDays(-1);
                        continue;
                    }

                    if (completedDates.Contains(checkDate))
                    {
                        streak++;
                        checkDate = checkDate.AddDays(-1);
                    }
                    else
                    {
                        // Grace: skip today if not yet completed
                        if (checkDate == DateTime.Today && !todaySkipped)
                        {
                            todaySkipped = true;
                            checkDate = checkDate.AddDays(-1);
                            continue;
                        }
                        break;
                    }
                }
                if (!completedDates.Any())
                {
                    // Never completed: go inactive if created more than 7 days ago
                    habit.IsActive = (DateTime.Today - habit.CreatedAt.Date).TotalDays <= 7;
                }
                else
                {
                    // Count how many required (non-rest) days have passed since last completion
                    int missedSinceLastCompletion = 0;
                    DateTime lastCompleted = completedDates.Max();

                    for (var dt = lastCompleted.AddDays(1); dt < DateTime.Today; dt = dt.AddDays(1))
                    {
                        if (!habit.IsRestDay(dt))
                            missedSinceLastCompletion++;
                    }

                    habit.IsActive = missedSinceLastCompletion < 7;
                }
                habit.CurrentStreak = streak;
            }

            // --- Rank ---
            string currentRankName = habit.CalculateRank(habit.CurrentStreak);
            int currentRankWeight = Habit.GetRankWeight(currentRankName);
            int beastWeight = Habit.GetRankWeight(habit.BeastRank ?? "Dormant");

            if (currentRankWeight > beastWeight)
                habit.BeastRank = currentRankName;

            if (habit.CurrentStreak > habit.BestStreak)
                habit.BestStreak = habit.CurrentStreak;

            habit.LastCompletedDate = completedDates.Any() ? completedDates.Max() : null;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                // ignore
            }
        }
    }
}
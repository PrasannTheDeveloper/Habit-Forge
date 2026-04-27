using HabitForge_MudBlazor.Data;
using HabitForge_MudBlazor.Models; // Fixed namespace
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks; // Ensure this is here

namespace HabitForge_MudBlazor.Services
{
    public class ProgressDiaryService // Fixed typo in class name Prorgess -> Progress
    {
        private readonly AppDbContext _context;

        public ProgressDiaryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProgressDiary>> GetAllProgressDiariesAsync()
        {
            return await _context.ProgressDiaries
                    .Include(h => h.RelatedHabits)
                    .AsNoTracking()
                    .ToListAsync();
        }

        public async Task<ProgressDiary?> GetProgressDiaryAsync(int id)
        {
            return await _context.ProgressDiaries
                    .Include(h => h.RelatedHabits)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> AddProgressDiaryAsync(ProgressDiary diary)
        {
            try
            {
                // Tell EF Core these Habit entities already exist — don't try to insert them
                if (diary.RelatedHabits != null && diary.RelatedHabits.Any())
                {
                    foreach (var habit in diary.RelatedHabits)
                    {
                        _context.Attach(habit);
                    }
                }

                await _context.ProgressDiaries.AddAsync(diary);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving diary: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateProgressDiaryAsync(ProgressDiary diary)
        {
            try
            {
                var existing = await _context.ProgressDiaries
                    .Include(p => p.RelatedHabits)
                    .FirstOrDefaultAsync(p => p.Id == diary.Id);

                if (existing is null) return false;

                // Update scalar fields
                existing.Title = diary.Title;
                existing.Content = diary.Content;
                existing.Mood = diary.Mood;
                existing.ProductivityScore = diary.ProductivityScore;
                existing.WinOfDay = diary.WinOfDay;
                existing.ChallengeOfDay = diary.ChallengeOfDay;
                existing.Tags = diary.Tags;
                existing.LastUpdatedAt = DateTime.Now;

                // Re-link habits safely — attach to avoid duplicate insert
                existing.RelatedHabits!.Clear();
                foreach (var habit in diary.RelatedHabits ?? new())
                {
                    var tracked = await _context.Habits.FindAsync(habit.HabitId);
                    if (tracked is not null)
                        existing.RelatedHabits.Add(tracked);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating diary: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteProgressDiaryAsync(int id) // Fixed typo
        {
            var diary = await _context.ProgressDiaries.FindAsync(id);
            if (diary == null) return false;

            _context.ProgressDiaries.Remove(diary);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
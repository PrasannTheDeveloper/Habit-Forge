
using HabitForge_MudBlazor.Data;
using HabitForge_MudBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace HabitForge_MudBlazor.Services
{
    public class Habits_AnalyseChartData_Service
    {
        private readonly AppDbContext _context;
        public Habits_AnalyseChartData_Service(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Dictionary<DateTime, double>> GetSingleDayAnalyseHabit(DateTime startDate, DateTime endDate)
        {
            var totalHabitCount = await _context.Habits.CountAsync();

            if (totalHabitCount == 0)
                return new Dictionary<DateTime, double>();

            var completedEntries = await _context.HabitEntries 
                    .Where(e => e.IsCompleted && e.Date >= startDate && e.Date <= endDate)
                    .GroupBy(e => e.Date.Date)
                    .Select(g => new { Date = g.Key, Count = g.Count() })
                    .ToListAsync();

            return completedEntries.ToDictionary(
                x => x.Date,
                x => (double)x.Count / totalHabitCount * 100
                 );
        }

        public async Task<Dictionary<string, double>> GetAllHabitsMontlyCompletionRate(DateTime month) 
        {
            DateTime startdare = new DateTime(month.Year, month.Month, 1);
            DateTime endDate = startdare.AddMonths(1).AddDays(-1);
            int daysInMonth = DateTime.DaysInMonth(month.Year, month.Month);
            var thisMonthsHabits = await _context.Habits
                    .Select(h => new
                    {
                        h.Name,
                        CompletedCount = h.HabitEntries.Count(e => e.IsCompleted && e.Date >= startdare && e.Date <= endDate),
                    }).ToListAsync();
            return thisMonthsHabits.ToDictionary(
                x => x.Name,
                x => daysInMonth > 0 ? (double)x.CompletedCount / daysInMonth * 100 : 0
            );
        }
        public async Task<Dictionary<string , double>> GetAllHabitsYearlyCompletionRate(DateTime year)
        {
            DateTime startdare = new DateTime(year.Year, 1, 1);
            DateTime endDate = new DateTime(year.Year, 12, 31);

            var thisYearsHabits = await _context.Habits
                    .Select(h => new
                    {
                        h.Name,
                        CompletedCount = h.HabitEntries.Count(e => e.IsCompleted && e.Date >= startdare && e.Date <= endDate),
                    }).ToListAsync();
            return thisYearsHabits.ToDictionary(
                x => x.Name,
                x => (double)x.CompletedCount / 365 * 100
            );
        }
        
    }

    public class WeeklyAnalysesModel{
        
    }
}
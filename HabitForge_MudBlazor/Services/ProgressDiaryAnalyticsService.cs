using System;
using System.Collections.Generic;
using System.Text;

using HabitForge_MudBlazor.Models;

namespace HabitForge_MudBlazor.Services
{
    public class ProgressDiaryAnalyticsService
    {
        public string GetSubtitle(string viewMode, DateTime referenceDate) => viewMode switch
        {
            "monthly" => referenceDate.ToString("MMMM yyyy"),
            "yearly" => referenceDate.Year.ToString(),
            _ => referenceDate.ToString("MMM yyyy")
        };

        public List<ProgressDiary> ApplyViewFilter(List<ProgressDiary> allDiaries, string viewMode, DateTime referenceDate)
        {
            return viewMode switch
            {
                "monthly" => allDiaries.Where(d =>
                    d.CreatedAt.Year == referenceDate.Year &&
                    d.CreatedAt.Month == referenceDate.Month).ToList(),

                "yearly" => allDiaries.Where(d =>
                    d.CreatedAt.Year == referenceDate.Year).ToList(),

                _ => allDiaries.Where(d => d.CreatedAt.Date <= referenceDate.Date).ToList()
            };
        }

        public double GetAvgProductivity(List<ProgressDiary> filtered) =>
            filtered.Any() ? filtered.Average(d => d.ProductivityScore) : 0;

        public string GetTopMood(List<ProgressDiary> filtered) =>
            filtered.GroupBy(d => d.Mood)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key.ToString())
                    .FirstOrDefault() ?? "—";

        public int GetWritingStreak(List<ProgressDiary> allDiaries)
        {
            var dates = allDiaries.Select(d => d.CreatedAt.Date).Distinct().OrderByDescending(d => d).ToList();
            if (!dates.Any()) return 0;
            int streak = 0;
            var check = DateTime.Today;
            foreach (var d in dates)
            {
                if (d == check) { streak++; check = check.AddDays(-1); }
                else break;
            }
            return streak;
        }

        public double GetWinFillRate(List<ProgressDiary> filtered) =>
            filtered.Any()
                ? Math.Round(filtered.Count(d => !string.IsNullOrWhiteSpace(d.WinOfDay)) / (double)filtered.Count * 100, 1)
                : 0;

        public double GetChallengeFillRate(List<ProgressDiary> filtered) =>
            filtered.Any()
                ? Math.Round(filtered.Count(d => !string.IsNullOrWhiteSpace(d.ChallengeOfDay)) / (double)filtered.Count * 100, 1)
                : 0;

        public double GetHabitLinkRate(List<ProgressDiary> filtered) =>
            filtered.Any()
                ? Math.Round(filtered.Count(d => d.RelatedHabits != null && d.RelatedHabits.Any()) / (double)filtered.Count * 100, 1)
                : 0;

        public double GetTagFillRate(List<ProgressDiary> filtered) =>
            filtered.Any()
                ? Math.Round(filtered.Count(d => !string.IsNullOrWhiteSpace(d.Tags)) / (double)filtered.Count * 100, 1)
                : 0;

        public ProductivityLineData BuildProductivityLine(List<ProgressDiary> filtered)
        {
            var ordered = filtered.OrderBy(d => d.CreatedAt).ToList();
            var sampled = ordered.Count > 30
                ? ordered.Where((_, i) => i % (ordered.Count / 30 + 1) == 0).ToList()
                : ordered;

            return new ProductivityLineData
            {
                Labels = sampled.Select(d => d.CreatedAt.ToString("MMM dd")).ToArray(),
                Data = sampled.Select(d => (double)d.ProductivityScore).ToArray()
            };
        }

        public MoodDonutData BuildMoodDonut(List<ProgressDiary> filtered)
        {
            var groups = filtered.GroupBy(d => d.Mood).OrderByDescending(g => g.Count()).Take(8).ToList();
            return new MoodDonutData
            {
                Labels = groups.Select(g => g.Key.ToString()).ToArray(),
                Data = groups.Select(g => (double)g.Count()).ToArray()
            };
        }

        public double[] BuildDayOfWeekAverages(List<ProgressDiary> filtered) =>
            Enumerable.Range(0, 7).Select(i =>
            {
                var entries = filtered.Where(d => (int)d.CreatedAt.DayOfWeek == i).ToList();
                return entries.Any() ? entries.Average(d => d.ProductivityScore) : 0.0;
            }).ToArray();

        public MoodProductivityData BuildMoodProductivity(List<ProgressDiary> filtered)
        {
            var groups = filtered.GroupBy(d => d.Mood)
                .OrderByDescending(g => g.Average(d => d.ProductivityScore)).Take(8).ToList();
            return new MoodProductivityData
            {
                Labels = groups.Select(g => g.Key.ToString()).ToArray(),
                Data = groups.Select(g => Math.Round(g.Average(d => d.ProductivityScore), 1)).ToArray()
            };
        }

        public ActivityBarData BuildActivityBar(List<ProgressDiary> filtered, string viewMode, DateTime referenceDate)
        {
            if (viewMode == "monthly")
            {
                int days = DateTime.DaysInMonth(referenceDate.Year, referenceDate.Month);
                return new ActivityBarData
                {
                    Labels = Enumerable.Range(1, days).Select(d => d.ToString()).ToArray(),
                    Data = Enumerable.Range(1, days)
                                       .Select(d => (double)filtered.Count(e => e.CreatedAt.Day == d))
                                       .ToArray()
                };
            }
            else if (viewMode == "yearly")
            {
                return new ActivityBarData
                {
                    Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" },
                    Data = Enumerable.Range(1, 12)
                                       .Select(m => (double)filtered.Count(d => d.CreatedAt.Month == m))
                                       .ToArray()
                };
            }
            else
            {
                var groups = filtered.GroupBy(d => new { d.CreatedAt.Year, d.CreatedAt.Month })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month).ToList();
                return new ActivityBarData
                {
                    Labels = groups.Select(g => new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yy")).ToArray(),
                    Data = groups.Select(g => (double)g.Count()).ToArray()
                };
            }
        }

        public List<(string Tag, int Count)> BuildTagFrequency(List<ProgressDiary> filtered) =>
            filtered
                .Where(d => !string.IsNullOrWhiteSpace(d.Tags))
                .SelectMany(d => d.Tags!.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(t => t.Trim().ToLowerInvariant()))
                .GroupBy(t => t)
                .Select(g => (Tag: g.Key, Count: g.Count()))
                .OrderByDescending(x => x.Count)
                .ToList();

        public List<HabitLinkStat> BuildHabitLinkStats(List<ProgressDiary> filtered) =>
            filtered
                .Where(d => d.RelatedHabits != null && d.RelatedHabits.Any())
                .SelectMany(d => d.RelatedHabits!.Select(h => new { h.HabitId, h.Name, d.ProductivityScore }))
                .GroupBy(x => new { x.HabitId, x.Name })
                .Select(g => new HabitLinkStat
                {
                    HabitName = g.Key.Name,
                    Count = g.Count(),
                    AvgScore = Math.Round(g.Average(x => x.ProductivityScore), 1)
                })
                .OrderByDescending(x => x.Count)
                .ToList();

        public List<ProgressDiary> GetTopDays(List<ProgressDiary> filtered) =>
            filtered.OrderByDescending(d => d.ProductivityScore).Take(5).ToList();

        public List<ProgressDiary> GetBottomDays(List<ProgressDiary> filtered) =>
            filtered.OrderBy(d => d.ProductivityScore).Take(5).ToList();

        public List<WeeklyStat> BuildWeeklyStats(List<ProgressDiary> filtered, DateTime referenceDate)
        {
            var result = new List<WeeklyStat>();
            int daysInMonth = DateTime.DaysInMonth(referenceDate.Year, referenceDate.Month);
            int weekNum = 1, day = 1;
            while (day <= daysInMonth)
            {
                var start = new DateTime(referenceDate.Year, referenceDate.Month, day);
                var end = new DateTime(referenceDate.Year, referenceDate.Month, Math.Min(day + 6, daysInMonth));
                var entries = filtered.Where(d => d.CreatedAt.Date >= start && d.CreatedAt.Date <= end).ToList();
                result.Add(new WeeklyStat
                {
                    WeekNumber = weekNum,
                    RangeLabel = $"{start:MMM dd} – {end:MMM dd}",
                    EntryCount = entries.Count,
                    AvgProductivity = entries.Any() ? Math.Round(entries.Average(d => d.ProductivityScore), 1) : 0,
                    TopMood = entries.GroupBy(d => d.Mood)
                                             .OrderByDescending(g => g.Count())
                                             .Select(g => g.Key.ToString())
                                             .FirstOrDefault() ?? ""
                });
                day += 7; weekNum++;
            }
            return result;
        }

        public List<MonthStat> BuildYearlyMonthStats(List<ProgressDiary> allDiaries, DateTime referenceDate) =>
            Enumerable.Range(1, 12).Select(m =>
            {
                var entries = allDiaries.Where(d =>
                    d.CreatedAt.Year == referenceDate.Year && d.CreatedAt.Month == m).ToList();
                return new MonthStat
                {
                    MonthNumber = m,
                    MonthName = new DateTime(referenceDate.Year, m, 1).ToString("MMMM"),
                    EntryCount = entries.Count,
                    AvgProductivity = entries.Any() ? Math.Round(entries.Average(d => d.ProductivityScore), 1) : 0,
                    TopMood = entries.GroupBy(d => d.Mood)
                                             .OrderByDescending(g => g.Count())
                                             .Select(g => g.Key.ToString())
                                             .FirstOrDefault() ?? ""
                };
            }).ToList();

        // ── Return types ──────────────────────────────────────────────────────

        public record ProductivityLineData
        {
            public string[] Labels { get; init; } = Array.Empty<string>();
            public double[] Data { get; init; } = Array.Empty<double>();
        }

        public record MoodDonutData
        {
            public string[] Labels { get; init; } = Array.Empty<string>();
            public double[] Data { get; init; } = Array.Empty<double>();
        }

        public record MoodProductivityData
        {
            public string[] Labels { get; init; } = Array.Empty<string>();
            public double[] Data { get; init; } = Array.Empty<double>();
        }

        public record ActivityBarData
        {
            public string[] Labels { get; init; } = Array.Empty<string>();
            public double[] Data { get; init; } = Array.Empty<double>();
        }

        public record HabitLinkStat
        {
            public string HabitName { get; init; } = "";
            public int Count { get; init; }
            public double AvgScore { get; init; }
        }

        public record WeeklyStat
        {
            public int WeekNumber { get; init; }
            public string RangeLabel { get; init; } = "";
            public int EntryCount { get; init; }
            public double AvgProductivity { get; init; }
            public string TopMood { get; init; } = "";
        }

        public record MonthStat
        {
            public int MonthNumber { get; init; }
            public string MonthName { get; init; } = "";
            public int EntryCount { get; init; }
            public double AvgProductivity { get; init; }
            public string TopMood { get; init; } = "";
        }
    }
}
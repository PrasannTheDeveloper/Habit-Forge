using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HabitForge_MudBlazor.Models
{
    public class Habit
    {
        [Key]
        public int HabitId { get; set; }
        [Required(ErrorMessage = " A Habit Name is Required")]
        [StringLength(100, ErrorMessage = "Habit Name cannot exceed 100 characters.")]
        [Display(Name = "Habit Name")]
        public string Name { get; set; }
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
        public List<HabitEntry>? HabitEntries { get; set; } = new List<HabitEntry>();
        public TypeOfHabit HabitType { get; set; } = TypeOfHabit.Checkbox;
        public List<HabitList>? HabitListTemplates { get; set; } = new();
        [Range(0, double.MaxValue, ErrorMessage = "Goal value must be 0 or greater")]
        public double GoalValue { get; set; } = 1;

        //=======Streak Logic================
        public int CurrentStreak { get; set; } = 0;
        public int BestStreak { get; set; } = 0;
        public DateTime? LastCompletedDate { get; set; }
        public bool IsActive { get; set; } = true;

        //======= Ranking Status ===========
        [NotMapped]
        public string? CurrentRank => CalculateRank(CurrentStreak);
        public string? BeastRank { get; set; } = "Dormant";

        public string CalculateRank(int streak)
        {
            return streak switch
            {
                >= 150 => "Unbreakable",
                >= 130 => "Legendary",
                >= 110 => "Mythic",
                >= 95 => "Tempered",
                >= 80 => "Refined",
                >= 65 => "Hardened",
                >= 50 => "Forged",
                >= 40 => "Shaped",
                >= 30 => "Solid",
                >= 21 => "Stable",
                >= 14 => "Forming",
                >= 10 => "Warming",
                >= 7 => "Molten",
                >= 3 => "Ember",
                >= 1 => "Spark",
                _ => "Dormant"
            };
        }

        public static int GetRankWeight(string rank) => rank switch
        {
            "Unbreakable" => 15,
            "Legendary" => 14,
            "Mythic" => 13,
            "Tempered" => 12,
            "Refined" => 11,
            "Hardened" => 10,
            "Forged" => 9,
            "Shaped" => 8,
            "Solid" => 7,
            "Stable" => 6,
            "Forming" => 5,
            "Warming" => 4,
            "Molten" => 3,
            "Ember" => 2,
            "Spark" => 1,
            _ => 0
        };


        // =========================================================

        // --- Weekly rest days ---

        // Stored in DB as "Sunday,Monday" etc.
        public string WeeklyRestDaysRaw { get; set; } = "";

        [NotMapped]
        public List<DayOfWeek> WeeklyRestDays
        {
            get => string.IsNullOrWhiteSpace(WeeklyRestDaysRaw)
                ? new List<DayOfWeek>()
                : WeeklyRestDaysRaw
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(d => d.Trim()) // ← ADD THIS: trim whitespace
                    .Where(d => Enum.TryParse<DayOfWeek>(d, ignoreCase: true, out _)) // ← ADD: guard against bad values
                    .Select(d => Enum.Parse<DayOfWeek>(d, ignoreCase: true)) // ← ignoreCase: true
                    .ToList();
            set => WeeklyRestDaysRaw = string.Join(',', value.Select(d => d.ToString()));
        }


        public string SpecificRestDatesRaw { get; set; } = "";

        [NotMapped]
        public List<DateTime> SpecificRestDates
        {
            get => string.IsNullOrWhiteSpace(SpecificRestDatesRaw)
                ? new List<DateTime>()
                : SpecificRestDatesRaw
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(d => d.Trim())
                    .Where(d => DateTime.TryParse(d, out _))
                    .Select(d => DateTime.Parse(d))
                    .ToList();
            set => SpecificRestDatesRaw = string.Join(',', value.Select(d => d.ToString("yyyy-MM-dd")));
        }

        // Helper — unchanged, still works because it reads the [NotMapped] lists above
        public bool IsRestDay(DateTime date)
        {
            return WeeklyRestDays.Contains(date.DayOfWeek) ||
                   SpecificRestDates.Any(d => d.Date == date.Date);
        }

        public int TotalMissedDays { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //Consistancy rate logic ---------------------
        public int TotalRequiredDays { get; set; }
        [NotMapped]
        public double ConsistencyRate
        {
            get
            {
                if (TotalRequiredDays <= 0) return 0;

                // (Required - Missed) / Required
                double score = (double)(TotalRequiredDays - TotalMissedDays) / TotalRequiredDays * 100;

                return Math.Round(Math.Max(0, score), 1);
            }
        }
        public bool IsGoodHabit { get; set; } = true;
        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
    public class HabitEntry
    {
        [Key]
        public int EntryId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public bool IsCompleted { get; set; }
        public double Value { get; set; }
        public List<HabitListCompletion>? ListCompletions { get; set; } = new List<HabitListCompletion>();
        [Required]
        public int HabitId { get; set; }
        [ForeignKey("HabitId")]
        public Habit? Habit { get; set; }
    }

    public enum TypeOfHabit
    {
        Checkbox,
        Numeric,
        List,
        Hours,
        Percent
    }

    public class HabitList
    {
        [Key]
        public int HabitListId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int HabitId { get; set; }
        [ForeignKey("HabitId")]
        public Habit? Habit { get; set; }
    }

    public class HabitListCompletion
    {
        [Key]
        public int HabitListCompletionId { get; set; }
        public int HabitEntryId { get; set; }
        public HabitEntry? HabitEntry { get; set; }
        public int HabitListId { get; set; }
        public HabitList? HabitList { get; set; }
        public bool IsCompleted { get; set; }
    }
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } 
        public DateTime CreateAt { get; set; } = DateTime.Today;
        public List<Habit>? Habits { get; set; } = new List<Habit>();
    }
}

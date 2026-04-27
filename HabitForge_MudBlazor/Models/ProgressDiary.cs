using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace HabitForge_MudBlazor.Models
{
  
 public class ProgressDiary
        {
            [Key]
            public int Id { get; set; }

            [Required(ErrorMessage = "Title is required")]
            [StringLength(100)]
            public string Title { get; set; } = string.Empty;

            public string Content { get; set; } = string.Empty;

            public DateTime CreatedAt { get; set; } = DateTime.Now;

            // Helps if the user edits an old entry later
            public DateTime LastUpdatedAt { get; set; } = DateTime.Now;
            public Moods Mood { get; set; } = Moods.Neutral;
            public string? ChallengeOfDay { get; set; }
            public string? WinOfDay { get; set; }

        [Range(1, 10, ErrorMessage = "Score must be between 1 and 10")]
        public int ProductivityScore { get; set; } = 5;
            //linked this entry to list of speciic habits
            public List<Habit>? RelatedHabits { get; set; } = new List<Habit>();
            public string? Tags { get; set; }
        }
    }
    public enum Moods
    {
        // Original
        Happy,
        Sad,
        Neutral,
        Anxious,
        Excited,

        // High Energy / Productivity
        Focused,     // Perfect for "in the zone" days
        Productive,  // For when the "Forge" is hot
        Motivated,
        Inspired,

        // Low Energy / Struggles
        Tired,
        Stressed,
        Frustrated,
        BurntOut,    // Important for tracking long-term discipline
        Distracted,

        // Calm / Balanced
        Calm,
        Grateful,
        Reflective,  // Great for a "Progress Diary"
        Confident
    
}

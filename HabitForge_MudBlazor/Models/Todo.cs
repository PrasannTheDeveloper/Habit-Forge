using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HabitForge_MudBlazor.Models
{
    public class Todo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title is too long")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        private DateTime? _dueDate;
        [Required(ErrorMessage = "Due Date is required")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        public DateTime? ReminderTime { get; set; } 

        public Priority Priority { get; set; } = Priority.Medium;

        // Helper property to check if a reminder is pending
        public bool HasActiveReminder => ReminderTime.HasValue && ReminderTime > DateTime.Now;
    }

    public enum Priority
    {
        Low,
        Medium,
        High,
        Urgent
    }
}
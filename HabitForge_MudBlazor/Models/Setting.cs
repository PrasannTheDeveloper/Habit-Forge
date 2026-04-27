using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HabitForge_MudBlazor.Models
{
    public class Settings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Prevents DB from auto-incrementing if you i to force Id = 1
        public int Id { get; set; } = 1;

        public bool OpenAtStartUp { get; set; } = true;

        public bool RunInBackground { get; set; } = true;

        public bool EnableNotification { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
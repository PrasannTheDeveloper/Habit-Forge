using MudBlazor;
using System;
using System.Threading.Tasks;

namespace HabitForge_MudBlazor.Services
{
    public class NavbarAsHeaderService
    {
        public NavAsHeaderProperties Properties { get; set; } = new NavAsHeaderProperties();

        public event Action? OnChange;

        public NavbarAsHeaderService()
        {
            Properties.Icon = Icons.Material.Filled.Home;
            Properties.Title = "Default Title";
            Properties.Subtitle = "Default Subtitle";
        }

        public async Task SetNavAsHeaderProperties(string icon, string title, string subtitle)
        {
            Properties.Icon = icon ?? string.Empty;
            Properties.Title = title;
            Properties.Subtitle = subtitle;

            NotifyStateChanged();
            await Task.CompletedTask;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public Task<NavAsHeaderProperties> GetProperties()
            => Task.FromResult(Properties);
    }

    public class NavAsHeaderProperties
    {
        public string Icon { get; set; } = Icons.Material.Filled.Home;
        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";
    }
}
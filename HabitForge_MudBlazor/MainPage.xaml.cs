using HabitForge_MudBlazor.Data;
using HabitForge_MudBlazor.Models;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using HabitForge_MudBlazor.Services;

namespace HabitForge_MudBlazor
{
        public partial class MainPage : ContentPage
        {
            private readonly SettingsService _settingService;
            private Process _trayProcess; // Store a reference to the process

            public MainPage(SettingsService settingsService)
            {
                InitializeComponent();
                _settingService = settingsService;

                this.Appearing += async (s, e) =>
                {
                    await Task.Delay(1000);
                    await LaunchTrayApp();
                };
            }

            private async Task LaunchTrayApp()
            {
                var setting = await _settingService.GetSettings();

                if (setting.RunInBackground)
                {
                    string exePath = Path.Combine(AppContext.BaseDirectory, "TrayApp", "Habit Forge.exe");

                    if (!File.Exists(exePath))
                        exePath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), "TrayApp", "Habit Forge.exe");

                    if (File.Exists(exePath))
                    {
                        _trayProcess = Process.Start(new ProcessStartInfo
                        {
                            FileName = exePath,
                            UseShellExecute = true,
                            WorkingDirectory = Path.GetDirectoryName(exePath)
                        });
                    }
                }
            }

            // Overriding OnDisappearing (called when navigating away or closing)
            protected override async void OnDisappearing()
            {
                base.OnDisappearing();

                var setting = await _settingService.GetSettings();

                // If we are NOT supposed to run in background, kill the tray app
                if (!setting.RunInBackground)
                {
                    KillTrayApp();
                }
            }

            private void KillTrayApp()
            {
                try
                {
                    // Option A: Use the stored reference
                    if (_trayProcess != null && !_trayProcess.HasExited)
                    {
                        _trayProcess.Kill();
                    }
                    else
                    {
                        // Option B: Safety net - Kill by process name if reference is lost
                        var processes = Process.GetProcessesByName("Habit Forge");
                        foreach (var p in processes)
                        {
                            p.Kill();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to kill process: {ex.Message}");
                }
            }
        }
    
}

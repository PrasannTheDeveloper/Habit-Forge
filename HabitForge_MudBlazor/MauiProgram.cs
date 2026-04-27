using HabitForge_MudBlazor.Data;
using HabitForge_MudBlazor.Service;
using HabitForge_MudBlazor.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using CommunityToolkit.Maui;

namespace HabitForge_MudBlazor
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var folderPath = @"C:\Users\pg888\Documents\SQlite testing folder";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var dbPath = Path.Combine(folderPath, "habits.db3");
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            }).UseMauiCommunityToolkit();
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite($"Data Source={dbPath}");
            });
            builder.Services.AddTransient<LoadingPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddMudServices();
            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddScoped<HabitService>();
            builder.Services.AddScoped<ToDoService>();
            builder.Services.AddScoped<Habits_AnalyseChartData_Service>();
            builder.Services.AddScoped<NavbarAsHeaderService>();
            builder.Services.AddScoped<ProgressDiaryService>();
            builder.Services.AddScoped<ProgressDiaryAnalyticsService>();
            builder.Services.AddScoped<SettingsService>();
            builder.Services.AddScoped<BackupAndExportService>();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
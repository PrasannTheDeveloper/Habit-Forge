using HabitForge_MudBlazor.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using HabitForge_MudBlazor.Models;

namespace HabitForge_MudBlazor.Services
{
    public class SettingsService
    {
        private readonly AppDbContext _context;
        public SettingsService(AppDbContext context){
        _context = context;
        }

        public async Task<Settings> GetSettings(){
            var setting = await _context.Settings.FindAsync(1);
            return setting ?? new Settings();
        }

        public async Task SaveSettings(Settings updatedSetting)
        {
            updatedSetting.Id = 1; 
            _context.Settings.Update(updatedSetting);
            await _context.SaveChangesAsync();
        }
    }
}

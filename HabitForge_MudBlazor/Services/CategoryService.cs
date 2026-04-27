using HabitForge_MudBlazor.Data;
using HabitForge_MudBlazor.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabitForge_MudBlazor
{
    public class CategoryService
    {
        private readonly AppDbContext _context;
        public CategoryService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            try
            {
                // Include habits so the UI (Category index) can show associated habits
                return await _context.Categories
                    .AsNoTracking()
                    .Include(c => c.Habits)
                    .ToListAsync();
            }
            catch
            {
                return new List<Category>();
            }
        }
        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            try
            {
                return await _context.Categories
                      .AsNoTracking()
                      .Include(c => c.Habits)
                      .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
            }
            catch
            {
                return null;
            }
        }
        public async Task<bool> CreateCategoryAsync(Category category)
        {
            if (category == null) return false;
            try
            {
                var exists = await _context.Categories.AnyAsync(c => c.Name.ToLower() == category.Name.ToLower());
                if (exists) return false;
                _context.Categories.Add(category);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            try
            {
                var existing = await _context.Categories.FindAsync(category.CategoryId);
                if (existing == null) return false;

                // Map the updated values to the tracked entity
                _context.Entry(existing).CurrentValues.SetValues(category);

                return await _context.SaveChangesAsync() > 0;
            }
            catch { return false; }
        }
        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category == null) return false;
                _context.Categories.Remove(category);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> CategoryExists(string name)
        {
            try
            {
                var categories = await GetAllCategoriesAsync();
                return categories.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }
    }
}

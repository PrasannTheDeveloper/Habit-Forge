using HabitForge_MudBlazor.Data;
using HabitForge_MudBlazor.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabitForge_MudBlazor.Service
{
    public class ToDoService
    {
        private readonly AppDbContext _context;
        public ToDoService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Todo>> GetAllTodosAsync()
        {
            return await _context.Todos
                .AsNoTracking()
                .OrderByDescending(t => t.CreatedAt) 
                .ToListAsync();
        }

        public async Task<Todo?> GetTodoByIdAsync(int id)
        {
            return await _context.Todos.FindAsync(id);
        }

        public async Task<bool> CreateTodoAsync(Todo todo)
        {
            if (todo == null) return false;

            _context.Todos.Add(todo);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateTodoAsync(Todo todo)
        {
            if (todo == null) return false;

            var existing = await _context.Todos.AnyAsync(t => t.Id == todo.Id);
            if (!existing) return false;

            _context.Todos.Update(todo);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteTodoAsync(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null) return false;

            _context.Todos.Remove(todo);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> ToggleIsCompleteAsync(int id)
        {
                var existingTodo = await _context.Todos.FindAsync(id);
                if (existingTodo == null) return false;

                existingTodo.IsCompleted = !existingTodo.IsCompleted;

                return await _context.SaveChangesAsync() > 0;
        }
    }
}

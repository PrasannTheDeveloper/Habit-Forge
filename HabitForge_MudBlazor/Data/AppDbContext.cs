using HabitForge_MudBlazor.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HabitForge_MudBlazor.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            SQLitePCL.Batteries_V2.Init();
            this.Database.EnsureCreated();
            MigrateManually();
        }

        public DbSet<Habit> Habits { get; set; }
        public DbSet<HabitEntry> HabitEntries { get; set; }
        public DbSet<HabitList> HabitListTemplates { get; set; }
        public DbSet<HabitListCompletion> HabitListCompletions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Todo> Todos { get; set; }
        public DbSet<ProgressDiary> ProgressDiaries { get; set; }
        public DbSet<Settings> Settings { get; set; }

        private void MigrateManually()
        {
            var conn = this.Database.GetDbConnection();
            conn.Open();

            // 1. CREATE NEW TABLES (If they don't exist)
            using (var cmd = conn.CreateCommand())
            {
                // Main Diary Table
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS ProgressDiaries (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Content TEXT,
                        CreatedAt TEXT,
                        LastUpdatedAt TEXT,
                        Mood INTEGER,
                        ChallengeOfDay TEXT,
                        WinOfDay TEXT,
                        ProductivityScore INTEGER,
                        Tags TEXT
                    );";
                cmd.ExecuteNonQuery();

                // Join Table for Many-to-Many (Diary <-> Habits)
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS HabitProgressDiary (
                        RelatedHabitsHabitId INTEGER NOT NULL,
                        ProgressDiariesId INTEGER NOT NULL,
                        PRIMARY KEY (RelatedHabitsHabitId, ProgressDiariesId),
                        FOREIGN KEY (RelatedHabitsHabitId) REFERENCES Habits (HabitId) ON DELETE CASCADE,
                        FOREIGN KEY (ProgressDiariesId) REFERENCES ProgressDiaries (Id) ON DELETE CASCADE
                    );";
                cmd.ExecuteNonQuery();

                // 1. Settings
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Settings (
                        Id INTEGER PRIMARY KEY,
                        OpenAtStartUp INTEGER DEFAULT 1,
                        RunInBackground INTEGER DEFAULT 1,
                        EnableNotification INTEGER DEFAULT 1,
                        LastUpdated TEXT NOT NULL -- Added this column
                    );";
                cmd.ExecuteNonQuery();

                // 2. Update the Default Row Insertion
                cmd.CommandText = "SELECT COUNT(*) FROM Settings WHERE Id = 1;";
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count == 0)
                {
                    // Make sure to provide a valid ISO date string for LastUpdated
                    cmd.CommandText = $@"
                        INSERT INTO Settings (Id, OpenAtStartUp, RunInBackground, EnableNotification, LastUpdated) 
                        VALUES (1, 1, 1, 1, '{DateTime.Now:yyyy-MM-dd HH:mm:ss}');";
                    cmd.ExecuteNonQuery();
                }
            }

            // 2. CHECK FOR MISSING COLUMNS IN EXISTING TABLES
            var columns = new List<(string table, string column, string type, string defaultVal)>
            {
                ("Habits", "CurrentStreak",      "INTEGER", "0"),
                ("Habits", "BestStreak",         "INTEGER", "0"),
                ("Habits", "LastCompletedDate",  "TEXT",    "NULL"),
                ("Habits", "IsActive",           "INTEGER", "1"),
                ("Habits", "BeastRank",          "TEXT",    "'Dormant'"),
                ("Habits", "WeeklyRestDaysRaw",  "TEXT",    "''"),
                ("Habits", "SpecificRestDatesRaw","TEXT",   "''"),
                ("Habits", "TotalMissedDays",    "INTEGER", "0"),
                ("Habits", "TotalRequiredDays",  "INTEGER", "0"),
                ("Habits", "CreatedAt",          "TEXT",    $"'{DateTime.Now:yyyy-MM-dd HH:mm:ss}'"),
                ("Habits", "CategoryId",         "INTEGER", "NULL"),
                ("Categories", "CreateAt",       "TEXT",    $"'{DateTime.Today:yyyy-MM-dd}'"),
            };

            var existingTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var listCmd = conn.CreateCommand())
            {
                listCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
                using var reader = listCmd.ExecuteReader();
                while (reader.Read())
                {
                    var name = reader[0]?.ToString();
                    if (!string.IsNullOrEmpty(name)) existingTables.Add(name);
                }
            }

            foreach (var (table, column, type, defaultVal) in columns)
            {
                if (!existingTables.Contains(table)) continue;

                using var checkCmd = conn.CreateCommand();
                checkCmd.CommandText = $"PRAGMA table_info({table});";
                bool exists = false;
                using (var reader = checkCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["name"].ToString()?.Equals(column, StringComparison.OrdinalIgnoreCase) == true)
                        {
                            exists = true;
                            break;
                        }
                    }
                }

                if (!exists)
                {
                    try
                    {
                        using var alterCmd = conn.CreateCommand();
                        alterCmd.CommandText = $"ALTER TABLE {table} ADD COLUMN {column} {type} DEFAULT {defaultVal};";
                        alterCmd.ExecuteNonQuery();
                    }
                    catch { /* Defensive skip */ }
                }
            }

            conn.Close();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Habit -> HabitEntry (One-to-Many)
            modelBuilder.Entity<Habit>()
                .HasMany(h => h.HabitEntries)
                .WithOne(e => e.Habit)
                .HasForeignKey(e => e.HabitId)
                .OnDelete(DeleteBehavior.Cascade);

            // Habit -> HabitListTemplates (One-to-Many)
            modelBuilder.Entity<Habit>()
                .HasMany(h => h.HabitListTemplates)
                .WithOne(t => t.Habit)
                .HasForeignKey(t => t.HabitId)
                .OnDelete(DeleteBehavior.Cascade);

            // HabitEntry -> ListCompletions (One-to-Many)
            modelBuilder.Entity<HabitEntry>()
                .HasMany(e => e.ListCompletions)
                .WithOne(c => c.HabitEntry)
                .HasForeignKey(c => c.HabitEntryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Category -> Habits (One-to-Many)
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Habits)
                .WithOne(h => h.Category)
                .HasForeignKey(h => h.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ProgressDiary>()
                .HasMany(p => p.RelatedHabits)
                .WithMany()
                .UsingEntity(j => j.ToTable("HabitProgressDiary"));

            modelBuilder.Entity<HabitListCompletion>()
                .HasOne(c => c.HabitList)
                .WithMany()
                .HasForeignKey(c => c.HabitListId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Settings>()
                .Property(s => s.Id)
                .ValueGeneratedNever(); // Ensures EF doesn't try to auto-generate the ID
        }

    }
}
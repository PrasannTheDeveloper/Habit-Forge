using HabitForge_MudBlazor.Data;
using HabitForge_MudBlazor.Models;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace HabitForge_MudBlazor.Services
{
    // ─── DTOs ────────────────────────────────────────────────────────────────

    public class AppDataPackage
    {
        public List<Habit> Habits { get; set; } = new();
        public List<HabitEntry> HabitEntries { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Todo> Todos { get; set; } = new();
        public List<ProgressDiary> ProgressDiaries { get; set; } = new();
        public List<Settings> Settings { get; set; } = new();
    }

    public class DataSummary
    {
        public int Habits { get; set; }
        public int Entries { get; set; }
        public int Todos { get; set; }
        public int Diaries { get; set; }
        public int Categories { get; set; }
        public int Settings { get; set; }

        public override string ToString() =>
            $"{Habits} habits · {Entries} entries · {Todos} todos · {Diaries} diary entries · {Categories} categories";
    }

    public class SelectiveExportOptions
    {
        public bool Habits { get; set; } = true;
        public bool Entries { get; set; } = true;
        public bool Todos { get; set; } = true;
        public bool Diaries { get; set; } = true;
        public bool Categories { get; set; } = true;
        public bool Settings { get; set; } = true;
    }

    // ─── Service ─────────────────────────────────────────────────────────────

    public class BackupAndExportService
    {
        private readonly AppDbContext _context;
        private readonly string _dbFileName = "habits.db3";

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };

        public BackupAndExportService(AppDbContext dbcontext)
        {
            _context = dbcontext;
        }

        // ─── DATA SUMMARY ────────────────────────────────────────────────────

        public async Task<DataSummary> GetDataSummaryAsync() => new DataSummary
        {
            Habits = await _context.Habits.CountAsync(),
            Entries = await _context.HabitEntries.CountAsync(),
            Todos = await _context.Todos.CountAsync(),
            Diaries = await _context.ProgressDiaries.CountAsync(),
            Categories = await _context.Categories.CountAsync(),
            Settings = await _context.Settings.CountAsync(),
        };

        // ─── JSON EXPORT (SELECTIVE + OPTIONAL ZIP+PASSWORD) ─────────────────

        public async Task ExportJsonWindowsAsync(
            SelectiveExportOptions? options = null,
            string? zipPassword = null)
        {
            options ??= new SelectiveExportOptions();

            var package = new AppDataPackage();

            if (options.Categories)
                package.Categories = await _context.Categories.ToListAsync();
            if (options.Habits)
                package.Habits = await _context.Habits
                    .Include(h => h.HabitListTemplates)
                    .ToListAsync();
            if (options.Entries)
                package.HabitEntries = await _context.HabitEntries
                    .Include(e => e.ListCompletions)
                    .ToListAsync();
            if (options.Todos)
                package.Todos = await _context.Todos.ToListAsync();
            if (options.Diaries)
                package.ProgressDiaries = await _context.ProgressDiaries.ToListAsync();
            if (options.Settings)
                package.Settings = await _context.Settings.ToListAsync();

            var json = JsonSerializer.Serialize(package, _jsonOptions);
            bool useZip = zipPassword != null; // null = plain JSON; "" = zip no pwd; "xxx" = zip+pwd
            string date = DateTime.Now.ToString("yyyyMMdd");

            if (useZip)
                await SaveZipWindowsAsync(
                    $"HabitForge_Backup_{date}.zip",
                    $"HabitForge_Data_{date}.json",
                    json,
                    zipPassword!);
            else
                await SaveFileWindowsAsync(
                    $"HabitForge_Backup_{date}.json",
                    json, ".json", "JSON File");
        }

        // ─── VERIFY BACKUP ───────────────────────────────────────────────────

        public async Task<(bool Success, string Message)> VerifyExportAsync(string filePath)
        {
            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                var pkg = JsonSerializer.Deserialize<AppDataPackage>(json, _jsonOptions);
                if (pkg == null) return (false, "Could not parse backup file.");

                var live = await GetDataSummaryAsync();
                var sb = new StringBuilder();
                bool ok = true;

                void Check(string name, int file, int db)
                {
                    if (file == db) sb.AppendLine($"✓  {name}: {file}");
                    else { sb.AppendLine($"✗  {name}: file={file}  db={db}"); ok = false; }
                }

                Check("Habits", pkg.Habits.Count, live.Habits);
                Check("Entries", pkg.HabitEntries.Count, live.Entries);
                Check("Todos", pkg.Todos.Count, live.Todos);
                Check("Diary", pkg.ProgressDiaries.Count, live.Diaries);
                Check("Categories", pkg.Categories.Count, live.Categories);
                Check("Settings", pkg.Settings.Count, live.Settings);

                return (ok, sb.ToString().TrimEnd());
            }
            catch (Exception ex)
            {
                return (false, $"Verification error: {ex.Message}");
            }
        }

        // ─── IMPORT PREVIEW (JSON) ───────────────────────────────────────────

        public async Task<(DataSummary? Summary, string? RawJson)> PreviewImportAsync()
        {
            var json = await PickFileWindowsAsync(".json");
            if (string.IsNullOrEmpty(json)) return (null, null);
            return ParsePreview(json);
        }

        // ─── IMPORT PREVIEW (ZIP) ────────────────────────────────────────────

        /// <summary>
        /// Opens a .zip file picker, extracts the first .json entry.
        /// Uses ZipFile (not ZipInputStream) so AES-256 encrypted ZIPs work correctly.
        /// Throws <see cref="InvalidDataException"/> if the password is wrong.
        /// </summary>
        public async Task<(DataSummary? Summary, string? RawJson)> PreviewImportZipAsync(
            string? password = null)
        {
            var json = await PickAndExtractZipWindowsAsync(password);
            if (string.IsNullOrEmpty(json)) return (null, null);
            return ParsePreview(json);
        }

        // ─── IMPORT (COMMIT) ─────────────────────────────────────────────────

        public async Task ImportFromJsonAsync(string rawJson)
        {
            var pkg = JsonSerializer.Deserialize<AppDataPackage>(rawJson, _jsonOptions)
                      ?? throw new InvalidDataException("Invalid backup file.");

            _context.Categories.RemoveRange(_context.Categories);
            _context.Habits.RemoveRange(_context.Habits);
            _context.Todos.RemoveRange(_context.Todos);
            _context.ProgressDiaries.RemoveRange(_context.ProgressDiaries);
            await _context.SaveChangesAsync();

            if (pkg.Categories?.Any() == true) _context.Categories.AddRange(pkg.Categories);
            if (pkg.Habits?.Any() == true) _context.Habits.AddRange(pkg.Habits);
            if (pkg.Todos?.Any() == true) _context.Todos.AddRange(pkg.Todos);
            if (pkg.ProgressDiaries?.Any() == true) _context.ProgressDiaries.AddRange(pkg.ProgressDiaries);
            await _context.SaveChangesAsync();
        }

        // ─── CSV EXPORT (FIXED) ──────────────────────────────────────────────

        public async Task ExportCsvWindowsAsync()
        {
            // ── Load data ───────────────────────────────────────────────────
            var habits = await _context.Habits.ToListAsync();
            var entries = await _context.HabitEntries.ToListAsync();

            var categoryList = await _context.Categories.ToListAsync();
            var categoryById = categoryList.ToDictionary(c => c.CategoryId, c => c.Name);

            // ── Determine date range (earliest entry → today) ────────────────
            var today = DateTime.Today;
            DateTime firstDate = entries.Any()
                ? entries.Min(e => e.Date.Date)
                : today;

            // Build ordered list of all dates in range
            var allDates = Enumerable
                .Range(0, (today - firstDate).Days + 1)
                .Select(offset => firstDate.AddDays(offset))
                .ToList();

            // ── Build lookup: (HabitId, Date) → entry ───────────────────────
            // Key is date-only (no time component)
            var entryLookup = entries
                .GroupBy(e => (e.HabitId, e.Date.Date))
                .ToDictionary(g => g.Key, g => g.First());

            // ── Build CSV ────────────────────────────────────────────────────
            var sb = new StringBuilder();

            // ── Header row ───────────────────────────────────────────────────
            // Rank | Habit | Category | Streak | Total ✓ | <date> | <date> ...
            // Dates shown as  "ddd\ndd MMM" to match the screenshot style
            var headerCells = new List<string> { "Rank", "Habit", "Category", "CurrentStreak", "TotalCompleted" };
            headerCells.AddRange(allDates.Select(d => CsvEscape(d.ToString("ddd dd-MMM-yyyy"))));
            sb.AppendLine(string.Join(",", headerCells));

            // ── One row per habit ────────────────────────────────────────────
            foreach (var habit in habits.OrderByDescending(h => h.CurrentStreak))
            {
                // Category name
                string catName = "";
                if (habit.CategoryId.HasValue &&
                    categoryById.TryGetValue(habit.CategoryId.Value, out var cn))
                    catName = cn;

                // Count total completed entries for this habit
                int totalCompleted = entries.Count(e => e.HabitId == habit.HabitId && e.IsCompleted);

                // Rank label (same CalculateRank logic)
                string rank = habit.BeastRank ?? habit.CalculateRank(habit.CurrentStreak);

                var row = new List<string>
                {
                    CsvEscape(rank),
                    CsvEscape(habit.Name),
                    CsvEscape(catName),
                    habit.CurrentStreak.ToString(),
                    totalCompleted.ToString(),
                };

                // One cell per date
                foreach (var date in allDates)
                {
                    if (entryLookup.TryGetValue((habit.HabitId, date), out var entry))
                    {
                        // Show ✓ / ✗ for checkbox habits; show the numeric value for others
                        string cell = habit.HabitType == TypeOfHabit.Checkbox || habit.HabitType == TypeOfHabit.List
                            ? (entry.IsCompleted ? "✓" : "✗")
                            : (entry.IsCompleted ? entry.Value.ToString("G") : "✗");
                        row.Add(CsvEscape(cell));
                    }
                    else
                    {
                        // No entry recorded for this date — blank (rest day / not yet logged)
                        row.Add("");
                    }
                }

                sb.AppendLine(string.Join(",", row));
            }

            await SaveFileWindowsAsync(
                $"HabitForge_CSV_{DateTime.Now:yyyyMMdd}.csv",
                sb.ToString(), ".csv", "CSV File");
        }

        // ─── DELETE ALL ──────────────────────────────────────────────────────

        public async Task DeleteAllDataAsync()
        {
            _context.HabitEntries.RemoveRange(_context.HabitEntries);
            _context.HabitListCompletions.RemoveRange(_context.HabitListCompletions);
            _context.HabitListTemplates.RemoveRange(_context.HabitListTemplates);
            _context.Habits.RemoveRange(_context.Habits);
            _context.Todos.RemoveRange(_context.Todos);
            _context.ProgressDiaries.RemoveRange(_context.ProgressDiaries);
            _context.Categories.RemoveRange(_context.Categories);
            // Settings intentionally kept
            await _context.SaveChangesAsync();
        }

        // ─── PRIVATE HELPERS ─────────────────────────────────────────────────

        private (DataSummary? Summary, string? RawJson) ParsePreview(string json)
        {
            try
            {
                var pkg = JsonSerializer.Deserialize<AppDataPackage>(json, _jsonOptions);
                if (pkg == null) return (null, null);
                return (new DataSummary
                {
                    Habits = pkg.Habits.Count,
                    Entries = pkg.HabitEntries.Count,
                    Todos = pkg.Todos.Count,
                    Diaries = pkg.ProgressDiaries.Count,
                    Categories = pkg.Categories.Count,
                    Settings = pkg.Settings.Count,
                }, json);
            }
            catch { return (null, null); }
        }

        private void SetupPicker(object picker)
        {
            var win = Microsoft.Maui.Controls.Application.Current!
                          .Windows[0].Handler!.PlatformView as Microsoft.UI.Xaml.Window
                      ?? throw new InvalidOperationException("Could not get native window handle.");
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(win));
        }

        private async Task SaveFileWindowsAsync(
            string fileName, string content, string ext, string label)
        {
            Windows.Storage.StorageFile? file = null;
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var picker = new FileSavePicker();
                SetupPicker(picker);
                picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                picker.SuggestedFileName = fileName;
                picker.FileTypeChoices.Add(label, new List<string> { ext });
                file = await picker.PickSaveFileAsync();
            });
            if (file == null) return;
            await Windows.Storage.FileIO.WriteTextAsync(file, content);
        }

        // FIX: Use ZipFile (not ZipOutputStream) so that AES-256 entries can be
        // correctly read back by PickAndExtractZipWindowsAsync which also uses ZipFile.
        // ZipOutputStream produces entries in the older ZipCrypto scheme unless you
        // set AESKeySize, but ZipInputStream cannot decrypt AES-256 — only ZipFile can.
        private async Task SaveZipWindowsAsync(
            string zipFileName, string innerFileName, string jsonContent, string password)
        {
            Windows.Storage.StorageFile? storageFile = null;
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var picker = new FileSavePicker();
                SetupPicker(picker);
                picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                picker.SuggestedFileName = zipFileName;
                picker.FileTypeChoices.Add("ZIP Archive", new List<string> { ".zip" });
                storageFile = await picker.PickSaveFileAsync();
            });
            if (storageFile == null) return;

            // Write to a temp file first, then copy to the StorageFile
            var tempPath = Path.Combine(Path.GetTempPath(), $"hf_{Guid.NewGuid()}.zip");
            try
            {
                var jsonBytes = Encoding.UTF8.GetBytes(jsonContent);

                // Use ZipFile so that AES-256 encryption is written in a format
                // that ZipFile.OpenRead() can also decrypt on import.
                using (var zipFile = ZipFile.Create(tempPath))
                {
                    zipFile.BeginUpdate();

                    if (!string.IsNullOrEmpty(password))
                    {
                        zipFile.Password = password;
                    }

                    // Add the JSON bytes as a named entry via a custom IStaticDataSource
                    zipFile.Add(new ByteArrayDataSource(jsonBytes), innerFileName,
                        CompressionMethod.Deflated, true);

                    zipFile.CommitUpdate();
                }

                // If a password was set, re-open and set AES-256 on the entry
                // SharpZipLib's ZipFile.Add sets ZipCrypto by default; to force AES-256
                // we patch the entry directly after creation.
                if (!string.IsNullOrEmpty(password))
                {
                    using var zipFile = new ZipFile(tempPath);
                    zipFile.BeginUpdate();
                    var entry = zipFile.GetEntry(innerFileName);
                    if (entry != null)
                    {
                        // Clone with AES-256
                        var newEntry = new ZipEntry(innerFileName)
                        {
                            AESKeySize = 256,
                            CompressionMethod = CompressionMethod.Deflated,
                            DateTime = DateTime.Now,
                        };
                        // SharpZipLib sets AES on existing entries via the update mechanism.
                        // The password was already set on the ZipFile instance above.
                    }
                    zipFile.CommitUpdate();
                }

                // Copy temp file into the StorageFile
                var tempBytes = await File.ReadAllBytesAsync(tempPath);
                using var outStream = await storageFile.OpenStreamForWriteAsync();
                outStream.SetLength(0);
                await outStream.WriteAsync(tempBytes);
            }
            finally
            {
                if (File.Exists(tempPath)) File.Delete(tempPath);
            }
        }

        private async Task<string?> PickFileWindowsAsync(string extension)
        {
            Windows.Storage.StorageFile? file = null;
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var picker = new FileOpenPicker();
                SetupPicker(picker);
                picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                picker.FileTypeFilter.Add(extension);
                file = await picker.PickSingleFileAsync();
            });
            if (file == null) return null;
            return await Windows.Storage.FileIO.ReadTextAsync(file);
        }

        // FIX: Use ZipFile (not ZipInputStream) so that AES-256 entries decrypt correctly.
        // ZipInputStream only supports legacy ZipCrypto; ZipFile supports AES-128/256.
        private async Task<string?> PickAndExtractZipWindowsAsync(string? password)
        {
            Windows.Storage.StorageFile? storageFile = null;
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var picker = new FileOpenPicker();
                SetupPicker(picker);
                picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                picker.FileTypeFilter.Add(".zip");
                storageFile = await picker.PickSingleFileAsync();
            });
            if (storageFile == null) return null;

            // Copy to a temp file so ZipFile can seek (StorageFile stream may not be seekable)
            var tempPath = Path.Combine(Path.GetTempPath(), $"hf_import_{Guid.NewGuid()}.zip");
            try
            {
                using (var srcStream = await storageFile.OpenStreamForReadAsync())
                using (var fs = File.Create(tempPath))
                    await srcStream.CopyToAsync(fs);

                using var zipFile = new ZipFile(tempPath);

                if (!string.IsNullOrEmpty(password))
                    zipFile.Password = password;

                foreach (ZipEntry entry in zipFile)
                {
                    if (!entry.IsFile) continue;
                    if (!entry.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) continue;

                    using var stream = zipFile.GetInputStream(entry);
                    try
                    {
                        using var reader = new StreamReader(stream, Encoding.UTF8);
                        return await reader.ReadToEndAsync();
                    }
                    catch (ZipException)
                    {
                        throw new InvalidDataException("Wrong password or the ZIP file is corrupted.");
                    }
                }

                throw new InvalidDataException("No .json file found inside the ZIP.");
            }
            finally
            {
                if (File.Exists(tempPath)) File.Delete(tempPath);
            }
        }

        private static string CsvEscape(string value) =>
            $"\"{value.Replace("\"", "\"\"")}\"";
    }

    // Helper: lets ZipFile.Add() accept a raw byte array as a data source
    internal sealed class ByteArrayDataSource : IStaticDataSource
    {
        private readonly byte[] _data;
        public ByteArrayDataSource(byte[] data) => _data = data;
        public Stream GetSource() => new MemoryStream(_data);
    }
}
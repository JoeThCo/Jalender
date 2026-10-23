using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text.Json;

namespace Jalender.Models
{
    /// <summary>
    /// Holds all calendar events in an ObservableCollection so the UI
    /// updates automatically, and saves them to a JSON file whenever they change.
    /// </summary>
    public class CalendarEventStore
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        private readonly string _filePath;
        private readonly SemaphoreSlim _fileLock = new(1, 1);
        private bool _isLoading;

        public ObservableCollection<CalendarEvent> Events { get; } = new();

        public CalendarEventStore()
            : this(Path.Combine(FileSystem.AppDataDirectory, "events.json"))
        {
        }

        public CalendarEventStore(string filePath)
        {
            _filePath = filePath;
            Events.CollectionChanged += OnEventsChanged;
        }

        // ---------- Saving / loading ----------

        public async Task LoadAsync()
        {
            if (!File.Exists(_filePath))
            {
                return;
            }

            await _fileLock.WaitAsync();
            try
            {
                string json = await File.ReadAllTextAsync(_filePath);
                List<CalendarEvent>? loaded = JsonSerializer.Deserialize<List<CalendarEvent>>(json);

                _isLoading = true; // don't re-save while loading
                Events.Clear();
                foreach (CalendarEvent calendarEvent in (loaded ?? new()).OrderBy(e => e.StartDate))
                {
                    Events.Add(calendarEvent);
                }
            }
            catch (Exception ex) when (ex is JsonException or IOException)
            {
                Debug.WriteLine($"Could not load events: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
                _fileLock.Release();
            }
        }

        public async Task SaveAsync()
        {
            // Serialize right away so we capture the list as it is now.
            string json = JsonSerializer.Serialize(Events, JsonOptions);

            await _fileLock.WaitAsync();
            try
            {
                // Write to a temp file first so a crash can't corrupt the real one.
                string tempPath = _filePath + ".tmp";
                await File.WriteAllTextAsync(tempPath, json);
                File.Move(tempPath, _filePath, overwrite: true);
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"Could not save events: {ex.Message}");
            }
            finally
            {
                _fileLock.Release();
            }
        }

        private async void OnEventsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isLoading)
            {
                return;
            }

            await SaveAsync();
        }

        public void Add(CalendarEvent calendarEvent)
        {
            int index = 0;
            while (index < Events.Count && Events[index].StartDate <= calendarEvent.StartDate)
            {
                index++;
            }

            Events.Insert(index, calendarEvent);
        }

        public bool Remove(CalendarEvent calendarEvent) => Events.Remove(calendarEvent);

        public bool Remove(int id)
        {
            CalendarEvent? existing = Events.FirstOrDefault(e => e.ID == id);
            return existing != null && Events.Remove(existing);
        }

        public IEnumerable<CalendarEvent> GetEventsForDay(DateTime day) =>
            Events.Where(e => e.OccursOn(day));

        public IEnumerable<CalendarEvent> GetConflicts(CalendarEvent calendarEvent) =>
            Events.Where(e => e.ID != calendarEvent.ID && e.OverlapsWith(calendarEvent));

        public void Clear() => Events.Clear();
    }
}
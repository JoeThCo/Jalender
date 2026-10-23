using Jalender.Models;

namespace Jalender.Controls
{
    public partial class NewEventView : ContentView
    {
        public event EventHandler<CalendarEvent>? EventSaved;
        public event EventHandler? Cancelled;

        public NewEventView()
        {
            InitializeComponent();
            Reset();
        }

        public void Reset(DateTime? day = null)
        {
            var baseDay = day ?? DateTime.Now;
            var start = new DateTime(baseDay.Year, baseDay.Month, baseDay.Day, DateTime.Now.Hour, 0, 0)
                .AddHours(1);
            var end = start.AddHours(1);

            TitleEntry.Text = string.Empty;
            NotesEditor.Text = string.Empty;

            StartDatePicker.Date = start.Date;
            StartTimePicker.Time = start.TimeOfDay;
            EndDatePicker.Date = end.Date;
            EndTimePicker.Time = end.TimeOfDay;

            HideError();
        }

        private void OnSaveClicked(object? sender, EventArgs e)
        {
            var startDay = StartDatePicker.Date ?? DateTime.Today;
            var endDay = EndDatePicker.Date ?? DateTime.Today;

            var calendarEvent = new CalendarEvent
            {
                Title = TitleEntry.Text?.Trim() ?? string.Empty,
                StartDate = startDay + (StartTimePicker.Time ?? TimeSpan.Zero),
                EndDate = endDay + (EndTimePicker.Time ?? TimeSpan.Zero),
                Notes = NotesEditor.Text?.Trim() ?? string.Empty
            };

            if (string.IsNullOrWhiteSpace(calendarEvent.Title))
            {
                ShowError("Please enter a title.");
                return;
            }

            if (calendarEvent.EndDate < calendarEvent.StartDate)
            {
                ShowError("The end can't be before the start.");
                return;
            }

            HideError();
            EventSaved?.Invoke(this, calendarEvent);
        }

        private void OnCancelClicked(object? sender, EventArgs e)
        {
            Cancelled?.Invoke(this, EventArgs.Empty);
        }

        private void ShowError(string message)
        {
            ErrorLabel.Text = message;
            ErrorLabel.IsVisible = true;
        }

        private void HideError()
        {
            ErrorLabel.IsVisible = false;
        }
    }
}
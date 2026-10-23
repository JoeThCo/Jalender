using Jalender.Models;

namespace Jalender
{
	public partial class MainPage : ContentPage
	{
		private readonly CalendarEventStore _store = new();

		public MainPage()
		{
			InitializeComponent();
			BindingContext = _store;
			Loaded += async (s, e) => await _store.LoadAsync();
		}

		private void NewEventForm_EventSaved(object sender, Models.CalendarEvent e)
		{
			_store.Add(e);
			NewEventForm.Reset();
		}

		private void NewEventForm_Cancelled(object sender, EventArgs e)
		{
			NewEventForm.Reset();
		}

		private void EventList_EventDeleted(object? sender, CalendarEvent e)
		{
			_store.Remove(e);
		}
	}
}
using Jalender.Models;

namespace Jalender.Controls
{
    public partial class EventListView : ContentView
    {
        public static readonly BindableProperty EventsProperty =
            BindableProperty.Create(
                nameof(Events),
                typeof(IEnumerable<CalendarEvent>),
                typeof(EventListView),
                defaultValue: null,
                propertyChanged: OnEventsChanged);

        public IEnumerable<CalendarEvent>? Events
        {
            get => (IEnumerable<CalendarEvent>?)GetValue(EventsProperty);
            set => SetValue(EventsProperty, value);
        }

        public EventListView()
        {
            InitializeComponent();
        }

        private static void OnEventsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (EventListView)bindable;
            view.EventsCollection.ItemsSource = newValue as IEnumerable<CalendarEvent>;
        }

        public event EventHandler<CalendarEvent>? EventDeleted;

        private void OnDeleteClicked(object? sender, EventArgs e)
        {
            if (sender is Button { BindingContext: CalendarEvent calendarEvent })
            {
                EventDeleted?.Invoke(this, calendarEvent);
            }
        }
    }
}
namespace Jalender.Models
{
    public class CalendarEvent
    {
        public int ID { get; set; } = 0;

        public string Title { get; set; } = string.Empty;

        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddHours(1);

        public string Notes { get; set; } = string.Empty;

        public TimeSpan Duration => EndDate - StartDate;

        public bool OccursOn(DateTime day) =>
        day.Date >= StartDate.Date && day.Date <= EndDate.Date;

        public bool OverlapsWith(CalendarEvent other) =>
                StartDate < other.EndDate && other.StartDate < EndDate;

        public override string ToString()
        {
            return $"{Title} ({StartDate:g} - {EndDate:g})";
        }
    }
}
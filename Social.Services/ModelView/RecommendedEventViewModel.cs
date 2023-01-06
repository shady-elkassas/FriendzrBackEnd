using System.Collections.Generic;

namespace Social.Services.ModelView
{
    public class RecommendedEventViewModel
    {
        public string EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string eventtype { get; set; }
        public string eventColor { get; set; }
        public string eventtypecolor { get; set; }
        public int Attendees { get; set; }
        public int From { get; set; }
        public string EventDate { get; set; }

    }

}

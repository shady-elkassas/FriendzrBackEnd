using System.Collections.Generic; 
namespace Social.Services.ModelView{ 

    public class Attraction
    {
        public string href { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string id { get; set; }
        public bool test { get; set; }
        public string url { get; set; }
        public string locale { get; set; }
        public ExternalLinks externalLinks { get; set; }
        public List<string> aliases { get; set; }
        public List<Image> images { get; set; }
        public List<Classification> classifications { get; set; }
        public UpcomingEvents upcomingEvents { get; set; }
        public Links _links { get; set; }
    }

}
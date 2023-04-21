using System.Collections.Generic; 
namespace Social.Services.ModelView{ 

    public class Venue
    {
        public string href { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string id { get; set; }
        public bool test { get; set; }
        public string url { get; set; }
        public string locale { get; set; }
        public List<string> aliases { get; set; }
        public string postalCode { get; set; }
        public string timezone { get; set; }
        public CityModel city { get; set; }
        public State state { get; set; }
        public CountryModel country { get; set; }
        public Address address { get; set; }
        public Location location { get; set; }
        public List<Market> markets { get; set; }
        public List<Dma> dmas { get; set; }
        public UpcomingEvents upcomingEvents { get; set; }
        public Links _links { get; set; }
    }

}
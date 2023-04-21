using System.Collections.Generic; 
namespace Social.Services.ModelView{ 

    public class Event
    {
        public string name { get; set; }
        public string type { get; set; }
        public string id { get; set; }
        public bool test { get; set; }
        public string url { get; set; }
        public string locale { get; set; }
        public List<Image> images { get; set; }
        public Sales sales { get; set; }
        public Dates dates { get; set; }
        public List<Classification> classifications { get; set; }
        public Promoter promoter { get; set; }
        public List<Promoter> promoters { get; set; }
        public List<PriceRange> priceRanges { get; set; }
        public Seatmap seatmap { get; set; }
        public Accessibility accessibility { get; set; }
        public TicketLimit ticketLimit { get; set; }
        public AgeRestrictions ageRestrictions { get; set; }
        public Ticketing ticketing { get; set; }
        public Links _links { get; set; }
        public Embedded _embedded { get; set; }
    }

}
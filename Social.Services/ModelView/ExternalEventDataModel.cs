using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Services.ModelView
{
    public class ExternalEventDataModel
    {
        public int error { get; set; }
        public int totalcount { get; set; }
        public int pagecount { get; set; }
        public List<ExternalEventData> results { get; set; }
        
    }
    public class ExternalEventData
    {
        public string id { get; set; }
        public string EventCode { get; set; }
        public string eventname { get; set; }
        public string xlargeimageurl { get; set; }
        public string link { get; set; }
        public DateTime date { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public string description { get; set; }
        public venue venue { get; set; }
        public openingtimes openingtimes { get; set; }
        public int? categorieId { get; set; }
        public string SubCategoriesIds { get; set; }
        public string status { get; set; }
        public int goingtos { get; set; }
        public int? eventTypeListid { get; set; }
        public bool? allday { get; set; }
        public bool? showAttendees { get; set; }
        public DateTime? StopFrom { get; set; }
        public DateTime? StopTo { get; set; }
        public int? CityID { get; set; }
        public int? CountryID { get; set; }
        public int cancelled { get; set; }
        public string ticketUrl { get; set; }

    }

    public class venue
    {
        public string id { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
    }
    public class openingtimes
    {
        public TimeSpan doorsopen { get; set; }
        public TimeSpan doorsclose { get; set; }        
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Services.ModelView
{
    public class AddExternalEventDataModel
    {
        public int? categorieId { get; set; }
        public string Title { get; set; }
        public string description { get; set; }
        public string status { get; set; }// set by Null if it from External Api
        public string image { get; set; }
        public string longitude { get; set; }
        public string checkout_details { get; set; }
        public string categorie { get; set; }
        public string latitude { get; set; }
        public int totalnumbert { get; set; }// number of attendence
        public int? EventTypeListid { get; set; }
        public bool? allday { get; set; }
        public bool? showAttendees { get; set; } //   true if you creator
        public DateTime? StopFrom { get; set; } //  => next setting
        public DateTime? StopTo { get; set; }//  => next setting
        public DateTime? eventdate { get; set; }
        public DateTime? eventdateto { get; set; }
        public TimeSpan? timefrom { get; set; }
        public TimeSpan? timeto { get; set; }
        public int? CityID { get; set; }
        public int? CountryID { get; set; }
    }
}

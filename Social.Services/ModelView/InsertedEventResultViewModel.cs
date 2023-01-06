using System;
using System.Collections.Generic;

using System.Text;

namespace Social.Services.ModelView
{
    public class InsertedEventResultViewModel
    {
        public int Total_File_Events { get; set; }
        public int Failed_Events { get; set; }
        public int Inserted_Events { get; set; }
        public int Updated_Events { get; set; }
        public EventMetaData EventMetaData { get; set; }

    }
    public class EventMetaData
    {
        public int EmptyCountDescription { get; set; }
        public int EmptyCountTitle { get; set; }
        public int EmptyCountAllDayNull { get; set; }
        public int EmptyCountEventDateOldThanEventDateto { get; set; }
        public int EmptyCountEventlongitudeLatitude { get; set; }
        public int EventDateOrEventDateToNull { get; set; }
        public int EventTimeFromToNull { get; set; }
        public int EventCheckoutDetailsNull { get; set; }
        public int EventDescriptionLengthLargerConfig { get; set; }
        public int EventTitleLengthLargerConfig { get; set; }


    }
}

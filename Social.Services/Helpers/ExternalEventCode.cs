using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Social.Services.Helpers
{   

    public static class ExternalEventCategory
    {
        public static string[] LiveAndClub { get; set; } = { "LIVE", "CLUB" }; // return 1
        public static string[] DateAndKids { get; set; } = { "DATE", "KIDS" }; // return 16
        public static string[] THEATRE { get; set; } = { "THEATRE" }; // return 20
        public static string[] COMEDY { get; set; } = { "COMEDY"}; // return 21
        public static string[] EXHIB { get; set; } = { "EXHIB" }; // return 23
        public static string[] BARPUB { get; set; } = { "BARPUB" }; // return 9
        public static string[] SPORT { get; set; } = { "SPORT" }; // return 22
        public static string[] ARTS { get; set; } = { "ARTS" }; // return 18
        public static string[] FEST { get; set; } = { "FEST" }; // return 19


        public static int GetCategoryId(this string eventCategory)
        {
            if(LiveAndClub.Contains(eventCategory))
            {
                return 1;
            }
            if (DateAndKids.Contains(eventCategory))
            {
                if (eventCategory.Contains("DATE"))
                {
                    return 0;
                }
                return 16;
            }
            if (THEATRE.Contains(eventCategory))
            {
                return 20;
            }
            if (COMEDY.Contains(eventCategory))
            {
                return 21;
            }
            if (EXHIB.Contains(eventCategory))
            {
                return 23;
            }
            if (BARPUB.Contains(eventCategory))
            {
                return 9;
            }
            if (SPORT.Contains(eventCategory))
            {
                return 22;
            }
            if (ARTS.Contains(eventCategory))
            {
                return 18;
            }
            if (FEST.Contains(eventCategory))
            {
                return 19;
            }
            return 16;
        }
    }
}

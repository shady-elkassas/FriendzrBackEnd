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
        /// <summary>
        /// This Function Returns Categories Ids in String with comma separated Based on below 
        /// FEST    => Food & Drink, Music, Creative/Art, Theatre/Film, Dance
        /// THEATRE => Theatre/Film, Dance
        /// EXHIB   => Exhibition, Creative/Art, Community, Food &amp; Drink, Talks/Lectures, Classes/Workshops
        /// SPORT   => Sports/Fitness, Walks/Tours
        /// ARTS    => Creative/Art, Theatre/Film, Dance,Talks/Lectures, Classes/Workshops
        /// </summary>
        /// <param name="eventCategory"></param>
        /// <returns></returns>
        public static string GetSubCategoriesIds(this string eventCategory)
        {
            if (FEST.Contains(eventCategory))
            {
                return "37f61089-dd3f-4ba9-85d5-427e11102f5f,2aa61561-0133-4bbe-aa18-02114285b287,704f16db-99d5-4145-81fb-bf577ac804e5,64617179-d340-48a6-85b3-d4a81d5c9360,752bb03b-c9a3-4cca-a869-65d9207127d7";
            }
            if (THEATRE.Contains(eventCategory))
            {
                return "37f61089-dd3f-4ba9-85d5-427e11102f5f,752bb03b-c9a3-4cca-a869-65d9207127d7";
            }
            if (EXHIB.Contains(eventCategory))
            {
                return "2c942955-fdbe-4be4-9200-d33d0ff98a04,64617179-d340-48a6-85b3-d4a81d5c9360,d025ce3f-a414-46d2-bea1-7da5850a5f57,2aa61561-0133-4bbe-aa18-02114285b287,8b72b1f3-4f5c-473b-a033-75b9ee82a2ae,ba00941e-45f7-45c0-8a32-21abf5f0d11e";
            }
            if (SPORT.Contains(eventCategory))
            {
                return "7dd3d9bf-6dd6-4c68-ae35-754f4c6c52a8,fce54fd8-8363-4f0e-88fa-7377852d0317";
            }
            if (ARTS.Contains(eventCategory))
            {
                return "64617179-d340-48a6-85b3-d4a81d5c9360,37f61089-dd3f-4ba9-85d5-427e11102f5f,752bb03b-c9a3-4cca-a869-65d9207127d7,8b72b1f3-4f5c-473b-a033-75b9ee82a2ae,ba00941e-45f7-45c0-8a32-21abf5f0d11e";
            }
            return string.Empty;
        }
        public static string GetSubCategoriesIdsForUpdate(this int? eventCategory)
        {
            if (eventCategory== 19)
            {
                return "37f61089-dd3f-4ba9-85d5-427e11102f5f,2aa61561-0133-4bbe-aa18-02114285b287,704f16db-99d5-4145-81fb-bf577ac804e5,64617179-d340-48a6-85b3-d4a81d5c9360,752bb03b-c9a3-4cca-a869-65d9207127d7";
            }
            if (eventCategory == 20)
            {
                return "37f61089-dd3f-4ba9-85d5-427e11102f5f,752bb03b-c9a3-4cca-a869-65d9207127d7";
            }
            if (eventCategory == 23)
            {
                return "2c942955-fdbe-4be4-9200-d33d0ff98a04,64617179-d340-48a6-85b3-d4a81d5c9360,d025ce3f-a414-46d2-bea1-7da5850a5f57,2aa61561-0133-4bbe-aa18-02114285b287,8b72b1f3-4f5c-473b-a033-75b9ee82a2ae,ba00941e-45f7-45c0-8a32-21abf5f0d11e";
            }
            if (eventCategory == 22)
            {
                return "7dd3d9bf-6dd6-4c68-ae35-754f4c6c52a8,fce54fd8-8363-4f0e-88fa-7377852d0317";
            }
            if (eventCategory == 18)
            {
                return "64617179-d340-48a6-85b3-d4a81d5c9360,37f61089-dd3f-4ba9-85d5-427e11102f5f,752bb03b-c9a3-4cca-a869-65d9207127d7,8b72b1f3-4f5c-473b-a033-75b9ee82a2ae,ba00941e-45f7-45c0-8a32-21abf5f0d11e";
            }
            return string.Empty;
        }
    }
}

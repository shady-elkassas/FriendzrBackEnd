using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.Models
{
    public class EventVM
    {

        public bool IsFavorite { get; set; }
        public string description { get; set; }
        public string categorie { get; set; }
        public string categorieimage { get; set; }
        public double DistanceBetweenLocationAndEvent { get; set; }
        public string lat { get; set; }
        public string lang { get; set; }
        public string Id { get; set; }
        public int OrderByDes { get; set; }
        public string eventdate { get; set; }
        public string eventtype { get; set; }
        public Guid eventtypeid { get; set; }
        public string eventtypecolor { get; set; }
        public string eventdateto { get; set; }
        public bool allday { get; set; }
        public string timefrom { get; set; }
        public string timeto { get; set; }
        public string Title { get; set; }
        public int joined { get; set; }
        public string image { get; set; }
        public string UserImage { get; set; }
        public string EventTypeName { get; set; }

        public int totalnumbert { get; set; }
        public int key { get; set; }
        public bool bloked { get; set; }
        public string color { get; set; }
        public string eventColor { get; set; }

    }
}

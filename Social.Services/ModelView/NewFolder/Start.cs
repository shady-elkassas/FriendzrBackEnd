using System; 
namespace Social.Services.ModelView{ 

    public class Start
    {
        public string localDate { get; set; }
        public string localTime { get; set; }
        public DateTime dateTime { get; set; }
        public bool dateTBD { get; set; }
        public bool dateTBA { get; set; }
        public bool timeTBA { get; set; }
        public bool noSpecificTime { get; set; }
    }

}
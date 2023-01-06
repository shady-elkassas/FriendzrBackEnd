using System;
namespace Social.Services.ModelView
{
    public class StatisticsByGenderAndAgeViewModel
    {
        public object All { get; set; }
        public object Male { get; set; }
        public object Female { get; set; }
        public object Other { get; set; }
        public object From18To25 { get; set; }
        public object From25To34 { get; set; }
        public object From35To44 { get; set; }
        public object From45To54 { get; set; }
        public object From55To64 { get; set; }
        public object From65AndMore { get; set; }
    }
}

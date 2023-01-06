using System.Collections.Generic;

namespace Social.Services.ModelView
{

    public class ExportStatisticsByGenderAndAgeViewModel
    {
        public List<StatisticsByGenderViewModel> StatisticsByGender { get; set; } = new List<StatisticsByGenderViewModel>();
        public List<StatisticsByAgeViewModel> StatisticsByAge { get; set; } = new List<StatisticsByAgeViewModel>();

        public List<StatisticsByGenderViewModel> StatisticsByGenderPerDay { get; set; } = new List<StatisticsByGenderViewModel>();
        public List<StatisticsByAgeViewModel> StatisticsByAgePerDay { get; set; } = new List<StatisticsByAgeViewModel>();
    }

    public class StatisticsByGenderViewModel
    {
        public string Month { get; set; }
        public string Day { get; set; }
        public string Date { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public int Other { get; set; }

    }

    public class StatisticsByAgeViewModel
    {
        public string Month { get; set; }
        public string Day { get; set; }
        public string Date { get; set; }
        public int From18To24 { get; set; }
        public int From25To34 { get; set; }
        public int From35To44 { get; set; }
        public int From45To54 { get; set; }
        public int From55To64 { get; set; }
        public int MoreThan65 { get; set; }

    }

}
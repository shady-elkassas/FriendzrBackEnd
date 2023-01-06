using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Services.ModelView
{
  public  class UpdateSettingsVM
    {
        public string MyAppearanceTypes { get; set; }
        public bool? personalSpace { get; set; }
        public bool? Filteringaccordingtoage { get; set; }
        public bool? distanceFilter { get; set; }
        public int? ageto { get; set; }
        public int? agefrom { get; set; }
        public decimal? Manualdistancecontrol { get; set; }
        public string language { get; set; }
        public string whatAmILookingFor { get; set; }
        public bool? ghostmode { get; set; }
        public bool? allowmylocation { get; set; }
        public bool? pushnotification { get; set; }

    }
}

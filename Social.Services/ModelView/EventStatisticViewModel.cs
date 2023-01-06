using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Services.ModelView
{
    public class EventStatisticViewModel
    {
        public int All { get; set; }
        public int Friendzr { get; set; }
        public double Rate { get { return Math.Round((((double)Friendzr / (double)All) * 100), 2); } }
    }
}

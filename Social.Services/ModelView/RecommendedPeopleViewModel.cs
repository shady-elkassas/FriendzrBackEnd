using System.Collections.Generic;

namespace Social.Services.ModelView
{
    public class RecommendedPeopleViewModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public bool? ImageIsVerified { get; set; }
        public string Image { get; set; }
        public decimal InterestMatchPercent { get; set; }
        public double DistanceFromYou { get; set; }
        public List<string> MatchedInterests { get; set; }

    }

}

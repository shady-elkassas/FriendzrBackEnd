using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Services.ModelView
{
    public class UserStatistics
    {
        public int RequestesCount { get; set; }
        public int BlockRequestesCount { get; set; }
        public int AcceptedRequestesCount { get; set; }
        public int PendindingRequestesCount { get; set; }
        public double UseresAverageAge { get; set; }
        public int deactivatespushnotifications_Count { get; set; }
        public int NotactiveUsers_Count { get { return Updated - ActiveUsers_Count; } } 
        public int UseresStillUseAppAfter3Months_Count { get; set; }
        public int UseresEnableGhostMode_Count { get; set; }
        public int TotalInAppearanceTypes { get; set; }
        public int AppearenceMaleInGhostMode_Count { get; set; }

        public double AppearenceMaleInGhostMode_Rate
        {
            get
            {
                var num = Math.Round((((double)AppearenceMaleInGhostMode_Count / (double)TotalInAppearanceTypes) * 100), 2);
                return double.IsNaN(num) ? 0 : num;

            }
        }

        public int AppearenceFemaleInGhostMode_Count { get; set; }

        public double AppearenceFemaleInGhostMode_Rate
        {
            get
            {
                var num = Math.Round((((double)AppearenceFemaleInGhostMode_Count / (double)TotalInAppearanceTypes) * 100), 2);
                return double.IsNaN(num) ? 0 : num;
            }
        }

        public int AppearenceEveryOneInGhostMode_Count { get; set; }
        public double AppearenceEveryOneInGhostMode_Rate
        {
            get
            {
                var num = Math.Round((((double)AppearenceEveryOneInGhostMode_Count / (double)TotalInAppearanceTypes) * 100), 2);
                return double.IsNaN(num) ? 0 : num;

            }
        }
        public int AppearenceOtherGenderInGhostMode_Count { get; set; }
        public double AppearenceOtherGenderInGhostMode_Rate
        {
            get
            {
                var num = Math.Round((((double)AppearenceOtherGenderInGhostMode_Count / (double)TotalInAppearanceTypes) * 100), 2);
                return double.IsNaN(num) ? 0 : num;
            }
        }

        public int ActiveUsers_Count { get; set; }
        public double NotactiveUsers_Rate { get { return Math.Round((((double)NotactiveUsers_Count / (double)Updated) * 100), 2); } }
        public double ActiveUsers_Rate { get { return Math.Round((((double)ActiveUsers_Count / (double)Updated) * 100), 2); } }
        public double UseresEnableGhostModeRate_Rate { get { return Math.Round((((double)UseresEnableGhostMode_Count / (double)Updated) * 100), 2); } }

        public int CurrenUsers_Count { get; set; }

        public int ConfirmedMailUsers_Count { get; set; }
        public int UnConfirmedMailUsers_Count { get; set; }
        public int DeletedUsers_Count { get; set; }
        public int TotalUserUsedAgeFiltring { get; set; }
        public string MostAgeFiltirngRangeUsed { get; set; }
        public double MostAgeFiltirngRangeUsed_Rate { get; set; }
        public int TotalUsers_Count { get { return CurrenUsers_Count; } }
        public int UsersVertified_Rate { get { return (int)(((double)ConfirmedMailUsers_Count / (double)CurrenUsers_Count) * 100); } }
        public int UsersUnVertified_Rate { get { return (int)(((double)UnConfirmedMailUsers_Count / (double)CurrenUsers_Count) * 100); } }
        public int Male_Count { get; set; }
        public int UserWithLessThan18Age_Count { get; set; }
        public int UsersWith18_24Age_Count { get; set; }
        public int UsersWith25_34Age_Count { get; set; }
        public int UsersWith35_54Age_Count { get; set; }
        public int UsersWithMoreThan55Age_Count { get; set; }
        public double Male_Rate { get { var num= Math.Round((((double)Male_Count / (double)Updated) * 100), 2);
                return double.IsNaN(num) ? 0 : num;
            } }
        public double UserWithLessThan18Age_Rate { get { return Math.Round((((double)UserWithLessThan18Age_Count / (double)Updated) * 100), 2); } }
        public double UsersWith18_24Age_Rate { get { return Math.Round((((double)UsersWith18_24Age_Count / (double)Updated) * 100), 2); } }
        public double UsersWith25_34Age_Rate { get { return Math.Round((((double)UsersWith25_34Age_Count / (double)Updated) * 100), 2); } }
        public double UsersWith35_54Age_Rate { get { return Math.Round((((double)UsersWith35_54Age_Count / (double)Updated) * 100), 2); } }
        public double UsersWithMoreThan55Age_Rate { get { return Math.Round((((double)UsersWithMoreThan55Age_Count / (double)Updated) * 100), 2); } }
        public int Othergender_Count { get { return (Updated) - (Female_Count + Male_Count); } }
        public double Othergender_Rate { get { var num=(Male_Rate == 0 && Female_Rate == 0)?0: Math.Round(100 - Male_Rate - Female_Rate, 2);
                return double.IsNaN(num) ? 0 : num;
            } }
        public int Female_Count { get; set; }
        public int NeedUpdate { get; set; }
        public int Updated { get; set; }
        public int personalspace { get; set; }
        public double UsersWithPersonalSpaceEnabled_Rate { get { return (int)(((double)personalspace / (double)Updated) * 100); } }
        public int PushNotificationsEnabled_Count { get; set; }
        public double UsersWithPushNotificationsEnabled_Rate { get { return (int)(((double)PushNotificationsEnabled_Count / (double)Updated) * 100); } }
        public int DeletedProfiles_Count { get; set; }
        public double DeletedProfiles_Rate { get { return (CurrenUsers_Count == 0 && DeletedProfiles_Count > 0) ? 100 : Math.Round((((double)DeletedProfiles_Count / (double)CurrenUsers_Count) * 100),2); } }

        public double Female_Rate { get {var num = Math.Round((((double)Female_Count / (double)Updated) * 100), 2);
                return double.IsNaN(num) ? 0 : num;
            }  }
    }
}

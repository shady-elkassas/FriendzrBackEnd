using GoogleMaps.LocationServices;
using Social.Services.ModelView;
using Social.Services.Services;
using System;

using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Implementation
{
    public class LocationService : IGoogleLocationService
    {
       const string Key = "AIzaSyCLmWYc00w0KZ-qj8hIymWCIs8K5Z0cG0g";
        public GoogleLocationService GoogleLocationService { get; set; }
        public LocationService()
        {
            GoogleLocationService = new GoogleLocationService(Key);
        }
        public async Task<AddressData> GetAddressData(double latitude, double longitude)
        {
            var task= await Task.Run(() => {return  GoogleLocationService.GetAddressFromLatLang(latitude, longitude); });
            return task;
        }


        public AddressData GetAddressDataNotAsync(double latitude, double longitude)
        {
            return GoogleLocationService.GetAddressFromLatLang(latitude, longitude);

        }

        public List<AddressData> GetListAddressDataNotAsync(List<LatAndlongListViewModel> latAndlongList)
        {
            List<AddressData> locations = new List<AddressData>();  

            foreach (var location in latAndlongList)
            {
                locations.Add(GoogleLocationService.GetAddressFromLatLang(location.Latitude, location.Longitude));
            }
            return locations;

        }

        //  Accurate (Tested) ✔
        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }

            return dist;
        }

    }
}

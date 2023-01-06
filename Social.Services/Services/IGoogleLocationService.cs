using GoogleMaps.LocationServices;
using Social.Services.ModelView;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{
    public interface IGoogleLocationService
    {
        Task<AddressData> GetAddressData(double latitude, double longitude);
        AddressData GetAddressDataNotAsync(double latitude, double longitude);
        double CalculateDistance(double lat1, double lon1, double lat2, double lon2, char unit);
       
    }
}

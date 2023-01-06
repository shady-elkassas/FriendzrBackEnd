using Microsoft.AspNetCore.Mvc;
using Social.Entity.DBContext;
using Social.Services.ModelView;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Areas.Admin.Controllers.ViewComponents
{
    public class EventsOnGoogleMapViewComponent : ViewComponent
    {

        private readonly AuthDBContext authDBContext;

        public EventsOnGoogleMapViewComponent(AuthDBContext authDBContext)
        {

            this.authDBContext = authDBContext;
        }
        public IViewComponentResult Invoke()
        {
            var allusereswithvalidlocation = authDBContext.EventData.Where(x => x.IsActive == true && (x.StopFrom != null ? (x.StopFrom.Value >= DateTime.Now.Date || x.StopTo.Value <= DateTime.Now.Date) : true)
                    && x.lang != null && x.lat != null && Convert.ToDouble(x.lang) <= 90 && Convert.ToDouble(x.lang) >= -90 && Convert.ToDouble(x.lat) <= 90 && Convert.ToDouble(x.lat) >= -90);
            var Users_GeoCoordinate = allusereswithvalidlocation.Select(x => new GeoCoordinate
            {
                Latitude = Convert.ToDouble(x.lat),
                Longitude = Convert.ToDouble(x.lang),

            }).ToList();

            var centerpoint = GetCentralGeoCoordinate(Users_GeoCoordinate);
            ViewBag.CenterLatitude = centerpoint.Latitude;
            ViewBag.CenterLongitude = centerpoint.Longitude;
            return View(allusereswithvalidlocation.Select(x => new GoogleMapMarker
            {
                lat = Convert.ToDouble(x.lat),
                lng = Convert.ToDouble(x.lang),
                DisplayName = x.Title,
            }).ToList());
        }
        GeoCoordinate GetCentralGeoCoordinate(
IList<GeoCoordinate> geoCoordinates)
        {
            if (geoCoordinates.Count == 1)
            {
                return geoCoordinates.Single();
            }

            double x = 0;
            double y = 0;
            double z = 0;

            foreach (var geoCoordinate in geoCoordinates)
            {
                var latitude = geoCoordinate.Latitude * Math.PI / 180;
                var longitude = geoCoordinate.Longitude * Math.PI / 180;

                x += Math.Cos(latitude) * Math.Cos(longitude);
                y += Math.Cos(latitude) * Math.Sin(longitude);
                z += Math.Sin(latitude);
            }

            var total = geoCoordinates.Count;

            x = x / total;
            y = y / total;
            z = z / total;

            var centralLongitude = Math.Atan2(y, x);
            var centralSquareRoot = Math.Sqrt(x * x + y * y);
            var centralLatitude = Math.Atan2(z, centralSquareRoot);

            return new GeoCoordinate(centralLatitude * 180 / Math.PI, centralLongitude * 180 / Math.PI);
        }
    }
}

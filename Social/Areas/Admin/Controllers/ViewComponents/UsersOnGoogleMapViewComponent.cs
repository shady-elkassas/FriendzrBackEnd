using Microsoft.AspNetCore.Mvc;
using Social.Entity.DBContext;
using Social.Services.ModelView;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;

namespace Social.Areas.Admin.Controllers.ViewComponents
{
    public class UsersOnGoogleMapViewComponent : ViewComponent
    {

        private readonly AuthDBContext authDBContext;
        public UsersOnGoogleMapViewComponent(AuthDBContext authDBContext)
        {

            this.authDBContext = authDBContext;
        }
       
        public IViewComponentResult Invoke()
        {
            var allusereswithvalidlocation = authDBContext.Users.Where(x => x.UserDetails != null && x.UserDetails.lang != null && x.UserDetails.lat != null).Select(x => new
            {
                x.UserDetails.lang,
                x.UserDetails.lat,
                x.DisplayedUserName
            }).ToList();
           
            var allEventsswithvalidlocation = authDBContext.EventData.Where(x => x.lang != null && x.lat != null && (x.eventdateto.Value.Date > DateTime.Now.Date || (x.eventdateto.Value.Date == DateTime.Now.Date && x.eventto > DateTime.Now.TimeOfDay) || (x.allday == true && x.eventdate.Value.Date == DateTime.Now.Date))).Select(x => new
            {
                x.lat,
                x.lang,
                x.Title
            }).ToList();

            List<GeoCoordinate> Users_GeoCoordinate = allusereswithvalidlocation.Select(x =>
            {
                if (!double.TryParse(x.lang, out double Lat) || !double.TryParse(x.lang, out double lang)) return null;
                if (Lat <= 90 || lang >= -90 || lang <= 90 || Lat >= -90) return null;

                return new GeoCoordinate
                {
                    Latitude = Lat,
                    Longitude = lang,

                };
            }).ToList();

            Users_GeoCoordinate = Users_GeoCoordinate.Where(x => x != null).Where(x => x.Longitude <= 90 && x.Longitude >= -90 && x.Latitude <= 90 && x.Latitude >= -90).ToList();

            List<GeoCoordinate> Events_GeoCoordinate = allEventsswithvalidlocation.ToList().Select(x =>
            {
                if (!double.TryParse(x.lat, out double Lat) || !double.TryParse(x.lang, out double lang)) return null;
                if (Lat <= 90 || lang >= -90 || lang <= 90 || Lat >= -90) return null;
                return new GeoCoordinate
                {
                    Latitude = Lat,
                    Longitude = lang,

                };
            }).Where(x => x != null).Where(x => x.Longitude <= 90 && x.Longitude >= -90 && x.Latitude <= 90 && x.Latitude >= -90).ToList();

            GeoCoordinate centerPoint = GetCentralGeoCoordinate(Users_GeoCoordinate);
            GeoCoordinate eventsCenterPoint = GetCentralGeoCoordinate(Events_GeoCoordinate);

            ViewBag.CenterLatitude = centerPoint.Latitude;
            ViewBag.CenterLongitude = centerPoint.Longitude;
            ViewBag.EventsCenterLatitude = eventsCenterPoint.Latitude;
            ViewBag.EventsCenterLongitude = eventsCenterPoint.Longitude;

            ViewBag.EventsGoogleMapMarker = allEventsswithvalidlocation.Select(x =>
            {
                if (!double.TryParse(x.lat, out double Lat) || !double.TryParse(x.lang, out double lang)) return null;

                return new GoogleMapMarker
                {
                    lat = Lat,
                    lng = lang,
                    DisplayName = x.Title
                };
            }).Where(x => x != null).ToList();

            List<GoogleMapMarker> Data = allusereswithvalidlocation.Select(x =>
            {
                if (!double.TryParse(x.lat, out double Lat) || !double.TryParse(x.lang, out double lang)) return null;

                return new GoogleMapMarker
                {
                    lat = Lat,
                    lng = lang,
                    DisplayName = x.DisplayedUserName
                };
            }).Where(x => x != null).ToList();
            return View(Data);
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

            foreach (GeoCoordinate geoCoordinate in geoCoordinates)
            {
                double latitude = geoCoordinate.Latitude * Math.PI / 180;
                double longitude = geoCoordinate.Longitude * Math.PI / 180;

                x += Math.Cos(latitude) * Math.Cos(longitude);
                y += Math.Cos(latitude) * Math.Sin(longitude);
                z += Math.Sin(latitude);
            }

            int total = geoCoordinates.Count;
            total = total == 0 ? 1 : total;

            x = x / total;
            y = y / total;
            z = z / total;

            double centralLongitude = Math.Atan2(y, x);
            double centralSquareRoot = Math.Sqrt(x * x + y * y);
            double centralLatitude = Math.Atan2(z, centralSquareRoot);

            return new GeoCoordinate(centralLatitude * 180 / Math.PI, centralLongitude * 180 / Math.PI);
        }
    }
}

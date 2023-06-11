using Kursach_MVC.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kursach_MVC.Areas.Admin.Controllers
{
    public class MapMarker
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public List<MapMarker> GetMarkers()
        {
            using (Db db = new Db())
            {
                var markers = db.CoordinateRest.Select(x => new MapMarker { Latitude = x.latitude, Longitude = x.longitude }).ToList();
               

                return markers;

               
            }
        }
    }
}
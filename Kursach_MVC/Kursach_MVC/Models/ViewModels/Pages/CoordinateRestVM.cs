using Kursach_MVC.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Kursach_MVC.Models.ViewModels.Pages
{
    public class CoordinateRestVM
    {
        public CoordinateRestVM() {}

        public CoordinateRestVM(CoordinateRestDTO row)
        {
            Id = row.Id;
            IdRest = row.IdRest;
            latitude = row.latitude;
            longitude = row.longitude;
        }

        public int Id { get; set; }

        public int IdRest { get; set; }

        public double latitude { get; set; }

        public double longitude { get; set; }

        public virtual RestaurantVM Restaraunt { get; set; }
    }
}
using Kursach_MVC.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Kursach_MVC.Models.ViewModels.Pages
{
    public class RestaurantVM
    {

        public RestaurantVM() { }


        public RestaurantVM(RestaurantDTO row)
        {
            Id = row.Id;
            name = row.Name;
            address = row.Address;
            latitude = row.latitude;
            longitude = row.longitude;
            numberPhone = row.NumberPhone;
            idTag = row.idTag;
            workingHours = row.WorkingHours;
            sorting = row.Sorting;
        }

        public virtual ICollection<CoordinateRestVM> Coordinates { get; set; }
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string name { get; set; }
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        public string address { get; set; }

        public double latitude { get; set; }

        public double longitude { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        public string numberPhone { get; set; }
        
       
        public string idTag { get; set; }

        public string workingHours { get; set; }

        public int sorting { get; set; }
    }
}
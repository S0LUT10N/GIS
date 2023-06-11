using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Kursach_MVC.Models.Data
{
    [Table("tblRestaurant")]
    public class RestaurantDTO
    {
        [Key]
        public int Id { get; set; }


        public string Name { get; set; }

        public string Address { get; set; }

        public double latitude { get; set; }

        public double longitude { get; set; }

        public string NumberPhone { get; set; }

        public string idTag { get; set; }

        public string WorkingHours { get; set; }

        public int Sorting { get; set; }


    }
}
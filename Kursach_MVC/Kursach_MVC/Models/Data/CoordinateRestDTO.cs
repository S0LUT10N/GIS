using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Kursach_MVC.Models.Data
{
    [Table("tblCoordinateRest")]
    public class CoordinateRestDTO
    {
        [Key]

        public int Id { get; set; }

        public int IdRest { get; set; }

        public double latitude { get; set; }

        public double longitude { get; set; }

        [ForeignKey("IdRest")]
        public virtual RestaurantDTO rest { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Kursach_MVC.Models.Data
{
    [Table("tblOrderDetails")]
    public class OrderDetailsDTO
    {
        [Key] 
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public string RestaurantName { get; set; }
        public int RestaurantId { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("OrderId")]
        public virtual OrderDTO Orders { get; set; }

        [ForeignKey("UserId")]
        public virtual UserDTO Users { get; set; }

        [ForeignKey("ProductId")]
        public virtual ProductDTO Products { get; set; }

        [ForeignKey("RestaurantId")]
        public virtual RestaurantDTO Restaurant { get; set; }
    }
}
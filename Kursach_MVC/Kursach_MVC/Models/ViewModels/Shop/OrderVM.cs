using Kursach_MVC.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Kursach_MVC.Models.ViewModels.Shop
{
    public class OrderVM
    {
        public OrderVM()
        {
        }

        public OrderVM(OrderDTO row)
        {
            OrderId = row.OrderId;
            UserId = row.UserId;
            CreatedAt = row.CreatedAt;
            RestaurantId = row.RestaurantId;
        }

        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public int RestaurantId { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Kursach_MVC.Models.Data
{
    // Урок 11
    [Table("tblProducts")]
    public class ProductDTO
    {
        public int Id { get; set; }

        
        public int RestarauntId { get; set; }

        public string RestarauntName { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string ImageName { get; set; }

        // Назначаем внешний ключ
        [ForeignKey("CategoryId")]
        public virtual CategoryDTO Category { get; set; }

        [ForeignKey("RestarauntId")]

        public virtual RestaurantDTO Restaurant { get; set; }
    }
}
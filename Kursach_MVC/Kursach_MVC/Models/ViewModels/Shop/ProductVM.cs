using Kursach_MVC.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Kursach_MVC.Models.ViewModels.Shop
{
    //Урок 11
    public class ProductVM
    {
        public ProductVM() { }

        public ProductVM(ProductDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            RestarauntId = row.RestarauntId;
            RestarauntName = row.RestarauntName;
            Slug = row.Slug;
            Description = row.Description;
            Price = row.Price;
            CategoryName = row.CategoryName;
            CategoryId = row.CategoryId;
            ImageName = row.ImageName;
        }

        public int Id { get; set; }

        
        public int RestarauntId { get; set; }
        [DisplayName("Restaraunt")]
        public string RestarauntName { get; set; }

        [Required]
        public string Name { get; set; }
        public string Slug { get; set; }
        [Required]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        [DisplayName("Image")]
        public string ImageName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }

        public IEnumerable<SelectListItem> Restaraunts { get; set; }
    }
}
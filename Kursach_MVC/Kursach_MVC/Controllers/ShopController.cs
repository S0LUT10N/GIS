using Kursach_MVC.Models.Data;
using Kursach_MVC.Models.ViewModels.Cart;
using Kursach_MVC.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace Kursach_MVC.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }



        public ActionResult CategoryMenuPartial()
        {
            // Объявляем модель типа List<> CategoryVM
            List<CategoryVM> categoryVMList;

            // Инициализируем модель данными
            using (Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x))
                    .ToList();
            }

            // Возвращаем частичное представление с моделью
            return PartialView("_CategoryMenuPartial", categoryVMList);
        }

        public ActionResult GetProductsByRestaurant(int id)
        {
            Session["restaurantId"] = id;

            List<ProductVM> productVMList;

            using(Db db = new Db())
            {
                productVMList = db.Products
                    .ToArray()
                    .Where(x => x.RestarauntId == id)
                    .Select(x => new ProductVM(x))
                    .ToList();

               
            }
         

            return View(productVMList);
        }
        public ActionResult Category(string name)
        {
            if (Session["restaurantId"] != null)
            {
                int restaurantId = (int)Session["restaurantId"];


                // Объявляем список типа лист
                List<ProductVM> productVMList;

                using (Db db = new Db())
                {

                    // Получаем ID категории
                    CategoryDTO categoryDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();

                    int catId = categoryDTO.Id;



                    // Инициализируем список данными
                    productVMList = db.Products.ToArray()
                        .Where(x => x.CategoryId == catId && x.RestarauntId == restaurantId)
                        .Select(x => new ProductVM(x))
                        .ToList();

                    // Получаем имя категории
                    var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();

                    // Делаем проверку на NULL
                    if (productCat == null)
                    {
                        var catName = db.Categories.Where(x => x.Slug == name).Select(x => x.Name).FirstOrDefault();
                        ViewBag.CategoryName = catName;
                    }
                    else
                    {
                        ViewBag.CategoryName = productCat.CategoryName;
                    }
                }



                // Возвращаем представление с моделью
                return View(productVMList);
            } 
            else
            {
                return RedirectToAction("Index");
            }
        }


        // GET: Shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            // Объявляем модели DTO и VM
            ProductDTO dto;
            ProductVM model;

            // Инициализируем ID продукта
            int id = 0;

            using (Db db = new Db())
            {
                // Проверяем, доступен ли продукт
                if (!db.Products.Any(x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }

                // Инициализируем модель DTO данными
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                // Получаем ID
                id = dto.Id;

                // Инициализируем модель VM данными
                model = new ProductVM(dto);
            }
            // Получаем изображения из галереи
            model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            // Возвращаем модель в представление
            return View("ProductDetails", model);
        }
    }
}
using Kursach_MVC.Models.Data;
using Kursach_MVC.Models.ViewModels.Cart;
using Kursach_MVC.Models.ViewModels.Pages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Kursach_MVC.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {


        //Get: Admin/Pages/GetCoordinates
        public ActionResult AddToReport(int id)
        {
            ReportVM model = new ReportVM();

            using (Db db = new Db())
            {
                List<OrderDetailsDTO> orderDetails = db.OrderDetails
                .Where(x => x.RestaurantId == id)
                .OrderBy(x => x.RestaurantId)
                .ToList();

                // Сгруппируем детали заказов по идентификатору заказа (OrderId)
                var orders = orderDetails.GroupBy(x => x.OrderId);

                // Рассчитываем среднюю стоимость заказа и общую сумму заказов
                int  totalOrders = 0;
                decimal totalSales = 0m;

                foreach (var order in orders)
                {
                    // Увеличиваем количество заказов
                    totalOrders++;

                    // Суммируем стоимость всех заказов в группе
                    totalSales += order.Sum(x => x.Price * x.Quantity);
                }
                // Рассчитываем среднюю стоимость заказа
                if (totalOrders > 0)
                {
                    model.AvarageOrderPrice = totalSales / totalOrders;
                }
                // Определяем самый популярный продукт
                model.TheMostPopularProudct = orderDetails
                .GroupBy(x => x.ProductName)
                .OrderByDescending(x => x.Sum(y => y.Quantity))
                .First()
                .Key;



                // Рассчитываем количество заказов
                model.HowManyOrders = totalOrders;
            }

            return View("AddToReport", model);
        }

        public ActionResult CoordinateList()
        {
            List<CoordinateRestVM> restList;

            using (Db db = new Db())
            {
                restList = db.CoordinateRest.ToArray().OrderBy(x => x.Id).Select(x => new CoordinateRestVM(x)).ToList();

            }

            return View(restList);
        }

        public double[] CreateMark(string address)
        {
            // Ключ API для доступа к сервису Яндекс.Карт
            string apiKey = "fbede055-0a60-4238-96b8-3b44b6dfdb38";
            // Сформированный URL для запроса к сервису Яндекс.Карт

            string url = $"https://geocode-maps.yandex.ru/1.x/?apikey={apiKey}&geocode={address}&format=json";
            // Создание запроса с помощью URL-адреса и установка метода GET
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            // Получение ответа от сервиса Яндекс.Карт
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Получение потока данных из ответа
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            // Чтение текста ответа в строку
            string responseText = reader.ReadToEnd();

            // Закрытие читателя, потока и ответа
            reader.Close();
            responseStream.Close();
            response.Close();
            // Разбор полученного ответа в формате JSON
            // Извлечение координат точки из JSON и преобразование их в массив значений типа double
            JObject json = JObject.Parse(responseText);
            JToken point = json["response"]["GeoObjectCollection"]["featureMember"][0]["GeoObject"]["Point"]["pos"];
            string[] coordinates = point.ToString().Split(' ');
            double latitude = double.Parse(coordinates[1], CultureInfo.InvariantCulture);
            double longitude = double.Parse(coordinates[0], CultureInfo.InvariantCulture);
            double[] coordinatesAddres = new double[2];
            coordinatesAddres[0] = latitude;
            coordinatesAddres[1] = longitude;
            // Возвращение массива координат
            return coordinatesAddres;
        }
       
        // GET: Admin/Pages

        //Get
        public ActionResult DeleteRest(int id)
        {
            using (Db db = new Db())
            {
                //Получаем страницу
                RestaurantDTO dto = db.Restaurants.Find(id);

                CoordinateRestDTO restDTO = db.CoordinateRest.Find(id);
                //Удаляем страницу

                db.CoordinateRest.Remove(restDTO);
                db.Restaurants.Remove(dto);

                //Сохраняем изменения в базе
                db.SaveChanges();

            }

            //Добавляем сообщение об успешном удалении 
            TempData["SM"] = "You have deleteed a page!";
            //Переадресовываем пользователя
            return RedirectToAction("RestaurantList");

        }


        public ActionResult RestaurantList()
        {
            // Создаем список ресторанов
            List<RestaurantVM> restList;
            // Используем оператор using для автоматического освобождения ресурсов
            using (Db db = new Db())
            {
                // Извлекаем список всех ресторанов из базы данных
                restList = db.Restaurants.ToArray().OrderBy(x => x.Sorting).Select(x => new RestaurantVM(x)).ToList();

            }
            // Возвращаем представление, передавая список ресторанов в качестве модели представления
            return View(restList);
        }
      
     
        //Get: Admin/Rests/AddRest
        [HttpGet]

        public ActionResult AddRestautant()
        {
            return View();
        }


        //Post:Admin/Restaraunts/AddRest

        public ActionResult GetMap()
        {
            return View();
        }

        public ActionResult Map()
        {
            using(Db db = new Db())
            {
                var coordinates = db.CoordinateRest.ToList();
                return View(coordinates);

            }
        }
        [HttpPost]
        public ActionResult AddRestautant(RestaurantVM model)
        {
            //Проверка модели на валидность 
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            { 
                string address;

                RestaurantDTO dto = new RestaurantDTO();
                CoordinateRestDTO coordinateDTO = new CoordinateRestDTO();

                dto.Name = model.name.ToUpper();

                if (string.IsNullOrWhiteSpace(model.address))
                {
                    address = model.address;
                }
                else
                {
                    address = model.address;
                }
                //Убеждаемся, что заголовок и краткое описание - уникальны 
                if (db.Restaurants.Any(x => x.Address == model.address))
                {
                    ModelState.AddModelError("", "That address already exist");
                    return View(model);
                }
                else if (db.Restaurants.Any(x => x.NumberPhone == model.numberPhone))
                {
                    ModelState.AddModelError("", "That number already exist");
                    return View(model);
                }

                
                dto.Address = address;
               
                double[] coordinates = CreateMark(address);
                coordinateDTO.latitude = coordinates[0];
                coordinateDTO.longitude = coordinates[1];
                dto.NumberPhone = model.numberPhone;
                dto.idTag = model.idTag;
                dto.WorkingHours = model.workingHours;
                dto.latitude = coordinates[0];
                dto.longitude = coordinates[1];
                db.Restaurants.Add(dto);
                coordinateDTO.IdRest = model.Id;
                //Сохраняем модель в базу данных 
                db.CoordinateRest.Add(coordinateDTO);
               
                db.SaveChanges();
            }

            //Передаем сообщение через TempData
            TempData["SM"] = "Вы добавили новый ресторан!";


            //Переадресовываем пользователя на метод index 
            return RedirectToAction("RestaurantList");
        }
        
        public string ReverseAddressToCoordinate(string data)
        {
            string url = "https://snipp.ru/tools/address-coord";

           // string data = "address=Москва Красная площадь&submit=submit";

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string responseText = reader.ReadToEnd();

            Console.WriteLine(responseText);
            return (responseText);
  
 

        }
        [HttpGet]
        public ActionResult EditRestaurant(int id)
        {

            //Объявляем модель PageVM
            RestaurantVM model;

            using (Db db = new Db())
            {
                //Получаем страницу
                RestaurantDTO dto = db.Restaurants.Find(id);
                //Проверяем доступна ли страница 
                if (dto == null)
                {
                    return Content("The page does not exist.");
                }
                //Иницилизируем модель данными из dto 
                model = new RestaurantVM(dto);
            }

            //Возвращаем представление с моделью 
            return View(model);
        }
        

        //Get: Admin/Pages/EditRestaurant//id
        [HttpPost]
        public ActionResult EditRestaurant(RestaurantVM model)
        {
            //Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                //Получаем id страницы 
                int id = model.Id;
                //Объявляем переменную краткого заголовка
                string address = "home";
               
                //Получаем страницу по (ID)
                RestaurantDTO dto = db.Restaurants.Find(id);
                //Присваевыаем название из полученной модели в DTO
                dto.Name = model.name;
                //Проверяем краткий заголовок и присваиваем его, если это необходимо 
                if (model.address != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.address))
                    {
                        address = model.address.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        address = model.address.Replace(" ", "-").ToLower();
                    }
                }
                //Проверяем slug и title на уникальность
                if (db.Restaurants.Where(x => x.Id != id).Any(x => x.NumberPhone == model.numberPhone))
                {
                    ModelState.AddModelError("", "That numberPhone already exist.");
                    return View(model);
                }
                else if (db.Restaurants.Where(x => x.Id != id).Any(x => x.Address == address))
                {
                    ModelState.AddModelError("", "That address already exist.");
                    return View(model);
                }
                //Записываем остальные значения в класс DTO 
                dto.Name = model.name;
                dto.Address = address;
                dto.NumberPhone = model.numberPhone;
                dto.idTag = model.idTag;
                dto.WorkingHours = model.workingHours;
 
                //Сохраняем изменения в базу 
                db.SaveChanges();
            }

            //Устанавливаем сообщения в TempData
            TempData["SM"] = "Вы изменили ресторан!.";
            //Переадресация пользователя 
            return RedirectToAction("EditRestaurant");
        }

        [HttpGet]
        //Get: Admin/Pages/PageDetails/id
        public ActionResult RestDetails(int id)
        {
            //Объявляем модель PageVM
            RestaurantVM model;

            using (Db db = new Db())
            {
                RestaurantDTO dto = db.Restaurants.Find(id);
                //Подтверждаем что страница доступна 
                if (dto == null)
                {
                    return Content("The Restaurant does not exist.");
                }
                //Присваиваем модели информацию из базы
                model = new RestaurantVM(dto);
            }
            //Возвращаем модель в представление
            return View(model);
        }

       

        //Метод сортировки
        //Get Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {
                //Реализуем начальный счётчику 
                int count = 1;
                //Иницилизируем модель данных
                RestaurantDTO dto;
                //Устаналивливаем сортировку для каждой страницы 
                foreach(var pageId in id)
                {
                    dto = db.Restaurants.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }
            }
        }

        // GET: Admin/Pages
        public ActionResult Index()
        {
            //Объявляем список для представления (PageVM)
            List<PageVM> pageList;

            //Инициализируем список (DB)
            using (Db db = new Db())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            //Возвращаем список в представление
            return View(pageList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Проверка модели на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {

                //Объявляем переменную для краткого описания (slug)
                string slug;

                //Инициализируем класс PageDTO
                PagesDTO dto = new PagesDTO();

                //Присваеваем заголовок модели
                dto.Title = model.Title.ToUpper();

                //Проверяем, есть ли краткое описание, если нет, присваиваем его
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                //Убеждаемся, что заголовок и краткое описание - уникальны
                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title already exist.");
                    return View(model);
                }
                else if (db.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "That slug already exist.");
                    return View(model);
                }

                //Присваиваем оставшиеся значения модели
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                //Сохраняем модель в базу данных
                db.Pages.Add(dto);
                db.SaveChanges();

            }

            //Передаём сообщение через TempData
            TempData["SM"] = "You have added a new page!";

            //Переадресовываем пользователя на метод INDEX
            return RedirectToAction("Index");
        }

        // GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //Объявляем модель PageVM
            PageVM model;

            using (Db db = new Db())
            {
                //Получаем страницу 
                PagesDTO dto = db.Pages.Find(id);

                //Проверяем, доступна ли страница
                if (dto == null)
                {
                    return Content("The page does not exist.");
                }
                //Инициализируем модель данными
                model = new PageVM(dto);
            }

            //Возвращаем модель в представление
            return View(model);
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                //Получаем ID страницы
                int id = model.Id;

                //Объявим переменную краткого заголовка
                string slug = "home";

                //Получаем страницу (по ID)
                PagesDTO dto = db.Pages.Find(id);

                //Присваиваем название из полученной мадели в DTO
                dto.Title = model.Title;

                //Проверяем краткий заголовок и присваиваем его, если это необходимо
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                //Проверяем slug и title на уникальность
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title alredy exist.");
                    return View(model);
                }
                else if (db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That slug alredy exist.");
                    return View(model);
                }

                //Записываем остальные значения в класс DTO
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                //Сохраняем изменения в базу
                db.SaveChanges();

            }
            //Устанавливаем сообщение в TemData
            TempData["SM"] = "You have edited the page.";

            //Переадресация пользователя
            return RedirectToAction("EditPage");
        }

        // GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            //Объявляем модель PageVM
            PageVM model;

            using (Db db = new Db())
            {
                //Получаем страницу
                PagesDTO dto = db.Pages.Find(id);

                //Подтверждаем, что страница доступна
                if (dto == null)
                {
                    return Content("The page does not exist.");
                }

                //Присваиваем модели информацию из базы
                model = new PageVM(dto);

            }
            //Возвращаем модель в представление
            return View(model);
        }

        // GET: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                //Получаем страницу
                PagesDTO dto = db.Pages.Find(id);

                //Удаляем страницу
                db.Pages.Remove(dto);

                //Сохраняем изменения
                db.SaveChanges();
            }
            TempData["SM"] = "You have deleted a page";

            //Переадресовываем
            return RedirectToAction("Index");
        }

       

        // GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            //Объявляем модель
            SidebarVM model;

            using (Db db = new Db())
            {
                //Получаем данные из DTO
                SidebarDTO dto = db.Sidebars.Find(1); //Говнокод! Жёсткие значения в коде не желательно добавлять!!!

                //Заполняем модель данными
                model = new SidebarVM(dto);

            }
            //Вернуть представление с моделью
            return View(model);
        }

  
        // POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {
                //Получаем данные из DTO
                SidebarDTO dto = db.Sidebars.Find(1); //Говнокод!

                //Присваиваем данные в тело (в свойство Body)
                dto.Body = model.Body;

                //Сохраняем
                db.SaveChanges();
            }
            //Присваиваем сообщение в TempData
            TempData["SM"] = "You have edited the sidebar!";

            //Переадресовываем пользователя
            return RedirectToAction("EditSidebar");
        }
    }
}

using Kursach_MVC.Models.Data;
using Kursach_MVC.Models.ViewModels.Account;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using static Kursach_MVC.Areas.Admin.Controllers.PagesController;

namespace Kursach_MVC.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // GET: account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        public double[] CreateMark(string address)
        {
            string apiKey = "fbede055-0a60-4238-96b8-3b44b6dfdb38";

            string url = $"https://geocode-maps.yandex.ru/1.x/?apikey={apiKey}&geocode={address}&format=json";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string responseText = reader.ReadToEnd();

            reader.Close();
            responseStream.Close();
            response.Close();

            JObject json = JObject.Parse(responseText);
            JToken point = json["response"]["GeoObjectCollection"]["featureMember"][0]["GeoObject"]["Point"]["pos"];
            string[] coordinates = point.ToString().Split(' ');
            double latitude = double.Parse(coordinates[1], CultureInfo.InvariantCulture);
            double longitude = double.Parse(coordinates[0], CultureInfo.InvariantCulture);
            double[] coordinatesAddres = new double[2];
            coordinatesAddres[0] = latitude;
            coordinatesAddres[1] = longitude;
            return coordinatesAddres;
        }
        // POST: account/create-account
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
                return View("CreateAccount", model);

            // Проверяем соответствие пароля
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password do not match!");
                return View("CreateAccount", model);
            }

            using (Db db = new Db())
            {
                // Проверяем имя на уникальность
                if (db.Users.Any(x => x.Username.Equals(model.Username)))
                {
                    ModelState.AddModelError("", $"Username {model.Username} is taken.");
                    model.Username = "";
                    return View("CreateAccount", model);
                }

                double[] coordinates = CreateMark(model.Address);
                double latitude = coordinates[0];
                double longitude = coordinates[1];

                // Создаём экземпляр класса UserDTO
                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAdress = model.EmailAdress,
                    Address = model.Address,                   
                    latitude = latitude,
                    longitude = longitude,
                    Username = model.Username,
                    Password = model.Password
                };

                // Добавляем данные в модель
                db.Users.Add(userDTO);

                // Сохраняем данные
                db.SaveChanges();

                // Добавляем роль пользователю
                int id = userDTO.Id;

                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2
                };

                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }

            // Записать сообщение в TempData
            TempData["SM"] = "You are now registered and can login.";

            // Переадресовываем пользователя
            return RedirectToAction("Login");
        }

        // GET: Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            // подтвердить, что пользователь не авторизован
            string userName = User.Identity.Name;

            if (!string.IsNullOrEmpty(userName))
                return RedirectToAction("user-profile");

            // Возвращаем представление
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
                return View(model);

            // Проверяем пользователя на валидность
            bool isValid = false;

            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.Username.Equals(model.Username) && x.Password.Equals(model.Password)))
                    isValid = true;

                if (!isValid)
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View(model);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
                }
            }
        }

        // GET: /account/logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult UserNavPartial()
        {
            // Получаем имя пользователя
            string userName = User.Identity.Name;

            // Объявляем модель
            UserNavPartialVM model;

            using (Db db = new Db())
            {
                // Получаем пользователя
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);


                // Заполняем модель данными из контекста (DTO)
                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }
            // Возвращаем частичное представление с моделью
            return PartialView(model);
        }

        // GET: /account/user-profile
        [HttpGet]
        [ActionName("user-profile")]
        public ActionResult UserProfile()
        {
            // Получаем имя пользователя
            string userName = User.Identity.Name;

            // Объявляем модель
            UserProfileVM model;

            using (Db db = new Db())
            {
                // Получаем пользователя
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);

                // Инициализируем модель данными
                model = new UserProfileVM(dto);
            }
            // Возвращаем модель в представление
            return View("UserProfile", model);
        }

        // POST: /account/user-profile
        [HttpPost]
        [ActionName("user-profile")]
        public ActionResult UserProfile(UserProfileVM model)
        {
            bool userNameIsChanged = false;

            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            // Проверяем пароль (если пользователь его меняет)
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Passwords do not match.");
                    return View("UserProfile", model);
                }
            }

            using (Db db = new Db())
            {
                // Получаем имя пользователя
                string userName = User.Identity.Name;

                // Проверяем, сменилось ли имя пользователя
                if (userName != model.Username)
                {
                    userName = model.Username;
                    userNameIsChanged = true;
                }

                // Проверяем имя на уникальность
                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.Username == userName))
                {
                    ModelState.AddModelError("", $"Username {model.Username} alredy exists.");
                    model.Username = "";
                    return View("UserProfile", model);
                }

                double[] coordinates = CreateMark(model.Address);
                double latitude = coordinates[0];
                double longitude = coordinates[1];

                // Изменяем модель контекста данных
                UserDTO dto = db.Users.Find(model.Id);

                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.Address = model.Address;
                dto.latitude = latitude;
                dto.longitude = longitude;
                dto.EmailAdress = model.EmailAdress;
                dto.Username = model.Username;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }

                // Сохраняем изменения
                db.SaveChanges();
            }

            // Устанавливаем сообщение в TempData
            TempData["SM"] = "You have edited your profile!";

            if (!userNameIsChanged)
                // Возвращаем представление с моделью
                return View("UserProfile", model);
            else
                return RedirectToAction("Logout");
        }   
    }
}
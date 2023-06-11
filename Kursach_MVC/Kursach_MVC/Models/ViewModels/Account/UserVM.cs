using Kursach_MVC.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace Kursach_MVC.Models.ViewModels.Account
{
    public class UserVM
    {
        public UserVM()
        {
        }

        public UserVM(UserDTO row)
        {
            Id = row.Id;
            FirstName = row.FirstName;
            LastName = row.LastName;
            Address = row.Address;
            latitude = row.latitude;
            longitude = row.longitude;
            EmailAdress = row.EmailAdress;
            Username = row.Username;
            Password = row.Password;
        }

        public int Id { get; set; }
        [Required]
        [DisplayName("Имя")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Фамилия")]
        public string LastName { get; set; }
        [DisplayName("Адрес")]
        public string Address { get; set; }

        public double latitude { get; set; }

        public double longitude { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email")]
        public string EmailAdress { get; set; }
        [Required]
        [DisplayName("Имя пользователя")]
        public string Username { get; set; }
        [Required]
        [DisplayName("Пароль")]
        public string Password { get; set; }
        [Required]
        [DisplayName("Подтвердите пароль")]
        public string ConfirmPassword { get; set; }
    }
}
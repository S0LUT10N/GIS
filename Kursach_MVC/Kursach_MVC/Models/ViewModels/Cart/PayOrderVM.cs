using Kursach_MVC.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kursach_MVC.Models.ViewModels.Cart
{
    public class PayOrderVM
    {

        public PayOrderVM() { }

        public PayOrderVM(PayOrderDTO row)
        {
            Id = row.Id;
            Numbercart = row.Numbercart;
            CVV = row.CVV;
            DataTime = row.DataTime;
            OrderId = row.OrderId;
            UserId = row.OrderId;
            OrderDetailsId = row.OrderDetailsId;
        }
        public int Id { get; set; }

        public int Numbercart { get; set; }

        public int CVV { get; set; }

        public string DataTime { get; set; }

        public int OrderId { get; set; }

        public int UserId { get; set; }

        public int OrderDetailsId { get; set; }


    }
}
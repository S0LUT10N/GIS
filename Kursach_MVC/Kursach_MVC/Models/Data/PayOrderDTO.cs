using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Kursach_MVC.Models.Data
{
    public class PayOrderDTO
    {
        public int Id { get; set; }

        public int Numbercart { get; set; }

        public int CVV { get; set;}

        public string DataTime { get; set; }

        public int OrderId { get; set; }

        public int UserId { get; set; }

        public int OrderDetailsId { get; set; }

        [ForeignKey("OrderId")]
        public virtual OrderDTO order { get; set; }

        [ForeignKey("UserId")]
        public virtual UserDTO user { get; set; }

        [ForeignKey("OrderDetailsId")]
        public virtual OrderDetailsDTO orderDetails { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.DataAnnotations;

namespace WebServer.Model
{
    [MySqlCollation("utf8_czech_ci")]
    [Table("orders")]
    public class Order
    {
        public Order()
        {

        }




        [Key]
        public int Id { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public int UserId { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime OrderedAt { get; set; }

        public virtual ICollection<OrderFood> OrderFood { get; set; }

    }

    [Table("order_has_food")]
    public class OrderFood
    {

        [ForeignKey("OrderId"), Column(Order = 0)]
        public int OrderId { get; set; }

        [ForeignKey("FoodId"), Column(Order = 1)]
        public int FoodId { get; set; }

        public virtual Food Food { get; set; }

        public virtual Order Order { get; set; }

        public int FoodCount { get; set; }
    }
}

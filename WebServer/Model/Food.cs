using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using MySql.Data.EntityFrameworkCore.DataAnnotations;

namespace WebServer.Model
{
    [Table("food")]
    [MySqlCollation("utf8_czech_ci")]
    public class Food
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string Name { get; set; }

        public int Gram { get; set; }

        public decimal Price { get; set; }

        public string Composition { get; set; }

        public int EnergyKj { get; set; }

        public int EnergyKcal { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal Protein { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal Carbohydrates { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal Sugar { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal TotalFat { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal SaturatedFat { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal Fiber { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal Salt { get; set; }

        [JsonIgnore]
        public ICollection<FoodAllergen> FoodAllergen { get; set; }

        [JsonIgnore]
        public ICollection<OrderFood> OrderFood { get; set; }

        [NotMapped]
        public ICollection<int> Allergenes { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }


        public string ToCliString()
        {
            return string.Format("{0} {1} {2} {3} {4}", Id, Name, CategoryId, Price, Composition);
        }

    }
}

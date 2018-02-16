using MySql.Data.EntityFrameworkCore.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebServer.Model
{
    [Table("allergenes")]
    [MySqlCollation("utf8_czech_ci")]
    public class Allergen
    {
        public Allergen()
        {

        }

        //Constructor
        public Allergen(string Name) => this.Name = Name;

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<FoodAllergen> FoodAllergen { get; set; }

    }

    [Table("food_allergen")]
    public class FoodAllergen
    {
        public FoodAllergen()
        {

        }

        public FoodAllergen(Food food, Allergen allergen)
        {
            this.Food = food;
            this.Allergen = allergen;
            this.FoodId = food.Id;
            this.AllergenId = allergen.Id;
        }


        [ForeignKey("FoodId"), Column(Order = 0)]
        public int FoodId { get; set; }
        public Food Food { get; set; }

        [ForeignKey("AllergenId"), Column(Order = 0)]
        public int AllergenId { get; set; }
        public Allergen Allergen { get; set; }



    }
}

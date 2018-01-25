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
    
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<FoodAllergen> FoodAllergen { get; set; }     

    }

    [Table("food_allergen")]
    public class FoodAllergen
    {

        [ForeignKey("FoodId"), Column(Order = 0)]
        public int FoodId { get; set; }
        public Food Food { get; set; }

        [ForeignKey("AllergenId"), Column(Order = 0)]
        public int AllergenId { get; set; }
        public Allergen Allergen { get; set; }



    }
}

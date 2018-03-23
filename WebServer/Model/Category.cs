using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.DataAnnotations;

namespace WebServer.Model
{
    [MySqlCollation("utf8_czech_ci")]
    public class Category
    {

        public Category()
        {
            ParentId = null;
        }

        public Category(string name, int? parentId)
        {
            Name = name;
            ParentId = parentId;
        }

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string Name { get; set; }

        public int? ParentId { get; set; }

        public ICollection<Food> Food { get; set; }

        public string ToCliString()
        {
            return string.Format("{0} {1} {2}", Id, Name, ParentId);
        }

    }
}

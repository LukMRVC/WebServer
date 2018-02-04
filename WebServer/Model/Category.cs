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

        [StringLength(100)]
        public string Name { get; set; }

        public int? ParentId { get; set; }

        public Food Food { get; set; }


    }
}

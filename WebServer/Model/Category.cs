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

        [Key]
        public int Id { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }

        public int ParentId { get; set; }

        public Food Food { get; set; }


    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using MySql.Data.EntityFrameworkCore.DataAnnotations;

namespace WebServer.Model
{
    [MySqlCollation("utf8_czech_ci")]
    public class User
    {

        public User() {
            this.CreatedDateTime = DateTime.Now;
        }

        public User(string Email, string Password)
        {
            this.CreatedDateTime = DateTime.Now;
            this.Password = Password;
            this.Email = Email;
        }


        [Key]
        public int Id { get; set; }

        public bool ValidatePassword(string password)
        {
            if (BCrypt.Net.BCrypt.Verify(password, this.Password) )
                return true;
            return false;
        }

        [Column(TypeName = "varchar(200)")]
        public string Email { get; set; }

        public ICollection<Order> Order { get; set; }

        //Tato vlastnot nebude zmapována
        [NotMapped]
        public string Password
        {
            get { return PasswordHash; }
            set { PasswordHash = BCrypt.Net.BCrypt.HashPassword(value); }
        }

        public DateTime CreatedDateTime { get; set; }

        public string PasswordHash { get; set; }


    }
}

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebServer.Model
{
    public class MenuDbContext : DbContext
    {
        public DbSet<Food> Food { get; set; }

        public DbSet<Category> Category { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<User> User { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseMySQL(connectionString: System.Configuration.ConfigurationManager.ConnectionStrings["MenuDbContext"].ConnectionString);
            optionsBuilder.UseMySQL("server=localhost;port=3306;database=webserverdb;uid=root;password=");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Define composite key.
            builder.Entity<FoodAllergen>()
                .HasKey(fa => new { fa.FoodId, fa.AllergenId });

            builder.Entity<OrderFood>()
                .HasKey(of => new { of.FoodId, of.OrderId });

            builder.Entity<User>().HasMany(u => u.Order).WithOne(o => o.User).IsRequired();

            builder.Entity<Category>().HasIndex(c => c.Name).IsUnique(true);

            builder.Entity<Food>().HasIndex(f => f.Name).IsUnique(true);


        }


    }
}

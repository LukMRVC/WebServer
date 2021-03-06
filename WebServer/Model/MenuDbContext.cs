﻿using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace WebServer.Model
{
     public class MenuDbContext : DbContext
    {

        public DbSet<Food> Food { get; set; }

        public DbSet<Category> Category { get; set; }

        public DbSet<Allergen> Allergens { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<User> User { get; set; }

        public readonly string [] allergenValues = { "Obiloviny obsahující lepek", "Korýši", "Vejce", "Ryby", "Jádra podzemnice olejné", "Sójové boby (sója)",
            "Mléko", "Skořápkové plody", "Celer", "Hořčice", "Sezamové semena", "Oxid siřičtý a siřičitany", "Vlčí bob", "Měkkýši"};


          protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
          {
            if (string.IsNullOrEmpty(Program.ConnString))
            {
                optionsBuilder.UseMySQL(connectionString: System.Configuration.ConfigurationManager.ConnectionStrings["MenuDbContext"].ConnectionString);
            }
            else
            {
                optionsBuilder.UseMySQL(Program.ConnString);
            }
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

            builder.Entity<Food>().Property(f => f.Name).HasColumnType("varchar(200)");

            builder.Entity<Category>().Property(c => c.Name).HasColumnType("varchar(200)");

            builder.Entity<Category>().HasMany(c => c.Food).WithOne(f => f.Category);

            builder.Entity<Category>().HasIndex(c => c.Name).IsUnique(true);

            builder.Entity<Food>().HasIndex(f => f.Name).IsUnique(true);

            builder.Entity<User>().HasIndex(u => u.Email).IsUnique(true);

        }


    }
}

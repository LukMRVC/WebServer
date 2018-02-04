using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebServer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "allergenes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_allergenes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Name = table.Column<string>(nullable: true),
                    ParentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(type: "varchar(200)", nullable: true),
                    PasswordHash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "food",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Carbohydrates = table.Column<decimal>(type: "decimal(5, 2)", nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    Composition = table.Column<string>(nullable: true),
                    EnergyKcal = table.Column<int>(nullable: false),
                    EnergyKj = table.Column<int>(nullable: false),
                    Fiber = table.Column<decimal>(type: "decimal(5, 2)", nullable: false),
                    Gram = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    Protein = table.Column<decimal>(type: "decimal(5, 2)", nullable: false),
                    Salt = table.Column<decimal>(type: "decimal(5, 2)", nullable: false),
                    SaturatedFat = table.Column<decimal>(type: "decimal(5, 2)", nullable: false),
                    Sugar = table.Column<decimal>(type: "decimal(5, 2)", nullable: false),
                    TotalFat = table.Column<decimal>(type: "decimal(5, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_food", x => x.Id);
                    table.ForeignKey(
                        name: "FK_food_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    OrderedAt = table.Column<DateTime>(nullable: false),
                    TotalPrice = table.Column<decimal>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_orders_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "food_allergen",
                columns: table => new
                {
                    FoodId = table.Column<int>(nullable: false),
                    AllergenId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_food_allergen", x => new { x.FoodId, x.AllergenId });
                    table.ForeignKey(
                        name: "FK_food_allergen_allergenes_AllergenId",
                        column: x => x.AllergenId,
                        principalTable: "allergenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_food_allergen_food_FoodId",
                        column: x => x.FoodId,
                        principalTable: "food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_has_food",
                columns: table => new
                {
                    FoodId = table.Column<int>(nullable: false),
                    OrderId = table.Column<int>(nullable: false),
                    FoodCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_has_food", x => new { x.FoodId, x.OrderId });
                    table.ForeignKey(
                        name: "FK_order_has_food_food_FoodId",
                        column: x => x.FoodId,
                        principalTable: "food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_has_food_orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_food_CategoryId",
                table: "food",
                column: "CategoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_food_allergen_AllergenId",
                table: "food_allergen",
                column: "AllergenId");

            migrationBuilder.CreateIndex(
                name: "IX_order_has_food_OrderId",
                table: "order_has_food",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_UserId",
                table: "orders",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "food_allergen");

            migrationBuilder.DropTable(
                name: "order_has_food");

            migrationBuilder.DropTable(
                name: "allergenes");

            migrationBuilder.DropTable(
                name: "food");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}

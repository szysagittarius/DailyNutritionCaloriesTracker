using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NT.Ef.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Nutrition");

            migrationBuilder.CreateTable(
                name: "FoodNutritions",
                schema: "Nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Measurement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Carbs = table.Column<double>(type: "float", nullable: false),
                    Fat = table.Column<double>(type: "float", nullable: false),
                    Protein = table.Column<double>(type: "float", nullable: false),
                    Calories = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodNutritions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FoodLogs",
                schema: "Nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodLogs_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Nutrition",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoodItems",
                schema: "Nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FoodNutritionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Unit = table.Column<int>(type: "int", nullable: false),
                    FoodLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodItems_FoodLogs_FoodLogId",
                        column: x => x.FoodLogId,
                        principalSchema: "Nutrition",
                        principalTable: "FoodLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoodItems_FoodNutritions_FoodNutritionId",
                        column: x => x.FoodNutritionId,
                        principalSchema: "Nutrition",
                        principalTable: "FoodNutritions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodItems_FoodLogId",
                schema: "Nutrition",
                table: "FoodItems",
                column: "FoodLogId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodItems_FoodNutritionId",
                schema: "Nutrition",
                table: "FoodItems",
                column: "FoodNutritionId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodLogs_UserId",
                schema: "Nutrition",
                table: "FoodLogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodItems",
                schema: "Nutrition");

            migrationBuilder.DropTable(
                name: "FoodLogs",
                schema: "Nutrition");

            migrationBuilder.DropTable(
                name: "FoodNutritions",
                schema: "Nutrition");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Nutrition");
        }
    }
}

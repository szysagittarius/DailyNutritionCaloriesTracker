using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NT.Ef.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalNutritionToFoodLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalCalories",
                schema: "Nutrition",
                table: "FoodLogs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalCarbs",
                schema: "Nutrition",
                table: "FoodLogs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalFat",
                schema: "Nutrition",
                table: "FoodLogs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalProtein",
                schema: "Nutrition",
                table: "FoodLogs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCalories",
                schema: "Nutrition",
                table: "FoodLogs");

            migrationBuilder.DropColumn(
                name: "TotalCarbs",
                schema: "Nutrition",
                table: "FoodLogs");

            migrationBuilder.DropColumn(
                name: "TotalFat",
                schema: "Nutrition",
                table: "FoodLogs");

            migrationBuilder.DropColumn(
                name: "TotalProtein",
                schema: "Nutrition",
                table: "FoodLogs");
        }
    }
}

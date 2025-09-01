using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NT.Ef.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddNutritionGoalsToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SuggestedCarbs",
                schema: "Nutrition",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SuggestedFat",
                schema: "Nutrition",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SuggestedProtein",
                schema: "Nutrition",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuggestedCarbs",
                schema: "Nutrition",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SuggestedFat",
                schema: "Nutrition",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SuggestedProtein",
                schema: "Nutrition",
                table: "Users");
        }
    }
}

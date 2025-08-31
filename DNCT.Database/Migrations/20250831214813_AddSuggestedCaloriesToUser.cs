using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NT.Ef.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSuggestedCaloriesToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SuggestedCalories",
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
                name: "SuggestedCalories",
                schema: "Nutrition",
                table: "Users");
        }
    }
}

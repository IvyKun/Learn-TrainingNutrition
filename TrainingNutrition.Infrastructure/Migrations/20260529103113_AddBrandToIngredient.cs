using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingNutrition.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBrandToIngredient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Ingredients",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Ingredients");
        }
    }
}

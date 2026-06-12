using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingNutrition.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemodelMealsAndRemoveDish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientEntries_Dishes_DishId",
                table: "IngredientEntries");

            migrationBuilder.DropTable(
                name: "Dishes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Meals");

            migrationBuilder.DropColumn(
                name: "OccurredAt",
                table: "Meals");

            migrationBuilder.RenameColumn(
                name: "DishId",
                table: "IngredientEntries",
                newName: "MealId");

            migrationBuilder.RenameIndex(
                name: "IX_IngredientEntries_DishId",
                table: "IngredientEntries",
                newName: "IX_IngredientEntries_MealId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientEntries_Meals_MealId",
                table: "IngredientEntries",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientEntries_Meals_MealId",
                table: "IngredientEntries");

            migrationBuilder.RenameColumn(
                name: "MealId",
                table: "IngredientEntries",
                newName: "DishId");

            migrationBuilder.RenameIndex(
                name: "IX_IngredientEntries_MealId",
                table: "IngredientEntries",
                newName: "IX_IngredientEntries_DishId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Meals",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "OccurredAt",
                table: "Meals",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateTable(
                name: "Dishes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MealId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dishes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dishes_Meals_MealId",
                        column: x => x.MealId,
                        principalTable: "Meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_MealId",
                table: "Dishes",
                column: "MealId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientEntries_Dishes_DishId",
                table: "IngredientEntries",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

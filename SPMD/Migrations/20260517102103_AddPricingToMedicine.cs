using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPMD.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingToMedicine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                table: "MedicineBatch",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerUnit",
                table: "Medicine",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "MedicineBatch");

            migrationBuilder.DropColumn(
                name: "PricePerUnit",
                table: "Medicine");
        }
    }
}

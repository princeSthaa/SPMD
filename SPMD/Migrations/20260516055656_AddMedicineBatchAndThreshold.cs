using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPMD.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicineBatchAndThreshold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Patient",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StockThreshold",
                table: "Medicine",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MedicineBatch",
                columns: table => new
                {
                    MedicineBatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineId = table.Column<int>(type: "int", nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuantityAvailable = table.Column<int>(type: "int", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineBatch", x => x.MedicineBatchId);
                    table.ForeignKey(
                        name: "FK_MedicineBatch_Medicine_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "Medicine",
                        principalColumn: "MedicineId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Patient_UserId",
                table: "Patient",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineBatch_MedicineId",
                table: "MedicineBatch",
                column: "MedicineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patient_User_UserId",
                table: "Patient",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patient_User_UserId",
                table: "Patient");

            migrationBuilder.DropTable(
                name: "MedicineBatch");

            migrationBuilder.DropIndex(
                name: "IX_Patient_UserId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "StockThreshold",
                table: "Medicine");
        }
    }
}

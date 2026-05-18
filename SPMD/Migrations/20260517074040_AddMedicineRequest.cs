using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPMD.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicineRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicineRequests",
                columns: table => new
                {
                    MedicineRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    RequestedQuantity = table.Column<int>(type: "int", nullable: false),
                    ClinicalReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineRequests", x => x.MedicineRequestId);
                    table.ForeignKey(
                        name: "FK_MedicineRequests_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicineRequests_Medicine_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "Medicine",
                        principalColumn: "MedicineId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicineRequests_DoctorId",
                table: "MedicineRequests",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineRequests_MedicineId",
                table: "MedicineRequests",
                column: "MedicineId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicineRequests");
        }
    }
}

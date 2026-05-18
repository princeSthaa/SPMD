using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPMD.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medicine",
                columns: table => new
                {
                    MedicineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GenericName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Strength = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DosageForm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsControlled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicine", x => x.MedicineId);
                });

            migrationBuilder.CreateTable(
                name: "Patient",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionalAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient", x => x.PatientId);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "AllergyRecord",
                columns: table => new
                {
                    AllergyRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Substance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reaction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    MedicineId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllergyRecord", x => x.AllergyRecordId);
                    table.ForeignKey(
                        name: "FK_AllergyRecord_Medicine_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "Medicine",
                        principalColumn: "MedicineId");
                    table.ForeignKey(
                        name: "FK_AllergyRecord_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_User_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    DoctorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Speciality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.DoctorId);
                    table.ForeignKey(
                        name: "FK_Doctors_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Pharmacist",
                columns: table => new
                {
                    PharmacistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pharmacist", x => x.PharmacistId);
                    table.ForeignKey(
                        name: "FK_Pharmacist_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Prescription",
                columns: table => new
                {
                    PrescriptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrescriptionNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescription", x => x.PrescriptionId);
                    table.ForeignKey(
                        name: "FK_Prescription_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prescription_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    AuditLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    PerformedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PerformedByRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PharmacistId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.AuditLogId);
                    table.ForeignKey(
                        name: "FK_AuditLog_Pharmacist_PharmacistId",
                        column: x => x.PharmacistId,
                        principalTable: "Pharmacist",
                        principalColumn: "PharmacistId");
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionItem",
                columns: table => new
                {
                    PrescriptionItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrescriptionId = table.Column<int>(type: "int", nullable: false),
                    MedicineId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DurationDays = table.Column<int>(type: "int", nullable: false),
                    RefillsAllowed = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionItem", x => x.PrescriptionItemId);
                    table.ForeignKey(
                        name: "FK_PrescriptionItem_Medicine_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "Medicine",
                        principalColumn: "MedicineId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrescriptionItem_Prescription_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescription",
                        principalColumn: "PrescriptionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DispensingRecord",
                columns: table => new
                {
                    DispensingRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DispensedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuantityDispensed = table.Column<double>(type: "float", nullable: false),
                    batchNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPartial = table.Column<bool>(type: "bit", nullable: false),
                    PrescriptionItemId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    PharmacistId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispensingRecord", x => x.DispensingRecordId);
                    table.ForeignKey(
                        name: "FK_DispensingRecord_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DispensingRecord_Pharmacist_PharmacistId",
                        column: x => x.PharmacistId,
                        principalTable: "Pharmacist",
                        principalColumn: "PharmacistId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DispensingRecord_PrescriptionItem_PrescriptionItemId",
                        column: x => x.PrescriptionItemId,
                        principalTable: "PrescriptionItem",
                        principalColumn: "PrescriptionItemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllergyRecord_MedicineId",
                table: "AllergyRecord",
                column: "MedicineId");

            migrationBuilder.CreateIndex(
                name: "IX_AllergyRecord_PatientId",
                table: "AllergyRecord",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_PharmacistId",
                table: "AuditLog",
                column: "PharmacistId");

            migrationBuilder.CreateIndex(
                name: "IX_DispensingRecord_PatientId",
                table: "DispensingRecord",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_DispensingRecord_PharmacistId",
                table: "DispensingRecord",
                column: "PharmacistId");

            migrationBuilder.CreateIndex(
                name: "IX_DispensingRecord_PrescriptionItemId",
                table: "DispensingRecord",
                column: "PrescriptionItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_UserId",
                table: "Doctors",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacist_UserId",
                table: "Pharmacist",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_DoctorId",
                table: "Prescription",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_PatientId",
                table: "Prescription",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionItem_MedicineId",
                table: "PrescriptionItem",
                column: "MedicineId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionItem_PrescriptionId",
                table: "PrescriptionItem",
                column: "PrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                table: "User",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllergyRecord");

            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "DispensingRecord");

            migrationBuilder.DropTable(
                name: "Pharmacist");

            migrationBuilder.DropTable(
                name: "PrescriptionItem");

            migrationBuilder.DropTable(
                name: "Medicine");

            migrationBuilder.DropTable(
                name: "Prescription");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Patient");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}

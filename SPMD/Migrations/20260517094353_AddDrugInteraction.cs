using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPMD.Migrations
{
    /// <inheritdoc />
    public partial class AddDrugInteraction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DrugInteractions",
                columns: table => new
                {
                    DrugInteractionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GenericNameA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GenericNameB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrugInteractions", x => x.DrugInteractionId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrugInteractions");
        }
    }
}

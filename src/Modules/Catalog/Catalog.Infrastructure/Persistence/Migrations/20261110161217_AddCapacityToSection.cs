using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCapacityToSection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                schema: "catalog",
                table: "Sections",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddCheckConstraint(
                name: "CK_Sections_Capacity_NonNegative",
                schema: "catalog",
                table: "Sections",
                sql: "[Capacity] >= 0"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Sections_Capacity_NonNegative",
                schema: "catalog",
                table: "Sections"
            );

            migrationBuilder.DropColumn(name: "Capacity", schema: "catalog", table: "Sections");
        }
    }
}

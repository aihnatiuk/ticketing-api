using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnforceSectionCapacityByType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Sections_Capacity_NonNegative",
                schema: "catalog",
                table: "Sections"
            );

            migrationBuilder.AddCheckConstraint(
                name: "CK_Sections_Capacity_MatchesType",
                schema: "catalog",
                table: "Sections",
                sql: "([Type] = 1 AND [Capacity] > 0) OR ([Type] <> 1 AND [Capacity] IS NULL)"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Sections_Capacity_MatchesType",
                schema: "catalog",
                table: "Sections"
            );

            migrationBuilder.AddCheckConstraint(
                name: "CK_Sections_Capacity_NonNegative",
                schema: "catalog",
                table: "Sections",
                sql: "[Capacity] >= 0"
            );
        }
    }
}

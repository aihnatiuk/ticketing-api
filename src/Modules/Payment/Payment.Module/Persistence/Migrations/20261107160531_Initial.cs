using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Module.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "payment");

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                schema: "payment",
                columns: table => new
                {
                    Id = table
                        .Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "datetimeoffset",
                        nullable: false
                    ),
                    ProcessedAt = table.Column<DateTimeOffset>(
                        type: "datetimeoffset",
                        nullable: true
                    ),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(
                        type: "char(3)",
                        unicode: false,
                        fixedLength: true,
                        maxLength: 3,
                        nullable: false
                    ),
                    Amount = table.Column<decimal>(
                        type: "decimal(18,2)",
                        precision: 18,
                        scale: 2,
                        nullable: false
                    ),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.CheckConstraint(
                        "CK_PaymentTransactions_Amount_NonNegative",
                        "[Amount] >= 0"
                    );
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PaymentTransactions", schema: "payment");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Module.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxAndPendingPaymentIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: false
                    ),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(
                        type: "datetimeoffset",
                        nullable: false
                    ),
                    ProcessedAt = table.Column<DateTimeOffset>(
                        type: "datetimeoffset",
                        nullable: true
                    ),
                    Attempts = table.Column<int>(type: "int", nullable: false),
                    Error = table.Column<string>(
                        type: "nvarchar(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_OrderId",
                schema: "payment",
                table: "PaymentTransactions",
                column: "OrderId",
                unique: true,
                filter: "[Status] = 0"
            );

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Unprocessed",
                schema: "payment",
                table: "OutboxMessages",
                column: "OccurredAt",
                filter: "[ProcessedAt] IS NULL"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "OutboxMessages", schema: "payment");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_OrderId",
                schema: "payment",
                table: "PaymentTransactions"
            );
        }
    }
}

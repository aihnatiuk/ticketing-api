using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "booking");

            migrationBuilder.CreateTable(
                name: "GeneralSeats",
                schema: "booking",
                columns: table => new
                {
                    Id = table
                        .Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    SectionId = table.Column<long>(type: "bigint", nullable: false),
                    TotalSeats = table.Column<int>(type: "int", nullable: false),
                    AvailableSeats = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralSeats", x => x.Id);
                    table.CheckConstraint(
                        "CK_GeneralSeats_Availability",
                        "[AvailableSeats] >= 0 AND [AvailableSeats] <= [TotalSeats]"
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "booking",
                columns: table => new
                {
                    Id = table
                        .Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "datetimeoffset",
                        nullable: false
                    ),
                    ExpiresAt = table.Column<DateTimeOffset>(
                        type: "datetimeoffset",
                        nullable: false
                    ),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    Currency = table.Column<string>(
                        type: "char(3)",
                        unicode: false,
                        fixedLength: true,
                        maxLength: 3,
                        nullable: false
                    ),
                    TotalPrice = table.Column<decimal>(
                        type: "decimal(18,2)",
                        precision: 18,
                        scale: 2,
                        nullable: false
                    ),
                    PaymentId = table.Column<long>(type: "bigint", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.CheckConstraint("CK_Orders_TotalPrice_NonNegative", "[TotalPrice] >= 0");
                }
            );

            migrationBuilder.CreateTable(
                name: "DedicatedSeats",
                schema: "booking",
                columns: table => new
                {
                    Id = table
                        .Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    SeatId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    OrderId = table.Column<long>(type: "bigint", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DedicatedSeats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DedicatedSeats_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "booking",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "OrderItems",
                schema: "booking",
                columns: table => new
                {
                    Id = table
                        .Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    CatalogRefId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    TicketTypeId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(
                        type: "decimal(18,2)",
                        precision: 18,
                        scale: 2,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.CheckConstraint(
                        "CK_OrderItems_Quantity",
                        "[Quantity] > 0 AND ([Type] <> 0 OR [Quantity] = 1)"
                    );
                    table.CheckConstraint(
                        "CK_OrderItems_UnitPrice_NonNegative",
                        "[UnitPrice] >= 0"
                    );
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "booking",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedSeats_EventId_SeatId",
                schema: "booking",
                table: "DedicatedSeats",
                columns: new[] { "EventId", "SeatId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedSeats_OrderId",
                schema: "booking",
                table: "DedicatedSeats",
                column: "OrderId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_GeneralSeats_EventId_SectionId",
                schema: "booking",
                table: "GeneralSeats",
                columns: new[] { "EventId", "SectionId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                schema: "booking",
                table: "OrderItems",
                column: "OrderId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "DedicatedSeats", schema: "booking");

            migrationBuilder.DropTable(name: "GeneralSeats", schema: "booking");

            migrationBuilder.DropTable(name: "OrderItems", schema: "booking");

            migrationBuilder.DropTable(name: "Orders", schema: "booking");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "catalog");

            migrationBuilder.CreateTable(
                name: "TicketTypes",
                schema: "catalog",
                columns: table => new
                {
                    Id = table
                        .Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTypes", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Venues",
                schema: "catalog",
                columns: table => new
                {
                    Id = table
                        .Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    Address_Street = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    Address_City = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Address_CountryCode = table.Column<string>(
                        type: "char(2)",
                        unicode: false,
                        fixedLength: true,
                        maxLength: 2,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Events",
                schema: "catalog",
                columns: table => new
                {
                    Id = table
                        .Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "nvarchar(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    StartsAt = table.Column<DateTimeOffset>(
                        type: "datetimeoffset",
                        nullable: false
                    ),
                    EndsAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    VenueId = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(
                        type: "char(3)",
                        unicode: false,
                        fixedLength: true,
                        maxLength: 3,
                        nullable: false
                    ),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.CheckConstraint(
                        "CK_Events_EndsAt_After_StartsAt",
                        "[EndsAt] > [StartsAt]"
                    );
                    table.ForeignKey(
                        name: "FK_Events_Venues_VenueId",
                        column: x => x.VenueId,
                        principalSchema: "catalog",
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "PricingZones",
                schema: "catalog",
                columns: table => new
                {
                    Id = table
                        .Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    VenueId = table.Column<long>(type: "bigint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingZones", x => x.Id);
                    table.UniqueConstraint(
                        "AK_PricingZones_Id_VenueId",
                        x => new { x.Id, x.VenueId }
                    );
                    table.ForeignKey(
                        name: "FK_PricingZones_Venues_VenueId",
                        column: x => x.VenueId,
                        principalSchema: "catalog",
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "EventPrices",
                schema: "catalog",
                columns: table => new
                {
                    Id = table
                        .Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(
                        type: "decimal(18,2)",
                        precision: 18,
                        scale: 2,
                        nullable: false
                    ),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    PricingZoneId = table.Column<long>(type: "bigint", nullable: false),
                    TicketTypeId = table.Column<long>(type: "bigint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventPrices", x => x.Id);
                    table.CheckConstraint("CK_EventPrices_Amount_NonNegative", "[Amount] >= 0");
                    table.ForeignKey(
                        name: "FK_EventPrices_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "catalog",
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_EventPrices_PricingZones_PricingZoneId",
                        column: x => x.PricingZoneId,
                        principalSchema: "catalog",
                        principalTable: "PricingZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_EventPrices_TicketTypes_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalSchema: "catalog",
                        principalTable: "TicketTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Sections",
                schema: "catalog",
                columns: table => new
                {
                    Id = table
                        .Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    VenueId = table.Column<long>(type: "bigint", nullable: false),
                    PricingZoneId = table.Column<long>(type: "bigint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sections_PricingZones_PricingZoneId_VenueId",
                        columns: x => new { x.PricingZoneId, x.VenueId },
                        principalSchema: "catalog",
                        principalTable: "PricingZones",
                        principalColumns: new[] { "Id", "VenueId" },
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_Sections_Venues_VenueId",
                        column: x => x.VenueId,
                        principalSchema: "catalog",
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Rows",
                schema: "catalog",
                columns: table => new
                {
                    Id = table
                        .Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    SectionId = table.Column<long>(type: "bigint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rows_Sections_SectionId",
                        column: x => x.SectionId,
                        principalSchema: "catalog",
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Seats",
                schema: "catalog",
                columns: table => new
                {
                    Id = table
                        .Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    RowId = table.Column<long>(type: "bigint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seats_Rows_RowId",
                        column: x => x.RowId,
                        principalSchema: "catalog",
                        principalTable: "Rows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_EventPrices_EventId_PricingZoneId_TicketTypeId",
                schema: "catalog",
                table: "EventPrices",
                columns: new[] { "EventId", "PricingZoneId", "TicketTypeId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_EventPrices_PricingZoneId",
                schema: "catalog",
                table: "EventPrices",
                column: "PricingZoneId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_EventPrices_TicketTypeId",
                schema: "catalog",
                table: "EventPrices",
                column: "TicketTypeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Events_VenueId",
                schema: "catalog",
                table: "Events",
                column: "VenueId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PricingZones_VenueId_Label",
                schema: "catalog",
                table: "PricingZones",
                columns: new[] { "VenueId", "Label" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Rows_SectionId_Label",
                schema: "catalog",
                table: "Rows",
                columns: new[] { "SectionId", "Label" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Seats_RowId_Label",
                schema: "catalog",
                table: "Seats",
                columns: new[] { "RowId", "Label" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Sections_PricingZoneId_VenueId",
                schema: "catalog",
                table: "Sections",
                columns: new[] { "PricingZoneId", "VenueId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Sections_VenueId_Label",
                schema: "catalog",
                table: "Sections",
                columns: new[] { "VenueId", "Label" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_TicketTypes_Label",
                schema: "catalog",
                table: "TicketTypes",
                column: "Label",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "EventPrices", schema: "catalog");

            migrationBuilder.DropTable(name: "Seats", schema: "catalog");

            migrationBuilder.DropTable(name: "Events", schema: "catalog");

            migrationBuilder.DropTable(name: "TicketTypes", schema: "catalog");

            migrationBuilder.DropTable(name: "Rows", schema: "catalog");

            migrationBuilder.DropTable(name: "Sections", schema: "catalog");

            migrationBuilder.DropTable(name: "PricingZones", schema: "catalog");

            migrationBuilder.DropTable(name: "Venues", schema: "catalog");
        }
    }
}

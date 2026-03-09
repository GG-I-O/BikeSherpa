using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeliveryEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PricingStrategy = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Urgency = table.Column<string>(type: "text", nullable: false),
                    TotalPrice = table.Column<double>(type: "double precision", nullable: false),
                    Discount = table.Column<double>(type: "double precision", nullable: true),
                    ReportId = table.Column<string>(type: "text", nullable: false),
                    Details = table.Column<string[]>(type: "text[]", nullable: false),
                    PackingSize = table.Column<string>(type: "text", nullable: false),
                    InsulatedBox = table.Column<bool>(type: "boolean", nullable: false),
                    ContractDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryZones",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryZones", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "PackingSizes",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MaxWeight = table.Column<int>(type: "integer", nullable: false),
                    MaxPackageLength = table.Column<int>(type: "integer", nullable: false),
                    TourPrice = table.Column<double>(type: "double precision", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackingSizes", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Urgencies",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PriceCoefficient = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urgencies", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "DeliverySteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StepType = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Completed = table.Column<bool>(type: "boolean", nullable: false),
                    StepAddress_Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StepAddress_StreetInfo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StepAddress_Complement = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StepAddress_Postcode = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    StepAddress_City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StepZoneName = table.Column<string>(type: "character varying(100)", nullable: false),
                    Distance = table.Column<double>(type: "double precision", nullable: false),
                    CourierId = table.Column<Guid>(type: "uuid", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    AttachmentFilePaths = table.Column<string[]>(type: "text[]", nullable: true),
                    EstimatedDeliveryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RealDeliveryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeliveryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliverySteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliverySteps_Deliveries_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Deliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliverySteps_DeliveryZones_StepZoneName",
                        column: x => x.StepZoneName,
                        principalTable: "DeliveryZones",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryZoneCities",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DeliveryZoneName = table.Column<string>(type: "character varying(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryZoneCities", x => x.Name);
                    table.ForeignKey(
                        name: "FK_DeliveryZoneCities_DeliveryZones_DeliveryZoneName",
                        column: x => x.DeliveryZoneName,
                        principalTable: "DeliveryZones",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DeliveryZones",
                column: "Name",
                values: new object[]
                {
                    "Centre",
                    "Extérieur",
                    "Limitrophe",
                    "Périphérie"
                });

            migrationBuilder.InsertData(
                table: "PackingSizes",
                columns: new[] { "Name", "MaxPackageLength", "MaxWeight", "Price", "TourPrice" },
                values: new object[,]
                {
                    { "L", 105, 20, 7.0, 4.0 },
                    { "M", 85, 10, 5.0, 2.0 },
                    { "S", 45, 3, 3.0, 0.0 },
                    { "Xl", 115, 30, 9.0, 6.0 },
                    { "Xxl", 500, 40, 11.0, 8.0 },
                    { "Xxxl", 500, 50, 13.0, 10.0 },
                    { "Xxxxl", 500, 60, 15.0, 12.0 }
                });

            migrationBuilder.InsertData(
                table: "Urgencies",
                columns: new[] { "Name", "PriceCoefficient" },
                values: new object[,]
                {
                    { "Eco", 0.75 },
                    { "Standard", 1.25 },
                    { "Urgent", 2.0 }
                });

            migrationBuilder.InsertData(
                table: "DeliveryZoneCities",
                columns: new[] { "Name", "DeliveryZoneName" },
                values: new object[,]
                {
                    { "Bresson", "Périphérie" },
                    { "Échirolles", "Limitrophe" },
                    { "Eybens", "Limitrophe" },
                    { "Fontaine", "Limitrophe" },
                    { "Gières", "Périphérie" },
                    { "Grenoble", "Centre" },
                    { "La Tronche", "Limitrophe" },
                    { "Le Pont-de-Claix", "Périphérie" },
                    { "Meylan", "Périphérie" },
                    { "Poisat", "Limitrophe" },
                    { "Saint-Égrève", "Périphérie" },
                    { "Saint-Martin-d’Hères", "Limitrophe" },
                    { "Saint-Martin-le-Vinoux", "Limitrophe" },
                    { "Sassenage", "Périphérie" },
                    { "Seyssinet-Pariset", "Limitrophe" },
                    { "Seyssins", "Limitrophe" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliverySteps_DeliveryId",
                table: "DeliverySteps",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliverySteps_StepZoneName",
                table: "DeliverySteps",
                column: "StepZoneName");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryZoneCities_DeliveryZoneName",
                table: "DeliveryZoneCities",
                column: "DeliveryZoneName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliverySteps");

            migrationBuilder.DropTable(
                name: "DeliveryZoneCities");

            migrationBuilder.DropTable(
                name: "PackingSizes");

            migrationBuilder.DropTable(
                name: "Urgencies");

            migrationBuilder.DropTable(
                name: "Deliveries");

            migrationBuilder.DropTable(
                name: "DeliveryZones");

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });
        }
    }
}

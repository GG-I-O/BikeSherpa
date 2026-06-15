using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReportModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "PackingSizes",
                newName: "SimplePrice");

            migrationBuilder.AddColumn<double>(
                name: "SimplePrice",
                table: "DeliveryZones",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TourPrice",
                table: "DeliveryZones",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "Delays",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Delays", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Delays",
                columns: new[] { "Id", "Label", "Price" },
                values: new object[,]
                {
                    { "EarlyOrderDiscount", "", -2.0 },
                    { "LastMinuteOrderExtraCost", "Prioritaire", 3.0 },
                    { "StandardCost", "Jour même", 0.0 }
                });

            migrationBuilder.UpdateData(
                table: "DeliveryZones",
                keyColumn: "Name",
                keyValue: "Centre",
                columns: new[] { "SimplePrice", "TourPrice" },
                values: new object[] { 1.0, 5.0 });

            migrationBuilder.UpdateData(
                table: "DeliveryZones",
                keyColumn: "Name",
                keyValue: "Extérieur",
                columns: new[] { "SimplePrice", "TourPrice" },
                values: new object[] { 11.0, 0.0 });

            migrationBuilder.UpdateData(
                table: "DeliveryZones",
                keyColumn: "Name",
                keyValue: "Limitrophe",
                columns: new[] { "SimplePrice", "TourPrice" },
                values: new object[] { 2.5, 8.0 });

            migrationBuilder.UpdateData(
                table: "DeliveryZones",
                keyColumn: "Name",
                keyValue: "Périphérie",
                columns: new[] { "SimplePrice", "TourPrice" },
                values: new object[] { 5.5, 0.0 });

            migrationBuilder.InsertData(
                table: "Parameters",
                columns: new[] { "Key", "Value" },
                values: new object[,]
                {
                    { "EARLY_ORDER_LIMIT_IN_HOURS", "18" },
                    { "LAST_MINUTE_ORDER_LIMIT_IN_HOURS", "2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Delays");

            migrationBuilder.DeleteData(
                table: "Parameters",
                keyColumn: "Key",
                keyValue: "EARLY_ORDER_LIMIT_IN_HOURS");

            migrationBuilder.DeleteData(
                table: "Parameters",
                keyColumn: "Key",
                keyValue: "LAST_MINUTE_ORDER_LIMIT_IN_HOURS");

            migrationBuilder.DropColumn(
                name: "SimplePrice",
                table: "DeliveryZones");

            migrationBuilder.DropColumn(
                name: "TourPrice",
                table: "DeliveryZones");

            migrationBuilder.RenameColumn(
                name: "SimplePrice",
                table: "PackingSizes",
                newName: "Price");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeliveryFormNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "Xl");

            migrationBuilder.DeleteData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "Xxl");

            migrationBuilder.DeleteData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "Xxxl");

            migrationBuilder.DeleteData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "Xxxxl");

            migrationBuilder.AddColumn<string>(
                name: "Label",
                table: "Urgencies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Urgencies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Label",
                table: "PackingSizes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "PackingSizes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Distance",
                table: "Deliveries",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ExtraCost",
                table: "Deliveries",
                type: "double precision",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "L",
                columns: new[] { "Label", "Order" },
                values: new object[] { "L = jusqu'à 20kg / 120cm", 3 });

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "M",
                columns: new[] { "Label", "Order" },
                values: new object[] { "M = jusqu'à 10kg / 100cm", 2 });

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "S",
                columns: new[] { "Label", "Order" },
                values: new object[] { "S = jusqu'à 3kg / 45cm", 1 });

            migrationBuilder.InsertData(
                table: "PackingSizes",
                columns: new[] { "Name", "Label", "MaxPackageLength", "MaxWeight", "Order", "Price", "TourPrice" },
                values: new object[,]
                {
                    { "XL", "XL = jusqu'à 30kg / 120cm", 115, 30, 4, 9.0, 6.0 },
                    { "XXL", "XXL = jusqu'à 40kg (sur devis)", 500, 40, 5, 11.0, 8.0 },
                    { "XXXL", "XXXL = jusqu'à 50kg (sur devis)", 500, 50, 6, 13.0, 10.0 },
                    { "XXXXL", "XXXXL = jusqu'à 60kg (sur devis)", 500, 60, 7, 15.0, 12.0 }
                });

            migrationBuilder.UpdateData(
                table: "Urgencies",
                keyColumn: "Name",
                keyValue: "Eco",
                columns: new[] { "Label", "Order" },
                values: new object[] { "Avant 17h le jour-même (Eco)", 1 });

            migrationBuilder.UpdateData(
                table: "Urgencies",
                keyColumn: "Name",
                keyValue: "Standard",
                columns: new[] { "Label", "Order" },
                values: new object[] { "2h30 (Standard)", 2 });

            migrationBuilder.UpdateData(
                table: "Urgencies",
                keyColumn: "Name",
                keyValue: "Urgent",
                columns: new[] { "Label", "Order" },
                values: new object[] { "1h (Urgent)", 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "XL");

            migrationBuilder.DeleteData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "XXL");

            migrationBuilder.DeleteData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "XXXL");

            migrationBuilder.DeleteData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "XXXXL");

            migrationBuilder.DropColumn(
                name: "Label",
                table: "Urgencies");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Urgencies");

            migrationBuilder.DropColumn(
                name: "Label",
                table: "PackingSizes");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "PackingSizes");

            migrationBuilder.DropColumn(
                name: "Distance",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "ExtraCost",
                table: "Deliveries");

            migrationBuilder.InsertData(
                table: "PackingSizes",
                columns: new[] { "Name", "MaxPackageLength", "MaxWeight", "Price", "TourPrice" },
                values: new object[,]
                {
                    { "Xl", 115, 30, 9.0, 6.0 },
                    { "Xxl", 500, 40, 11.0, 8.0 },
                    { "Xxxl", 500, 50, 13.0, 10.0 },
                    { "Xxxxl", 500, 60, 15.0, 12.0 }
                });
        }
    }
}

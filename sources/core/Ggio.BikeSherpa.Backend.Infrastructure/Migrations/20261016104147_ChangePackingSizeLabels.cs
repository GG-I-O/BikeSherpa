using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangePackingSizeLabels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "XXXXL");

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "L",
                column: "Label",
                value: "L = 10kg à 19.9kg & L max 120cm");

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "M",
                column: "Label",
                value: "M = 3kg à 9.9kg & L max 100cm");

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "S",
                column: "Label",
                value: "S = 0kg à 2.9kg & L max 45cm");

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "XL",
                column: "Label",
                value: "XL = 20kg à 29.9kg & L max 120cm");

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "XXL",
                column: "Label",
                value: "XXL = 30kg à 39.9kg & L max 120cm");

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "XXXL",
                column: "Label",
                value: "XXXL = supérieur à 40kg (sur devis)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "L",
                column: "Label",
                value: "L = jusqu'à 20kg / 120cm");

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "M",
                column: "Label",
                value: "M = jusqu'à 10kg / 100cm");

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "S",
                column: "Label",
                value: "S = jusqu'à 3kg / 45cm");

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "XL",
                column: "Label",
                value: "XL = jusqu'à 30kg / 120cm");

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "XXL",
                column: "Label",
                value: "XXL = jusqu'à 40kg (sur devis)");

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "XXXL",
                column: "Label",
                value: "XXXL = jusqu'à 50kg (sur devis)");

            migrationBuilder.InsertData(
                table: "PackingSizes",
                columns: new[] { "Name", "Label", "Order", "SimplePrice", "TourPrice" },
                values: new object[] { "XXXXL", "XXXXL = jusqu'à 60kg (sur devis)", 7, 15.0, 12.0 });
        }
    }
}

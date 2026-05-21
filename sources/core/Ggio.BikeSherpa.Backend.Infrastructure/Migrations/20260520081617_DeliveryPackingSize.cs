using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeliveryPackingSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxPackageLength",
                table: "PackingSizes");

            migrationBuilder.DropColumn(
                name: "MaxWeight",
                table: "PackingSizes");

            migrationBuilder.DropColumn(
                name: "PackingSize",
                table: "Deliveries");

            migrationBuilder.AddColumn<string>(
                name: "PackingSizeName",
                table: "Deliveries",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_PackingSizeName",
                table: "Deliveries",
                column: "PackingSizeName");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_PackingSizes_PackingSizeName",
                table: "Deliveries",
                column: "PackingSizeName",
                principalTable: "PackingSizes",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_PackingSizes_PackingSizeName",
                table: "Deliveries");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_PackingSizeName",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "PackingSizeName",
                table: "Deliveries");

            migrationBuilder.AddColumn<int>(
                name: "MaxPackageLength",
                table: "PackingSizes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxWeight",
                table: "PackingSizes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PackingSize",
                table: "Deliveries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "L",
                columns: new[] { "MaxPackageLength", "MaxWeight" },
                values: new object[] { 105, 20 });

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "M",
                columns: new[] { "MaxPackageLength", "MaxWeight" },
                values: new object[] { 85, 10 });

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "S",
                columns: new[] { "MaxPackageLength", "MaxWeight" },
                values: new object[] { 45, 3 });

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "XL",
                columns: new[] { "MaxPackageLength", "MaxWeight" },
                values: new object[] { 115, 30 });

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "XXL",
                columns: new[] { "MaxPackageLength", "MaxWeight" },
                values: new object[] { 500, 40 });

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "XXXL",
                columns: new[] { "MaxPackageLength", "MaxWeight" },
                values: new object[] { 500, 50 });

            migrationBuilder.UpdateData(
                table: "PackingSizes",
                keyColumn: "Name",
                keyValue: "XXXXL",
                columns: new[] { "MaxPackageLength", "MaxWeight" },
                values: new object[] { 500, 60 });
        }
    }
}

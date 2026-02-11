using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EntityUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address_streetInfo",
                table: "Customers",
                newName: "Address_StreetInfo");

            migrationBuilder.RenameColumn(
                name: "Address_postcode",
                table: "Customers",
                newName: "Address_Postcode");

            migrationBuilder.RenameColumn(
                name: "Address_name",
                table: "Customers",
                newName: "Address_Name");

            migrationBuilder.RenameColumn(
                name: "Address_complement",
                table: "Customers",
                newName: "Address_Complement");

            migrationBuilder.RenameColumn(
                name: "Address_city",
                table: "Customers",
                newName: "Address_City");

            migrationBuilder.RenameColumn(
                name: "Address_streetInfo",
                table: "Couriers",
                newName: "Address_StreetInfo");

            migrationBuilder.RenameColumn(
                name: "Address_postcode",
                table: "Couriers",
                newName: "Address_Postcode");

            migrationBuilder.RenameColumn(
                name: "Address_name",
                table: "Couriers",
                newName: "Address_Name");

            migrationBuilder.RenameColumn(
                name: "Address_complement",
                table: "Couriers",
                newName: "Address_Complement");

            migrationBuilder.RenameColumn(
                name: "Address_city",
                table: "Couriers",
                newName: "Address_City");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address_StreetInfo",
                table: "Customers",
                newName: "Address_streetInfo");

            migrationBuilder.RenameColumn(
                name: "Address_Postcode",
                table: "Customers",
                newName: "Address_postcode");

            migrationBuilder.RenameColumn(
                name: "Address_Name",
                table: "Customers",
                newName: "Address_name");

            migrationBuilder.RenameColumn(
                name: "Address_Complement",
                table: "Customers",
                newName: "Address_complement");

            migrationBuilder.RenameColumn(
                name: "Address_City",
                table: "Customers",
                newName: "Address_city");

            migrationBuilder.RenameColumn(
                name: "Address_StreetInfo",
                table: "Couriers",
                newName: "Address_streetInfo");

            migrationBuilder.RenameColumn(
                name: "Address_Postcode",
                table: "Couriers",
                newName: "Address_postcode");

            migrationBuilder.RenameColumn(
                name: "Address_Name",
                table: "Couriers",
                newName: "Address_name");

            migrationBuilder.RenameColumn(
                name: "Address_Complement",
                table: "Couriers",
                newName: "Address_complement");

            migrationBuilder.RenameColumn(
                name: "Address_City",
                table: "Couriers",
                newName: "Address_city");
        }
    }
}

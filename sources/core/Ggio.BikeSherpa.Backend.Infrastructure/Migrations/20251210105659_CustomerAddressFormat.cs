using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CustomerAddressFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Clients",
                newName: "Address_streetInfo");

            migrationBuilder.AddColumn<string>(
                name: "Address_city",
                table: "Clients",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_complement",
                table: "Clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_name",
                table: "Clients",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_postcode",
                table: "Clients",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_city",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Address_complement",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Address_name",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Address_postcode",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "Address_streetInfo",
                table: "Clients",
                newName: "Address");
        }
    }
}

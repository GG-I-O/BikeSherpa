using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class customer_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VatNumber",
                table: "Customers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VatNumber",
                table: "Customers");
        }
    }
}

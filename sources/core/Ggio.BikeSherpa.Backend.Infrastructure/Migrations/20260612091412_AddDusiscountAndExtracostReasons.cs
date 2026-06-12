using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDusiscountAndExtracostReasons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiscountReason",
                table: "Deliveries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraCostReason",
                table: "Deliveries",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountReason",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "ExtraCostReason",
                table: "Deliveries");
        }
    }
}

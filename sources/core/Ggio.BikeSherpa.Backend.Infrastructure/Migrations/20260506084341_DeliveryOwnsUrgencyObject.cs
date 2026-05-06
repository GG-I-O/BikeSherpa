using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeliveryOwnsUrgencyObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Urgency",
                table: "Deliveries");

            migrationBuilder.AddColumn<string>(
                name: "UrgencyName",
                table: "Deliveries",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_UrgencyName",
                table: "Deliveries",
                column: "UrgencyName");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Urgencies_UrgencyName",
                table: "Deliveries",
                column: "UrgencyName",
                principalTable: "Urgencies",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_Urgencies_UrgencyName",
                table: "Deliveries");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_UrgencyName",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "UrgencyName",
                table: "Deliveries");

            migrationBuilder.AddColumn<string>(
                name: "Urgency",
                table: "Deliveries",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

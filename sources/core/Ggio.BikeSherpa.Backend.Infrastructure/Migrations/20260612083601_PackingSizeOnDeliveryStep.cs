using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PackingSizeOnDeliveryStep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "PackingSizeName",
                table: "DeliverySteps",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_DeliverySteps_PackingSizeName",
                table: "DeliverySteps",
                column: "PackingSizeName");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliverySteps_PackingSizes_PackingSizeName",
                table: "DeliverySteps",
                column: "PackingSizeName",
                principalTable: "PackingSizes",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliverySteps_PackingSizes_PackingSizeName",
                table: "DeliverySteps");

            migrationBuilder.DropIndex(
                name: "IX_DeliverySteps_PackingSizeName",
                table: "DeliverySteps");

            migrationBuilder.DropColumn(
                name: "PackingSizeName",
                table: "DeliverySteps");

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
    }
}

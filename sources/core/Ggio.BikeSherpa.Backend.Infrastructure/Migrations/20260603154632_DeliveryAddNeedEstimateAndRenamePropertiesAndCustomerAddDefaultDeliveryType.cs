using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeliveryAddNeedEstimateAndRenamePropertiesAndCustomerAddDefaultDeliveryType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Distance",
                table: "Deliveries");

            migrationBuilder.RenameColumn(
                name: "ReportId",
                table: "Deliveries",
                newName: "CustomerReference");

            migrationBuilder.AddColumn<bool>(
                name: "NeedEstimate",
                table: "Deliveries",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DefaultDeliveryType",
                table: "Customers",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NeedEstimate",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "DefaultDeliveryType",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "CustomerReference",
                table: "Deliveries",
                newName: "ReportId");

            migrationBuilder.AddColumn<double>(
                name: "Distance",
                table: "Deliveries",
                type: "double precision",
                nullable: true);
        }
    }
}

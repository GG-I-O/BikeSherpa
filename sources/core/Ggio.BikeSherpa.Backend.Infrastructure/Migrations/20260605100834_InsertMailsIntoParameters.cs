using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InsertMailsIntoParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Parameters",
                columns: new[] { "Key", "Value" },
                values: new object[,]
                {
                    { "SIMPLE_DELIVERY_MAIL_SUBJECT", "Delivery Ready for Pickup" },
                    { "SIMPLE_DELIVERY_MAIL_TEMPLATE", "Your delivery is ready for pickup at {pickupLocation} on {pickupDate}. {deliverycode} { pickupaddress} { destinationaddress} {PickupDate} {LoadingSlot} " },
                    { "TOUR_DELIVERY_MAIL_SUBJECT", "Tour delivery Ready for Pickup" },
                    { "TOUR_DELIVERY_MAIL_TEMPLATE", "Your delivery is ready for pickup at {pickupLocation} on {pickupDate}. {deliverycode} { pickupaddress} { destinationaddress} {PickupDate} {LoadingSlot} " }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Parameters",
                keyColumn: "Key",
                keyValue: "SIMPLE_DELIVERY_MAIL_SUBJECT");

            migrationBuilder.DeleteData(
                table: "Parameters",
                keyColumn: "Key",
                keyValue: "SIMPLE_DELIVERY_MAIL_TEMPLATE");

            migrationBuilder.DeleteData(
                table: "Parameters",
                keyColumn: "Key",
                keyValue: "TOUR_DELIVERY_MAIL_SUBJECT");

            migrationBuilder.DeleteData(
                table: "Parameters",
                keyColumn: "Key",
                keyValue: "TOUR_DELIVERY_MAIL_TEMPLATE");
        }
    }
}

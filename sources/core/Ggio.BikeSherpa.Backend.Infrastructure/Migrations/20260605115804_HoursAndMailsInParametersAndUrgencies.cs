using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HoursAndMailsInParametersAndUrgencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastHourToOrder",
                table: "Urgencies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Parameters",
                columns: new[] { "Key", "Value" },
                values: new object[,]
                {
                    { "LAST_HOUR_TO_ORDER", "15" },
                    { "SIMPLE_DELIVERY_MAIL_SUBJECT", "Delivery Ready for Pickup" },
                    { "SIMPLE_DELIVERY_MAIL_TEMPLATE", "Your delivery is ready for pickup at {pickupLocation} on {pickupDate}. {deliverycode} { pickupaddress} { destinationaddress} {PickupDate} {LoadingSlot} " },
                    { "TOUR_DELIVERY_MAIL_SUBJECT", "Tour delivery Ready for Pickup" },
                    { "TOUR_DELIVERY_MAIL_TEMPLATE", "Your delivery is ready for pickup at {pickupLocation} on {pickupDate}. {deliverycode} { pickupaddress} { destinationaddress} {PickupDate} {LoadingSlot} " },
                    { "WORK_END_DATE", "0001-01-01T19:00:00" },
                    { "WORK_START_DATE", "0001-01-01T08:00:00" }
                });

            migrationBuilder.UpdateData(
                table: "Urgencies",
                keyColumn: "Name",
                keyValue: "Eco",
                column: "LastHourToOrder",
                value: 12);

            migrationBuilder.UpdateData(
                table: "Urgencies",
                keyColumn: "Name",
                keyValue: "Standard",
                column: "LastHourToOrder",
                value: 15);

            migrationBuilder.UpdateData(
                table: "Urgencies",
                keyColumn: "Name",
                keyValue: "Urgent",
                column: "LastHourToOrder",
                value: 20);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Parameters",
                keyColumn: "Key",
                keyValue: "LAST_HOUR_TO_ORDER");

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

            migrationBuilder.DeleteData(
                table: "Parameters",
                keyColumn: "Key",
                keyValue: "WORK_END_DATE");

            migrationBuilder.DeleteData(
                table: "Parameters",
                keyColumn: "Key",
                keyValue: "WORK_START_DATE");

            migrationBuilder.DropColumn(
                name: "LastHourToOrder",
                table: "Urgencies");
        }
    }
}

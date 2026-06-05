using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LastHourToOrderInParametersAndUrgencies : Migration
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
                values: new object[] { "LAST_HOUR_TO_ORDER", "15" });

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
                value: 14);

            migrationBuilder.UpdateData(
                table: "Urgencies",
                keyColumn: "Name",
                keyValue: "Urgent",
                column: "LastHourToOrder",
                value: 16);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Parameters",
                keyColumn: "Key",
                keyValue: "LAST_HOUR_TO_ORDER");

            migrationBuilder.DropColumn(
                name: "LastHourToOrder",
                table: "Urgencies");
        }
    }
}

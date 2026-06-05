using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WorkHoursAndSeedingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Parameters",
                columns: new[] { "Key", "Value" },
                values: new object[,]
                {
                    { "WORK_END_DATE", "0001-01-01T19:00:00" },
                    { "WORK_START_DATE", "0001-01-01T08:00:00" }
                });

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
                keyValue: "WORK_END_DATE");

            migrationBuilder.DeleteData(
                table: "Parameters",
                keyColumn: "Key",
                keyValue: "WORK_START_DATE");

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
    }
}

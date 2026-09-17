using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixingZoneCityCharacter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DeliveryZoneCities",
                keyColumn: "Name",
                keyValue: "Saint-Martin-d’Hères");

            migrationBuilder.InsertData(
                table: "DeliveryZoneCities",
                columns: new[] { "Name", "DeliveryZoneName" },
                values: new object[] { "Saint-Martin-d'Hères", "Limitrophe" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DeliveryZoneCities",
                keyColumn: "Name",
                keyValue: "Saint-Martin-d'Hères");

            migrationBuilder.InsertData(
                table: "DeliveryZoneCities",
                columns: new[] { "Name", "DeliveryZoneName" },
                values: new object[] { "Saint-Martin-d’Hères", "Limitrophe" });
        }
    }
}

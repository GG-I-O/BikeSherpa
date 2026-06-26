using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StackholderInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Parameters",
                columns: new[] { "Key", "Value" },
                values: new object[] { "STACK_HOLDER_INFO", "{\"CompanyName\":\"Compagnie\",\"Email\":\"contact@company.com\",\"Phone\":\"phone\",\"Adresse\":\"address\",\"Label\":\"the company\"}" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Parameters",
                keyColumn: "Key",
                keyValue: "STACK_HOLDER_INFO");
        }
    }
}

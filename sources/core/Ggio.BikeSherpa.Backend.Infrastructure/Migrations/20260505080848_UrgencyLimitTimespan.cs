using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UrgencyLimitTimespan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "AddTimeLimit",
                table: "Urgencies",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "FixedTimeLimit",
                table: "Urgencies",
                type: "interval",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Urgencies",
                keyColumn: "Name",
                keyValue: "Eco",
                columns: new[] { "AddTimeLimit", "FixedTimeLimit" },
                values: new object[] { null, new TimeSpan(0, 17, 0, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Urgencies",
                keyColumn: "Name",
                keyValue: "Standard",
                columns: new[] { "AddTimeLimit", "FixedTimeLimit" },
                values: new object[] { new TimeSpan(0, 2, 30, 0, 0), null });

            migrationBuilder.UpdateData(
                table: "Urgencies",
                keyColumn: "Name",
                keyValue: "Urgent",
                columns: new[] { "AddTimeLimit", "FixedTimeLimit" },
                values: new object[] { new TimeSpan(0, 1, 0, 0, 0), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddTimeLimit",
                table: "Urgencies");

            migrationBuilder.DropColumn(
                name: "FixedTimeLimit",
                table: "Urgencies");
        }
    }
}

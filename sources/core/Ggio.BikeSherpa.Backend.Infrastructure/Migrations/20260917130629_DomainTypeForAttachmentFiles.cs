using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ggio.BikeSherpa.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DomainTypeForAttachmentFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentFilePaths",
                table: "DeliverySteps");

            migrationBuilder.CreateTable(
                name: "DeliveryStepAttachmentFiles",
                columns: table => new
                {
                    DeliveryStepId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DomainType = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryStepAttachmentFiles", x => new { x.DeliveryStepId, x.Id });
                    table.ForeignKey(
                        name: "FK_DeliveryStepAttachmentFiles_DeliverySteps_DeliveryStepId",
                        column: x => x.DeliveryStepId,
                        principalTable: "DeliverySteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryStepAttachmentFiles");

            migrationBuilder.AddColumn<string[]>(
                name: "AttachmentFilePaths",
                table: "DeliverySteps",
                type: "text[]",
                nullable: true);
        }
    }
}

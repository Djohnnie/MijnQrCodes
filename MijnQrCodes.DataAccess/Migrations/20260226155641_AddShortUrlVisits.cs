using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MijnQrCodes.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddShortUrlVisits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SHORT_URL_VISITS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SysId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShortUrlId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SHORT_URL_VISITS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SHORT_URL_VISITS_SHORT_URLS_ShortUrlId",
                        column: x => x.ShortUrlId,
                        principalTable: "SHORT_URLS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SHORT_URL_VISITS_ShortUrlId",
                table: "SHORT_URL_VISITS",
                column: "ShortUrlId");

            migrationBuilder.CreateIndex(
                name: "IX_SHORT_URL_VISITS_SysId",
                table: "SHORT_URL_VISITS",
                column: "SysId",
                unique: true)
                .Annotation("SqlServer:Clustered", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SHORT_URL_VISITS");
        }
    }
}

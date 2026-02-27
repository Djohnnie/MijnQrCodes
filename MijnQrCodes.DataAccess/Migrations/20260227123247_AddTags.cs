using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MijnQrCodes.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TAGS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SysId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "#6366f1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TAGS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SHORT_URL_TAGS",
                columns: table => new
                {
                    ShortUrlId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SHORT_URL_TAGS", x => new { x.ShortUrlId, x.TagId });
                    table.ForeignKey(
                        name: "FK_SHORT_URL_TAGS_SHORT_URLS_ShortUrlId",
                        column: x => x.ShortUrlId,
                        principalTable: "SHORT_URLS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SHORT_URL_TAGS_TAGS_TagId",
                        column: x => x.TagId,
                        principalTable: "TAGS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SHORT_URL_TAGS_TagId",
                table: "SHORT_URL_TAGS",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_TAGS_Name",
                table: "TAGS",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TAGS_SysId",
                table: "TAGS",
                column: "SysId",
                unique: true)
                .Annotation("SqlServer:Clustered", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SHORT_URL_TAGS");

            migrationBuilder.DropTable(
                name: "TAGS");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MijnQrCodes.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddQrCodeColors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackgroundColor",
                table: "SHORT_URLS",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "#FFFFFF");

            migrationBuilder.AddColumn<string>(
                name: "FinderPatternColor",
                table: "SHORT_URLS",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "#212121");

            migrationBuilder.AddColumn<string>(
                name: "ForegroundColor",
                table: "SHORT_URLS",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "#212121");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundColor",
                table: "SHORT_URLS");

            migrationBuilder.DropColumn(
                name: "FinderPatternColor",
                table: "SHORT_URLS");

            migrationBuilder.DropColumn(
                name: "ForegroundColor",
                table: "SHORT_URLS");
        }
    }
}

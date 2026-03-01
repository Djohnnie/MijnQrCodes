using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MijnQrCodes.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCenterImageColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CenterImageColor",
                table: "SHORT_URLS",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CenterImageColor",
                table: "SHORT_URLS");
        }
    }
}

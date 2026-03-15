using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAllowedFormate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AllowedFormats",
                table: "Sports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedFormats",
                table: "Sports");
        }
    }
}

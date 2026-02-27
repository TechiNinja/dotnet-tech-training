using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventAndEventCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TournamentType",
                table: "EventCategories");

            migrationBuilder.AddColumn<int>(
                name: "MaxParticipantsCount",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TournamentType",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxParticipantsCount",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TournamentType",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "TournamentType",
                table: "EventCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

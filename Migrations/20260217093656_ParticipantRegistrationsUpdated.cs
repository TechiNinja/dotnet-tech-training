using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class ParticipantRegistrationsUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantRegistrations_Events_EventId",
                table: "ParticipantRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_ParticipantRegistrations_EventId",
                table: "ParticipantRegistrations");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "ParticipantRegistrations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "ParticipantRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantRegistrations_EventId",
                table: "ParticipantRegistrations",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantRegistrations_Events_EventId",
                table: "ParticipantRegistrations",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

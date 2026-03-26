using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventRequestId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventRequestId",
                table: "Events",
                column: "EventRequestId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventRequests_EventRequestId",
                table: "Events",
                column: "EventRequestId",
                principalTable: "EventRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventRequests_EventRequestId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_EventRequestId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EventRequestId",
                table: "Events");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheckIN.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketPermissionInUserEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketTypes_AspNetUsers_UserId",
                table: "TicketTypes");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TicketTypes",
                newName: "UserEventId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketTypes_UserId",
                table: "TicketTypes",
                newName: "IX_TicketTypes_UserEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTypes_UserEvents_UserEventId",
                table: "TicketTypes",
                column: "UserEventId",
                principalTable: "UserEvents",
                principalColumn: "UserEventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketTypes_UserEvents_UserEventId",
                table: "TicketTypes");

            migrationBuilder.RenameColumn(
                name: "UserEventId",
                table: "TicketTypes",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketTypes_UserEventId",
                table: "TicketTypes",
                newName: "IX_TicketTypes_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTypes_AspNetUsers_UserId",
                table: "TicketTypes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}

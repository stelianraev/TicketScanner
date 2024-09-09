using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheckIN.Migrations
{
    /// <inheritdoc />
    public partial class AddUserEventPermissionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserEventTicketPermission_AspNetUsers_UserId",
                table: "UserEventTicketPermission");

            migrationBuilder.DropForeignKey(
                name: "FK_UserEventTicketPermission_TicketTypes_TicketTypeId",
                table: "UserEventTicketPermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEventTicketPermission",
                table: "UserEventTicketPermission");

            migrationBuilder.RenameTable(
                name: "UserEventTicketPermission",
                newName: "UserEventPermissions");

            migrationBuilder.RenameIndex(
                name: "IX_UserEventTicketPermission_UserId",
                table: "UserEventPermissions",
                newName: "IX_UserEventPermissions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserEventTicketPermission_TicketTypeId",
                table: "UserEventPermissions",
                newName: "IX_UserEventPermissions_TicketTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEventPermissions",
                table: "UserEventPermissions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserEventPermissions_AspNetUsers_UserId",
                table: "UserEventPermissions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserEventPermissions_TicketTypes_TicketTypeId",
                table: "UserEventPermissions",
                column: "TicketTypeId",
                principalTable: "TicketTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserEventPermissions_AspNetUsers_UserId",
                table: "UserEventPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserEventPermissions_TicketTypes_TicketTypeId",
                table: "UserEventPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEventPermissions",
                table: "UserEventPermissions");

            migrationBuilder.RenameTable(
                name: "UserEventPermissions",
                newName: "UserEventTicketPermission");

            migrationBuilder.RenameIndex(
                name: "IX_UserEventPermissions_UserId",
                table: "UserEventTicketPermission",
                newName: "IX_UserEventTicketPermission_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserEventPermissions_TicketTypeId",
                table: "UserEventTicketPermission",
                newName: "IX_UserEventTicketPermission_TicketTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEventTicketPermission",
                table: "UserEventTicketPermission",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserEventTicketPermission_AspNetUsers_UserId",
                table: "UserEventTicketPermission",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserEventTicketPermission_TicketTypes_TicketTypeId",
                table: "UserEventTicketPermission",
                column: "TicketTypeId",
                principalTable: "TicketTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

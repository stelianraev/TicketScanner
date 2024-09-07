using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheckIN.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketPermissionInUserEvent1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketTypes_UserEvents_UserEventId",
                table: "TicketTypes");

            migrationBuilder.DropIndex(
                name: "IX_TicketTypes_UserEventId",
                table: "TicketTypes");

            migrationBuilder.DropColumn(
                name: "UserEventId",
                table: "TicketTypes");

            migrationBuilder.AddColumn<string>(
                name: "TicketPermissions",
                table: "UserEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketPermissions",
                table: "UserEvents");

            migrationBuilder.AddColumn<Guid>(
                name: "UserEventId",
                table: "TicketTypes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketTypes_UserEventId",
                table: "TicketTypes",
                column: "UserEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTypes_UserEvents_UserEventId",
                table: "TicketTypes",
                column: "UserEventId",
                principalTable: "UserEvents",
                principalColumn: "UserEventId");
        }
    }
}

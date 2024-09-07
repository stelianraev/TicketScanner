using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheckIN.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketTypeToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "TicketTypes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketTypes_UserId",
                table: "TicketTypes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTypes_AspNetUsers_UserId",
                table: "TicketTypes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketTypes_AspNetUsers_UserId",
                table: "TicketTypes");

            migrationBuilder.DropIndex(
                name: "IX_TicketTypes_UserId",
                table: "TicketTypes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TicketTypes");
        }
    }
}

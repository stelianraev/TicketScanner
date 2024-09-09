using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheckIN.Migrations
{
    /// <inheritdoc />
    public partial class UserChangesAddTicketsPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketPermissions",
                table: "UserEvents");

            migrationBuilder.RenameColumn(
                name: "Permision",
                table: "AspNetUsers",
                newName: "Permission");

            migrationBuilder.CreateTable(
                name: "EventTicketPermission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketTipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketTypePermisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTicketPermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventTicketPermission_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventTicketPermission_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventTicketPermission_TicketTypes_TicketTypePermisionId",
                        column: x => x.TicketTypePermisionId,
                        principalTable: "TicketTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventTicketPermission_EventId",
                table: "EventTicketPermission",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTicketPermission_TicketTypePermisionId",
                table: "EventTicketPermission",
                column: "TicketTypePermisionId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTicketPermission_UserId",
                table: "EventTicketPermission",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventTicketPermission");

            migrationBuilder.RenameColumn(
                name: "Permission",
                table: "AspNetUsers",
                newName: "Permision");

            migrationBuilder.AddColumn<string>(
                name: "TicketPermissions",
                table: "UserEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

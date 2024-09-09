using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheckIN.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTicketTypePermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventTicketPermission");

            migrationBuilder.CreateTable(
                name: "UserEventTicketPermission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketTipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEventTicketPermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserEventTicketPermission_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserEventTicketPermission_TicketTypes_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "TicketTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserEventTicketPermission_TicketTypeId",
                table: "UserEventTicketPermission",
                column: "TicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEventTicketPermission_UserId",
                table: "UserEventTicketPermission",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserEventTicketPermission");

            migrationBuilder.CreateTable(
                name: "EventTicketPermission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketTypePermisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketTipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                        onDelete: ReferentialAction.Cascade);
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
    }
}

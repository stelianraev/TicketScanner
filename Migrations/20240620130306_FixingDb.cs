using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheckIN.Migrations
{
    /// <inheritdoc />
    public partial class FixingDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Customers");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "UserCustomer",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserCustomer_OwnerId",
                table: "UserCustomer",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCustomer_AspNetUsers_OwnerId",
                table: "UserCustomer",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCustomer_AspNetUsers_OwnerId",
                table: "UserCustomer");

            migrationBuilder.DropIndex(
                name: "IX_UserCustomer_OwnerId",
                table: "UserCustomer");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "UserCustomer");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

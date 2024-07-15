using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheckIN.Migrations
{
    /// <inheritdoc />
    public partial class AddSelectedToTitoAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedTitoAccountId",
                table: "Customers");

            migrationBuilder.AddColumn<bool>(
                name: "IsSelected",
                table: "TitoAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSelected",
                table: "TitoAccounts");

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedTitoAccountId",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}

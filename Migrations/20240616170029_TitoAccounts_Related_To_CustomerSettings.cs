using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheckIN.Migrations
{
    /// <inheritdoc />
    public partial class TitoAccounts_Related_To_CustomerSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TitoAccounts_Customers_CustomerId",
                table: "TitoAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserEvents_AspNetUsers_UserId",
                table: "UserEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_UserEvents_Events_EventId",
                table: "UserEvents");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "TitoAccounts",
                newName: "CustomerSettingsId");

            migrationBuilder.RenameIndex(
                name: "IX_TitoAccounts_CustomerId",
                table: "TitoAccounts",
                newName: "IX_TitoAccounts_CustomerSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_TitoAccounts_CustomerSettings_CustomerSettingsId",
                table: "TitoAccounts",
                column: "CustomerSettingsId",
                principalTable: "CustomerSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserEvents_AspNetUsers_UserId",
                table: "UserEvents",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserEvents_Events_EventId",
                table: "UserEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TitoAccounts_CustomerSettings_CustomerSettingsId",
                table: "TitoAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserEvents_AspNetUsers_UserId",
                table: "UserEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_UserEvents_Events_EventId",
                table: "UserEvents");

            migrationBuilder.RenameColumn(
                name: "CustomerSettingsId",
                table: "TitoAccounts",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_TitoAccounts_CustomerSettingsId",
                table: "TitoAccounts",
                newName: "IX_TitoAccounts_CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TitoAccounts_Customers_CustomerId",
                table: "TitoAccounts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CanonicalId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserEvents_AspNetUsers_UserId",
                table: "UserEvents",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserEvents_Events_EventId",
                table: "UserEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheckIN.Migrations
{
    /// <inheritdoc />
    public partial class AddQRCodeToTIcket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsScanned",
                table: "Tickets",
                newName: "IsCheckedIn");

            migrationBuilder.AddColumn<byte[]>(
                name: "QrCodeImage",
                table: "Tickets",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QrCodeImage",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "IsCheckedIn",
                table: "Tickets",
                newName: "IsScanned");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheckIN.Migrations
{
    /// <inheritdoc />
    public partial class ExtendEventTableIsSelectedProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSelected",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSelected",
                table: "Events");
        }
    }
}

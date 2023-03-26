using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class AddLocationToMessageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "Messagedata",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "Messagedata",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Messagedata");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Messagedata");
        }
    }
}

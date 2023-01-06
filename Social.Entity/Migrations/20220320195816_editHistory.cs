using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class editHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgeFrom",
                table: "FilteringAccordingToAgeHistory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AgeTo",
                table: "FilteringAccordingToAgeHistory",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgeFrom",
                table: "FilteringAccordingToAgeHistory");

            migrationBuilder.DropColumn(
                name: "AgeTo",
                table: "FilteringAccordingToAgeHistory");
        }
    }
}

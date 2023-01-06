using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class Editinterestes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAcive",
                table: "Interests");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Interests",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Interests");

            migrationBuilder.AddColumn<bool>(
                name: "IsAcive",
                table: "Interests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

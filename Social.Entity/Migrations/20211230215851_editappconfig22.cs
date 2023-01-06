using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class editappconfig22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "AppConfigrations");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AppConfigrations",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AppConfigrations");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "AppConfigrations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

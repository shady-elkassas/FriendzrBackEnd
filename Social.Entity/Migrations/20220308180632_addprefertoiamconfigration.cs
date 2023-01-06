using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class addprefertoiamconfigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserIAM_MaxLength",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserIAM_MinLength",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserIPreferTo_MaxLength",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserIPreferTo_MinLength",
                table: "AppConfigrations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserIAM_MaxLength",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "UserIAM_MinLength",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "UserIPreferTo_MaxLength",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "UserIPreferTo_MinLength",
                table: "AppConfigrations");
        }
    }
}

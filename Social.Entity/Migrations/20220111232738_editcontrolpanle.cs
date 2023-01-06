using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class editcontrolpanle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventCreationLimitNumber_MaxLength",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventCreationLimitNumber_MinLength",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EventTimeValidation_MaxLength",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EventTimeValidation_MinLength",
                table: "AppConfigrations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventCreationLimitNumber_MaxLength",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "EventCreationLimitNumber_MinLength",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "EventTimeValidation_MaxLength",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "EventTimeValidation_MinLength",
                table: "AppConfigrations");
        }
    }
}

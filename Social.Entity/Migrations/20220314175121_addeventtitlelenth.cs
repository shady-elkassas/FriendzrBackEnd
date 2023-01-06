using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class addeventtitlelenth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventTitle_MaxLength",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventTitle_MinLength",
                table: "AppConfigrations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventTitle_MaxLength",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "EventTitle_MinLength",
                table: "AppConfigrations");
        }
    }
}

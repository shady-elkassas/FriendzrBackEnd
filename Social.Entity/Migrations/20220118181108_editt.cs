using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class editt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ChatGroupSubscribers");

            migrationBuilder.AddColumn<int>(
                name: "LeaveGroup",
                table: "ChatGroupSubscribers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeaveGroup",
                table: "ChatGroupSubscribers");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ChatGroupSubscribers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

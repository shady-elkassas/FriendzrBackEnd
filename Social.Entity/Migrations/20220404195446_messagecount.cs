using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class messagecount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ToUserNotreadcount",
                table: "UserMessages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserNotreadcount",
                table: "UserMessages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserNotreadcount",
                table: "EventChatAttend",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserNotreadcount",
                table: "ChatGroupSubscribers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ToUserNotreadcount",
                table: "UserMessages");

            migrationBuilder.DropColumn(
                name: "UserNotreadcount",
                table: "UserMessages");

            migrationBuilder.DropColumn(
                name: "UserNotreadcount",
                table: "EventChatAttend");

            migrationBuilder.DropColumn(
                name: "UserNotreadcount",
                table: "ChatGroupSubscribers");
        }
    }
}

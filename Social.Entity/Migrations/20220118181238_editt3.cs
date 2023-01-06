using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class editt3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "ChatGroupSubscribers");

            migrationBuilder.AddColumn<int>(
                name: "IsAdminGroup",
                table: "ChatGroupSubscribers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdminGroup",
                table: "ChatGroupSubscribers");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ChatGroupSubscribers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

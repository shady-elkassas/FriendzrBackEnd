using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class isrecivedremindernotification2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isrecivedremindernotification",
                table: "eventattend");

            migrationBuilder.AddColumn<bool>(
                name: "isrecivedremindernotification",
                table: "EventChatAttend",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isrecivedremindernotification",
                table: "EventChatAttend");

            migrationBuilder.AddColumn<bool>(
                name: "isrecivedremindernotification",
                table: "eventattend",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

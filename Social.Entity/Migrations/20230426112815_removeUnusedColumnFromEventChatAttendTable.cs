using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class removeUnusedColumnFromEventChatAttendTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketMasterEventDataid",
                table: "EventChatAttend");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TicketMasterEventDataid",
                table: "EventChatAttend",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

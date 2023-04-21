using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class addEventCodeType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messagedata_EventChatAttend_EventChatAttendId",
                table: "Messagedata");

            migrationBuilder.AddColumn<int>(
                name: "TicketMasterEventDataid",
                table: "Messagedata",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventTypeCode",
                table: "EventData",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TicketMasterEventDataid",
                table: "EventChatAttend",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Messagedata_EventChatAttend_EventChatAttendId",
                table: "Messagedata",
                column: "EventChatAttendId",
                principalTable: "EventChatAttend",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messagedata_EventChatAttend_EventChatAttendId",
                table: "Messagedata");

            migrationBuilder.DropColumn(
                name: "TicketMasterEventDataid",
                table: "Messagedata");

            migrationBuilder.DropColumn(
                name: "EventTypeCode",
                table: "EventData");

            migrationBuilder.DropColumn(
                name: "TicketMasterEventDataid",
                table: "EventChatAttend");

            migrationBuilder.AddForeignKey(
                name: "FK_Messagedata_EventChatAttend_EventChatAttendId",
                table: "Messagedata",
                column: "EventChatAttendId",
                principalTable: "EventChatAttend",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

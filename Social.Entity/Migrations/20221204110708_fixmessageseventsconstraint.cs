using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class fixmessageseventsconstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messagedata_EventChatAttend_EventChatAttendId",
                table: "Messagedata");

            migrationBuilder.DropForeignKey(
                name: "FK_Messagedata_EventData_EventDataid",
                table: "Messagedata");

            migrationBuilder.AddForeignKey(
                name: "FK_Messagedata_EventChatAttend_EventChatAttendId",
                table: "Messagedata",
                column: "EventChatAttendId",
                principalTable: "EventChatAttend",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messagedata_EventData_EventDataid",
                table: "Messagedata",
                column: "EventDataid",
                principalTable: "EventData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messagedata_EventChatAttend_EventChatAttendId",
                table: "Messagedata");

            migrationBuilder.DropForeignKey(
                name: "FK_Messagedata_EventData_EventDataid",
                table: "Messagedata");

            migrationBuilder.AddForeignKey(
                name: "FK_Messagedata_EventChatAttend_EventChatAttendId",
                table: "Messagedata",
                column: "EventChatAttendId",
                principalTable: "EventChatAttend",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messagedata_EventData_EventDataid",
                table: "Messagedata",
                column: "EventDataid",
                principalTable: "EventData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

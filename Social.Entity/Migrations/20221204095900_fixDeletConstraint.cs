using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class fixDeletConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventChatAttend_EventData_EventDataid",
                table: "EventChatAttend");

            migrationBuilder.DropForeignKey(
                name: "FK_Messagedata_EventData_EventDataid",
                table: "Messagedata");

            migrationBuilder.DropForeignKey(
                name: "FK_SkippedEvents_EventData_EventId",
                table: "SkippedEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_EventChatAttend_EventData_EventDataid",
                table: "EventChatAttend",
                column: "EventDataid",
                principalTable: "EventData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messagedata_EventData_EventDataid",
                table: "Messagedata",
                column: "EventDataid",
                principalTable: "EventData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SkippedEvents_EventData_EventId",
                table: "SkippedEvents",
                column: "EventId",
                principalTable: "EventData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventChatAttend_EventData_EventDataid",
                table: "EventChatAttend");

            migrationBuilder.DropForeignKey(
                name: "FK_Messagedata_EventData_EventDataid",
                table: "Messagedata");

            migrationBuilder.DropForeignKey(
                name: "FK_SkippedEvents_EventData_EventId",
                table: "SkippedEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_EventChatAttend_EventData_EventDataid",
                table: "EventChatAttend",
                column: "EventDataid",
                principalTable: "EventData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messagedata_EventData_EventDataid",
                table: "Messagedata",
                column: "EventDataid",
                principalTable: "EventData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SkippedEvents_EventData_EventId",
                table: "SkippedEvents",
                column: "EventId",
                principalTable: "EventData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

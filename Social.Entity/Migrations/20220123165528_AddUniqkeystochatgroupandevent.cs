using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class AddUniqkeystochatgroupandevent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventChatAttend_EventDataid",
                table: "EventChatAttend");

            migrationBuilder.DropIndex(
                name: "IX_eventattend_EventDataid",
                table: "eventattend");

            migrationBuilder.DropIndex(
                name: "IX_ChatGroupSubscribers_ChatGroupID",
                table: "ChatGroupSubscribers");

            migrationBuilder.CreateIndex(
                name: "IX_EventChatAttend_EventDataid_UserattendId",
                table: "EventChatAttend",
                columns: new[] { "EventDataid", "UserattendId" },
                unique: true,
                filter: "[UserattendId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_eventattend_EventDataid_UserattendId",
                table: "eventattend",
                columns: new[] { "EventDataid", "UserattendId" },
                unique: true,
                filter: "[UserattendId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroupSubscribers_ChatGroupID_UserID",
                table: "ChatGroupSubscribers",
                columns: new[] { "ChatGroupID", "UserID" },
                unique: true,
                filter: "[UserID] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventChatAttend_EventDataid_UserattendId",
                table: "EventChatAttend");

            migrationBuilder.DropIndex(
                name: "IX_eventattend_EventDataid_UserattendId",
                table: "eventattend");

            migrationBuilder.DropIndex(
                name: "IX_ChatGroupSubscribers_ChatGroupID_UserID",
                table: "ChatGroupSubscribers");

            migrationBuilder.CreateIndex(
                name: "IX_EventChatAttend_EventDataid",
                table: "EventChatAttend",
                column: "EventDataid");

            migrationBuilder.CreateIndex(
                name: "IX_eventattend_EventDataid",
                table: "eventattend",
                column: "EventDataid");

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroupSubscribers_ChatGroupID",
                table: "ChatGroupSubscribers",
                column: "ChatGroupID");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class cascadeDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatGroupSubscribers_ChatGroups_ChatGroupID",
                table: "ChatGroupSubscribers");

            migrationBuilder.DropForeignKey(
                name: "FK_Messagedata_UserDetails_UserId",
                table: "Messagedata");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatGroupSubscribers_ChatGroups_ChatGroupID",
                table: "ChatGroupSubscribers",
                column: "ChatGroupID",
                principalTable: "ChatGroups",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messagedata_UserDetails_UserId",
                table: "Messagedata",
                column: "UserId",
                principalTable: "UserDetails",
                principalColumn: "PrimaryId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatGroupSubscribers_ChatGroups_ChatGroupID",
                table: "ChatGroupSubscribers");

            migrationBuilder.DropForeignKey(
                name: "FK_Messagedata_UserDetails_UserId",
                table: "Messagedata");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatGroupSubscribers_ChatGroups_ChatGroupID",
                table: "ChatGroupSubscribers",
                column: "ChatGroupID",
                principalTable: "ChatGroups",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messagedata_UserDetails_UserId",
                table: "Messagedata",
                column: "UserId",
                principalTable: "UserDetails",
                principalColumn: "PrimaryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

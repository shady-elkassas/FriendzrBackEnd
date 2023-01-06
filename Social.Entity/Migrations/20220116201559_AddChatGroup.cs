using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class AddChatGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChatGroupID",
                table: "Messagedata",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChatGroups",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false,defaultValue:"NewID()"),
                    Name = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    UserID = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    RegistrationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatGroups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChatGroups_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatGroupSubscribers",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false, defaultValue: "NewID()"),

                    JoinDateTime = table.Column<DateTime>(nullable: false),
                    LeaveDateTime = table.Column<DateTime>(nullable: true),
                    RemovedDateTime = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    ChatGroupID = table.Column<Guid>(nullable: false),
                    UserID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatGroupSubscribers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChatGroupSubscribers_ChatGroups_ChatGroupID",
                        column: x => x.ChatGroupID,
                        principalTable: "ChatGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatGroupSubscribers_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateIndex(
                name: "IX_Messagedata_ChatGroupID",
                table: "Messagedata",
                column: "ChatGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroups_UserID",
                table: "ChatGroups",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroupSubscribers_ChatGroupID",
                table: "ChatGroupSubscribers",
                column: "ChatGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroupSubscribers_UserID",
                table: "ChatGroupSubscribers",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Messagedata_ChatGroups_ChatGroupID",
                table: "Messagedata",
                column: "ChatGroupID",
                principalTable: "ChatGroups",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messagedata_ChatGroups_ChatGroupID",
                table: "Messagedata");

            migrationBuilder.DropTable(
                name: "ChatGroupSubscribers");

            migrationBuilder.DropTable(
                name: "ChatGroups");

            migrationBuilder.DropIndex(
                name: "IX_Messagedata_ChatGroupID",
                table: "Messagedata");

            migrationBuilder.DropColumn(
                name: "ChatGroupID",
                table: "Messagedata");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class intial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messagedata_EventChat_EventChatID",
                table: "Messagedata");

            migrationBuilder.DropTable(
                name: "EventChat");

            migrationBuilder.DropIndex(
                name: "IX_Messagedata_EventChatID",
                table: "Messagedata");

            migrationBuilder.AlterColumn<string>(
                name: "EventChatID",
                table: "Messagedata",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EventChatID",
                table: "Messagedata",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "EventChat",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EventDataId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    muit = table.Column<bool>(type: "bit", nullable: false),
                    startedin = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventChat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventChat_EventData_EventDataId",
                        column: x => x.EventDataId,
                        principalTable: "EventData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventChat_UserDetails_UserId",
                        column: x => x.UserId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messagedata_EventChatID",
                table: "Messagedata",
                column: "EventChatID");

            migrationBuilder.CreateIndex(
                name: "IX_EventChat_EventDataId",
                table: "EventChat",
                column: "EventDataId");

            migrationBuilder.CreateIndex(
                name: "IX_EventChat_UserId",
                table: "EventChat",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messagedata_EventChat_EventChatID",
                table: "Messagedata",
                column: "EventChatID",
                principalTable: "EventChat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

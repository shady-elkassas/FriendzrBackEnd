using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class AddEventTracker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Shared",
                table: "EventData");

            migrationBuilder.AddColumn<int>(
                name: "Attendees",
                table: "EventData",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Shars",
                table: "EventData",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventTrackers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    ActionType = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTrackers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventTrackers_EventData_EventId",
                        column: x => x.EventId,
                        principalTable: "EventData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventTrackers_UserDetails_UserId",
                        column: x => x.UserId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventTrackers_EventId",
                table: "EventTrackers",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTrackers_UserId",
                table: "EventTrackers",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventTrackers");

            migrationBuilder.DropColumn(
                name: "Attendees",
                table: "EventData");

            migrationBuilder.DropColumn(
                name: "Shars",
                table: "EventData");

            migrationBuilder.AddColumn<int>(
                name: "Shared",
                table: "EventData",
                type: "int",
                nullable: true);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class RemoveEventAttend : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "eventattend");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "eventattend",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventDataid = table.Column<int>(type: "int", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserattendId = table.Column<int>(type: "int", nullable: true),
                    deletedate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deletefromeventDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deletefromeventtime = table.Column<TimeSpan>(type: "time", nullable: true),
                    delettime = table.Column<TimeSpan>(type: "time", nullable: true),
                    leveeventchatDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    leveeventchattime = table.Column<TimeSpan>(type: "time", nullable: true),
                    muit = table.Column<bool>(type: "bit", nullable: false),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    stutus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_eventattend", x => x.Id);
                    table.ForeignKey(
                        name: "FK_eventattend_EventData_EventDataid",
                        column: x => x.EventDataid,
                        principalTable: "EventData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_eventattend_UserDetails_UserattendId",
                        column: x => x.UserattendId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_eventattend_UserattendId",
                table: "eventattend",
                column: "UserattendId");

            migrationBuilder.CreateIndex(
                name: "IX_eventattend_EventDataid_UserattendId",
                table: "eventattend",
                columns: new[] { "EventDataid", "UserattendId" },
                unique: true,
                filter: "[UserattendId] IS NOT NULL");
        }
    }
}

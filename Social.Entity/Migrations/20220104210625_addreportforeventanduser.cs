using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class addreportforeventanduser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventReports",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false,defaultValue: "NewID()"),
                    EventDataID = table.Column<int>(nullable: false),
                    CreatedBy_UserID = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    ReportReasonID = table.Column<Guid>(nullable: false),
                    RegistrationDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventReports", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EventReports_EventData_EventDataID",
                        column: x => x.EventDataID,
                        principalTable: "EventData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventReports_ReportReasons_ReportReasonID",
                        column: x => x.ReportReasonID,
                        principalTable: "ReportReasons",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserReports",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false,defaultValue:"NewID()"),
                    UserID = table.Column<string>(nullable: true),
                    CreatedBy_UserID = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    ReportReasonID = table.Column<Guid>(nullable: false),
                    RegistrationDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReports", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserReports_ReportReasons_ReportReasonID",
                        column: x => x.ReportReasonID,
                        principalTable: "ReportReasons",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserReports_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventReports_EventDataID",
                table: "EventReports",
                column: "EventDataID");

            migrationBuilder.CreateIndex(
                name: "IX_EventReports_ReportReasonID",
                table: "EventReports",
                column: "ReportReasonID");

            migrationBuilder.CreateIndex(
                name: "IX_UserReports_ReportReasonID",
                table: "UserReports",
                column: "ReportReasonID");

            migrationBuilder.CreateIndex(
                name: "IX_UserReports_UserID",
                table: "UserReports",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventReports");

            migrationBuilder.DropTable(
                name: "UserReports");
        }
    }
}

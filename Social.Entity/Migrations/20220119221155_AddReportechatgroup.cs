using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class AddReportechatgroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatGroupReports",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    ChatGroupID = table.Column<Guid>(nullable: false),
                    CreatedBy_UserID = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    ReportReasonID = table.Column<Guid>(nullable: false),
                    RegistrationDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatGroupReports", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChatGroupReports_ChatGroups_ChatGroupID",
                        column: x => x.ChatGroupID,
                        principalTable: "ChatGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatGroupReports_AspNetUsers_CreatedBy_UserID",
                        column: x => x.CreatedBy_UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatGroupReports_ReportReasons_ReportReasonID",
                        column: x => x.ReportReasonID,
                        principalTable: "ReportReasons",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroupReports_ChatGroupID",
                table: "ChatGroupReports",
                column: "ChatGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroupReports_CreatedBy_UserID",
                table: "ChatGroupReports",
                column: "CreatedBy_UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroupReports_ReportReasonID",
                table: "ChatGroupReports",
                column: "ReportReasonID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatGroupReports");
        }
    }
}

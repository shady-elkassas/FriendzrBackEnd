using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class RemoveNotUsedColumnsAndAddNewTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboutUs",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Help",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PrivacyPolicy",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Share",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SkipTutorial",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SupportRequest",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TermsAndConditions",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TipsAndGuidance",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "UserLinkClicks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLinkClicks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLinkClicks_UserDetails_UserId",
                        column: x => x.UserId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLinkClicks_UserId",
                table: "UserLinkClicks",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLinkClicks");

            migrationBuilder.AddColumn<bool>(
                name: "AboutUs",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Help",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PrivacyPolicy",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Share",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SkipTutorial",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SupportRequest",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TermsAndConditions",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TipsAndGuidance",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }
    }
}

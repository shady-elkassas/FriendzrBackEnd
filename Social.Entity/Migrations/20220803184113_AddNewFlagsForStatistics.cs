using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class AddNewFlagsForStatistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Shared",
                table: "EventData",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Views",
                table: "EventData",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AboutUs",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Help",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PrivacyPolicy",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Share",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SkipTutorial",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SupportRequest",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TermsAndConditions",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TipsAndGuidance",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeletedUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityUserId = table.Column<string>(nullable: true),
                    UserDetail = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletedUsers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeletedUsers");

            migrationBuilder.DropColumn(
                name: "Shared",
                table: "EventData");

            migrationBuilder.DropColumn(
                name: "Views",
                table: "EventData");

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
        }
    }
}

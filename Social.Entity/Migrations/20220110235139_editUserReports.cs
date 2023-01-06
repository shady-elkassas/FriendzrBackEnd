using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class editUserReports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserReports_AspNetUsers_CreatedBy_UserID",
                table: "UserReports");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReports_AspNetUsers_UserID",
                table: "UserReports");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RegistrationDate",
                table: "UserReports",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddForeignKey(
                name: "FK_UserReports_AspNetUsers_CreatedBy_UserID",
                table: "UserReports",
                column: "CreatedBy_UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserReports_AspNetUsers_UserID",
                table: "UserReports",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserReports_AspNetUsers_CreatedBy_UserID",
                table: "UserReports");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReports_AspNetUsers_UserID",
                table: "UserReports");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RegistrationDate",
                table: "UserReports",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserReports_AspNetUsers_CreatedBy_UserID",
                table: "UserReports",
                column: "CreatedBy_UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserReports_AspNetUsers_UserID",
                table: "UserReports",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

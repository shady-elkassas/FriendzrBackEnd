using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class editUserDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserReports_AspNetUsers_UserID",
                table: "UserReports");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RegistrationDate",
                table: "UserReports",
                nullable: false,
                defaultValue:"getdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy_UserID",
                table: "UserReports",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserReports_CreatedBy_UserID",
                table: "UserReports",
                column: "CreatedBy_UserID");

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

            migrationBuilder.DropIndex(
                name: "IX_UserReports_CreatedBy_UserID",
                table: "UserReports");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RegistrationDate",
                table: "UserReports",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy_UserID",
                table: "UserReports",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserReports_AspNetUsers_UserID",
                table: "UserReports",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

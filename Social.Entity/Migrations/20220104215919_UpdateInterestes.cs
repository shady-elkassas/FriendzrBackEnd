using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class UpdateInterestes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_eventattend_Interests_InterestsId",
                table: "eventattend");

            migrationBuilder.DropIndex(
                name: "IX_eventattend_InterestsId",
                table: "eventattend");

            migrationBuilder.DropColumn(
                name: "InterestsId",
                table: "eventattend");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserID",
                table: "Interests",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAcive",
                table: "Interests",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSharedForAllUsers",
                table: "Interests",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationDate",
                table: "Interests",
                nullable: false,
                //defaultValue: "getdate()");
        defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Interests_CreatedByUserID",
                table: "Interests",
                column: "CreatedByUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Interests_AspNetUsers_CreatedByUserID",
                table: "Interests",
                column: "CreatedByUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interests_AspNetUsers_CreatedByUserID",
                table: "Interests");

            migrationBuilder.DropIndex(
                name: "IX_Interests_CreatedByUserID",
                table: "Interests");

            migrationBuilder.DropColumn(
                name: "CreatedByUserID",
                table: "Interests");

            migrationBuilder.DropColumn(
                name: "IsAcive",
                table: "Interests");

            migrationBuilder.DropColumn(
                name: "IsSharedForAllUsers",
                table: "Interests");

            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "Interests");

            migrationBuilder.AddColumn<int>(
                name: "InterestsId",
                table: "eventattend",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_eventattend_InterestsId",
                table: "eventattend",
                column: "InterestsId");

            migrationBuilder.AddForeignKey(
                name: "FK_eventattend_Interests_InterestsId",
                table: "eventattend",
                column: "InterestsId",
                principalTable: "Interests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

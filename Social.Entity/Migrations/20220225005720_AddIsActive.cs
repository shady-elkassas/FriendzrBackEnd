using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class AddIsActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BanFrom",
                table: "UserDetails",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BanTo",
                table: "UserDetails",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserDetails",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BanFrom",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "BanTo",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserDetails");
        }
    }
}

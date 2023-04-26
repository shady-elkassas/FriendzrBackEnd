using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class AddLocationOptionsToMessageDataTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLiveLocation",
                table: "Messagedata",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationPeriod",
                table: "Messagedata",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LocationStartTime",
                table: "Messagedata",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLiveLocation",
                table: "Messagedata");

            migrationBuilder.DropColumn(
                name: "LocationPeriod",
                table: "Messagedata");

            migrationBuilder.DropColumn(
                name: "LocationStartTime",
                table: "Messagedata");
        }
    }
}

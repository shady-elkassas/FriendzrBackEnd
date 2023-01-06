using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class AddStopColumnToEventData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "StopFrom",
                table: "EventData",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StopTo",
                table: "EventData",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StopFrom",
                table: "EventData");

            migrationBuilder.DropColumn(
                name: "StopTo",
                table: "EventData");
        }
    }
}

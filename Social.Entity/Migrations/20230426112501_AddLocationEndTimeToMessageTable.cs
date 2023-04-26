using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class AddLocationEndTimeToMessageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LocationStartTime",
                table: "Messagedata",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationEndTime",
                table: "Messagedata",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationEndTime",
                table: "Messagedata");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LocationStartTime",
                table: "Messagedata",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}

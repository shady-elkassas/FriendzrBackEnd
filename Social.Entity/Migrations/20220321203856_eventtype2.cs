using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class eventtype2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventTypeListid",
                table: "EventData",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventTypeList",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    entityID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    color = table.Column<string>(nullable: true),
                    key = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypeList", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventData_EventTypeListid",
                table: "EventData",
                column: "EventTypeListid");

            migrationBuilder.AddForeignKey(
                name: "FK_EventData_EventTypeList_EventTypeListid",
                table: "EventData",
                column: "EventTypeListid",
                principalTable: "EventTypeList",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventData_EventTypeList_EventTypeListid",
                table: "EventData");

            migrationBuilder.DropTable(
                name: "EventTypeList");

            migrationBuilder.DropIndex(
                name: "IX_EventData_EventTypeListid",
                table: "EventData");

            migrationBuilder.DropColumn(
                name: "EventTypeListid",
                table: "EventData");
        }
    }
}

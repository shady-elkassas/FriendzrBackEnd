using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class intial3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageAttached");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageAttached",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MessagedataId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Messages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Messagesdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Messagestime = table.Column<TimeSpan>(type: "time", nullable: false),
                    attached = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageAttached", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageAttached_Messagedata_MessagedataId",
                        column: x => x.MessagedataId,
                        principalTable: "Messagedata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageAttached_MessagedataId",
                table: "MessageAttached",
                column: "MessagedataId");
        }
    }
}

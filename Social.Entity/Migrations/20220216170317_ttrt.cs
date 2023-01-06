using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class ttrt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventDataid",
                table: "Messagedata",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "linkable",
                table: "Messagedata",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "preferto",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    CreatedByUserID = table.Column<string>(nullable: true),
                    RegistrationDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_preferto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_preferto_AspNetUsers_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Iprefertolist",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<string>(nullable: true),
                    Tagsname = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    prefertoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Iprefertolist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Iprefertolist_UserDetails_UserId",
                        column: x => x.UserId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Iprefertolist_preferto_prefertoId",
                        column: x => x.prefertoId,
                        principalTable: "preferto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messagedata_EventDataid",
                table: "Messagedata",
                column: "EventDataid");

            migrationBuilder.CreateIndex(
                name: "IX_Iprefertolist_UserId",
                table: "Iprefertolist",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Iprefertolist_prefertoId",
                table: "Iprefertolist",
                column: "prefertoId");

            migrationBuilder.CreateIndex(
                name: "IX_preferto_CreatedByUserID",
                table: "preferto",
                column: "CreatedByUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Messagedata_EventData_EventDataid",
                table: "Messagedata",
                column: "EventDataid",
                principalTable: "EventData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messagedata_EventData_EventDataid",
                table: "Messagedata");

            migrationBuilder.DropTable(
                name: "Iprefertolist");

            migrationBuilder.DropTable(
                name: "preferto");

            migrationBuilder.DropIndex(
                name: "IX_Messagedata_EventDataid",
                table: "Messagedata");

            migrationBuilder.DropColumn(
                name: "EventDataid",
                table: "Messagedata");

            migrationBuilder.DropColumn(
                name: "linkable",
                table: "Messagedata");
        }
    }
}

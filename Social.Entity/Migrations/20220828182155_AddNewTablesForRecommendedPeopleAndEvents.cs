using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class AddNewTablesForRecommendedPeopleAndEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecommendedEventArea",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecommendedPeopleArea",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SkippedEvents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    EventId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkippedEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkippedEvents_EventData_EventId",
                        column: x => x.EventId,
                        principalTable: "EventData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkippedEvents_UserDetails_UserId",
                        column: x => x.UserId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SkippedUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    SkippedUserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkippedUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkippedUsers_UserDetails_SkippedUserId",
                        column: x => x.SkippedUserId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId");
                    table.ForeignKey(
                        name: "FK_SkippedUsers_UserDetails_UserId",
                        column: x => x.UserId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SkippedEvents_EventId",
                table: "SkippedEvents",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_SkippedEvents_UserId",
                table: "SkippedEvents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SkippedUsers_SkippedUserId",
                table: "SkippedUsers",
                column: "SkippedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SkippedUsers_UserId",
                table: "SkippedUsers",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SkippedEvents");

            migrationBuilder.DropTable(
                name: "SkippedUsers");

            migrationBuilder.DropColumn(
                name: "RecommendedEventArea",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "RecommendedPeopleArea",
                table: "AppConfigrations");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class AddUserImagesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_EventData_UserDetails_UserId",
            //    table: "EventData");

            migrationBuilder.CreateTable(
                name: "UserImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserDetailsId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserImages_UserDetails_UserDetailsId",
                        column: x => x.UserDetailsId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserImages_UserDetailsId",
                table: "UserImages",
                column: "UserDetailsId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_EventData_UserDetails_UserId",
            //    table: "EventData",
            //    column: "UserId",
            //    principalTable: "UserDetails",
            //    principalColumn: "PrimaryId",
            //    onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_EventData_UserDetails_UserId",
            //    table: "EventData");

            migrationBuilder.DropTable(
                name: "UserImages");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_EventData_UserDetails_UserId",
            //    table: "EventData",
            //    column: "UserId",
            //    principalTable: "UserDetails",
            //    principalColumn: "PrimaryId",
            //    onDelete: ReferentialAction.Restrict);
        }
    }
}

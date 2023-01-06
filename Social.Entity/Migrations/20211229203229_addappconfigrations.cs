using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class addappconfigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppConfigrations",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UserName_MaxLength = table.Column<int>(nullable: true),
                    UserName_MinLength = table.Column<int>(nullable: true),
                    Password_MaxLength = table.Column<int>(nullable: true),
                    Password_MinLength = table.Column<int>(nullable: true),
                    PasswordAtleastNumbers = table.Column<int>(nullable: true),
                    PasswordAtleastSpecialCharacters = table.Column<int>(nullable: true),
                    UserMinAge = table.Column<int>(nullable: true),
                    UserMaxAge = table.Column<int>(nullable: true),
                    UserBio_MaxLength = table.Column<int>(nullable: true),
                    UserBio_MinLength = table.Column<int>(nullable: true),
                    EventDetailsDescription_MinLength = table.Column<int>(nullable: true),
                    EventDetailsDescription_MaxLength = table.Column<int>(nullable: true),
                    UserTagM_MaxNumber = table.Column<int>(nullable: true),
                    UserTagM_MinNumber = table.Column<int>(nullable: true),
                    AgeFiltering_Min = table.Column<int>(nullable: true),
                    AgeFiltering_Max = table.Column<int>(nullable: true),
                    DistanceFiltering_Min = table.Column<int>(nullable: true),
                    DistanceFiltering_Max = table.Column<int>(nullable: true),
                    RegstrationDate = table.Column<DateTime>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    UserID = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppConfigrations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AppConfigrations_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppConfigrations_UserID",
                table: "AppConfigrations",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppConfigrations");
        }
    }
}

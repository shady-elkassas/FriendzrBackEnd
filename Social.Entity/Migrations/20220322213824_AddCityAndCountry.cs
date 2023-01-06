using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class AddCityAndCountry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CityID",
                table: "UserDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountryID",
                table: "UserDetails",
                nullable: true);

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "eventtype",
            //    table: "EventData",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoogleName = table.Column<string>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoogleName = table.Column<string>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_CityID",
                table: "UserDetails",
                column: "CityID");

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_CountryID",
                table: "UserDetails",
                column: "CountryID");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_GoogleName",
                table: "Cities",
                column: "GoogleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_GoogleName",
                table: "Countries",
                column: "GoogleName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDetails_Cities_CityID",
                table: "UserDetails",
                column: "CityID",
                principalTable: "Cities",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDetails_Countries_CountryID",
                table: "UserDetails",
                column: "CountryID",
                principalTable: "Countries",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDetails_Cities_CityID",
                table: "UserDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDetails_Countries_CountryID",
                table: "UserDetails");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_UserDetails_CityID",
                table: "UserDetails");

            migrationBuilder.DropIndex(
                name: "IX_UserDetails_CountryID",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "CityID",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "CountryID",
                table: "UserDetails");

            //migrationBuilder.AlterColumn<string>(
            //    name: "eventtype",
            //    table: "EventData",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(Guid),
            //    oldNullable: true);
        }
    }
}

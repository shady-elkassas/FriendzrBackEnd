using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class AddcityandCountryToEventData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CityID",
                table: "EventData",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountryID",
                table: "EventData",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventData_CityID",
                table: "EventData",
                column: "CityID");

            migrationBuilder.CreateIndex(
                name: "IX_EventData_CountryID",
                table: "EventData",
                column: "CountryID");

            migrationBuilder.AddForeignKey(
                name: "FK_EventData_Cities_CityID",
                table: "EventData",
                column: "CityID",
                principalTable: "Cities",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventData_Countries_CountryID",
                table: "EventData",
                column: "CountryID",
                principalTable: "Countries",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventData_Cities_CityID",
                table: "EventData");

            migrationBuilder.DropForeignKey(
                name: "FK_EventData_Countries_CountryID",
                table: "EventData");

            migrationBuilder.DropIndex(
                name: "IX_EventData_CityID",
                table: "EventData");

            migrationBuilder.DropIndex(
                name: "IX_EventData_CountryID",
                table: "EventData");

            migrationBuilder.DropColumn(
                name: "CityID",
                table: "EventData");

            migrationBuilder.DropColumn(
                name: "CountryID",
                table: "EventData");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class editappconfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordAtleastNumbers",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "PasswordAtleastSpecialCharacters",
                table: "AppConfigrations");

            migrationBuilder.AddColumn<int>(
                name: "DistanceShowNearbyAccountsInFeed_Max",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DistanceShowNearbyAccountsInFeed_Min",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DistanceShowNearbyEventsOnMap_Max",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DistanceShowNearbyEventsOnMap_Min",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DistanceShowNearbyEvents_Max",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DistanceShowNearbyEvents_Min",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Password_MaxNumbers",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Password_MaxSpecialCharacters",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Password_MinNumbers",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Password_MinSpecialCharacters",
                table: "AppConfigrations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistanceShowNearbyAccountsInFeed_Max",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "DistanceShowNearbyAccountsInFeed_Min",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "DistanceShowNearbyEventsOnMap_Max",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "DistanceShowNearbyEventsOnMap_Min",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "DistanceShowNearbyEvents_Max",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "DistanceShowNearbyEvents_Min",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "Password_MaxNumbers",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "Password_MaxSpecialCharacters",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "Password_MinNumbers",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "Password_MinSpecialCharacters",
                table: "AppConfigrations");

            migrationBuilder.AddColumn<int>(
                name: "PasswordAtleastNumbers",
                table: "AppConfigrations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PasswordAtleastSpecialCharacters",
                table: "AppConfigrations",
                type: "int",
                nullable: true);
        }
    }
}

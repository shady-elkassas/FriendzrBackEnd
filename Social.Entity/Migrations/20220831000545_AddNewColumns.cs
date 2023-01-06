using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class AddNewColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecommendedEventArea",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "RecommendedPeopleArea",
                table: "AppConfigrations");

            migrationBuilder.AddColumn<int>(
                name: "RecommendedEventArea_Max",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecommendedEventArea_Min",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecommendedPeopleArea_Max",
                table: "AppConfigrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecommendedPeopleArea_Min",
                table: "AppConfigrations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecommendedEventArea_Max",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "RecommendedEventArea_Min",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "RecommendedPeopleArea_Max",
                table: "AppConfigrations");

            migrationBuilder.DropColumn(
                name: "RecommendedPeopleArea_Min",
                table: "AppConfigrations");

            migrationBuilder.AddColumn<int>(
                name: "RecommendedEventArea",
                table: "AppConfigrations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecommendedPeopleArea",
                table: "AppConfigrations",
                type: "int",
                nullable: true);
        }
    }
}

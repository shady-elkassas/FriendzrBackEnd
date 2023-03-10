using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class AddExternalCategoriesColumnToEventTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubCategoriesIds",
                table: "EventData",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubCategoriesIds",
                table: "EventData");
        }
    }
}

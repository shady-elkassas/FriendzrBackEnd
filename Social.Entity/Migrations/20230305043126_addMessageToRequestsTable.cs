using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class addMessageToRequestsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Requestes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "Requestes");
        }
    }
}

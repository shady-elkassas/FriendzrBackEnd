using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class addImageIsVerifiedFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ImageIsVerified",
                table: "UserDetails",
                nullable: true,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageIsVerified",
                table: "UserDetails");
        }
    }
}

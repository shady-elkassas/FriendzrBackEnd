using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "UserDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_Code",
                table: "UserDetails",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserDetails_Code",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "UserDetails");
        }
    }
}

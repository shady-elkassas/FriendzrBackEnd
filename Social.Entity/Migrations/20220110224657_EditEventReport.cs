using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class EditEventReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy_UserID",
                table: "EventReports",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventReports_CreatedBy_UserID",
                table: "EventReports",
                column: "CreatedBy_UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_EventReports_AspNetUsers_CreatedBy_UserID",
                table: "EventReports",
                column: "CreatedBy_UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventReports_AspNetUsers_CreatedBy_UserID",
                table: "EventReports");

            migrationBuilder.DropIndex(
                name: "IX_EventReports_CreatedBy_UserID",
                table: "EventReports");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy_UserID",
                table: "EventReports",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}

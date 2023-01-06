using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class cascadeDeletefirebase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FireBaseDatamodel_UserDetails_userid",
                table: "FireBaseDatamodel");

            migrationBuilder.AlterColumn<int>(
                name: "userid",
                table: "FireBaseDatamodel",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FireBaseDatamodel_UserDetails_userid",
                table: "FireBaseDatamodel",
                column: "userid",
                principalTable: "UserDetails",
                principalColumn: "PrimaryId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FireBaseDatamodel_UserDetails_userid",
                table: "FireBaseDatamodel");

            migrationBuilder.AlterColumn<int>(
                name: "userid",
                table: "FireBaseDatamodel",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_FireBaseDatamodel_UserDetails_userid",
                table: "FireBaseDatamodel",
                column: "userid",
                principalTable: "UserDetails",
                principalColumn: "PrimaryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

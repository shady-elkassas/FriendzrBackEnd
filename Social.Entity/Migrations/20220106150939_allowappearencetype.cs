using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class allowappearencetype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "allowmylocationtype",
                table: "UserDetails");

            migrationBuilder.CreateTable(
                name: "AppearanceTypes_UserDetails",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppearanceTypeID = table.Column<int>(nullable: false),
                    UserDetailsID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppearanceTypes_UserDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AppearanceTypes_UserDetails_UserDetails_UserDetailsID",
                        column: x => x.UserDetailsID,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppearanceTypes_UserDetails_UserDetailsID",
                table: "AppearanceTypes_UserDetails",
                column: "UserDetailsID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppearanceTypes_UserDetails");

            migrationBuilder.AddColumn<int>(
                name: "allowmylocationtype",
                table: "UserDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

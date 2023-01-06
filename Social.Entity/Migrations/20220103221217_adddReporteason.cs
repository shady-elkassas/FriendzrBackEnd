using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class adddReporteason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportReasons",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false,defaultValue: "NEWID()"),
                    Name = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    RegistrationDate = table.Column<DateTime>(nullable: false,defaultValue:"getDate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportReasons", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportReasons");
        }
    }
}

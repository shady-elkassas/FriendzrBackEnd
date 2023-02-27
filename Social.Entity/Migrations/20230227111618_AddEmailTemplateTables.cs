using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class AddEmailTemplateTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    HtmlTemplate = table.Column<string>(nullable: true),
                    TemplateCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailTemplates");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class ss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "whatAmILookingFor",
                table: "UserDetails",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WhatBestDescripsMe",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    IsSharedForAllUsers = table.Column<bool>(nullable: false),
                    CreatedByUserID = table.Column<string>(nullable: true),
                    RegistrationDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatBestDescripsMe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WhatBestDescripsMe_AspNetUsers_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WhatBestDescripsMeList",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<string>(nullable: true),
                    Tagsname = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    WhatBestDescripsMeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatBestDescripsMeList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WhatBestDescripsMeList_UserDetails_UserId",
                        column: x => x.UserId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WhatBestDescripsMeList_WhatBestDescripsMe_WhatBestDescripsMeId",
                        column: x => x.WhatBestDescripsMeId,
                        principalTable: "WhatBestDescripsMe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WhatBestDescripsMe_CreatedByUserID",
                table: "WhatBestDescripsMe",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_WhatBestDescripsMeList_UserId",
                table: "WhatBestDescripsMeList",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WhatBestDescripsMeList_WhatBestDescripsMeId",
                table: "WhatBestDescripsMeList",
                column: "WhatBestDescripsMeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WhatBestDescripsMeList");

            migrationBuilder.DropTable(
                name: "WhatBestDescripsMe");

            migrationBuilder.DropColumn(
                name: "whatAmILookingFor",
                table: "UserDetails");
        }
    }
}

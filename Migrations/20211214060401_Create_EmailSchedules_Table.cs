using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Email_Scheduler_WebApi.Migrations
{
    public partial class Create_EmailSchedules_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    SendTo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    SendTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailSchedules_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailSchedules_UserId",
                table: "EmailSchedules",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailSchedules");
        }
    }
}

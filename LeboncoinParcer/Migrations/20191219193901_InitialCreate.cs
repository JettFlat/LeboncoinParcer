using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LeboncoinParcer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Realtys",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Url = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Phone = table.Column<string>(nullable: true),
                    LocalisationTown = table.Column<string>(nullable: true),
                    District = table.Column<string>(nullable: true),
                    Index = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Rooms = table.Column<int>(nullable: false),
                    Surface = table.Column<string>(nullable: true),
                    Furniture = table.Column<string>(nullable: true),
                    Ges = table.Column<string>(nullable: true),
                    EnergyClass = table.Column<string>(nullable: true),
                    Desciption = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Realtys", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Realtys");
        }
    }
}

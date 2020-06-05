using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MVCWithBlazor.Migrations
{
    public partial class AddIndexesToEF : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Indexes",
                columns: table => new
                {
                    IndexModelID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataOra = table.Column<DateTime>(nullable: false),
                    EdisStatus = table.Column<int>(nullable: false),
                    IndexEnergyPlusA = table.Column<float>(nullable: false),
                    IndexEnergyMinusA = table.Column<float>(nullable: false),
                    IndexEnergyPlusRi = table.Column<float>(nullable: false),
                    IndexEnergyPlusRc = table.Column<float>(nullable: false),
                    IndexEnergyMinusRi = table.Column<float>(nullable: false),
                    IndexEnergyMinusRc = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indexes", x => x.IndexModelID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Indexes");
        }
    }
}

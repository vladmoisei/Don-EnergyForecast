using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MVCWithBlazor.Migrations
{
    public partial class AddPrognozaEnergieModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrognozaEnergieModels",
                columns: table => new
                {
                    PrognozaEnergieModelID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataOra = table.Column<DateTime>(nullable: false),
                    Ora = table.Column<int>(nullable: false),
                    Valoare = table.Column<float>(nullable: false),
                    IndexModelID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrognozaEnergieModels", x => x.PrognozaEnergieModelID);
                    table.ForeignKey(
                        name: "FK_PrognozaEnergieModels_Indexes_IndexModelID",
                        column: x => x.IndexModelID,
                        principalTable: "Indexes",
                        principalColumn: "IndexModelID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrognozaEnergieModels_IndexModelID",
                table: "PrognozaEnergieModels",
                column: "IndexModelID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrognozaEnergieModels");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace MVCWithBlazor.Migrations
{
    public partial class UpdatePrognozaModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrognozaEnergieModels_Indexes_IndexModelID",
                table: "PrognozaEnergieModels");

            migrationBuilder.DropIndex(
                name: "IX_PrognozaEnergieModels_IndexModelID",
                table: "PrognozaEnergieModels");

            migrationBuilder.DropColumn(
                name: "IndexModelID",
                table: "PrognozaEnergieModels");

            migrationBuilder.AlterColumn<double>(
                name: "Valoare",
                table: "PrognozaEnergieModels",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Valoare",
                table: "PrognozaEnergieModels",
                type: "real",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<int>(
                name: "IndexModelID",
                table: "PrognozaEnergieModels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PrognozaEnergieModels_IndexModelID",
                table: "PrognozaEnergieModels",
                column: "IndexModelID");

            migrationBuilder.AddForeignKey(
                name: "FK_PrognozaEnergieModels_Indexes_IndexModelID",
                table: "PrognozaEnergieModels",
                column: "IndexModelID",
                principalTable: "Indexes",
                principalColumn: "IndexModelID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace MVCWithBlazor.Migrations
{
    public partial class CompleteIndexModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CosFiCapacitiv",
                table: "Indexes",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CosFiInductiv",
                table: "Indexes",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EnergiiOrareFacturareRiPlus",
                table: "Indexes",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CosFiCapacitiv",
                table: "Indexes");

            migrationBuilder.DropColumn(
                name: "CosFiInductiv",
                table: "Indexes");

            migrationBuilder.DropColumn(
                name: "EnergiiOrareFacturareRiPlus",
                table: "Indexes");
        }
    }
}

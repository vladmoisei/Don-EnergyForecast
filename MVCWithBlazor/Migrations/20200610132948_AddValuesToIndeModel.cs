using Microsoft.EntityFrameworkCore.Migrations;

namespace MVCWithBlazor.Migrations
{
    public partial class AddValuesToIndeModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ora",
                table: "Indexes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "ValueEnergyMinusRc",
                table: "Indexes",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ValueEnergyPlusA",
                table: "Indexes",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ValueEnergyPlusRi",
                table: "Indexes",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ora",
                table: "Indexes");

            migrationBuilder.DropColumn(
                name: "ValueEnergyMinusRc",
                table: "Indexes");

            migrationBuilder.DropColumn(
                name: "ValueEnergyPlusA",
                table: "Indexes");

            migrationBuilder.DropColumn(
                name: "ValueEnergyPlusRi",
                table: "Indexes");
        }
    }
}

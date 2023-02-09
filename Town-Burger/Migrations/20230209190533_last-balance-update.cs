using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TownBurger.Migrations
{
    /// <inheritdoc />
    public partial class lastbalanceupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepositsDay",
                table: "Balances");

            migrationBuilder.DropColumn(
                name: "DepositsMonth",
                table: "Balances");

            migrationBuilder.DropColumn(
                name: "DepositsYear",
                table: "Balances");

            migrationBuilder.DropColumn(
                name: "EarningsDay",
                table: "Balances");

            migrationBuilder.DropColumn(
                name: "EarningsMonth",
                table: "Balances");

            migrationBuilder.DropColumn(
                name: "EarningsYear",
                table: "Balances");

            migrationBuilder.DropColumn(
                name: "SpendsDay",
                table: "Balances");

            migrationBuilder.DropColumn(
                name: "SpendsMonth",
                table: "Balances");

            migrationBuilder.DropColumn(
                name: "SpendsYear",
                table: "Balances");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DepositsDay",
                table: "Balances",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DepositsMonth",
                table: "Balances",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DepositsYear",
                table: "Balances",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EarningsDay",
                table: "Balances",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EarningsMonth",
                table: "Balances",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EarningsYear",
                table: "Balances",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SpendsDay",
                table: "Balances",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SpendsMonth",
                table: "Balances",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SpendsYear",
                table: "Balances",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}

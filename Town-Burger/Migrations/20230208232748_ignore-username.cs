using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TownBurger.Migrations
{
    /// <inheritdoc />
    public partial class ignoreusername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                schema: "Security",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "DaysOfWord",
                schema: "Security",
                table: "Users",
                newName: "DaysOfWork");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DaysOfWork",
                schema: "Security",
                table: "Users",
                newName: "DaysOfWord");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                schema: "Security",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}

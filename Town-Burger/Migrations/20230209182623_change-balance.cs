using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TownBurger.Migrations
{
    /// <inheritdoc />
    public partial class changebalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Users_WhereId",
                table: "Deposits");

            migrationBuilder.DropForeignKey(
                name: "FK_Spends_Users_WhereId",
                table: "Spends");

            migrationBuilder.RenameColumn(
                name: "WhereId",
                table: "Spends",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Spends_WhereId",
                table: "Spends",
                newName: "IX_Spends_UserId");

            migrationBuilder.RenameColumn(
                name: "WhereId",
                table: "Deposits",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Deposits_WhereId",
                table: "Deposits",
                newName: "IX_Deposits_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Users_UserId",
                table: "Deposits",
                column: "UserId",
                principalSchema: "Security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Spends_Users_UserId",
                table: "Spends",
                column: "UserId",
                principalSchema: "Security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Users_UserId",
                table: "Deposits");

            migrationBuilder.DropForeignKey(
                name: "FK_Spends_Users_UserId",
                table: "Spends");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Spends",
                newName: "WhereId");

            migrationBuilder.RenameIndex(
                name: "IX_Spends_UserId",
                table: "Spends",
                newName: "IX_Spends_WhereId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Deposits",
                newName: "WhereId");

            migrationBuilder.RenameIndex(
                name: "IX_Deposits_UserId",
                table: "Deposits",
                newName: "IX_Deposits_WhereId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Users_WhereId",
                table: "Deposits",
                column: "WhereId",
                principalSchema: "Security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Spends_Users_WhereId",
                table: "Spends",
                column: "WhereId",
                principalSchema: "Security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

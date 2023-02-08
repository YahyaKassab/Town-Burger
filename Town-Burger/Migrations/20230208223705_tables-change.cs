using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TownBurger.Migrations
{
    /// <inheritdoc />
    public partial class tableschange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposit<User>_Users_WhereId",
                table: "Deposit<User>");

            migrationBuilder.DropForeignKey(
                name: "FK_Spend<User>_Users_WhereId",
                table: "Spend<User>");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Spend<User>",
                table: "Spend<User>");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deposit<User>",
                table: "Deposit<User>");

            migrationBuilder.RenameTable(
                name: "Spend<User>",
                newName: "Spends");

            migrationBuilder.RenameTable(
                name: "Deposit<User>",
                newName: "Deposits");

            migrationBuilder.RenameIndex(
                name: "IX_Spend<User>_WhereId",
                table: "Spends",
                newName: "IX_Spends_WhereId");

            migrationBuilder.RenameIndex(
                name: "IX_Deposit<User>_WhereId",
                table: "Deposits",
                newName: "IX_Deposits_WhereId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Spends",
                table: "Spends",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deposits",
                table: "Deposits",
                column: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Users_WhereId",
                table: "Deposits");

            migrationBuilder.DropForeignKey(
                name: "FK_Spends_Users_WhereId",
                table: "Spends");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Spends",
                table: "Spends");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deposits",
                table: "Deposits");

            migrationBuilder.RenameTable(
                name: "Spends",
                newName: "Spend<User>");

            migrationBuilder.RenameTable(
                name: "Deposits",
                newName: "Deposit<User>");

            migrationBuilder.RenameIndex(
                name: "IX_Spends_WhereId",
                table: "Spend<User>",
                newName: "IX_Spend<User>_WhereId");

            migrationBuilder.RenameIndex(
                name: "IX_Deposits_WhereId",
                table: "Deposit<User>",
                newName: "IX_Deposit<User>_WhereId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Spend<User>",
                table: "Spend<User>",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deposit<User>",
                table: "Deposit<User>",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposit<User>_Users_WhereId",
                table: "Deposit<User>",
                column: "WhereId",
                principalSchema: "Security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Spend<User>_Users_WhereId",
                table: "Spend<User>",
                column: "WhereId",
                principalSchema: "Security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

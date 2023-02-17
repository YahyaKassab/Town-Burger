using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TownBurger.Migrations
{
    /// <inheritdoc />
    public partial class nameschanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_MenuItems_MenuItemId",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "MenuItemId",
                table: "CartItems",
                newName: "ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_MenuItemId",
                table: "CartItems",
                newName: "IX_CartItems_ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_MenuItems_ItemId",
                table: "CartItems",
                column: "ItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_MenuItems_ItemId",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "CartItems",
                newName: "MenuItemId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_ItemId",
                table: "CartItems",
                newName: "IX_CartItems_MenuItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_MenuItems_MenuItemId",
                table: "CartItems",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

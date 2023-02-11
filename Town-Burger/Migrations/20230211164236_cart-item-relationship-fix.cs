using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TownBurger.Migrations
{
    /// <inheritdoc />
    public partial class cartitemrelationshipfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_CartItem_CartItemId",
                table: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_MenuItems_CartItemId",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "CartItemId",
                table: "MenuItems");

            migrationBuilder.AddColumn<int>(
                name: "MenuItemId",
                table: "CartItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_MenuItemId",
                table: "CartItem",
                column: "MenuItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_MenuItems_MenuItemId",
                table: "CartItem",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_MenuItems_MenuItemId",
                table: "CartItem");

            migrationBuilder.DropIndex(
                name: "IX_CartItem_MenuItemId",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "MenuItemId",
                table: "CartItem");

            migrationBuilder.AddColumn<int>(
                name: "CartItemId",
                table: "MenuItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_CartItemId",
                table: "MenuItems",
                column: "CartItemId",
                unique: true,
                filter: "[CartItemId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_CartItem_CartItemId",
                table: "MenuItems",
                column: "CartItemId",
                principalTable: "CartItem",
                principalColumn: "Id");
        }
    }
}

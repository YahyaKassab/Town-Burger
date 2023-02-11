using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TownBurger.Migrations
{
    /// <inheritdoc />
    public partial class seedroles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                schema: "Security",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { Guid.NewGuid().ToString(), "Employee", "EMPLOYEE", Guid.NewGuid().ToString() }
                );
            migrationBuilder.InsertData(
                table: "Roles",
                schema: "Security",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { Guid.NewGuid().ToString(), "Customer", "CUSTOMER", Guid.NewGuid().ToString() }
                );
            migrationBuilder.InsertData(
                table: "Roles",
                schema: "Security",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { Guid.NewGuid().ToString(), "Admin", "ADMIN", Guid.NewGuid().ToString() }
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [SECURITY].[ROLES]");
        }
    }
}

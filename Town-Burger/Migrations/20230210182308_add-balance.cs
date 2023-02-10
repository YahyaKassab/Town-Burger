using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TownBurger.Migrations
{
    /// <inheritdoc />
    public partial class addbalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [DBO].[BALANCES] (MYBALANCE,TOTALSPENDS,TOTALDEPOSITS,TOTALEARNINGS) VALUES (0,0,0,0)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [DBO].[BALANCES]");
        }
    }
}

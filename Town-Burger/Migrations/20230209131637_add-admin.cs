using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TownBurger.Migrations
{
    /// <inheritdoc />
    public partial class addadmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [SECURITY].[USERROLES] (USERID,ROLEID) SELECT 'dff1bc71-b0e7-4018-a294-ef1380dcf728', ID FROM [SECURITY].[ROLES] ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [SECURITY].[USERROLES] WHERE USERID = 'dff1bc71-b0e7-4018-a294-ef1380dcf728'");
        }
    }
}

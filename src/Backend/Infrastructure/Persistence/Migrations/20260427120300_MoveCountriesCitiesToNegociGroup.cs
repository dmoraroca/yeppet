using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveCountriesCitiesToNegociGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE menus SET parent_key = 'admin.negoci', sort_order = 50 WHERE key = 'admin.countries';
                UPDATE menus SET parent_key = 'admin.negoci', sort_order = 60 WHERE key = 'admin.cities';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE menus SET parent_key = 'admin.tecnic', sort_order = 30 WHERE key = 'admin.countries';
                UPDATE menus SET parent_key = 'admin.tecnic', sort_order = 40 WHERE key = 'admin.cities';
                """);
        }
    }
}

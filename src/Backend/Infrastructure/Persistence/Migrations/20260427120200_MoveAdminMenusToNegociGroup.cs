using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveAdminMenusToNegociGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Admin "Menús" (nav catalog maintenance) is grouped under Negoci with doc/users/catalog.
            migrationBuilder.Sql(
                """
                UPDATE menus SET parent_key = 'admin.negoci', sort_order = 30 WHERE key = 'admin.menus';
                UPDATE menus SET parent_key = 'admin.negoci', sort_order = 40 WHERE key = 'admin.places';
                UPDATE menus SET sort_order = 20 WHERE key = 'admin.roles' AND parent_key = 'admin.tecnic';
                UPDATE menus SET sort_order = 30 WHERE key = 'admin.countries' AND parent_key = 'admin.tecnic';
                UPDATE menus SET sort_order = 40 WHERE key = 'admin.cities' AND parent_key = 'admin.tecnic';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE menus SET parent_key = 'admin.tecnic', sort_order = 30 WHERE key = 'admin.menus';
                UPDATE menus SET parent_key = 'admin.negoci', sort_order = 30 WHERE key = 'admin.places';
                UPDATE menus SET sort_order = 40 WHERE key = 'admin.countries' AND parent_key = 'admin.tecnic';
                UPDATE menus SET sort_order = 50 WHERE key = 'admin.cities' AND parent_key = 'admin.tecnic';
                """);
        }
    }
}

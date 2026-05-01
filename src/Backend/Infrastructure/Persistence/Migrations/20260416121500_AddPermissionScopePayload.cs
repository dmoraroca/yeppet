using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Zuppeto.Infrastructure.Persistence;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ZuppetoDbContext))]
    [Migration("20260416121500_AddPermissionScopePayload")]
    public partial class AddPermissionScopePayload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "scope_payload",
                table: "permissions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "scope_payload",
                table: "permissions");
        }
    }
}

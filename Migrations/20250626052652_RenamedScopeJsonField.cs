using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VidizmoBackend.Migrations
{
    /// <inheritdoc />
    public partial class RenamedScopeJsonField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScopeJson",
                table: "ScopedTokens",
                newName: "Permissions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Permissions",
                table: "ScopedTokens",
                newName: "ScopeJson");
        }
    }
}

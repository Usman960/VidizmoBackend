using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VidizmoBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddedRoleCollectionInUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Roles_CreatedByUserId",
                table: "Roles",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Users_CreatedByUserId",
                table: "Roles",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Users_CreatedByUserId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_CreatedByUserId",
                table: "Roles");
        }
    }
}

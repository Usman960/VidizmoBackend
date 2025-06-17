using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VidizmoBackend.Migrations
{
    /// <inheritdoc />
    public partial class NullUserIdInUserOgGpRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScopedTokens_Users_UserId",
                table: "ScopedTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_Users_AddedUserId",
                table: "UserGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_Users_UserAddedByUserId",
                table: "UserGroups");

            migrationBuilder.RenameColumn(
                name: "UserAddedByUserId",
                table: "UserGroups",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "AddedUserId",
                table: "UserGroups",
                newName: "AddedById");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroups_UserAddedByUserId",
                table: "UserGroups",
                newName: "IX_UserGroups_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroups_AddedUserId",
                table: "UserGroups",
                newName: "IX_UserGroups_AddedById");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserOgGpRoles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ScopedTokens_Users_UserId",
                table: "ScopedTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_Users_AddedById",
                table: "UserGroups",
                column: "AddedById",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_Users_UserId",
                table: "UserGroups",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScopedTokens_Users_UserId",
                table: "ScopedTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_Users_AddedById",
                table: "UserGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_Users_UserId",
                table: "UserGroups");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserGroups",
                newName: "UserAddedByUserId");

            migrationBuilder.RenameColumn(
                name: "AddedById",
                table: "UserGroups",
                newName: "AddedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroups_UserId",
                table: "UserGroups",
                newName: "IX_UserGroups_UserAddedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroups_AddedById",
                table: "UserGroups",
                newName: "IX_UserGroups_AddedUserId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserOgGpRoles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ScopedTokens_Users_UserId",
                table: "ScopedTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_Users_AddedUserId",
                table: "UserGroups",
                column: "AddedUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_Users_UserAddedByUserId",
                table: "UserGroups",
                column: "UserAddedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

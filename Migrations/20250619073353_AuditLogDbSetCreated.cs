using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VidizmoBackend.Migrations
{
    /// <inheritdoc />
    public partial class AuditLogDbSetCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLog_ScopedTokens_TokenId",
                table: "AuditLog");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLog_Users_PerformedById",
                table: "AuditLog");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLog_Users_UserId",
                table: "AuditLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLog",
                table: "AuditLog");

            migrationBuilder.RenameTable(
                name: "AuditLog",
                newName: "AuditLogs");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLog_UserId",
                table: "AuditLogs",
                newName: "IX_AuditLogs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLog_TokenId",
                table: "AuditLogs",
                newName: "IX_AuditLogs_TokenId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLog_PerformedById",
                table: "AuditLogs",
                newName: "IX_AuditLogs_PerformedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs",
                column: "AuditLogId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_ScopedTokens_TokenId",
                table: "AuditLogs",
                column: "TokenId",
                principalTable: "ScopedTokens",
                principalColumn: "ScopedTokenId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_PerformedById",
                table: "AuditLogs",
                column: "PerformedById",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_ScopedTokens_TokenId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_PerformedById",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                newName: "AuditLog");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLog",
                newName: "IX_AuditLog_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_TokenId",
                table: "AuditLog",
                newName: "IX_AuditLog_TokenId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_PerformedById",
                table: "AuditLog",
                newName: "IX_AuditLog_PerformedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLog",
                table: "AuditLog",
                column: "AuditLogId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLog_ScopedTokens_TokenId",
                table: "AuditLog",
                column: "TokenId",
                principalTable: "ScopedTokens",
                principalColumn: "ScopedTokenId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLog_Users_PerformedById",
                table: "AuditLog",
                column: "PerformedById",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLog_Users_UserId",
                table: "AuditLog",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

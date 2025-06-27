using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VidizmoBackend.Migrations
{
    /// <inheritdoc />
    public partial class RestructuredLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_PerformedById",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "PerformedById",
                table: "AuditLogs",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Payload",
                table: "AuditLogs",
                newName: "ResponseBody");

            migrationBuilder.RenameColumn(
                name: "Entity",
                table: "AuditLogs",
                newName: "RequestUrl");

            migrationBuilder.RenameColumn(
                name: "Action",
                table: "AuditLogs",
                newName: "RequestBody");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_PerformedById",
                table: "AuditLogs",
                newName: "IX_AuditLogs_UserId");

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
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AuditLogs",
                newName: "PerformedById");

            migrationBuilder.RenameColumn(
                name: "ResponseBody",
                table: "AuditLogs",
                newName: "Payload");

            migrationBuilder.RenameColumn(
                name: "RequestUrl",
                table: "AuditLogs",
                newName: "Entity");

            migrationBuilder.RenameColumn(
                name: "RequestBody",
                table: "AuditLogs",
                newName: "Action");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                newName: "IX_AuditLogs_PerformedById");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_PerformedById",
                table: "AuditLogs",
                column: "PerformedById",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

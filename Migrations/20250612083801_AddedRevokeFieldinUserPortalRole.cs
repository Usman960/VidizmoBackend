using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VidizmoBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddedRevokeFieldinUserPortalRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPortalRoles_Users_AssignedByUserId",
                table: "UserPortalRoles");

            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedAt",
                table: "UserPortalRoles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RevokedByUserId",
                table: "UserPortalRoles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "UserPortalRoles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserPortalRoles_RevokedByUserId",
                table: "UserPortalRoles",
                column: "RevokedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPortalRoles_Users_AssignedByUserId",
                table: "UserPortalRoles",
                column: "AssignedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPortalRoles_Users_RevokedByUserId",
                table: "UserPortalRoles",
                column: "RevokedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPortalRoles_Users_AssignedByUserId",
                table: "UserPortalRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPortalRoles_Users_RevokedByUserId",
                table: "UserPortalRoles");

            migrationBuilder.DropIndex(
                name: "IX_UserPortalRoles_RevokedByUserId",
                table: "UserPortalRoles");

            migrationBuilder.DropColumn(
                name: "RevokedAt",
                table: "UserPortalRoles");

            migrationBuilder.DropColumn(
                name: "RevokedByUserId",
                table: "UserPortalRoles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserPortalRoles");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPortalRoles_Users_AssignedByUserId",
                table: "UserPortalRoles",
                column: "AssignedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

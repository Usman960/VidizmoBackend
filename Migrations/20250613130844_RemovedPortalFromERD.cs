using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VidizmoBackend.Migrations
{
    /// <inheritdoc />
    public partial class RemovedPortalFromERD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Portals_PortalId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_ScopedTokens_Portals_PortalId",
                table: "ScopedTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Portals_PortalId",
                table: "Videos");

            migrationBuilder.DropTable(
                name: "UserPortalRoles");

            migrationBuilder.DropTable(
                name: "Portals");

            migrationBuilder.DropIndex(
                name: "IX_Videos_PortalId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "PortalId",
                table: "Videos");

            migrationBuilder.RenameColumn(
                name: "PortalId",
                table: "ScopedTokens",
                newName: "OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ScopedTokens_PortalId",
                table: "ScopedTokens",
                newName: "IX_ScopedTokens_OrganizationId");

            migrationBuilder.RenameColumn(
                name: "PortalId",
                table: "Groups",
                newName: "OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_PortalId",
                table: "Groups",
                newName: "IX_Groups_OrganizationId");

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Videos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserOrgRoles",
                columns: table => new
                {
                    UserOrgRoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedByUserId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrgRoles", x => x.UserOrgRoleId);
                    table.ForeignKey(
                        name: "FK_UserOrgRoles_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserOrgRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserOrgRoles_Users_AssignedByUserId",
                        column: x => x.AssignedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserOrgRoles_Users_RevokedByUserId",
                        column: x => x.RevokedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserOrgRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Videos_OrganizationId",
                table: "Videos",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrgRoles_AssignedByUserId",
                table: "UserOrgRoles",
                column: "AssignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrgRoles_OrganizationId",
                table: "UserOrgRoles",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrgRoles_RevokedByUserId",
                table: "UserOrgRoles",
                column: "RevokedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrgRoles_RoleId",
                table: "UserOrgRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrgRoles_UserId",
                table: "UserOrgRoles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Organizations_OrganizationId",
                table: "Groups",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "OrganizationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScopedTokens_Organizations_OrganizationId",
                table: "ScopedTokens",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "OrganizationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Organizations_OrganizationId",
                table: "Videos",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "OrganizationId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Organizations_OrganizationId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_ScopedTokens_Organizations_OrganizationId",
                table: "ScopedTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Organizations_OrganizationId",
                table: "Videos");

            migrationBuilder.DropTable(
                name: "UserOrgRoles");

            migrationBuilder.DropIndex(
                name: "IX_Videos_OrganizationId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Videos");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "ScopedTokens",
                newName: "PortalId");

            migrationBuilder.RenameIndex(
                name: "IX_ScopedTokens_OrganizationId",
                table: "ScopedTokens",
                newName: "IX_ScopedTokens_PortalId");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "Groups",
                newName: "PortalId");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_OrganizationId",
                table: "Groups",
                newName: "IX_Groups_PortalId");

            migrationBuilder.AddColumn<int>(
                name: "PortalId",
                table: "Videos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Portals",
                columns: table => new
                {
                    PortalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portals", x => x.PortalId);
                    table.ForeignKey(
                        name: "FK_Portals_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Portals_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPortalRoles",
                columns: table => new
                {
                    UserPortalRoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignedByUserId = table.Column<int>(type: "int", nullable: false),
                    PortalId = table.Column<int>(type: "int", nullable: false),
                    RevokedByUserId = table.Column<int>(type: "int", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPortalRoles", x => x.UserPortalRoleId);
                    table.ForeignKey(
                        name: "FK_UserPortalRoles_Portals_PortalId",
                        column: x => x.PortalId,
                        principalTable: "Portals",
                        principalColumn: "PortalId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPortalRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPortalRoles_Users_AssignedByUserId",
                        column: x => x.AssignedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPortalRoles_Users_RevokedByUserId",
                        column: x => x.RevokedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPortalRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Videos_PortalId",
                table: "Videos",
                column: "PortalId");

            migrationBuilder.CreateIndex(
                name: "IX_Portals_CreatedByUserId",
                table: "Portals",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Portals_OrganizationId",
                table: "Portals",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPortalRoles_AssignedByUserId",
                table: "UserPortalRoles",
                column: "AssignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPortalRoles_PortalId",
                table: "UserPortalRoles",
                column: "PortalId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPortalRoles_RevokedByUserId",
                table: "UserPortalRoles",
                column: "RevokedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPortalRoles_RoleId",
                table: "UserPortalRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPortalRoles_UserId",
                table: "UserPortalRoles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Portals_PortalId",
                table: "Groups",
                column: "PortalId",
                principalTable: "Portals",
                principalColumn: "PortalId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScopedTokens_Portals_PortalId",
                table: "ScopedTokens",
                column: "PortalId",
                principalTable: "Portals",
                principalColumn: "PortalId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Portals_PortalId",
                table: "Videos",
                column: "PortalId",
                principalTable: "Portals",
                principalColumn: "PortalId");
        }
    }
}

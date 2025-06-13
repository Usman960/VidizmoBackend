using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VidizmoBackend.Migrations
{
    /// <inheritdoc />
    public partial class OneToOneUserOrg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organizations_CreatedByUserId",
                table: "Organizations");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Organizations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_CreatedByUserId",
                table: "Organizations",
                column: "CreatedByUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Name",
                table: "Organizations",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organizations_CreatedByUserId",
                table: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_Name",
                table: "Organizations");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Organizations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_CreatedByUserId",
                table: "Organizations",
                column: "CreatedByUserId");
        }
    }
}

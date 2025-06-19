using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VidizmoBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddedReqBodyinLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiReqBody",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiReqBody",
                table: "AuditLogs");
        }
    }
}

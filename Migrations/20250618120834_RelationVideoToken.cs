using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VidizmoBackend.Migrations
{
    /// <inheritdoc />
    public partial class RelationVideoToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Users_UploadedByUserId",
                table: "Videos");

            migrationBuilder.AlterColumn<int>(
                name: "UploadedByUserId",
                table: "Videos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ScopedTokenId",
                table: "Videos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ScopedTokenId",
                table: "Videos",
                column: "ScopedTokenId");

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_ScopedTokens_ScopedTokenId",
                table: "Videos",
                column: "ScopedTokenId",
                principalTable: "ScopedTokens",
                principalColumn: "ScopedTokenId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Users_UploadedByUserId",
                table: "Videos",
                column: "UploadedByUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_ScopedTokens_ScopedTokenId",
                table: "Videos");

            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Users_UploadedByUserId",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Videos_ScopedTokenId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "ScopedTokenId",
                table: "Videos");

            migrationBuilder.AlterColumn<int>(
                name: "UploadedByUserId",
                table: "Videos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Users_UploadedByUserId",
                table: "Videos",
                column: "UploadedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

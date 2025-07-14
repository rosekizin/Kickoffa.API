using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kickoffa.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMaxSizeMbPropertyFromUploadComponentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UploadComponents_MaxSizeMB",
                table: "UploadComponents");

            migrationBuilder.DropIndex(
                name: "IX_UploadComponents_SectionId_MaxSizeMB",
                table: "UploadComponents");

            migrationBuilder.DropColumn(
                name: "MaxSizeMB",
                table: "UploadComponents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxSizeMB",
                table: "UploadComponents",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UploadComponents_MaxSizeMB",
                table: "UploadComponents",
                column: "MaxSizeMB");

            migrationBuilder.CreateIndex(
                name: "IX_UploadComponents_SectionId_MaxSizeMB",
                table: "UploadComponents",
                columns: new[] { "SectionId", "MaxSizeMB" });
        }
    }
}

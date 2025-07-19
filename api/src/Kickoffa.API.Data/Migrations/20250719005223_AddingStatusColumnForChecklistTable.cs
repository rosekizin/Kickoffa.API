using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kickoffa.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingStatusColumnForChecklistTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Checklists_AccessToken_OwnerId",
                table: "Checklists");

            migrationBuilder.DropIndex(
                name: "IX_Checklists_OwnerId_IsPublished",
                table: "Checklists");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Checklists");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Checklists",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_AccessToken_OwnerId",
                table: "Checklists",
                columns: new[] { "AccessToken", "OwnerId" },
                filter: "\"Status\" = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_OwnerId_Status",
                table: "Checklists",
                columns: new[] { "OwnerId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Checklists_AccessToken_OwnerId",
                table: "Checklists");

            migrationBuilder.DropIndex(
                name: "IX_Checklists_OwnerId_Status",
                table: "Checklists");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Checklists");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Checklists",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_AccessToken_OwnerId",
                table: "Checklists",
                columns: new[] { "AccessToken", "OwnerId" },
                filter: "\"IsPublished\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_OwnerId_IsPublished",
                table: "Checklists",
                columns: new[] { "OwnerId", "IsPublished" });
        }
    }
}

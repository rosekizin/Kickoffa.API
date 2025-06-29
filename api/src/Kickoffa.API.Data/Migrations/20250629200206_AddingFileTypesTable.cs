using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Kickoffa.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingFileTypesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MimeType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Extension = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    RecommendedMaxSizeMB = table.Column<int>(type: "integer", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileTypes_Category",
                table: "FileTypes",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_FileTypes_Category_DisplayOrder",
                table: "FileTypes",
                columns: new[] { "Category", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_FileTypes_Extension",
                table: "FileTypes",
                column: "Extension");

            migrationBuilder.CreateIndex(
                name: "IX_FileTypes_IsActive",
                table: "FileTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_FileTypes_MimeType",
                table: "FileTypes",
                column: "MimeType");

            migrationBuilder.CreateIndex(
                name: "IX_FileTypes_MimeType_Extension",
                table: "FileTypes",
                columns: new[] { "MimeType", "Extension" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileTypes");
        }
    }
}

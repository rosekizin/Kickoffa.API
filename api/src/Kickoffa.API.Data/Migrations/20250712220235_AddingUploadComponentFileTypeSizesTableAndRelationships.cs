using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Kickoffa.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingUploadComponentFileTypeSizesTableAndRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UploadComponentFileTypeSizes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UploadComponentId = table.Column<long>(type: "bigint", nullable: false),
                    FileTypeId = table.Column<long>(type: "bigint", nullable: false),
                    MaxSizeMB = table.Column<int>(type: "integer", nullable: false),
                    CreatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadComponentFileTypeSizes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UploadComponentFileTypeSizes_FileTypes_FileTypeId",
                        column: x => x.FileTypeId,
                        principalTable: "FileTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UploadComponentFileTypeSizes_UploadComponents_UploadCompone~",
                        column: x => x.UploadComponentId,
                        principalTable: "UploadComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UploadComponentFileTypeSizes_FileTypeId",
                table: "UploadComponentFileTypeSizes",
                column: "FileTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadComponentFileTypeSizes_UploadComponentId",
                table: "UploadComponentFileTypeSizes",
                column: "UploadComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadComponentFileTypeSizes_UploadComponentId_FileTypeId",
                table: "UploadComponentFileTypeSizes",
                columns: new[] { "UploadComponentId", "FileTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UploadComponentFileTypeSizes_UploadComponentId_FileTypeId_M~",
                table: "UploadComponentFileTypeSizes",
                columns: new[] { "UploadComponentId", "FileTypeId", "MaxSizeMB" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UploadComponentFileTypeSizes");
        }
    }
}

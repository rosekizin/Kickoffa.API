using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Kickoffa.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateChecklistAndItsRelatedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "ComponentSequence");

            migrationBuilder.CreateTable(
                name: "Checklists",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AccessToken = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checklists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComponentStatuses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ComponentId = table.Column<long>(type: "bigint", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Response = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChecklistId = table.Column<long>(type: "bigint", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SectionType = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    ContentJson = table.Column<string>(type: "TEXT", nullable: true),
                    ContentHtml = table.Column<string>(type: "TEXT", nullable: true),
                    ContentLastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sections_Checklists_ChecklistId",
                        column: x => x.ChecklistId,
                        principalTable: "Checklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BriefingMedias",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SectionId = table.Column<long>(type: "bigint", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    StoragePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: true),
                    Height = table.Column<int>(type: "integer", nullable: true),
                    AltText = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BriefingMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BriefingMedias_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckboxComponents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"ComponentSequence\"')"),
                    SectionId = table.Column<long>(type: "bigint", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckboxComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckboxComponents_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfirmationComponents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"ComponentSequence\"')"),
                    SectionId = table.Column<long>(type: "bigint", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ConfirmationText = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmationComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfirmationComponents_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SignatureComponents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"ComponentSequence\"')"),
                    SectionId = table.Column<long>(type: "bigint", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignatureComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignatureComponents_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TextComponents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"ComponentSequence\"')"),
                    SectionId = table.Column<long>(type: "bigint", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Placeholder = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MaxLength = table.Column<int>(type: "integer", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TextComponents_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UploadComponents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"ComponentSequence\"')"),
                    SectionId = table.Column<long>(type: "bigint", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    MaxSizeMB = table.Column<int>(type: "integer", nullable: true),
                    Placeholder = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UploadComponents_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UploadComponentAllowedFileTypes",
                columns: table => new
                {
                    AllowedFileTypesId = table.Column<long>(type: "bigint", nullable: false),
                    UploadComponentId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadComponentAllowedFileTypes", x => new { x.AllowedFileTypesId, x.UploadComponentId });
                    table.ForeignKey(
                        name: "FK_UploadComponentAllowedFileTypes_FileTypes_AllowedFileTypesId",
                        column: x => x.AllowedFileTypesId,
                        principalTable: "FileTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UploadComponentAllowedFileTypes_UploadComponents_UploadComp~",
                        column: x => x.UploadComponentId,
                        principalTable: "UploadComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UploadComponentFiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    StoragePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Sha256Hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UploadComponentId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadComponentFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UploadComponentFiles_UploadComponents_UploadComponentId",
                        column: x => x.UploadComponentId,
                        principalTable: "UploadComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BriefingMedias_ContentType",
                table: "BriefingMedias",
                column: "ContentType");

            migrationBuilder.CreateIndex(
                name: "IX_BriefingMedias_SectionId",
                table: "BriefingMedias",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_BriefingMedias_UploadedAt",
                table: "BriefingMedias",
                column: "UploadedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CheckboxComponents_IsRequired",
                table: "CheckboxComponents",
                column: "IsRequired",
                filter: "\"IsRequired\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_CheckboxComponents_SectionId",
                table: "CheckboxComponents",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckboxComponents_SectionId_IsRequired",
                table: "CheckboxComponents",
                columns: new[] { "SectionId", "IsRequired" });

            migrationBuilder.CreateIndex(
                name: "IX_CheckboxComponents_SectionId_Order",
                table: "CheckboxComponents",
                columns: new[] { "SectionId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_AccessToken",
                table: "Checklists",
                column: "AccessToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_AccessToken_OwnerId",
                table: "Checklists",
                columns: new[] { "AccessToken", "OwnerId" },
                filter: "\"IsPublished\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_OwnerId_CreatedDateUtc",
                table: "Checklists",
                columns: new[] { "OwnerId", "CreatedDateUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_OwnerId_IsPublished",
                table: "Checklists",
                columns: new[] { "OwnerId", "IsPublished" });

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_OwnerId_Slug",
                table: "Checklists",
                columns: new[] { "OwnerId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComponentStatuses_ComponentId",
                table: "ComponentStatuses",
                column: "ComponentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComponentStatuses_IsCompleted",
                table: "ComponentStatuses",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmationComponents_ConfirmationText",
                table: "ConfirmationComponents",
                column: "ConfirmationText");

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmationComponents_SectionId",
                table: "ConfirmationComponents",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmationComponents_SectionId_ConfirmationText",
                table: "ConfirmationComponents",
                columns: new[] { "SectionId", "ConfirmationText" });

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmationComponents_SectionId_Order",
                table: "ConfirmationComponents",
                columns: new[] { "SectionId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_Sections_ChecklistId",
                table: "Sections",
                column: "ChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_ChecklistId_Order",
                table: "Sections",
                columns: new[] { "ChecklistId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_Sections_SectionType",
                table: "Sections",
                column: "SectionType");

            migrationBuilder.CreateIndex(
                name: "IX_SignatureComponents_IsRequired",
                table: "SignatureComponents",
                column: "IsRequired",
                filter: "\"IsRequired\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_SignatureComponents_SectionId",
                table: "SignatureComponents",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_SignatureComponents_SectionId_IsRequired",
                table: "SignatureComponents",
                columns: new[] { "SectionId", "IsRequired" });

            migrationBuilder.CreateIndex(
                name: "IX_SignatureComponents_SectionId_IsRequired_Order",
                table: "SignatureComponents",
                columns: new[] { "SectionId", "IsRequired", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_SignatureComponents_SectionId_Order",
                table: "SignatureComponents",
                columns: new[] { "SectionId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_TextComponents_MaxLength",
                table: "TextComponents",
                column: "MaxLength");

            migrationBuilder.CreateIndex(
                name: "IX_TextComponents_Placeholder",
                table: "TextComponents",
                column: "Placeholder");

            migrationBuilder.CreateIndex(
                name: "IX_TextComponents_SectionId",
                table: "TextComponents",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_TextComponents_SectionId_Order",
                table: "TextComponents",
                columns: new[] { "SectionId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_TextComponents_SectionId_Placeholder",
                table: "TextComponents",
                columns: new[] { "SectionId", "Placeholder" });

            migrationBuilder.CreateIndex(
                name: "IX_UploadComponentAllowedFileTypes_UploadComponentId",
                table: "UploadComponentAllowedFileTypes",
                column: "UploadComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadComponentFiles_FileName",
                table: "UploadComponentFiles",
                column: "FileName");

            migrationBuilder.CreateIndex(
                name: "IX_UploadComponentFiles_Sha256Hash",
                table: "UploadComponentFiles",
                column: "Sha256Hash");

            migrationBuilder.CreateIndex(
                name: "IX_UploadComponentFiles_UploadComponentId",
                table: "UploadComponentFiles",
                column: "UploadComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadComponentFiles_UploadComponentId_CreatedDateUtc",
                table: "UploadComponentFiles",
                columns: new[] { "Id", "CreatedDateUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_UploadComponents_MaxSizeMB",
                table: "UploadComponents",
                column: "MaxSizeMB");

            migrationBuilder.CreateIndex(
                name: "IX_UploadComponents_Placeholder",
                table: "UploadComponents",
                column: "Placeholder");

            migrationBuilder.CreateIndex(
                name: "IX_UploadComponents_SectionId",
                table: "UploadComponents",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadComponents_SectionId_MaxSizeMB",
                table: "UploadComponents",
                columns: new[] { "SectionId", "MaxSizeMB" });

            migrationBuilder.CreateIndex(
                name: "IX_UploadComponents_SectionId_Order",
                table: "UploadComponents",
                columns: new[] { "SectionId", "Order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BriefingMedias");

            migrationBuilder.DropTable(
                name: "CheckboxComponents");

            migrationBuilder.DropTable(
                name: "ComponentStatuses");

            migrationBuilder.DropTable(
                name: "ConfirmationComponents");

            migrationBuilder.DropTable(
                name: "SignatureComponents");

            migrationBuilder.DropTable(
                name: "TextComponents");

            migrationBuilder.DropTable(
                name: "UploadComponentAllowedFileTypes");

            migrationBuilder.DropTable(
                name: "UploadComponentFiles");

            migrationBuilder.DropTable(
                name: "UploadComponents");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "Checklists");

            migrationBuilder.DropSequence(
                name: "ComponentSequence");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kickoffa.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialKickOffaDatabaseWithCustomersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastUpdatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Cnpj",
                table: "Customers",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Cpf",
                table: "Customers",
                column: "Cpf",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}

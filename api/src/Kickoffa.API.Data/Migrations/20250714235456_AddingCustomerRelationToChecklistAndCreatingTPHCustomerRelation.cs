using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kickoffa.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingCustomerRelationToChecklistAndCreatingTPHCustomerRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_Cnpj",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_Cpf",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "Customers",
                newName: "Address");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Customers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Customers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "Customers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OwnerId",
                table: "Customers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Customers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "CustomerId",
                table: "Checklists",
                type: "bigint",
                nullable: false,
                defaultValue: 6L);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Cnpj",
                table: "Customers",
                column: "Cnpj",
                unique: true,
                filter: "\"Cnpj\" IS NOT NULL AND \"Type\" = 2");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Cpf",
                table: "Customers",
                column: "Cpf",
                unique: true,
                filter: "\"Cpf\" IS NOT NULL AND \"Type\" = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Type",
                table: "Customers",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_CustomerId",
                table: "Checklists",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_OwnerId_CustomerId",
                table: "Checklists",
                columns: new[] { "OwnerId", "CustomerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Checklists_Customers_CustomerId",
                table: "Checklists",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checklists_Customers_CustomerId",
                table: "Checklists");

            migrationBuilder.DropIndex(
                name: "IX_Customers_Cnpj",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_Cpf",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_Type",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Checklists_CustomerId",
                table: "Checklists");

            migrationBuilder.DropIndex(
                name: "IX_Checklists_OwnerId_CustomerId",
                table: "Checklists");

            migrationBuilder.DropColumn(
                name: "Company",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Checklists");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Customers",
                newName: "address");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Customers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Customers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

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
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gdn.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductManagement : Migration
    {
        /// <summary>
        /// Applies the product management schema changes: updates the Products table (adds Type and MeasurementUnit,
        /// removes Name and Stock, widens Code and Description), and adds ProductId FK to the three row tables.
        /// </summary>
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Products",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<int>(
                name: "MeasurementUnitId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Products",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "PRD");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "InvoiceRows",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "InterventionRows",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "CreditNoteRows",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Code",
                table: "Products",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Products_MeasurementUnitId",
                table: "Products",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceRows_ProductId",
                table: "InvoiceRows",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionRows_ProductId",
                table: "InterventionRows",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNoteRows_ProductId",
                table: "CreditNoteRows",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_MeasurementUnits_MeasurementUnitId",
                table: "Products",
                column: "MeasurementUnitId",
                principalTable: "MeasurementUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceRows_Products_ProductId",
                table: "InvoiceRows",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_InterventionRows_Products_ProductId",
                table: "InterventionRows",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditNoteRows_Products_ProductId",
                table: "CreditNoteRows",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_MeasurementUnits_MeasurementUnitId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceRows_Products_ProductId",
                table: "InvoiceRows");

            migrationBuilder.DropForeignKey(
                name: "FK_InterventionRows_Products_ProductId",
                table: "InterventionRows");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditNoteRows_Products_ProductId",
                table: "CreditNoteRows");

            migrationBuilder.DropIndex(
                name: "IX_Products_Code",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_MeasurementUnitId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceRows_ProductId",
                table: "InvoiceRows");

            migrationBuilder.DropIndex(
                name: "IX_InterventionRows_ProductId",
                table: "InterventionRows");

            migrationBuilder.DropIndex(
                name: "IX_CreditNoteRows_ProductId",
                table: "CreditNoteRows");

            migrationBuilder.DropColumn(
                name: "MeasurementUnitId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "InvoiceRows");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "InterventionRows");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "CreditNoteRows");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Products",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Stock",
                table: "Products",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gdn.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultTaxRateToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultTaxRateId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_DefaultTaxRateId",
                table: "Customers",
                column: "DefaultTaxRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_TaxRates_DefaultTaxRateId",
                table: "Customers",
                column: "DefaultTaxRateId",
                principalTable: "TaxRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_TaxRates_DefaultTaxRateId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_DefaultTaxRateId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DefaultTaxRateId",
                table: "Customers");
        }
    }
}

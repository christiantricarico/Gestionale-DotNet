using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gdn.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteDuesOnInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dues_Invoices_InvoiceId",
                table: "Dues");

            migrationBuilder.AddForeignKey(
                name: "FK_Dues_Invoices_InvoiceId",
                table: "Dues",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dues_Invoices_InvoiceId",
                table: "Dues");

            migrationBuilder.AddForeignKey(
                name: "FK_Dues_Invoices_InvoiceId",
                table: "Dues",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gdn.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStampDutyToInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "StampDutyAmount",
                table: "Invoices",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "StampDutyChargedToCustomer",
                table: "Invoices",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StampDutyAmount",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "StampDutyChargedToCustomer",
                table: "Invoices");
        }
    }
}

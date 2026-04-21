using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gdn.Persistence.Migrations
{
    /// <inheritdoc />
public partial class AddInterventionReportManagement : Migration
{
    /// <summary>
    /// Adds intervention report management persistence by creating intervention report header and row tables,
    /// with links to customer, invoice, measurement unit, and tax rate data.
    /// </summary>
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InterventionReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    IsInvoiced = table.Column<bool>(type: "bit", nullable: false),
                    InvoiceId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterventionReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterventionReports_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                     table.ForeignKey(
                         name: "FK_InterventionReports_Invoices_InvoiceId",
                         column: x => x.InvoiceId,
                         principalTable: "Invoices",
                         principalColumn: "Id");
                 });

            migrationBuilder.CreateTable(
                name: "InterventionReportRows",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RowType = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    InterventionReportId = table.Column<int>(type: "int", nullable: false),
                    MeasurementUnitId = table.Column<int>(type: "int", nullable: true),
                    TaxRateId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterventionReportRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterventionReportRows_InterventionReports_InterventionReportId",
                        column: x => x.InterventionReportId,
                        principalTable: "InterventionReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterventionReportRows_MeasurementUnits_MeasurementUnitId",
                        column: x => x.MeasurementUnitId,
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InterventionReportRows_TaxRates_TaxRateId",
                        column: x => x.TaxRateId,
                        principalTable: "TaxRates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterventionReportRows_InterventionReportId",
                table: "InterventionReportRows",
                column: "InterventionReportId");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionReportRows_MeasurementUnitId",
                table: "InterventionReportRows",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionReportRows_TaxRateId",
                table: "InterventionReportRows",
                column: "TaxRateId");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionReports_CustomerId",
                table: "InterventionReports",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionReports_Date",
                table: "InterventionReports",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionReports_InvoiceId",
                table: "InterventionReports",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionReports_IsInvoiced",
                table: "InterventionReports",
                column: "IsInvoiced");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterventionReportRows");

            migrationBuilder.DropTable(
                name: "InterventionReports");
        }
    }
}

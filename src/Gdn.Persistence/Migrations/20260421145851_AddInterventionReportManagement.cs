using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gdn.Persistence.Migrations
{
    /// <inheritdoc />
public partial class AddInterventionManagement : Migration
{
    /// <summary>
    /// Adds intervention report management persistence by creating intervention report header and row tables,
    /// with links to customer, invoice, measurement unit, and tax rate data.
    /// </summary>
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Interventions",
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
                    table.PrimaryKey("PK_Interventions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interventions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                     table.ForeignKey(
                         name: "FK_Interventions_Invoices_InvoiceId",
                         column: x => x.InvoiceId,
                         principalTable: "Invoices",
                         principalColumn: "Id");
                 });

            migrationBuilder.CreateTable(
                name: "InterventionRows",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RowType = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    InterventionId = table.Column<int>(type: "int", nullable: false),
                    MeasurementUnitId = table.Column<int>(type: "int", nullable: true),
                    TaxRateId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterventionRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterventionRows_Interventions_InterventionId",
                        column: x => x.InterventionId,
                        principalTable: "Interventions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterventionRows_MeasurementUnits_MeasurementUnitId",
                        column: x => x.MeasurementUnitId,
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InterventionRows_TaxRates_TaxRateId",
                        column: x => x.TaxRateId,
                        principalTable: "TaxRates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterventionRows_InterventionId",
                table: "InterventionRows",
                column: "InterventionId");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionRows_MeasurementUnitId",
                table: "InterventionRows",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionRows_TaxRateId",
                table: "InterventionRows",
                column: "TaxRateId");

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_CustomerId",
                table: "Interventions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_Date",
                table: "Interventions",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_InvoiceId",
                table: "Interventions",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_IsInvoiced",
                table: "Interventions",
                column: "IsInvoiced");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterventionRows");

            migrationBuilder.DropTable(
                name: "Interventions");
        }
    }
}

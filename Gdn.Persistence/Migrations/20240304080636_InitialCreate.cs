using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Gdn.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Level = table.Column<int>(type: "int", nullable: false),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCategories_ProductCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaxRateNatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxRateNatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaxRateNatureId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxRates_TaxRateNatures_TaxRateNatureId",
                        column: x => x.TaxRateNatureId,
                        principalTable: "TaxRateNatures",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "TaxRateNatures",
                columns: new[] { "Id", "Code", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, "N1", false, "N1 : Escluso art.15" },
                    { 2, "N2", false, "N2 : Non soggette" },
                    { 3, "N2.1", false, "N2.1 : Non soggette artt. Da 7 a 7-septies" },
                    { 4, "N2.2", false, "N2.2 : Non soggette - altri casi" },
                    { 5, "N3", false, "N2.2 : Non soggette - altri casi" },
                    { 6, "N3.1", false, "N3.1 : Non imponibili - esportazioni" },
                    { 7, "N3.2", false, "N3.2 : Non imponibili - cessioni intracomunitarie" },
                    { 8, "N3.3", false, "N3.3 : Non imponibili - cessioni verso San Marino" },
                    { 9, "N3.4", false, "N3.4 : Non imponibili - op. assimilate alle esportazioni" },
                    { 10, "N3.5", false, "N3.5 : Non imponibili - a seguito di dichiarazioni d?intento" },
                    { 11, "N3.6", false, "N3.6 : Non imponibili - altre op. no plafond" },
                    { 12, "N4", false, "N4 : Esenti" },
                    { 13, "N5", false, "N5 : Regime del margine / IVA non esposta" },
                    { 14, "N6", false, "N6 : Inversione contabile" },
                    { 15, "N6.1", false, "N6.1 : Inversione contabile - cessione di rottami" },
                    { 16, "N6.2", false, "N6.2 : Inversione contabile - cessione di oro e argento puro" },
                    { 17, "N6.3", false, "N6.3 : Inversione contabile - subappalto nel settore edile" },
                    { 18, "N6.4", false, "N6.4 : Inversione contabile - cessione di fabbricati" },
                    { 19, "N6.5", false, "N6.5 : Inversione contabile - cessione di telefoni cellulari" },
                    { 20, "N6.6", false, "N6.6 : Inversione contabile - cessione di prodotti elettronici" },
                    { 21, "N6.7", false, "N6.7 : Inversione contabile - prestazioni comparto edile" },
                    { 22, "N6.8", false, "N6.8 : Inversione contabile - op. settore energetico" },
                    { 23, "N6.9", false, "N6.9 : Inversione contabile - altri casi" },
                    { 24, "N7", false, "N7 : IVA assolta in altro stato UE" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ParentCategoryId",
                table: "ProductCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_TaxRateNatureId",
                table: "TaxRates",
                column: "TaxRateNatureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "TaxRates");

            migrationBuilder.DropTable(
                name: "TaxRateNatures");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gdn.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false, defaultValue: 0m),
                    InvoiceId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dues_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dues_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DigitalInvoiceCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentDues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DueId = table.Column<int>(type: "int", nullable: false),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentDues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentDues_Dues_DueId",
                        column: x => x.DueId,
                        principalTable: "Dues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentDues_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dues_CustomerId",
                table: "Dues",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Dues_InvoiceId",
                table: "Dues",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDues_DueId_PaymentId",
                table: "PaymentDues",
                columns: new[] { "DueId", "PaymentId" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDues_PaymentId",
                table: "PaymentDues",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentMethodId",
                table: "Payments",
                column: "PaymentMethodId");

            migrationBuilder.Sql("""
                CREATE TRIGGER TR_PaymentDues_AfterInsert
                ON PaymentDues
                AFTER INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE d
                    SET d.PaidAmount = d.PaidAmount + i.Amount
                    FROM Dues d
                    INNER JOIN inserted i ON d.Id = i.DueId;
                END;
                """);

            migrationBuilder.Sql("""
                CREATE TRIGGER TR_PaymentDues_AfterDelete
                ON PaymentDues
                AFTER DELETE
                AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE d
                    SET d.PaidAmount = d.PaidAmount - del.Amount
                    FROM Dues d
                    INNER JOIN deleted del ON d.Id = del.DueId;
                END;
                """);

            migrationBuilder.Sql("""
                CREATE TRIGGER TR_PaymentDues_AfterUpdate
                ON PaymentDues
                AFTER UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE d
                    SET d.PaidAmount = d.PaidAmount + (i.Amount - del.Amount)
                    FROM Dues d
                    INNER JOIN inserted i ON d.Id = i.DueId
                    INNER JOIN deleted del ON del.Id = i.Id;
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TR_PaymentDues_AfterUpdate;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TR_PaymentDues_AfterDelete;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TR_PaymentDues_AfterInsert;");

            migrationBuilder.DropTable(
                name: "PaymentDues");

            migrationBuilder.DropTable(
                name: "Dues");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "PaymentMethods");
        }
    }
}

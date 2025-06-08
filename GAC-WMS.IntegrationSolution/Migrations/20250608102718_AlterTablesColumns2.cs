using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GAC_WMS.IntegrationSolution.Migrations
{
    /// <inheritdoc />
    public partial class AlterTablesColumns2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderItems_Products_ProductId",
                table: "PurchaseOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderItems_ProductId",
                table: "PurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "PurchaseOrders");

            migrationBuilder.AddColumn<string>(
                name: "CustomerIdentifier",
                table: "PurchaseOrders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Customers_CustomerIdentifier",
                table: "Customers",
                column: "CustomerIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_CustomerId",
                table: "SalesOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_CustomerIdentifier",
                table: "PurchaseOrders",
                column: "CustomerIdentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Customers_CustomerIdentifier",
                table: "PurchaseOrders",
                column: "CustomerIdentifier",
                principalTable: "Customers",
                principalColumn: "CustomerIdentifier",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrders_Customers_CustomerId",
                table: "SalesOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Customers_CustomerIdentifier",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrders_Customers_CustomerId",
                table: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrders_CustomerId",
                table: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_CustomerIdentifier",
                table: "PurchaseOrders");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Customers_CustomerIdentifier",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CustomerIdentifier",
                table: "PurchaseOrders");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "PurchaseOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_ProductId",
                table: "PurchaseOrderItems",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderItems_Products_ProductId",
                table: "PurchaseOrderItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

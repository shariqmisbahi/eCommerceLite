using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerceLite.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceAndDispatchStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Dispatched",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InvoiceGenerated",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dispatched",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "InvoiceGenerated",
                table: "Orders");
        }
    }
}

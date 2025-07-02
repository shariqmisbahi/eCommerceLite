using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerceLite.Migrations
{
    /// <inheritdoc />
    public partial class columnTypeChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Dispatched",
                table: "Orders",
                newName: "IsDispatched");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deliveryDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDispatched",
                table: "Orders",
                newName: "Dispatched");

            migrationBuilder.AlterColumn<string>(
                name: "deliveryDate",
                table: "Orders",
               type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "nvarchar(max)");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerceLite.Migrations
{
    /// <inheritdoc />
    public partial class FixProductEmbeddingsPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductEmbeddings",
                columns: table => new
                {
                    productEmbeddingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    productDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    productDimension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    productPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductEmbeddings", x => x.productEmbeddingId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductEmbeddings");
        }
    }
}

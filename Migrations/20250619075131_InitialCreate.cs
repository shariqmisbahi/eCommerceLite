using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerceLite.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    whatAppNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cakeSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    flavor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    customFlavor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    colourTheme = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    referenceDesign = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    messageOnCake = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fullDeliveryAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    deliveryDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    deliveryTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    specialInstructions = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}

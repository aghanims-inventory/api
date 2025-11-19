using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AghanimsInventoryApi.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderForRarityEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Order",
                table: "Rarities",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Rarities");
        }
    }
}

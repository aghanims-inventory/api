using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AghanimsInventoryApi.Migrations
{
    /// <inheritdoc />
    public partial class AddNameForHeroEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Heroes",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Heroes");
        }
    }
}

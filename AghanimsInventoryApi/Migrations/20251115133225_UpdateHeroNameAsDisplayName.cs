using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AghanimsInventoryApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHeroNameAsDisplayName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Heroes",
                newName: "DisplayName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "Heroes",
                newName: "Name");
        }
    }
}

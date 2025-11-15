using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AghanimsInventoryApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHeroIdAsUniqueForHeroAttributeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_HeroAttributes_HeroId",
                table: "HeroAttributes",
                column: "HeroId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HeroAttributes_HeroId",
                table: "HeroAttributes");
        }
    }
}

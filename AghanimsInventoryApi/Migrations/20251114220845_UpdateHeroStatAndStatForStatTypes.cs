using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AghanimsInventoryApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHeroStatAndStatForStatTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HeroStats_StatId_StatTypeId_HeroId",
                table: "HeroStats");

            migrationBuilder.DropColumn(
                name: "StatTypeId",
                table: "HeroStats");

            migrationBuilder.AddColumn<int>(
                name: "StatTypeId",
                table: "Stats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Stats_Name",
                table: "Stats",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroStats_StatId_HeroId",
                table: "HeroStats",
                columns: new[] { "StatId", "HeroId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stats_Name",
                table: "Stats");

            migrationBuilder.DropIndex(
                name: "IX_HeroStats_StatId_HeroId",
                table: "HeroStats");

            migrationBuilder.DropColumn(
                name: "StatTypeId",
                table: "Stats");

            migrationBuilder.AddColumn<int>(
                name: "StatTypeId",
                table: "HeroStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HeroStats_StatId_StatTypeId_HeroId",
                table: "HeroStats",
                columns: new[] { "StatId", "StatTypeId", "HeroId" },
                unique: true);
        }
    }
}

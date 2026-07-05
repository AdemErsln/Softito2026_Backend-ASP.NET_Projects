using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Franchise.Migrations
{
    /// <inheritdoc />
    public partial class guncelleme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PackageID",
                table: "Franchise_Buyers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Franchise_Buyers_PackageID",
                table: "Franchise_Buyers",
                column: "PackageID");

            migrationBuilder.AddForeignKey(
                name: "FK_Franchise_Buyers_Franchise_Packages_PackageID",
                table: "Franchise_Buyers",
                column: "PackageID",
                principalTable: "Franchise_Packages",
                principalColumn: "PackageID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Franchise_Buyers_Franchise_Packages_PackageID",
                table: "Franchise_Buyers");

            migrationBuilder.DropIndex(
                name: "IX_Franchise_Buyers_PackageID",
                table: "Franchise_Buyers");

            migrationBuilder.DropColumn(
                name: "PackageID",
                table: "Franchise_Buyers");
        }
    }
}

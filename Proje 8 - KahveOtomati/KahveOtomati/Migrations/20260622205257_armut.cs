using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KahveOtomati.Migrations
{
    /// <inheritdoc />
    public partial class armut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArizaKayitlari",
                columns: table => new
                {
                    ArizaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArizaKayitlari", x => x.ArizaID);
                });

            migrationBuilder.CreateTable(
                name: "Icecekler",
                columns: table => new
                {
                    IcecekID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IcecekAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fiyat = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Icecekler", x => x.IcecekID);
                });

            migrationBuilder.CreateTable(
                name: "MalzemeStoklari",
                columns: table => new
                {
                    StokID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MalzemeAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Miktar = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MalzemeStoklari", x => x.StokID);
                });

            migrationBuilder.CreateTable(
                name: "Satislar",
                columns: table => new
                {
                    SatisID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IcecekAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Satislar", x => x.SatisID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArizaKayitlari");

            migrationBuilder.DropTable(
                name: "Icecekler");

            migrationBuilder.DropTable(
                name: "MalzemeStoklari");

            migrationBuilder.DropTable(
                name: "Satislar");
        }
    }
}

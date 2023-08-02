using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProaktifArizaTahmini.DAL.Migrations
{
    /// <inheritdoc />
    public partial class mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MyDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TM_No = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    kV = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    Hucre_No = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    Tm_kV_Hucre = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Fider_Adi = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IP = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Role_Model = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Port = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    User = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Password = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Path = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Disturbances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TM_No = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    kV = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Hucre_No = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Tm_KV_Hucre = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Fider_Adi = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    IP = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Role_Model = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Ariza_Saati = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Cfg_Dosya_Yolu = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Cfg_Dosyasi = table.Column<string>(type: "CLOB", nullable: false),
                    Dat_Dosya_Yolu = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Dat_Dosyasi = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Comtrade_Dosya_Ismi = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Rms_Dosya_Yolu = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Instant_Dosya_Yolu = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Status = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    MyDataId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disturbances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Disturbances_MyDatas_MyDataId",
                        column: x => x.MyDataId,
                        principalTable: "MyDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoryOfChanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    New_IP = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Old_IP = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Degistirilme_Tarihi = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    MyDataId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryOfChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoryOfChanges_MyDatas_MyDataId",
                        column: x => x.MyDataId,
                        principalTable: "MyDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Disturbances_MyDataId",
                table: "Disturbances",
                column: "MyDataId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryOfChanges_MyDataId",
                table: "HistoryOfChanges",
                column: "MyDataId");

            migrationBuilder.CreateIndex(
                name: "IX_MyDatas_TM_No_kV_Hucre_No",
                table: "MyDatas",
                columns: new[] { "TM_No", "kV", "Hucre_No" },
                unique: true,
                filter: "\"kV\" IS NOT NULL AND \"Hucre_No\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Disturbances");

            migrationBuilder.DropTable(
                name: "HistoryOfChanges");

            migrationBuilder.DropTable(
                name: "MyDatas");
        }
    }
}

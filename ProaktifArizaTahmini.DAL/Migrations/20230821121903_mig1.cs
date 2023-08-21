using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProaktifArizaTahmini.DAL.Migrations
{
    /// <inheritdoc />
    public partial class mig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RelayInformations",
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
                    table.PrimaryKey("PK_RelayInformations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Message = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Exception = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ServiceName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    LogLevel = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    UserTypeName = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypes", x => x.Id);
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
                    Fider_Adi = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IP = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Role_Model = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Ariza_Saati_Baslangic = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Ariza_Saati_Bitis = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Ariza_Saati_Suresi = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Cfg_Dosya_Yolu = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Cfg_Dosyasi = table.Column<string>(type: "CLOB", nullable: true),
                    Dat_Dosya_Yolu = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Dat_Dosyasi = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Comtrade_Dosya_Ismi = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Rms_Dosya_Yolu = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Instant_Dosya_Yolu = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Gönderilme_Tarihi = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Status = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    SFTP_Durumu = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    RelayInformationId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disturbances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Disturbances_RelayInformations_RelayInformationId",
                        column: x => x.RelayInformationId,
                        principalTable: "RelayInformations",
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
                    RelayInformationId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryOfChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoryOfChanges_RelayInformations_RelayInformationId",
                        column: x => x.RelayInformationId,
                        principalTable: "RelayInformations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TC_Kimlik = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    Kullanici_Adi = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Sifre = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    İsim = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    Soyisim = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    Telefon = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    Cep_Telefonu = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    Şirket = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    Departman = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    Unvan = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    Yönetici = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    Dogum_Tarihi = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Adres = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    Resim = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    UserTypeId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Son_Giris_Tarihi = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_UserTypes_UserTypeId",
                        column: x => x.UserTypeId,
                        principalTable: "UserTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    İsim = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Soyisim = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Kullanici_Adi = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Metot_İsmi = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Tarih = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UserId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "UserTypes",
                columns: new[] { "Id", "Description", "UserTypeName" },
                values: new object[,]
                {
                    { 1, "Domain", 1 },
                    { 2, "Misafir", 2 },
                    { 3, "Test", 3 },
                    { 4, "Kurum Dışı", 4 },
                    { 5, "Kurum İçi Domainsiz", 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Disturbances_RelayInformationId",
                table: "Disturbances",
                column: "RelayInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryOfChanges_RelayInformationId",
                table: "HistoryOfChanges",
                column: "RelayInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_RelayInformations_TM_No_kV_Hucre_No",
                table: "RelayInformations",
                columns: new[] { "TM_No", "kV", "Hucre_No" },
                unique: true,
                filter: "\"kV\" IS NOT NULL AND \"Hucre_No\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogs_UserId",
                table: "UserLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserTypeId",
                table: "Users",
                column: "UserTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Disturbances");

            migrationBuilder.DropTable(
                name: "HistoryOfChanges");

            migrationBuilder.DropTable(
                name: "ServiceLogs");

            migrationBuilder.DropTable(
                name: "UserLogs");

            migrationBuilder.DropTable(
                name: "RelayInformations");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserTypes");
        }
    }
}

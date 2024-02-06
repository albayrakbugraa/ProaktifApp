using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProaktifArizaTahmini.DAL.Migrations
{
    /// <inheritdoc />
    public partial class mig10224 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sifre",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Kullanici_Adi",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "Soyisim",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "İsim",
                table: "UserLogs");

            migrationBuilder.RenameColumn(
                name: "Metot_İsmi",
                table: "UserLogs",
                newName: "Metot_Ismi");

            migrationBuilder.RenameColumn(
                name: "Açıklama",
                table: "UserLogs",
                newName: "Aciklama");

            migrationBuilder.AddColumn<int>(
                name: "ThreadID",
                table: "ServiceLogs",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "RelayInformations",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThreadID",
                table: "ServiceLogs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "RelayInformations");

            migrationBuilder.RenameColumn(
                name: "Metot_Ismi",
                table: "UserLogs",
                newName: "Metot_İsmi");

            migrationBuilder.RenameColumn(
                name: "Aciklama",
                table: "UserLogs",
                newName: "Açıklama");

            migrationBuilder.AddColumn<string>(
                name: "Sifre",
                table: "Users",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Kullanici_Adi",
                table: "UserLogs",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Soyisim",
                table: "UserLogs",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "İsim",
                table: "UserLogs",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "");
        }
    }
}

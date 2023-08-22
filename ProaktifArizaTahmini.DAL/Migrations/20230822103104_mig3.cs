using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProaktifArizaTahmini.DAL.Migrations
{
    /// <inheritdoc />
    public partial class mig3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogDate",
                table: "UserLogs");

            migrationBuilder.RenameColumn(
                name: "LogLevel",
                table: "UserLogs",
                newName: "Log_Seviyesi");

            migrationBuilder.RenameColumn(
                name: "Exception",
                table: "UserLogs",
                newName: "Hata");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Tarih",
                table: "UserLogs",
                type: "TIMESTAMP(7)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Açıklama",
                table: "UserLogs",
                type: "NVARCHAR2(2000)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Açıklama",
                table: "UserLogs");

            migrationBuilder.RenameColumn(
                name: "Log_Seviyesi",
                table: "UserLogs",
                newName: "LogLevel");

            migrationBuilder.RenameColumn(
                name: "Hata",
                table: "UserLogs",
                newName: "Exception");

            migrationBuilder.AlterColumn<string>(
                name: "Tarih",
                table: "UserLogs",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");

            migrationBuilder.AddColumn<DateTime>(
                name: "LogDate",
                table: "UserLogs",
                type: "TIMESTAMP(7)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}

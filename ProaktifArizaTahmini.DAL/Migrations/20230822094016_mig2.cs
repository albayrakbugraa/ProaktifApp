using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProaktifArizaTahmini.DAL.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Tarih",
                table: "UserLogs",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");

            migrationBuilder.AddColumn<string>(
                name: "Exception",
                table: "UserLogs",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LogDate",
                table: "UserLogs",
                type: "TIMESTAMP(7)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LogLevel",
                table: "UserLogs",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Exception",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "LogDate",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "LogLevel",
                table: "UserLogs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Tarih",
                table: "UserLogs",
                type: "TIMESTAMP(7)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);
        }
    }
}

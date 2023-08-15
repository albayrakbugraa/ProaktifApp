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
                name: "MethodName",
                table: "UserLogs",
                type: "NVARCHAR2(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "UserLogs",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "UserLogs",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "UserLogs",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "UserLogs");

            migrationBuilder.AlterColumn<string>(
                name: "MethodName",
                table: "UserLogs",
                type: "NVARCHAR2(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");
        }
    }
}

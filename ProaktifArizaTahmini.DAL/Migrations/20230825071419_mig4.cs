using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProaktifArizaTahmini.DAL.Migrations
{
    /// <inheritdoc />
    public partial class mig4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RelayInformations_TM_No_kV_Hucre_No",
                table: "RelayInformations");

            migrationBuilder.AlterColumn<string>(
                name: "kV",
                table: "RelayInformations",
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "RelayInformations",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IP",
                table: "RelayInformations",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Hucre_No",
                table: "RelayInformations",
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RelayInformations_TM_No_kV_Hucre_No",
                table: "RelayInformations",
                columns: new[] { "TM_No", "kV", "Hucre_No" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RelayInformations_TM_No_kV_Hucre_No",
                table: "RelayInformations");

            migrationBuilder.AlterColumn<string>(
                name: "kV",
                table: "RelayInformations",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "RelayInformations",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "IP",
                table: "RelayInformations",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "Hucre_No",
                table: "RelayInformations",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)");

            migrationBuilder.CreateIndex(
                name: "IX_RelayInformations_TM_No_kV_Hucre_No",
                table: "RelayInformations",
                columns: new[] { "TM_No", "kV", "Hucre_No" },
                unique: true,
                filter: "\"kV\" IS NOT NULL AND \"Hucre_No\" IS NOT NULL");
        }
    }
}

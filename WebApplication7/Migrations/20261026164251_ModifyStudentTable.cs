using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication7.Migrations
{
    /// <inheritdoc />
    public partial class ModifyStudentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartementId",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Students_DepartementId",
                table: "Students",
                column: "DepartementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Departements_DepartementId",
                table: "Students",
                column: "DepartementId",
                principalTable: "Departements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Departements_DepartementId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_DepartementId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "DepartementId",
                table: "Students");
        }
    }
}

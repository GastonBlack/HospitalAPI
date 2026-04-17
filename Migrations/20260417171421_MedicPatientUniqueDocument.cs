using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalAPI.Migrations
{
    /// <inheritdoc />
    public partial class MedicPatientUniqueDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Patients_Document",
                table: "Patients",
                column: "Document",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medics_Document",
                table: "Medics",
                column: "Document",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patients_Document",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Medics_Document",
                table: "Medics");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecondWebApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointmnets_Doctors_DoctorId",
                table: "appointmnets");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorSpecialities_Doctors_DoctorId",
                table: "DoctorSpecialities");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorSpecialities_specialities_SpecialityId",
                table: "DoctorSpecialities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DoctorSpecialities",
                table: "DoctorSpecialities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Doctors",
                table: "Doctors");

            migrationBuilder.RenameTable(
                name: "DoctorSpecialities",
                newName: "doctorSpecialities");

            migrationBuilder.RenameTable(
                name: "Doctors",
                newName: "doctors");

            migrationBuilder.RenameIndex(
                name: "IX_DoctorSpecialities_SpecialityId",
                table: "doctorSpecialities",
                newName: "IX_doctorSpecialities_SpecialityId");

            migrationBuilder.RenameIndex(
                name: "IX_DoctorSpecialities_DoctorId",
                table: "doctorSpecialities",
                newName: "IX_doctorSpecialities_DoctorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_doctorSpecialities",
                table: "doctorSpecialities",
                column: "SerialNumber");

            migrationBuilder.AddPrimaryKey(
                name: "PK_doctors",
                table: "doctors",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_appointmnets_doctors_DoctorId",
                table: "appointmnets",
                column: "DoctorId",
                principalTable: "doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_doctorSpecialities_doctors_DoctorId",
                table: "doctorSpecialities",
                column: "DoctorId",
                principalTable: "doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_doctorSpecialities_specialities_SpecialityId",
                table: "doctorSpecialities",
                column: "SpecialityId",
                principalTable: "specialities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointmnets_doctors_DoctorId",
                table: "appointmnets");

            migrationBuilder.DropForeignKey(
                name: "FK_doctorSpecialities_doctors_DoctorId",
                table: "doctorSpecialities");

            migrationBuilder.DropForeignKey(
                name: "FK_doctorSpecialities_specialities_SpecialityId",
                table: "doctorSpecialities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_doctorSpecialities",
                table: "doctorSpecialities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_doctors",
                table: "doctors");

            migrationBuilder.RenameTable(
                name: "doctorSpecialities",
                newName: "DoctorSpecialities");

            migrationBuilder.RenameTable(
                name: "doctors",
                newName: "Doctors");

            migrationBuilder.RenameIndex(
                name: "IX_doctorSpecialities_SpecialityId",
                table: "DoctorSpecialities",
                newName: "IX_DoctorSpecialities_SpecialityId");

            migrationBuilder.RenameIndex(
                name: "IX_doctorSpecialities_DoctorId",
                table: "DoctorSpecialities",
                newName: "IX_DoctorSpecialities_DoctorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DoctorSpecialities",
                table: "DoctorSpecialities",
                column: "SerialNumber");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Doctors",
                table: "Doctors",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_appointmnets_Doctors_DoctorId",
                table: "appointmnets",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorSpecialities_Doctors_DoctorId",
                table: "DoctorSpecialities",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorSpecialities_specialities_SpecialityId",
                table: "DoctorSpecialities",
                column: "SpecialityId",
                principalTable: "specialities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

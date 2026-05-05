using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeFirst.Migrations
{
    /// <inheritdoc />
    public partial class FinalFixForNullableFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_Categories_CategoryId",
                table: "CandidateProfiles");

            // Skip operations on Employer table since it's already renamed to Employers
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Employer_Users_UserId",
            //     table: "Employer");

            // migrationBuilder.DropForeignKey(
            //     name: "FK_JobListings_Employer_EmployerId",
            //     table: "JobListings");

            // Skip operations on FK_JobListings_Employers_EmployerId since it's already commented
            // migrationBuilder.DropForeignKey(
            //     name: "FK_JobListings_Employers_EmployerId",
            //     table: "JobListings");

            // migrationBuilder.DropIndex(
            //     name: "IX_Users_Email",
            //     table: "Users");

            // migrationBuilder.DropPrimaryKey(
            //     name: "PK_Employer",
            //     table: "Employer");

            // migrationBuilder.DropIndex(
            //     name: "IX_Employer_UserId",
            //     table: "Employer");

            // migrationBuilder.RenameTable(
            //     name: "Employer",
            //     newName: "Employers");

            migrationBuilder.RenameColumn(
                name: "userType",
                table: "Users",
                newName: "UserType");

            // migrationBuilder.RenameColumn(
            //     name: "Password",
            //     table: "Users",
            //     newName: "PasswordHash");

            // migrationBuilder.RenameColumn(
            //     name: "FullName",
            //     table: "Users",
            //     newName: "Name");

            // migrationBuilder.RenameColumn(
            //     name: "Selected",
            //     table: "Match",
            //     newName: "IsSelectedByAlgorithm");

            // migrationBuilder.RenameColumn(
            //     name: "IsSelected",
            //     table: "JobListings",
            //     newName: "IsRemote");

            migrationBuilder.AlterColumn<int>(
                name: "UserType",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // migrationBuilder.AddColumn<bool>(
            //     name: "IsEnable",
            //     table: "Users",
            //     type: "bit",
            //     nullable: false,
            //     defaultValue: false);

            // migrationBuilder.AddColumn<DateTime>(
            //     name: "MatchDate",
            //     table: "Match",
            //     type: "datetime2",
            //     nullable: false,
            //     defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // migrationBuilder.AddColumn<double>(
            //     name: "MatchScore",
            //     table: "Match",
            //     type: "float",
            //     nullable: false,
            //     defaultValue: 0.0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Payment",
                table: "JobListings",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            // migrationBuilder.AddColumn<string>(
            //     name: "Description",
            //     table: "JobListings",
            //     type: "nvarchar(max)",
            //     nullable: false,
            //     defaultValue: "");

            // migrationBuilder.AddColumn<bool>(
            //     name: "IsCatch",
            //     table: "JobListings",
            //     type: "bit",
            //     nullable: false,
            //     defaultValue: false);

            // migrationBuilder.AddColumn<bool>(
            //     name: "IsJobWithPepole",
            //     table: "JobListings",
            //     type: "bit",
            //     nullable: false,
            //     defaultValue: false);

            // migrationBuilder.AddColumn<DateTime>(
            //     name: "RequiredDate",
            //     table: "JobListings",
            //     type: "datetime2",
            //     nullable: false,
            //     defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // migrationBuilder.AddColumn<string>(
            //     name: "Title",
            //     table: "JobListings",
            //     type: "nvarchar(max)",
            //     nullable: false,
            //     defaultValue: "");

            // migrationBuilder.AddColumn<int>(
            //     name: "leveJob",
            //     table: "JobListings",
            //     type: "int",
            //     nullable: false,
            //     defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxDistance",
                table: "CandidateProfiles",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "CandidateProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // migrationBuilder.AlterColumn<int>(
            //     name: "CategoryId",
            //     table: "CandidateProfiles",
            //     type: "int",
            //     nullable: false,
            //     defaultValue: 0,
            //     oldClrType: typeof(int),
            //     oldType: "int",
            //     oldNullable: true);

            // migrationBuilder.AddColumn<string>(
            //     name: "CompanyName",
            //     table: "Employers",
            //     type: "nvarchar(max)",
            //     nullable: true);

            // migrationBuilder.AddColumn<bool>(
            //     name: "status",
            //     table: "Employers",
            //     type: "bit",
            //     nullable: false,
            //     defaultValue: false);

            // Skip adding primary key and indexes for Employers since they already exist
            // migrationBuilder.AddPrimaryKey(
            //     name: "PK_Employers",
            //     table: "Employers",
            //     column: "Id");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Employers_UserId",
            //     table: "Employers",
            //     column: "UserId");

            // migrationBuilder.AddForeignKey(
            //     name: "FK_CandidateProfiles_Categories_CategoryId",
            //     table: "CandidateProfiles",
            //     column: "CategoryId",
            //     principalTable: "Categories",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Cascade);

            // migrationBuilder.AddForeignKey(
            //     name: "FK_Employers_Users_UserId",
            //     table: "Employers",
            //     column: "UserId",
            //     principalTable: "Users",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Cascade);

            // migrationBuilder.AddForeignKey(
            //     name: "FK_JobListings_Employers_EmployerId",
            //     table: "JobListings",
            //     column: "EmployerId",
            //     principalTable: "Employers",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_Categories_CategoryId",
                table: "CandidateProfiles");

            // migrationBuilder.DropForeignKey(
            //     name: "FK_Employers_Users_UserId",
            //     table: "Employers");

            // migrationBuilder.DropForeignKey(
            //     name: "FK_JobListings_Employers_EmployerId",
            //     table: "JobListings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employers",
                table: "Employers");

            migrationBuilder.DropIndex(
                name: "IX_Employers_UserId",
                table: "Employers");

            // migrationBuilder.DropColumn(
            //     name: "IsEnable",
            //     table: "Users");

            // migrationBuilder.DropColumn(
            //     name: "MatchDate",
            //     table: "Match");

            // migrationBuilder.DropColumn(
            //     name: "MatchScore",
            //     table: "Match");

            // migrationBuilder.DropColumn(
            //     name: "Description",
            //     table: "JobListings");

            // migrationBuilder.DropColumn(
            //     name: "IsCatch",
            //     table: "JobListings");

            migrationBuilder.DropColumn(
                name: "IsJobWithPepole",
                table: "JobListings");

            migrationBuilder.DropColumn(
                name: "RequiredDate",
                table: "JobListings");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "JobListings");

            migrationBuilder.DropColumn(
                name: "leveJob",
                table: "JobListings");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "status",
                table: "Employers");

            migrationBuilder.RenameTable(
                name: "Employers",
                newName: "Employer");

            migrationBuilder.RenameColumn(
                name: "UserType",
                table: "Users",
                newName: "userType");

            // migrationBuilder.RenameColumn(
            //     name: "PasswordHash",
            //     table: "Users",
            //     newName: "Password");

            // migrationBuilder.RenameColumn(
            //     name: "Name",
            //     table: "Users",
            //     newName: "FullName");

            // migrationBuilder.RenameColumn(
            //     name: "IsSelectedByAlgorithm",
            //     table: "Match",
            //     newName: "Selected");

            // migrationBuilder.RenameColumn(
            //     name: "IsRemote",
            //     table: "JobListings",
            //     newName: "IsSelected");

            migrationBuilder.AlterColumn<string>(
                name: "userType",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Payment",
                table: "JobListings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaxDistance",
                table: "CandidateProfiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "CandidateProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "CandidateProfiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employer",
                table: "Employer",
                column: "Id");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Users_Email",
            //     table: "Users",
            //     column: "Email",
            //     unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employer_UserId",
                table: "Employer",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProfiles_Categories_CategoryId",
                table: "CandidateProfiles",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employer_Users_UserId",
                table: "Employer",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // migrationBuilder.AddForeignKey(
            //     name: "FK_JobListings_Employer_EmployerId",
            //     table: "JobListings",
            //     column: "EmployerId",
            //     principalTable: "Employer",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Cascade);
        }
    }
}

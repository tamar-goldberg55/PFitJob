using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeFirst.Migrations
{
    /// <inheritdoc />
    public partial class candidatemodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "CandidateProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_CategoryId",
                table: "CandidateProfiles",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProfiles_Categories_CategoryId",
                table: "CandidateProfiles",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_Categories_CategoryId",
                table: "CandidateProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_CategoryId",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "CandidateProfiles");
        }
    }
}

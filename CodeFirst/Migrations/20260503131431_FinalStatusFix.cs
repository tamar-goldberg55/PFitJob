using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeFirst.Migrations
{
    /// <inheritdoc />
    public partial class FinalStatusFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // השארנו רק את הוספת העמודה לטבלת המשרות כדי למנוע שגיאות על קשרים שלא קיימים
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Match",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "pending");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // פעולת ביטול - מחיקת העמודה במידת הצורך
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Match");
        }
    }
}
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyProject.Migrations
{
    /// <inheritdoc />
    public partial class _1123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Feedbacks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Feedbacks");
        }
    }
}

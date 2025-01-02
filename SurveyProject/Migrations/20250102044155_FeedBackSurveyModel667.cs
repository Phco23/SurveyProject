using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyProject.Migrations
{
    /// <inheritdoc />
    public partial class FeedBackSurveyModel667 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Response",
                table: "Feedbacks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Response",
                table: "Feedbacks");
        }
    }
}

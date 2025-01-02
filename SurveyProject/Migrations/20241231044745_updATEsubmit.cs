using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyProject.Migrations
{
    /// <inheritdoc />
    public partial class updATEsubmit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Feedbacks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Feedbacks",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

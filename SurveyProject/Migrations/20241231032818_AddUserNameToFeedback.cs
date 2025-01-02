using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyProject.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameToFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Feedbacks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Feedbacks");
        }
    }
}

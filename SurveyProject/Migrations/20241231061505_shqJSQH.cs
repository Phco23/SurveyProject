using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyProject.Migrations
{
    /// <inheritdoc />
    public partial class shqJSQH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Feedbacks",
                newName: "EmailSender");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmailSender",
                table: "Feedbacks",
                newName: "Email");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherManagement.Migrations
{
    /// <inheritdoc />
    public partial class AppUserLinkedEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LinkedEntityId",
                table: "AppUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkedEntityId",
                table: "AppUsers");
        }
    }
}

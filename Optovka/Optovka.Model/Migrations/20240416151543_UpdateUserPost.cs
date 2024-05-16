using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optovka.Model.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableQuantity",
                table: "UserPosts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableQuantity",
                table: "UserPosts");
        }
    }
}

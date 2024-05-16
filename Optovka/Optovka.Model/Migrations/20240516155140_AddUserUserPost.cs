using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optovka.Model.Migrations
{
    /// <inheritdoc />
    public partial class AddUserUserPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvailableQuantity",
                table: "UserPosts",
                newName: "TakenQuantity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TakenQuantity",
                table: "UserPosts",
                newName: "AvailableQuantity");
        }
    }
}

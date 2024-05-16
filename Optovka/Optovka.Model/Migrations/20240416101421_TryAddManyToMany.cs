using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optovka.Model.Migrations
{
    /// <inheritdoc />
    public partial class TryAddManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPost_AspNetUsers_AuthorUserId",
                table: "UserPost");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPost",
                table: "UserPost");

            migrationBuilder.RenameTable(
                name: "UserPost",
                newName: "UserPosts");

            migrationBuilder.RenameIndex(
                name: "IX_UserPost_AuthorUserId",
                table: "UserPosts",
                newName: "IX_UserPosts_AuthorUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPosts",
                table: "UserPosts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ApplicationUserUserPost",
                columns: table => new
                {
                    ParticipatedUserPostsId = table.Column<int>(type: "integer", nullable: false),
                    ParticipatingUsersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserUserPost", x => new { x.ParticipatedUserPostsId, x.ParticipatingUsersId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserUserPost_AspNetUsers_ParticipatingUsersId",
                        column: x => x.ParticipatingUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserUserPost_UserPosts_ParticipatedUserPostsId",
                        column: x => x.ParticipatedUserPostsId,
                        principalTable: "UserPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserUserPost_ParticipatingUsersId",
                table: "ApplicationUserUserPost",
                column: "ParticipatingUsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPosts_AspNetUsers_AuthorUserId",
                table: "UserPosts",
                column: "AuthorUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPosts_AspNetUsers_AuthorUserId",
                table: "UserPosts");

            migrationBuilder.DropTable(
                name: "ApplicationUserUserPost");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPosts",
                table: "UserPosts");

            migrationBuilder.RenameTable(
                name: "UserPosts",
                newName: "UserPost");

            migrationBuilder.RenameIndex(
                name: "IX_UserPosts_AuthorUserId",
                table: "UserPost",
                newName: "IX_UserPost_AuthorUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPost",
                table: "UserPost",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPost_AspNetUsers_AuthorUserId",
                table: "UserPost",
                column: "AuthorUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

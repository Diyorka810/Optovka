using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optovka.Model.Migrations
{
    /// <inheritdoc />
    public partial class AddTakenQuantityToUserUserPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserUserPost");

            migrationBuilder.CreateTable(
                name: "ApplicationUserUserPosts",
                columns: table => new
                {
                    ParticipatedUserPostId = table.Column<int>(type: "integer", nullable: false),
                    ParticipatingUserId = table.Column<string>(type: "text", nullable: false),
                    TakenQuantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserUserPosts", x => new { x.ParticipatedUserPostId, x.ParticipatingUserId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserUserPosts_AspNetUsers_ParticipatingUserId",
                        column: x => x.ParticipatingUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserUserPosts_UserPosts_ParticipatedUserPostId",
                        column: x => x.ParticipatedUserPostId,
                        principalTable: "UserPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserUserPosts_ParticipatedUserPostId",
                table: "ApplicationUserUserPosts",
                column: "ParticipatedUserPostId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserUserPosts_ParticipatingUserId",
                table: "ApplicationUserUserPosts",
                column: "ParticipatingUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserUserPosts");

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
        }
    }
}

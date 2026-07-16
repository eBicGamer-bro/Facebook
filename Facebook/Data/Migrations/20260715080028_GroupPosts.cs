using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class GroupPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupID",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_GroupID",
                table: "Posts",
                column: "GroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Groups_GroupID",
                table: "Posts",
                column: "GroupID",
                principalTable: "Groups",
                principalColumn: "GroupID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Groups_GroupID",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_GroupID",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "GroupID",
                table: "Posts");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdminID",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_AdminID",
                table: "Groups",
                column: "AdminID");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Users_AdminID",
                table: "Groups",
                column: "AdminID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Users_AdminID",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_AdminID",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "AdminID",
                table: "Groups");
        }
    }
}

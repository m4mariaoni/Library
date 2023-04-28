using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryAPI.Infrastructure.Migrations
{
    public partial class modifyborrowedbook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ISBN",
                table: "BorrowedBooks");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "BorrowedBooks");

            migrationBuilder.AddColumn<long>(
                name: "bookId",
                table: "BorrowedBooks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "userId",
                table: "BorrowedBooks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_BorrowedBooks_bookId",
                table: "BorrowedBooks",
                column: "bookId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowedBooks_userId",
                table: "BorrowedBooks",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowedBooks_Books_bookId",
                table: "BorrowedBooks",
                column: "bookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowedBooks_Users_userId",
                table: "BorrowedBooks",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowedBooks_Books_bookId",
                table: "BorrowedBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowedBooks_Users_userId",
                table: "BorrowedBooks");

            migrationBuilder.DropIndex(
                name: "IX_BorrowedBooks_bookId",
                table: "BorrowedBooks");

            migrationBuilder.DropIndex(
                name: "IX_BorrowedBooks_userId",
                table: "BorrowedBooks");

            migrationBuilder.DropColumn(
                name: "bookId",
                table: "BorrowedBooks");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "BorrowedBooks");

            migrationBuilder.AddColumn<string>(
                name: "ISBN",
                table: "BorrowedBooks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "BorrowedBooks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

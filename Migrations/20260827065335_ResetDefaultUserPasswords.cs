using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyInventory.Migrations
{
    /// <inheritdoc />
    public partial class ResetDefaultUserPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "ig0za9XUfGfivqDJZVwMNw==.uQFoojQzPCOSISW3e1fHJXTyNGqnv9u6mPd3/lvE4f4=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "SOKIlfjxDfqkENFT+Xu1tQ==.x/P2E9uTI+l04R9RioWn3SCwcSmLg4csIgSzTmiS3Mw=");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "8a1IXcuwiWerQZlVs4KtdA==.tRrTlZ2/7ulXGp0UOd0i/mAsykZjBTv41iPjN9zgtOg=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "0E08ehpYGi9UyxK8Z+Gh4g==.HeHPKKoe2QcbUIaVjqUnaKpsTs4m2UpcKlmSEBkbsCY=");
        }
    }
}

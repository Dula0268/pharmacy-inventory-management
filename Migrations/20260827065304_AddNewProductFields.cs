using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyInventory.Migrations
{
    /// <inheritdoc />
    public partial class AddNewProductFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_GroceryName",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "BatchNo",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ExdDate",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemType",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoOfPacks",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NoOfUnits",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OutColour",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PackPrice",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Packing",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Speciality",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalValue",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "UnitsPerPack",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_Products_Brand",
                table: "Products",
                column: "Brand");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Category",
                table: "Products",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ItemType",
                table: "Products",
                column: "ItemType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Brand",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_Category",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ItemType",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BatchNo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Count",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ExdDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "NoOfPacks",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "NoOfUnits",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OutColour",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PackPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Packing",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Speciality",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TotalValue",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UnitsPerPack",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "vPnT0nRJhTMtk1SNlD19Gw==.1Wz48j+SVLmKQ09Nw9lVZgrHN39QkYI6VyO6dkw3lHc=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "wlcd4NLFv9XgnYupEXnxjw==.MmLq0x46BHZE6s+qYrYzLWQKjiYt1Xvc5vqH+S/mm4Y=");

            migrationBuilder.CreateIndex(
                name: "IX_Products_GroceryName",
                table: "Products",
                column: "GroceryName");
        }
    }
}

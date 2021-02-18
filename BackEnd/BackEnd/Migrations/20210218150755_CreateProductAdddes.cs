using Microsoft.EntityFrameworkCore.Migrations;

namespace BackEnd.Migrations
{
    public partial class CreateProductAdddes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description2",
                table: "ProductDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Information",
                table: "ProductDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "ProductDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description2",
                table: "ProductDetails");

            migrationBuilder.DropColumn(
                name: "Information",
                table: "ProductDetails");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "ProductDetails");
        }
    }
}

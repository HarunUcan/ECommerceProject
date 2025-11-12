using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceProject.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddProductHasSizeOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasSizeOptions",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE Products SET HasSizeOptions = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasSizeOptions",
                table: "Products");
        }
    }
}

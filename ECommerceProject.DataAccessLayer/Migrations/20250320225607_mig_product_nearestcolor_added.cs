using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceProject.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig_product_nearestcolor_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NearestColor",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NearestColor",
                table: "Products");
        }
    }
}

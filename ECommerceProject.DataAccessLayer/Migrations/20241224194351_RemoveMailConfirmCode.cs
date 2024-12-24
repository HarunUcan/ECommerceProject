using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceProject.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMailConfirmCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MailConfirmCode",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MailConfirmCode",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }
    }
}

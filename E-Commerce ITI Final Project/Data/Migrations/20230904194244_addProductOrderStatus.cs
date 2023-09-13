using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce_ITI_Final_Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class addProductOrderStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OrderProducts",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "OrderProducts");
        }
    }
}

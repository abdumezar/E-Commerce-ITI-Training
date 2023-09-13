using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce_ITI_Final_Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class addDateToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AddedOn",
                table: "Products",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddedOn",
                table: "Products");
        }
    }
}

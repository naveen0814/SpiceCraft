using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpicecraftAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRestaurantDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankDetails",
                table: "Restaurants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FSSAILicensePath",
                table: "Restaurants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GSTDocumentPath",
                table: "Restaurants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GSTIN",
                table: "Restaurants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PANCardNumber",
                table: "Restaurants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankDetails",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "FSSAILicensePath",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "GSTDocumentPath",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "GSTIN",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "PANCardNumber",
                table: "Restaurants");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpicecraftAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantAndDeliveryPartnerLoginSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "DeliveryPartners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "DeliveryPartners");
        }
    }
}

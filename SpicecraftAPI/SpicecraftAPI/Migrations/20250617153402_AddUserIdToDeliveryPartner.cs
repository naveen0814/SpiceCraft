using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpicecraftAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToDeliveryPartner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "DeliveryPartners",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPartners_UserId",
                table: "DeliveryPartners",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryPartners_Users_UserId",
                table: "DeliveryPartners",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryPartners_Users_UserId",
                table: "DeliveryPartners");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryPartners_UserId",
                table: "DeliveryPartners");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DeliveryPartners");
        }
    }
}

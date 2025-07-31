using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpicecraftAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryPartnerAndAssignToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryPartnerId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeliveryPartners",
                columns: table => new
                {
                    DeliveryPartnerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PAN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DrivingLicensePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BikeRCPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryPartners", x => x.DeliveryPartnerId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryPartnerId",
                table: "Orders",
                column: "DeliveryPartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DeliveryPartners_DeliveryPartnerId",
                table: "Orders",
                column: "DeliveryPartnerId",
                principalTable: "DeliveryPartners",
                principalColumn: "DeliveryPartnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DeliveryPartners_DeliveryPartnerId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "DeliveryPartners");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DeliveryPartnerId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryPartnerId",
                table: "Orders");
        }
    }
}

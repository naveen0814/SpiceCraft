using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpicecraftAPI.Migrations
{
    /// <inheritdoc />
    public partial class ConvertRoleToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        ALTER TABLE [Users]
        ALTER COLUMN [Role] NVARCHAR(20) NOT NULL;
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        ALTER TABLE [Users]
        ALTER COLUMN [Role] INT NOT NULL;
    ");
        }
    }
}

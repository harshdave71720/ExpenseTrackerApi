using Microsoft.EntityFrameworkCore.Migrations;

namespace Expentracker.Identity.Infrastructure.Migrations
{
    public partial class AddedPropertyToTokenTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Revoked",
                table: "AspNetUserTokens",
                type: "tinyint(1)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Revoked",
                table: "AspNetUserTokens");
        }
    }
}

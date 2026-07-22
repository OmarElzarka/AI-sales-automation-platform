using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddResearchStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResearchStatus",
                table: "Leads",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResearchStatus",
                table: "Leads");
        }
    }
}

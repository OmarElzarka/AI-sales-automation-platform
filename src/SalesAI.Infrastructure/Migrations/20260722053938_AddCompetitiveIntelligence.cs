using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompetitiveIntelligence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompetitiveIntelligenceJson",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompetitiveIntelligenceJson",
                table: "Leads");
        }
    }
}

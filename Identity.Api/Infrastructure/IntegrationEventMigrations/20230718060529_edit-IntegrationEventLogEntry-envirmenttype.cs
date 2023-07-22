using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Api.Infrastructure.IntegrationEventMigrations
{
    /// <inheritdoc />
    public partial class editIntegrationEventLogEntryenvirmenttype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnvironmentType",
                table: "IntegrationEventLog",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnvironmentType",
                table: "IntegrationEventLog");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Api.Infrastructure.IntegrationEventMigrations
{
    /// <inheritdoc />
    public partial class editIntegrationEventLogEntryenvirmenttypechange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnvironmentType",
                table: "IntegrationEventLog",
                newName: "EventEnvironmentType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventEnvironmentType",
                table: "IntegrationEventLog",
                newName: "EnvironmentType");
        }
    }
}

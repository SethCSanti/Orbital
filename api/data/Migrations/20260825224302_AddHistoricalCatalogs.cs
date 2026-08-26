using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Orbital.Api.data.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoricalCatalogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceId",
                table: "Rockets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceUrl",
                table: "Rockets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceId",
                table: "Missions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceUrl",
                table: "Missions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceUrl",
                table: "Launches",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceId",
                table: "Astronauts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceUrl",
                table: "Astronauts",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CatalogSyncStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Catalog = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CurrentPage = table.Column<int>(type: "integer", nullable: false),
                    PageSize = table.Column<int>(type: "integer", nullable: false),
                    TotalAvailable = table.Column<int>(type: "integer", nullable: true),
                    RecordsImported = table.Column<int>(type: "integer", nullable: false),
                    LastStartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastCompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastError = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogSyncStates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rockets_Name_Variant",
                table: "Rockets",
                columns: new[] { "Name", "Variant" });

            migrationBuilder.CreateIndex(
                name: "IX_Rockets_SourceId",
                table: "Rockets",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_Name",
                table: "Missions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_SourceId",
                table: "Missions",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Launches_ExternalId",
                table: "Launches",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Launches_Net",
                table: "Launches",
                column: "Net");

            migrationBuilder.CreateIndex(
                name: "IX_Astronauts_Name",
                table: "Astronauts",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Astronauts_SourceId",
                table: "Astronauts",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogSyncStates_Catalog",
                table: "CatalogSyncStates",
                column: "Catalog",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogSyncStates");

            migrationBuilder.DropIndex(
                name: "IX_Rockets_Name_Variant",
                table: "Rockets");

            migrationBuilder.DropIndex(
                name: "IX_Rockets_SourceId",
                table: "Rockets");

            migrationBuilder.DropIndex(
                name: "IX_Missions_Name",
                table: "Missions");

            migrationBuilder.DropIndex(
                name: "IX_Missions_SourceId",
                table: "Missions");

            migrationBuilder.DropIndex(
                name: "IX_Launches_ExternalId",
                table: "Launches");

            migrationBuilder.DropIndex(
                name: "IX_Launches_Net",
                table: "Launches");

            migrationBuilder.DropIndex(
                name: "IX_Astronauts_Name",
                table: "Astronauts");

            migrationBuilder.DropIndex(
                name: "IX_Astronauts_SourceId",
                table: "Astronauts");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Rockets");

            migrationBuilder.DropColumn(
                name: "SourceUrl",
                table: "Rockets");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Missions");

            migrationBuilder.DropColumn(
                name: "SourceUrl",
                table: "Missions");

            migrationBuilder.DropColumn(
                name: "SourceUrl",
                table: "Launches");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Astronauts");

            migrationBuilder.DropColumn(
                name: "SourceUrl",
                table: "Astronauts");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Orbital.Api.data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHeavyFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Keep one canonical row for exact duplicate rocket records created
            // by the earlier name-based importer, then preserve launch links.
            migrationBuilder.Sql("""
                WITH ranked AS (
                    SELECT "Id",
                           first_value("Id") OVER (
                               PARTITION BY "Name", "Variant"
                               ORDER BY ("SourceId" IS NOT NULL) DESC,
                                        "Id"
                           ) AS "CanonicalId"
                    FROM "Rockets"
                )
                UPDATE "Launches" AS launch
                SET "RocketId" = ranked."CanonicalId"
                FROM ranked
                WHERE launch."RocketId" = ranked."Id"
                  AND ranked."Id" <> ranked."CanonicalId";

                WITH ranked AS (
                    SELECT "Id",
                           first_value("Id") OVER (
                               PARTITION BY "Name", "Variant"
                               ORDER BY ("SourceId" IS NOT NULL) DESC,
                                        "Id"
                           ) AS "CanonicalId"
                    FROM "Rockets"
                )
                DELETE FROM "Rockets" AS rocket
                USING ranked
                WHERE rocket."Id" = ranked."Id"
                  AND ranked."Id" <> ranked."CanonicalId";
                """);

            migrationBuilder.DropTable(
                name: "AstronautLaunch");

            migrationBuilder.DropTable(
                name: "Exoplanets");

            migrationBuilder.DropTable(
                name: "Astronauts");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastLaunchDate",
                table: "Rockets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastLaunchDate",
                table: "Missions",
                type: "timestamp with time zone",
                nullable: true);

            // Seed the ordering fields from the launch history so catalogs are
            // correctly sorted immediately after this migration is deployed.
            migrationBuilder.Sql("""
                UPDATE "Rockets" AS rocket
                SET "LastLaunchDate" = history."LastLaunchDate"
                FROM (
                    SELECT "RocketId", MAX("Net") AS "LastLaunchDate"
                    FROM "Launches"
                    WHERE "RocketId" IS NOT NULL
                    GROUP BY "RocketId"
                ) AS history
                WHERE rocket."Id" = history."RocketId";

                UPDATE "Missions" AS mission
                SET "LastLaunchDate" = history."LastLaunchDate"
                FROM (
                    SELECT "MissionId", MAX("Net") AS "LastLaunchDate"
                    FROM "Launches"
                    WHERE "MissionId" IS NOT NULL
                    GROUP BY "MissionId"
                ) AS history
                WHERE mission."Id" = history."MissionId";
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Rockets_LastLaunchDate",
                table: "Rockets",
                column: "LastLaunchDate");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_LastLaunchDate",
                table: "Missions",
                column: "LastLaunchDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rockets_LastLaunchDate",
                table: "Rockets");

            migrationBuilder.DropIndex(
                name: "IX_Missions_LastLaunchDate",
                table: "Missions");

            migrationBuilder.DropColumn(
                name: "LastLaunchDate",
                table: "Rockets");

            migrationBuilder.DropColumn(
                name: "LastLaunchDate",
                table: "Missions");

            migrationBuilder.CreateTable(
                name: "Astronauts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Biography = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    DateOfDeath = table.Column<DateOnly>(type: "date", nullable: true),
                    FlightsCount = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Nationality = table.Column<string>(type: "text", nullable: true),
                    ProfileImageUrl = table.Column<string>(type: "text", nullable: true),
                    SourceId = table.Column<string>(type: "text", nullable: true),
                    SourceUrl = table.Column<string>(type: "text", nullable: true),
                    WikipediaUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Astronauts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Exoplanets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiscoveryFacility = table.Column<string>(type: "text", nullable: false),
                    DiscoveryMethod = table.Column<string>(type: "text", nullable: false),
                    DiscoveryYear = table.Column<int>(type: "integer", nullable: false),
                    HostName = table.Column<string>(type: "text", nullable: false),
                    MassEarthMasses = table.Column<decimal>(type: "numeric", nullable: true),
                    OrbitalPeriodDays = table.Column<decimal>(type: "numeric", nullable: true),
                    PlanetName = table.Column<string>(type: "text", nullable: false),
                    RadiusEarthRadii = table.Column<decimal>(type: "numeric", nullable: true),
                    SemiMajorAxisAu = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exoplanets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AstronautLaunch",
                columns: table => new
                {
                    CrewId = table.Column<int>(type: "integer", nullable: false),
                    LaunchesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AstronautLaunch", x => new { x.CrewId, x.LaunchesId });
                    table.ForeignKey(
                        name: "FK_AstronautLaunch_Astronauts_CrewId",
                        column: x => x.CrewId,
                        principalTable: "Astronauts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AstronautLaunch_Launches_LaunchesId",
                        column: x => x.LaunchesId,
                        principalTable: "Launches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AstronautLaunch_LaunchesId",
                table: "AstronautLaunch",
                column: "LaunchesId");

            migrationBuilder.CreateIndex(
                name: "IX_Astronauts_Name",
                table: "Astronauts",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Astronauts_SourceId",
                table: "Astronauts",
                column: "SourceId");
        }
    }
}

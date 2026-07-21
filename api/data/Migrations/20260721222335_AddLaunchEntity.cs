using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Orbital.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLaunchEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Launches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RocketId = table.Column<int>(type: "integer", nullable: false),
                    MissionId = table.Column<int>(type: "integer", nullable: false),
                    ExternalId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StatusName = table.Column<string>(type: "text", nullable: false),
                    Net = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    WindowStart = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    WindowEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Probability = table.Column<int>(type: "integer", nullable: true),
                    HoldReason = table.Column<string>(type: "text", nullable: true),
                    FailReason = table.Column<string>(type: "text", nullable: true),
                    Hashtag = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Launches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Launches_Missions_MissionId",
                        column: x => x.MissionId,
                        principalTable: "Missions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Launches_Rockets_RocketId",
                        column: x => x.RocketId,
                        principalTable: "Rockets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_Launches_MissionId",
                table: "Launches",
                column: "MissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Launches_RocketId",
                table: "Launches",
                column: "RocketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AstronautLaunch");

            migrationBuilder.DropTable(
                name: "Launches");
        }
    }
}

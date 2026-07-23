using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbital.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCloseApproachFieldsToAsteroid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "CloseApproachDate",
                table: "Asteroids",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<decimal>(
                name: "MissDistanceKm",
                table: "Asteroids",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RelativeVelocityKph",
                table: "Asteroids",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloseApproachDate",
                table: "Asteroids");

            migrationBuilder.DropColumn(
                name: "MissDistanceKm",
                table: "Asteroids");

            migrationBuilder.DropColumn(
                name: "RelativeVelocityKph",
                table: "Asteroids");
        }
    }
}

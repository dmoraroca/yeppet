using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaceProvenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "data_provenance",
                table: "places",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "Internal");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "google_coordinates_cached_until",
                table: "places",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "google_place_id",
                table: "places",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_google_sync_at",
                table: "places",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ux_places_google_place_id",
                table: "places",
                column: "google_place_id",
                unique: true,
                filter: "google_place_id IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_places_google_place_id",
                table: "places");

            migrationBuilder.DropColumn(
                name: "data_provenance",
                table: "places");

            migrationBuilder.DropColumn(
                name: "google_coordinates_cached_until",
                table: "places");

            migrationBuilder.DropColumn(
                name: "google_place_id",
                table: "places");

            migrationBuilder.DropColumn(
                name: "last_google_sync_at",
                table: "places");
        }
    }
}

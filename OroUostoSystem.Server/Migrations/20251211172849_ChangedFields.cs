using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OroUostoSystem.Server.Migrations
{
    /// <inheritdoc />adsf
    public partial class ChangedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BaggageTrackings_BaggageId",
                table: "BaggageTrackings");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "BaggageTrackings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BaggageTrackings");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "BaggageTrackings",
                newName: "UpdatedAt");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "BaggageTrackings",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "BaggageTrackings",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_BaggageTrackings_BaggageId",
                table: "BaggageTrackings",
                column: "BaggageId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BaggageTrackings_BaggageId",
                table: "BaggageTrackings");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "BaggageTrackings");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "BaggageTrackings");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "BaggageTrackings",
                newName: "Time");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "BaggageTrackings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "BaggageTrackings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BaggageTrackings_BaggageId",
                table: "BaggageTrackings",
                column: "BaggageId");
        }
    }
}

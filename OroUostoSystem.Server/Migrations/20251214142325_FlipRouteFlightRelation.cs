using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OroUostoSystem.Server.Migrations
{
    /// <inheritdoc />
    public partial class FlipRouteFlightRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Flights_FlightId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_FlightId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "FlightId",
                table: "Routes");

            migrationBuilder.AlterColumn<string>(
                name: "TakeoffAirport",
                table: "Routes",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AddColumn<int>(
                name: "RouteId",
                table: "Flights",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flights_RouteId",
                table: "Flights",
                column: "RouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Routes_RouteId",
                table: "Flights",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Routes_RouteId",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_RouteId",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "RouteId",
                table: "Flights");

            migrationBuilder.AlterColumn<double>(
                name: "TakeoffAirport",
                table: "Routes",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "FlightId",
                table: "Routes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Routes_FlightId",
                table: "Routes",
                column: "FlightId");

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Flights_FlightId",
                table: "Routes",
                column: "FlightId",
                principalTable: "Flights",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

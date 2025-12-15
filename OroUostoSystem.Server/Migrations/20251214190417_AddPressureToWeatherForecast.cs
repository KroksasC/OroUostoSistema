using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OroUostoSystem.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddPressureToWeatherForecast : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Pressure",
                table: "WeatherForecasts",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pressure",
                table: "WeatherForecasts");
        }
    }
}

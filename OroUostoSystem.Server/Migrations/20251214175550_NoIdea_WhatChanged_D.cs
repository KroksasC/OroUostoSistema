using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OroUostoSystem.Server.Migrations
{
    /// <inheritdoc />
    public partial class NoIdea_WhatChanged_D : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Pilots_PilotId",
                table: "Flights");

            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Routes_RouteId",
                table: "Flights");

            migrationBuilder.AlterColumn<int>(
                name: "RouteId",
                table: "Flights",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AssignedPilot",
                table: "Flights",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "AssignedMainPilot",
                table: "Flights",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "RepeatIntervalHours",
                table: "Flights",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flights_AssignedMainPilot",
                table: "Flights",
                column: "AssignedMainPilot");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_AssignedPilot",
                table: "Flights",
                column: "AssignedPilot");

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Pilots_AssignedMainPilot",
                table: "Flights",
                column: "AssignedMainPilot",
                principalTable: "Pilots",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Pilots_AssignedPilot",
                table: "Flights",
                column: "AssignedPilot",
                principalTable: "Pilots",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Pilots_PilotId",
                table: "Flights",
                column: "PilotId",
                principalTable: "Pilots",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Routes_RouteId",
                table: "Flights",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Pilots_AssignedMainPilot",
                table: "Flights");

            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Pilots_AssignedPilot",
                table: "Flights");

            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Pilots_PilotId",
                table: "Flights");

            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Routes_RouteId",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_AssignedMainPilot",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_AssignedPilot",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "RepeatIntervalHours",
                table: "Flights");

            migrationBuilder.AlterColumn<int>(
                name: "RouteId",
                table: "Flights",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<bool>(
                name: "AssignedPilot",
                table: "Flights",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "AssignedMainPilot",
                table: "Flights",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Pilots_PilotId",
                table: "Flights",
                column: "PilotId",
                principalTable: "Pilots",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Routes_RouteId",
                table: "Flights",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

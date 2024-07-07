using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelemetryStash.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegisterKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegisterId = table.Column<int>(type: "int", nullable: false),
                    Subset = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisterKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Registers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegisterSetId = table.Column<int>(type: "int", nullable: false),
                    RegisterIdentifier = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegisterSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<int>(type: "int", nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisterSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Telemetries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegisterKeyId = table.Column<int>(type: "int", nullable: false),
                    TimestampId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telemetries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TelemetryHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegisterKeyId = table.Column<int>(type: "int", nullable: false),
                    FromTimestampHistoryId = table.Column<int>(type: "int", nullable: false),
                    ToTimestampHistoryId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemetryHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimestampHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<short>(type: "smallint", nullable: false),
                    Ts = table.Column<DateTimeOffset>(type: "datetimeoffset(4)", precision: 4, nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset(4)", precision: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimestampHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Timestamps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<short>(type: "smallint", nullable: false),
                    Ts = table.Column<DateTimeOffset>(type: "datetimeoffset(4)", precision: 4, nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset(4)", precision: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timestamps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceId",
                table: "Devices",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegisterKeys_RegisterId_Subset",
                table: "RegisterKeys",
                columns: new[] { "RegisterId", "Subset" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Registers_RegisterSetId_RegisterIdentifier",
                table: "Registers",
                columns: new[] { "RegisterSetId", "RegisterIdentifier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegisterSets_DeviceId_Identifier",
                table: "RegisterSets",
                columns: new[] { "DeviceId", "Identifier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Telemetries_RegisterKeyId_TimestampId",
                table: "Telemetries",
                columns: new[] { "RegisterKeyId", "TimestampId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimestampHistory_DeviceId_Ts",
                table: "TimestampHistory",
                columns: new[] { "DeviceId", "Ts" },
                unique: true)
                .Annotation("SqlServer:Clustered", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "RegisterKeys");

            migrationBuilder.DropTable(
                name: "Registers");

            migrationBuilder.DropTable(
                name: "RegisterSets");

            migrationBuilder.DropTable(
                name: "Telemetries");

            migrationBuilder.DropTable(
                name: "TelemetryHistory");

            migrationBuilder.DropTable(
                name: "TimestampHistory");

            migrationBuilder.DropTable(
                name: "Timestamps");
        }
    }
}

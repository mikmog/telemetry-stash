using Microsoft.EntityFrameworkCore.Migrations;

namespace TelemetryStash.Database.MigrationsStore;

public partial class UpsertTelemetryUsp : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var telemetryDataTable = $$"""
                                IF TYPE_ID(N'TelemetryDataTableType') IS NULL
                                BEGIN
                                	CREATE TYPE [dbo].[TelemetryDataTableType] AS TABLE(
                                		[RegisterId] [int] NOT NULL,
                                		[Value] [decimal](19, 4) NOT NULL
                                	)
                                END;
                                """;

        //var upsertTelemetry = @"""
        //                        CREATE PROCEDURE [dbo].[uspUpserTelemetry] ...
        //                        GO
        //                        """;

        migrationBuilder.Sql(telemetryDataTable);
        //migrationBuilder.Sql(upsertTelemetry);
    }
}

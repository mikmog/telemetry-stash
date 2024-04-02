//using Microsoft.EntityFrameworkCore.Migrations;

//namespace TelemetryStash.Database.MigrationsStore
//{
//    public partial class UpsertTelemetryUsp : Migration
//    {
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            var telemetryDataTable = $$"""
//                                    IF TYPE_ID(N'TelemetryDataTableType') IS NULL
//                                    BEGIN
//                                    	CREATE TYPE [dbo].[TelemetryDataTableType] AS TABLE(
//                                    		[RegisterId] [int] NOT NULL,
//                                    		[Value] [decimal](19, 4) NOT NULL
//                                    	)
//                                    END;
//                                    """;

//            //var upsertTelemetry = @"""
//            //                        CREATE PROCEDURE [dbo].[uspUpserTelemetry] ...
//            //                        GO
//            //                        """;

//            migrationBuilder.Sql(telemetryDataTable);
//            //migrationBuilder.Sql(upsertTelemetry);
//        }
//    }
//}


/*
 CREATE TYPE TelemetriesType
   AS TABLE
      (
		RegisterKeyId int NOT NULL,
		Value decimal(19,4) NOT NULL
	  );
 */

/*
 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Telemetry Stash
-- Description: Insert timestamp/telemetry.
-- =============================================
ALTER PROCEDURE[dbo].[uspUpsertTelemetry]
(
    @DeviceId SMALLINT,
    @Timestamp DATETIMEOFFSET(4),
	@Telemetry dbo.TelemetriesType READONLY
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TsId INT;

    SELECT @TsId = [Id]
    FROM TimeStamps
    WHERE DeviceId = @DeviceId AND Ts = @Timestamp;

    IF @TsId IS NULL
    BEGIN
		-- Insert timestamp
		DECLARE @Id TABLE(Id INT);
        INSERT INTO Timestamps(DeviceId, Ts, Created)
        OUTPUT inserted.Id INTO @Id
        VALUES(@DeviceId, @Timestamp, GETUTCDATE());
		SELECT @TsId = [Id] from @Id
    END

    -- Insert telemetry
	INSERT INTO dbo.Telemetries(TimestampId, RegisterKeyId, [Value])
	SELECT @TsId, RegisterKeyId, Value 
	FROM @Telemetry AS T
	WHERE NOT EXISTS (
		SELECT 1 
		FROM dbo.Telemetries 
		WHERE TimestampId = @TsId AND RegisterKeyId = T.RegisterKeyId
	);

END

 */
/*
 
USE [sqldb-telemetrystash-dev]
CREATE USER [func-telemetrystash-dev] FROM EXTERNAL PROVIDER;
EXEC sp_addrolemember 'db_datareader', [func-telemetrystash-dev];
EXEC sp_addrolemember 'db_datawriter', [func-telemetrystash-dev];
 
grant execute on dbo.uspUpsertTelemetry to [func-telemetrystash-dev]

 */




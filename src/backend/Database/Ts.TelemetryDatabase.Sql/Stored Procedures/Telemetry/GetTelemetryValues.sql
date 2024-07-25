-- =============================================
-- Description: Get telemetry values
-- =============================================
CREATE PROCEDURE dbo.GetTelemetryValues
(
    @DeviceId SMALLINT,
    @FromClientTimestamp DATETIMEOFFSET(4),
    @ToClientTimestamp DATETIMEOFFSET(4),
    @Skip INT,
    @Take INT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    BEGIN TRY  

    SELECT
            te.RegisterId
            ,te.TimestampId
            ,TS.ClientTimestamp
            ,te.Value
        FROM
            dbo.Timestamps TS
            JOIN dbo.Telemetries te ON TS.Id = te.TimestampId
        WHERE
            TS.DeviceId = @DeviceId
            AND TS.ClientTimestamp >= @FromClientTimestamp
            AND TS.ClientTimestamp <= @ToClientTimestamp
        ORDER BY
            TS.ClientTimestamp
        OFFSET @Skip ROWS
        FETCH NEXT @Take ROWS ONLY;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(MAX);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;
 
        SELECT
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();
 
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;

END
GO

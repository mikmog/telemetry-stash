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

    SELECT
        T.RegisterId
        ,T.TimestampId
        ,TS.ClientTimestamp
        ,T.Value
    FROM
        dbo.Timestamps TS
        JOIN dbo.Telemetries T ON TS.Id = T.TimestampId
    WHERE
        TS.DeviceId = @DeviceId
        AND TS.ClientTimestamp >= @FromClientTimestamp
        AND TS.ClientTimestamp <= @ToClientTimestamp
    ORDER BY
        TS.ClientTimestamp
    OFFSET @Skip ROWS
    FETCH NEXT @Take ROWS ONLY;

END
GO

GRANT EXECUTE
    ON OBJECT::[dbo].[GetTelemetryValues] TO [db_execute_procedure_role]
    AS [dbo];
GO

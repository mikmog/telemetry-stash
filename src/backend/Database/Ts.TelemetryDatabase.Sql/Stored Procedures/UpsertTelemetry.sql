-- =============================================
-- Author:      Telemetry Stash
-- Description: Upsert timestamp/telemetry.
-- =============================================
CREATE PROCEDURE[dbo].[UpsertTelemetry]
(
    @DeviceId SMALLINT,
    @Timestamp DATETIMEOFFSET(4),
	@Telemetry dbo.TelemetriesType READONLY
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TsId INT;

    SELECT @TsId = Id
    FROM Timestamps
    WHERE DeviceId = @DeviceId AND [Timestamp] = @Timestamp;

    IF @TsId IS NULL
    BEGIN
		-- Insert timestamp
		DECLARE @Id TABLE(Id INT);
        INSERT INTO Timestamps(DeviceId, [Timestamp], Created)
        OUTPUT inserted.Id INTO @Id
        VALUES(@DeviceId, @Timestamp, GETUTCDATE());
		SELECT @TsId = Id from @Id
    END

    -- Insert telemetry
	INSERT INTO dbo.Telemetries(TimestampId, RegisterId, [Value])
	SELECT @TsId, RegisterId, Value 
	FROM @Telemetry AS T
	WHERE NOT EXISTS (
		SELECT 1 
		FROM dbo.Telemetries 
		WHERE TimestampId = @TsId AND RegisterId = T.RegisterId
	);

END
GO

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
GO

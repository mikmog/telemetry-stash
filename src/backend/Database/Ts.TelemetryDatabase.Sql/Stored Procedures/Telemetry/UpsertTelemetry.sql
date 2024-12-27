-- =============================================
-- Description: AddOrGet timestamp and upsert telemetry.
-- =============================================
CREATE PROCEDURE dbo.UpsertTelemetry
(
    @DeviceId SMALLINT,
    @ClientTimestamp DATETIMEOFFSET(4),
	@Telemetry [dbo].[TelemetriesType] READONLY
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TimestampId INT;

    -- Insert timestamp.
    -- Happy path, assume not exists
    BEGIN TRY

        INSERT INTO
            dbo.Timestamps(DeviceId, ClientTimestamp, Created)
        VALUES
            (@DeviceId, @ClientTimestamp, GETUTCDATE());

		SET @TimestampId = SCOPE_IDENTITY()

    END TRY
    BEGIN CATCH
        IF (ERROR_NUMBER() = 2601) -- Duplicate key constraint
            SELECT
                @TimestampId = Id
            FROM
                dbo.Timestamps
            WHERE
                DeviceId = @DeviceId AND ClientTimestamp = @ClientTimestamp;
        ELSE
           THROW;

    END CATCH;
        
    -- Upsert telemetry
    MERGE dbo.Telemetries AS Target
    USING @Telemetry AS Source
    ON (Target.TimestampId = @TimestampId AND Target.RegisterId = Source.RegisterId)

    WHEN NOT MATCHED BY Target THEN
    INSERT (TimestampId, RegisterId, Value)
    VALUES
    (
        @TimestampId
        ,Source.RegisterId
        ,Source.Value
    )

    WHEN MATCHED THEN UPDATE SET
        Target.Value = Source.Value;

END
GO

GRANT EXECUTE
    ON OBJECT::[dbo].[UpsertTelemetry] TO [db_execute_procedure_role]
    AS [dbo];
GO

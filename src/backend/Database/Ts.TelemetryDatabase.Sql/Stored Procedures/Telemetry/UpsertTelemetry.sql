-- =============================================
-- Description: Upsert timestamp/telemetry.
-- TODO: Refactor
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
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    BEGIN TRY  
        DECLARE @TimestampId INT;

        SELECT TOP 1
            @TimestampId = Id
        FROM
            dbo.Timestamps
        WHERE
            DeviceId = @DeviceId
            AND ClientTimestamp = @ClientTimestamp;

        IF @TimestampId IS NULL
        BEGIN
		    -- Insert timestamp
            INSERT INTO
                dbo.Timestamps
                (
                    DeviceId
                    ,ClientTimestamp
                    ,Created
                )
            VALUES
            (
                @DeviceId
                ,@ClientTimestamp
                ,GETUTCDATE()
            );

		    SET @TimestampId = SCOPE_IDENTITY() 
        END

        -- Upsert telemetry
        MERGE dbo.Telemetries AS Target
        USING @Telemetry AS Source
        ON (Target.TimestampId = @TimestampId AND Target.RegisterId = Source.RegisterId)

        WHEN NOT MATCHED BY Target THEN
        INSERT (RegisterId, TimestampId ,Value)
        VALUES
        (
            Source.RegisterId
            ,@TimestampId
            ,Source.Value
        )

        WHEN MATCHED THEN UPDATE SET
            Target.Value = Source.Value
            ;

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

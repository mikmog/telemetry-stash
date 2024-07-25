-- =============================================
-- Description: Upsert timestamp/telemetry.
-- TODO: Add error handling.
-- TODO: Refactor
-- =============================================
CREATE PROCEDURE dbo.UpsertTelemetry
(
    @DeviceId SMALLINT,
    @ClientTimestamp DATETIMEOFFSET(4),
	@Telemetry dbo.TelemetriesType READONLY
)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    BEGIN TRY  
        DECLARE @TsId INT;

        SELECT
            @TsId = Id
        FROM
            dbo.Timestamps
        WHERE
            DeviceId = @DeviceId
            AND ClientTimestamp = @ClientTimestamp;

        IF @TsId IS NULL
        BEGIN
		    -- Insert timestamp
		    DECLARE @Id TABLE(Id INT);
            INSERT INTO
                dbo.Timestamps
                (
                    DeviceId
                    ,ClientTimestamp
                    ,Created
                )
            OUTPUT
                inserted.Id INTO @Id
            VALUES
            (
                @DeviceId
                ,@ClientTimestamp
                ,GETUTCDATE()
            );

		    SELECT
                @TsId = Id
            FROM
                @Id
        END

        -- Insert telemetry
	    INSERT INTO dbo.Telemetries
        (
            TimestampId
            ,RegisterId
            ,Value
        )
	    SELECT
            @TsId
            ,RegisterId
            ,Value 
	    FROM
            @Telemetry AS T
	    WHERE NOT EXISTS (
		    SELECT
                1 
		    FROM
                dbo.Telemetries 
		    WHERE
                TimestampId = @TsId
                AND RegisterId = T.RegisterId
	    );

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

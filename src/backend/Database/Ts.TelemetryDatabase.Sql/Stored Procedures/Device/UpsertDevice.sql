-- =============================================
-- Description: Upsert Device
-- =============================================

CREATE PROCEDURE dbo.UpsertDevice
(
    @DeviceId NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    BEGIN TRY  
        IF NOT EXISTS (SELECT 1 FROM dbo.Devices WHERE DeviceId = @DeviceId) 
        BEGIN
            INSERT INTO dbo.Devices (DeviceId, Created)
            VALUES
            (
                @DeviceId
                ,GETUTCDATE()
            )
        END

        SELECT TOP 1
            Id
            ,DeviceId
        FROM
            dbo.Devices
        WHERE
            DeviceId = @DeviceId

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

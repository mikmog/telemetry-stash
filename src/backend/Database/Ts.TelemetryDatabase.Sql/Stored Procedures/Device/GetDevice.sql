-- =============================================
-- Description: Get device by identifier
-- =============================================

CREATE PROCEDURE dbo.GetDevice
(
    @DeviceId NVARCHAR
)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    BEGIN TRY  
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

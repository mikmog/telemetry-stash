-- =============================================
-- Description: Get RegisterSet
-- =============================================

CREATE PROCEDURE dbo.GetRegisterSet
(
    @DeviceId INT,
    @Identifier NVARCHAR
)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    BEGIN TRY  
        SELECT TOP 1
            Id
            ,DeviceId
            ,Identifier
        FROM
            dbo.RegisterSets
        WHERE
            DeviceId = @DeviceId
            AND Identifier = @Identifier
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

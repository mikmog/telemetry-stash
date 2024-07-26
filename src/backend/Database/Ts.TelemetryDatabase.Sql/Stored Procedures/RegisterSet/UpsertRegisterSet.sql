-- =============================================
-- Description: Upsert RegisterSet
-- =============================================

CREATE PROCEDURE dbo.GetOrCreateRegisterSet
(
    @DeviceId INT,
    @Identifier NVARCHAR(450)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    BEGIN TRY  
        IF NOT EXISTS (SELECT 1 FROM dbo.RegisterSets WHERE DeviceId = @DeviceId AND Identifier = @Identifier) 
        BEGIN
            INSERT INTO dbo.RegisterSets (DeviceId, Identifier, Created)
            VALUES
            (
                @DeviceId
                ,@Identifier
                ,GETUTCDATE()
            )
        END

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

-- =============================================
-- Description: Upsert Device
-- =============================================

CREATE PROCEDURE dbo.GetOrCreateDevice
(
    @Identifier NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    BEGIN TRY  
        IF NOT EXISTS (SELECT 1 FROM dbo.Devices WHERE Identifier = @Identifier) 
        BEGIN
            INSERT INTO dbo.Devices (Identifier, Created)
            VALUES
            (
                @Identifier
                ,GETUTCDATE()
            )
        END

        SELECT TOP 1
            Id
            ,Identifier
        FROM
            dbo.Devices
        WHERE
            Identifier = @Identifier

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

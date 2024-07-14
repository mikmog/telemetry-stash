-- =============================================
-- Description: Get device by RegisterTemplateId
-- =============================================

CREATE PROCEDURE dbo.GetRegister
(
    @RegisterTemplateId INT,
    @Subset NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    BEGIN TRY  
        SELECT TOP 1
            Id
            ,RegisterTemplateId
            ,Subset
        FROM
            dbo.Registers
        WHERE
            RegisterTemplateId = @RegisterTemplateId
            AND Subset = @Subset
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

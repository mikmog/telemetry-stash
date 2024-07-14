-- =============================================
-- Description: Get RegisterTemplate
-- =============================================

CREATE PROCEDURE dbo.GetRegisterTemplate
(
    @RegisterSetId INT,
    @RegisterIdentifier NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    BEGIN TRY  
        SELECT TOP 1
            Id
            ,RegisterSetId
            ,RegisterIdentifier
            ,Name
            ,Type
            ,Unit
            ,Description
        FROM
            dbo.RegisterTemplates
        WHERE
            RegisterSetId = @RegisterSetId
            AND RegisterIdentifier = @RegisterIdentifier
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

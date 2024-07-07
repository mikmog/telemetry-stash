-- =============================================
-- Description: Upsert RegisterTemplate all fields
-- =============================================

CREATE PROCEDURE dbo.UpsertRegisterTemplate
(
    @RegisterSetId INT,
    @RegisterIdentifier NVARCHAR(MAX),
    @Name NVARCHAR(MAX),
    @Type NVARCHAR(MAX),
    @Unit NVARCHAR(MAX),
    @Description NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    BEGIN TRY  
        IF NOT EXISTS (SELECT 1 FROM dbo.RegisterTemplates WHERE RegisterSetId = @RegisterSetId AND RegisterIdentifier = @RegisterIdentifier)
        BEGIN
            INSERT INTO dbo.RegisterTemplates
            (
                RegisterSetId
                ,RegisterIdentifier
                ,Name
                ,Type
                ,Unit
                ,Description
                ,Created
            )
            VALUES
            (
                @RegisterSetId
                ,@RegisterIdentifier
                ,@Name
                ,@Type
                ,@Unit
                ,@Description
                ,GETUTCDATE()
            )
        END

        -- TODO Update if not null

        EXEC GetRegisterTemplate @RegisterSetId, @RegisterIdentifier;
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

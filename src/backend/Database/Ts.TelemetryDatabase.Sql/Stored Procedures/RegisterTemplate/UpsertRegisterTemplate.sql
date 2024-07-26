-- =============================================
-- Description: Upsert RegisterTemplate all fields
-- =============================================

CREATE PROCEDURE dbo.GetOrCreateRegisterTemplate
(
    @RegisterSetId INT,
    @Identifier NVARCHAR(MAX),
    @Name NVARCHAR(MAX) = NULL,
    @Type NVARCHAR(MAX) = NULL,
    @Unit NVARCHAR(MAX) = NULL,
    @Description NVARCHAR(MAX) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    BEGIN TRY  
        IF NOT EXISTS (SELECT 1 FROM dbo.RegisterTemplates WHERE RegisterSetId = @RegisterSetId AND Identifier = @Identifier)
        BEGIN
            INSERT INTO dbo.RegisterTemplates
            (
                RegisterSetId
                ,Identifier
                ,Name
                ,Type
                ,Unit
                ,Description
                ,Created
            )
            VALUES
            (
                @RegisterSetId
                ,@Identifier
                ,@Name
                ,@Type
                ,@Unit
                ,@Description
                ,GETUTCDATE()
            )
        END

        SELECT TOP 1
            Id
            ,RegisterSetId
            ,Identifier
            ,Name
            ,Type
            ,Unit
            ,Description
        FROM
            dbo.RegisterTemplates
        WHERE
            RegisterSetId = @RegisterSetId
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

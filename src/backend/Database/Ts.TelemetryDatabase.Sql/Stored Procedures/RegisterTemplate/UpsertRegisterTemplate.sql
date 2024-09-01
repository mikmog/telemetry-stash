-- =============================================
-- Description: Upsert RegisterTemplate all fields
-- =============================================

CREATE PROCEDURE dbo.GetOrCreateRegisterTemplate
(
    @RegisterSetId INT,
    @Identifier NVARCHAR(450),
    @Name NVARCHAR(450) = NULL,
    @Type NVARCHAR(450) = NULL,
    @Unit NVARCHAR(450) = NULL,
    @Description NVARCHAR(450) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

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

END
GO

GRANT EXECUTE
    ON OBJECT::[dbo].[GetOrCreateRegisterTemplate] TO [db_execute_procedure_role]
    AS [dbo];
GO

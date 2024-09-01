-- =============================================
-- Description: Upsert Register
-- =============================================

CREATE PROCEDURE dbo.GetOrCreateRegister
(
    @RegisterTemplateId INT,
    @Subset NVARCHAR(450)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    IF NOT EXISTS (SELECT 1 FROM dbo.Registers WHERE RegisterTemplateId = @RegisterTemplateId AND Subset = @Subset)
    BEGIN
        INSERT INTO dbo.Registers (RegisterTemplateId, Subset, Created)
        VALUES
        (
            @RegisterTemplateId
            ,@Subset
            ,GETUTCDATE()
        )
    END

    SELECT TOP 1
        Id
        ,RegisterTemplateId
        ,Subset
    FROM
        dbo.Registers
    WHERE
        RegisterTemplateId = @RegisterTemplateId
        AND Subset = @Subset

END
GO

GRANT EXECUTE
    ON OBJECT::[dbo].[GetOrCreateRegister] TO [db_execute_procedure_role]
    AS [dbo];
GO

-- =============================================
-- Description: Upsert RegisterSet
-- =============================================

CREATE PROCEDURE dbo.GetOrCreateRegisterSet
(
    @DeviceId SMALLINT,
    @Identifier NVARCHAR(450)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

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

END
GO

GRANT EXECUTE
    ON OBJECT::[dbo].[GetOrCreateRegisterSet] TO [db_execute_procedure_role]
    AS [dbo];
GO

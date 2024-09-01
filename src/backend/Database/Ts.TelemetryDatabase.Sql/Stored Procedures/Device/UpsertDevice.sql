-- =============================================
-- Description: Upsert Device
-- =============================================

CREATE PROCEDURE dbo.GetOrCreateDevice
(
    @Identifier NVARCHAR(450)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

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

END
GO

GRANT EXECUTE
    ON OBJECT::[dbo].[GetOrCreateDevice] TO [db_execute_procedure_role]
    AS [dbo];
GO

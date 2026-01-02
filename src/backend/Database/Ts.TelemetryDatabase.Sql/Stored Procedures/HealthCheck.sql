CREATE PROCEDURE dbo.HealthCheck
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 1
END
GO

GRANT EXECUTE
    ON OBJECT::[dbo].[HealthCheck] TO [db_execute_procedure_role]
    AS [dbo];
GO

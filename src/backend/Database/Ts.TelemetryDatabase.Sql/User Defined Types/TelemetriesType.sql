CREATE TYPE [dbo].[TelemetriesType] AS TABLE(
	[RegisterId]        INT                 NOT NULL,
	[Value]             DECIMAL(19, 4)      NOT NULL
)
GO

GRANT EXECUTE
    ON TYPE::[dbo].[TelemetriesType] TO [db_execute_procedure_role]
    AS [dbo];
GO

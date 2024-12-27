CREATE TYPE [dbo].[TelemetriesType] AS TABLE(
	[RegisterId]        INT                 NOT NULL,
	[Value]             NVARCHAR (MAX)      NOT NULL
)
GO

GRANT EXECUTE
    ON TYPE::[dbo].[TelemetriesType] TO [db_execute_procedure_role]
    AS [dbo];
GO

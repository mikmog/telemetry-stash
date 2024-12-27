CREATE TYPE [dbo].[RegistersType] AS TABLE(
	[RegisterSet]        NVARCHAR (400)      NOT NULL,
	[Register]           NVARCHAR (400)      NOT NULL
)
GO

GRANT EXECUTE
    ON TYPE::[dbo].[RegistersType] TO [db_execute_procedure_role]
    AS [dbo];
GO

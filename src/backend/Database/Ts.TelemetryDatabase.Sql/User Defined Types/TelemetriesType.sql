CREATE TYPE [dbo].[TelemetriesType] AS TABLE(
	[RegisterId]        INT                 NOT NULL,
	[Value]             DECIMAL(19, 4)      NOT NULL
)

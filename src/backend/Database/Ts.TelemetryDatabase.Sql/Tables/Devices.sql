CREATE TABLE [dbo].[Devices] (
    [Id]                    SMALLINT IDENTITY (1, 1)    NOT NULL,
    [Identifier]            NVARCHAR (450)              NOT NULL,
    [Created]               DATETIME2                   NOT NULL,

    CONSTRAINT [PK_Devices] PRIMARY KEY NONCLUSTERED ( [Id] ASC )
);

GO
CREATE UNIQUE CLUSTERED INDEX [IX_Devices_Identifier] ON [dbo].[Devices]
(
	[Identifier] ASC
)

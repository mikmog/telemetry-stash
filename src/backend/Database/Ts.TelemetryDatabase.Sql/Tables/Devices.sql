CREATE TABLE [dbo].[Devices] (
    [Id]                    SMALLINT IDENTITY (1, 1)    NOT NULL,
    [Identifier]            NVARCHAR (450)              NOT NULL,
    [Created]               DATETIMEOFFSET (4)          NOT NULL,

    CONSTRAINT [PK_Devices] PRIMARY KEY CLUSTERED ( [Id] ASC )
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Devices_Identifier] ON [dbo].[Devices]
(
	[Identifier] ASC
)

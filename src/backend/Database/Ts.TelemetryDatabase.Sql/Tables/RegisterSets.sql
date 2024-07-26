CREATE TABLE [dbo].[RegisterSets] (
    [Id]                    INT IDENTITY (1, 1)         NOT NULL,
    [DeviceId]              SMALLINT                    NOT NULL,
    [Identifier]            NVARCHAR (450)              NOT NULL,
    [Created]               DATETIMEOFFSET (4)          NOT NULL,

    CONSTRAINT [PK_RegisterSets] PRIMARY KEY CLUSTERED ( [Id] ASC ),
    CONSTRAINT [FK_RegisterSets_Devices] FOREIGN KEY ( [DeviceId] ) REFERENCES [dbo].[Devices] ( [Id] )
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_RegisterSets_DeviceId_Identifier] ON [dbo].[RegisterSets]
(
	[DeviceId] ASC,
	[Identifier] ASC
)

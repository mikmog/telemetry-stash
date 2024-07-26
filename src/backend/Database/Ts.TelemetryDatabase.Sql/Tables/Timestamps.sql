CREATE TABLE [dbo].[Timestamps] (
    [Id]                    INT IDENTITY (1, 1)         NOT NULL,
    [DeviceId]              SMALLINT                    NOT NULL,
    [ClientTimestamp]       DATETIMEOFFSET (4)          NOT NULL,
    [Created]               DATETIME2                   NOT NULL,

    CONSTRAINT [PK_Timestamps] PRIMARY KEY CLUSTERED ( [Id] ASC ),
    CONSTRAINT [FK_Timestamps_Devices] FOREIGN KEY ( [DeviceId] ) REFERENCES [dbo].[Devices] ( [Id] )
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_DeviceId_ClientTimestamp] ON [dbo].[Timestamps]
(
	[DeviceId] ASC,
	[ClientTimestamp] ASC
)

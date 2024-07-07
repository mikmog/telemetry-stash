CREATE TABLE [dbo].[Devices] (
    [Id]                    SMALLINT IDENTITY (1, 1)    NOT NULL,
    [DeviceId]              NVARCHAR (450)              NOT NULL,

    CONSTRAINT [PK_Devices] PRIMARY KEY CLUSTERED ( [Id] ASC )
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Devices_DeviceId] ON [dbo].[Devices]
(
	[DeviceId] ASC
)

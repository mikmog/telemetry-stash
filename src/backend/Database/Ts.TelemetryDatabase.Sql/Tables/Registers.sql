CREATE TABLE [dbo].[Registers] (
    [Id]                    INT IDENTITY (1, 1)         NOT NULL,
    [DeviceId]              SMALLINT                    NOT NULL,
    [RegisterSet]           NVARCHAR (400)              NOT NULL,
    [Register]              NVARCHAR (400)              NOT NULL,
    [Created]               DATETIME2                   NOT NULL,

    CONSTRAINT [PK_Registers] PRIMARY KEY CLUSTERED ( [Id] ASC ),
    CONSTRAINT [FK_Registers_Devices] FOREIGN KEY ( [DeviceId] ) REFERENCES [dbo].[Devices] ( [Id] )
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Registers_RegisterSet_Register] ON [dbo].[Registers]
(
    [DeviceId] ASC,
	[RegisterSet] ASC,
	[Register] ASC
)

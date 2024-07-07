CREATE TABLE [dbo].[Telemetries] (
    [Id]                    INT IDENTITY (1, 1)         NOT NULL,
    [RegisterId]            INT                         NOT NULL,
    [TimestampId]           INT                         NOT NULL,
    [Value]                 DECIMAL (19, 4)             NOT NULL,

    CONSTRAINT [PK_Telemetries] PRIMARY KEY CLUSTERED ( [Id] ASC ),
    CONSTRAINT [FK_Telemetries_Registers] FOREIGN KEY ( [RegisterId] ) REFERENCES [dbo].[Registers] ( [Id] ),
    CONSTRAINT [FK_Telemetries_Timestamps] FOREIGN KEY ( [TimestampId] ) REFERENCES [dbo].[Timestamps] ( [Id] )
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Telemetries_RegisterId_TimestampId] ON [dbo].[Telemetries]
(
	[RegisterId] ASC,
	[TimestampId] ASC
)

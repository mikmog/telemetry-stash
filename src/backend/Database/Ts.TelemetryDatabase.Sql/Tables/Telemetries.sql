CREATE TABLE [dbo].[Telemetries] (
    [TimestampId]           INT             NOT NULL,
    [RegisterId]            INT             NOT NULL,
    [Value]                 NVARCHAR (MAX)  NOT NULL,

    CONSTRAINT [PK_Telemetries] PRIMARY KEY CLUSTERED ( [TimestampId], [RegisterId] ),
    CONSTRAINT [FK_Telemetries_Registers] FOREIGN KEY ( [RegisterId] ) REFERENCES [dbo].[Registers] ( [Id] ),
    CONSTRAINT [FK_Telemetries_Timestamps] FOREIGN KEY ( [TimestampId] ) REFERENCES [dbo].[Timestamps] ( [Id] )
);

GO

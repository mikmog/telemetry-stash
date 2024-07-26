-- Previous name: Registers.sql

CREATE TABLE [dbo].[RegisterTemplates] (
    [Id]                    INT IDENTITY (1, 1)         NOT NULL,
    [RegisterSetId]         INT                         NOT NULL,
    [Identifier]            NVARCHAR (450)              NOT NULL,
    [Name]                  NVARCHAR (MAX)              NULL,
    [Type]                  NVARCHAR (MAX)              NULL,
    [Unit]                  NVARCHAR (MAX)              NULL,
    [Description]           NVARCHAR (MAX)              NULL,
    [Created]               DATETIMEOFFSET (4)          NOT NULL,

    CONSTRAINT [PK_RegisterTemplates] PRIMARY KEY CLUSTERED ( [Id] ASC ),
    CONSTRAINT [FK_RegisterTemplates_RegisterSets] FOREIGN KEY ( [RegisterSetId] ) REFERENCES [dbo].[RegisterSets] ( [Id] )
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Registers_RegisterSetId_Identifier] ON [dbo].[RegisterTemplates]
(
	[RegisterSetId] ASC,
	[Identifier] ASC
)

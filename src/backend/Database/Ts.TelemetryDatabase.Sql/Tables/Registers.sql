-- Previous name: RegisterKeys.sql

CREATE TABLE [dbo].[Registers] (
    [Id]                    INT IDENTITY (1, 1)         NOT NULL,
    [RegisterTemplateId]    INT                         NOT NULL,
    [Subset]                NVARCHAR (450)              NOT NULL,
    [Created]               DATETIME2                   NOT NULL,

    CONSTRAINT [PK_Registers] PRIMARY KEY CLUSTERED ( [Id] ASC ),
    CONSTRAINT [FK_Registers_RegisterTemplates] FOREIGN KEY ( [RegisterTemplateId] ) REFERENCES [dbo].[RegisterTemplates] ( [Id] )
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Registers_RegisterTemplateId_Subset] ON [dbo].[Registers]
(
	[RegisterTemplateId] ASC,
	[Subset] ASC
)

CREATE TABLE [dbo].[WeatherPrognosis] (
    [Id]                    INT IDENTITY (1, 1)         NOT NULL,
    [Name]                  NVARCHAR (450)              NOT NULL,
    [Timestamp]             DATETIMEOFFSET(7)           NOT NULL,
    [Data]                  NVARCHAR(MAX)               NOT NULL

    CONSTRAINT [PK_WeatherPrognosis] PRIMARY KEY CLUSTERED ([Id])
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_WeatherPrognosis_Name_Timestamp] ON [dbo].[WeatherPrognosis]
(
	[Name] ASC,
	[Timestamp] ASC
)

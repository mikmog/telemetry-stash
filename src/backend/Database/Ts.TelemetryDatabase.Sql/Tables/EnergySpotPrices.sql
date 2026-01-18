CREATE TABLE [dbo].[EnergySpotPrices] (
    [DeliveryArea]          NVARCHAR (450)              NOT NULL,
    [From]                  DATETIMEOFFSET(0)           NOT NULL,
    [Price]                 Decimal(18, 3)              NOT NULL,
    [Unit]                  NVARCHAR (450)              NOT NULL

    CONSTRAINT [PK_EnergySpotPrices] PRIMARY KEY CLUSTERED ( [DeliveryArea], [From] )
);

GO

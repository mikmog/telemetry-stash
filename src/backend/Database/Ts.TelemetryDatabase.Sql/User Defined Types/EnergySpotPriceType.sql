CREATE TYPE [dbo].[EnergySpotPriceType] AS TABLE(
	[DeliveryArea]          NVARCHAR (450)              NOT NULL,
    [From]                  DATETIMEOFFSET(0)           NOT NULL,
    [Price]                 Decimal(18, 3)              NOT NULL,
    [Unit]                  NVARCHAR (450)              NOT NULL
)
GO

GRANT EXECUTE
    ON TYPE::[dbo].[EnergySpotPriceType] TO [db_execute_procedure_role]
    AS [dbo];
GO

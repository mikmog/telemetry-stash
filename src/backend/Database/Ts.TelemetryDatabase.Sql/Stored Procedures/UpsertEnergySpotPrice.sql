CREATE PROCEDURE UpsertEnergySpotPrice
(
    @EnergySpotPrice EnergySpotPriceType READONLY
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO EnergySpotPrices (DeliveryArea, [From], Price, Unit)
    SELECT
        DeliveryArea,
        [From],
        Price,
        Unit
    FROM @EnergySpotPrice ESP
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM EnergySpotPrices E
        WHERE
            E.DeliveryArea = ESP.DeliveryArea
            AND E.[From] = ESP.[From]
    );

    UPDATE ESP
    SET
        Price = E.Price,
        Unit = E.Unit
    FROM EnergySpotPrices ESP
    INNER JOIN @EnergySpotPrice E
        ON E.DeliveryArea = ESP.DeliveryArea
        AND E.[From] = ESP.[From];

END
GO

GRANT EXECUTE
    ON OBJECT::[dbo].[UpsertEnergySpotPrice] TO [db_execute_procedure_role]
    AS [dbo];
GO

-- =============================================
-- Description: Get device by identifier
-- =============================================

CREATE PROCEDURE [dbo].[GetDevice]
(
    @DeviceId NVARCHAR
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        [Id]
        ,[DeviceId]
    FROM
        [dbo].[Devices]
    WHERE
        [DeviceId] = @DeviceId

END
GO

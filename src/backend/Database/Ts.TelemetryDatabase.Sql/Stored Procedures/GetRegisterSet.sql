-- =============================================
-- Description: Get device by Identifier
-- =============================================

CREATE PROCEDURE [dbo].[GetRegisterSet]
(
    @DeviceId INT,
    @Identifier NVARCHAR
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        [Id]
        ,[DeviceId]
        ,[Identifier]
    FROM
        [dbo].[RegisterSets]
    WHERE
        [DeviceId] = @DeviceId
        AND [Identifier] = @Identifier

END
GO

-- =============================================
-- Description: Get device by RegisterTemplateId
-- =============================================

CREATE PROCEDURE [dbo].[GetRegister]
(
    @RegisterTemplateId INT,
    @Subset NVARCHAR
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        [Id]
        ,[RegisterTemplateId]
        ,[Subset]
    FROM
        [dbo].[Registers]
    WHERE
        [RegisterTemplateId] = @RegisterTemplateId
        AND [Subset] = @Subset

END
GO

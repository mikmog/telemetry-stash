-- =============================================
-- Description: Get device by RegisterIdentifier
-- =============================================

CREATE PROCEDURE [dbo].[GetRegisterTemplate]
(
    @RegisterSetId INT,
    @RegisterIdentifier NVARCHAR
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        [Id]
        ,[RegisterSetId]
        ,[RegisterIdentifier]
        ,[Name]
        ,[Type]
        ,[Unit]
        ,[Description]
    FROM
        [dbo].[RegisterTemplates]
    WHERE
        [RegisterSetId] = @RegisterSetId
        AND [RegisterIdentifier] = @RegisterIdentifier

END
GO

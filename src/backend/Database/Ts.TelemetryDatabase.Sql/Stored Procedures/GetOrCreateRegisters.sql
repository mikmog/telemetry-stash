-- =============================================
-- Description: Get or create registers
-- =============================================

CREATE PROCEDURE dbo.GetOrCreateRegisters
(
    @DeviceId       SMALLINT,
    @Registers [dbo].[RegistersType] READONLY
)
AS
BEGIN
    SET NOCOUNT ON;

    MERGE dbo.Registers AS Target
    USING @Registers AS Source
    ON (Target.DeviceId = @DeviceId AND Target.RegisterSet = Source.RegisterSet AND Target.Register = Source.Register)

    WHEN NOT MATCHED BY Target THEN
    INSERT (DeviceId, RegisterSet, Register, Created)
    VALUES
    (
        @DeviceId
        ,Source.RegisterSet
        ,Source.Register
        ,GETUTCDATE()
    )

    WHEN MATCHED THEN UPDATE SET
        Target.Register = Source.Register
    OUTPUT
        inserted.Id, inserted.DeviceId, inserted.RegisterSet, inserted.Register;

END
GO

GRANT EXECUTE
    ON OBJECT::[dbo].GetOrCreateRegisters TO [db_execute_procedure_role]
    AS [dbo];
GO

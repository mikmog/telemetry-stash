CREATE PROCEDURE dbo.UpsertWeatherPrognosis
(
    @Name NVARCHAR(450),
    @Timestamp DATETIMEOFFSET(7),
	@Data NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.WeatherPrognosis WHERE Name = @Name AND Timestamp = @Timestamp)
    BEGIN
        UPDATE dbo.WeatherPrognosis
        SET Data = @Data
        WHERE Name = @Name AND Timestamp = @Timestamp;
    END
    ELSE
    BEGIN
        INSERT INTO dbo.WeatherPrognosis (Name, Timestamp, Data)
        VALUES (@Name, @Timestamp, @Data);
    END
END
GO

GRANT EXECUTE
    ON OBJECT::[dbo].[UpsertWeatherPrognosis] TO [db_execute_procedure_role]
    AS [dbo];
GO

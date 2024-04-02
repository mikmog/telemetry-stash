# Telemetry Stash Database

## Development

A SQL database with Entity framework is used. The database is created and updated using Entity Framework Core tools.
See [Entity Framework Core tools reference](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) for a complete reference

### Open terminal
In Visual Studio. Right click ```Ts.Functions``` Select _Open in Terminal_

#### Install dotnet tool
```
dotnet tool install --global dotnet-ef
```

#### Update dotnet tool
```
dotnet tool update --global dotnet-ef
```

#### Adding migrations
```
dotnet ef migrations add Initial --project ../Ts.Database
```

#### Create/update database 
```
dotnet ef database update --project ../Ts.Database -- --environment "Development"
```

#### Generate SQL script from migrations
Generate SQL script for all migrations after a migration
```
dotnet ef migrations script 20240101120000_MyMigration --idempotent 
```

#### Generate SQL script for Initial migration
```
dotnet ef migrations script 0 --idempotent 
```

## Infrastructure

## Add user to database
```
USE [sqldb-telemetrystash-dev]
CREATE USER [func-telemetrystash-dev] FROM EXTERNAL PROVIDER;
EXEC sp_addrolemember 'db_datareader', [func-telemetrystash-dev];
EXEC sp_addrolemember 'db_datawriter', [func-telemetrystash-dev];
 
GRANT EXECUTE ON dbo.uspUpsertTelemetry to [func-telemetrystash-dev]
```

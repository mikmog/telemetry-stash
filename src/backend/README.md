# TelemetryStash, Backend

## Infrastructure as code

The setup of azure resources is done with bicep and az cli.

### Deploy azure resources to a new resource group

1. Create the resource group

```shell
az group create --name {RESOURCE_GROUP_NAME} --location swedencentral
```

2. Run the az cli command to deploy resources

```shell
az deployment group create --resource-group {RESOURCE_GROUP_NAME} --parameters infrastructure/ts.dev.bicepparam
```

## Database

A SQL database with Entity Framework is used. The database is created and updated using Entity Framework Core tools.
See [Entity Framework Core tools reference](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) for a complete reference.

In Visual Studio. Right click ```Ts.Functions``` Select _Open in Terminal_

### Dotnet tool install

```shell
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```

### Dotnet tool usage

```shell
dotnet ef migrations add Initial --project ../Ts.Database
dotnet ef database update --project ../Ts.Database -- --environment "Development"
```

### Allow functions to access database

```
USE {DATABASE_NAME}
CREATE USER [func-telemetrystash-dev] FROM EXTERNAL PROVIDER;
EXEC sp_addrolemember 'db_datareader', [func-telemetrystash-dev];
EXEC sp_addrolemember 'db_datawriter', [func-telemetrystash-dev];
 
GRANT EXECUTE ON dbo.uspUpsertTelemetry to [func-telemetrystash-dev]
```

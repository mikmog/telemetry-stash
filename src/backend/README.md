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

dotnet tool install --global dotnet-ef --version 9.0.0-preview.5.24306.3
```

### Dotnet tool usage

```shell
dotnet ef migrations add Initial --project ../../Database/Ts.TelemetryDatabase
dotnet ef database update --project ../../Database/Ts.TelemetryDatabase -- --environment "Production"
```

### Allow functions to access database

```
USE [{DATABASE_NAME}]
CREATE USER [{IDENTITY}] FROM EXTERNAL PROVIDER;
EXEC sp_addrolemember 'db_datareader', [{IDENTITY}];
EXEC sp_addrolemember 'db_datawriter', [{IDENTITY}];
 
GRANT EXECUTE ON dbo.uspUpsertTelemetry to [{IDENTITY}]
```

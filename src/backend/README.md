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
az deployment group create --resource-group {RESOURCE_GROUP_NAME} --parameters ts.{ENV}.bicepparam
```
> :bulb: Note. Publish __Ts.Functions__ and run again if deployment fails

## Testing

Integration tests are done on SQL server with TestContainers and Docker on WSL2.

### Configure Docker on WSL2
- TBD


## Database

A SQL database with RepoDb ORM is used. The database is created and updated using Ts.TelemetryDatabase.Sql project.

### Allow functions to access database

```sql
USE [{DATABASE_NAME}]
CREATE USER [{IDENTITY}] FROM EXTERNAL PROVIDER;
EXEC sp_addrolemember 'db_datareader', [{IDENTITY}];
EXEC sp_addrolemember 'db_datawriter', [{IDENTITY}];
 
GRANT EXECUTE ON dbo.uspUpsertTelemetry to [{IDENTITY}]
```

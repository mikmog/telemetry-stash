# TelemetryStash, Backend

## Testing

Integration tests are done on SQL server with TestContainers and Docker on WSL2.

### Configure Docker on WSL2
- TBD

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

## Deploy SQL database

A SQL database with [RepoDb](https://github.com/mikependon/RepoDB) ORM is used. The database is created and updated via Ts.TelemetryDatabase.Sql project.

### Publish database project

Publish the database project via Visual Studio Schema Compare... or CLI.

```bash
dotnet tool install -g microsoft.sqlpackage
```

```bash
./SqlPackage /Action:Publish /SourceFile:bin/{DEBUG_OR_RELEASE}/Ts.TelemetryDatabase.Sql.dacpac /TargetServerName:{SQL_SERVER} /TargetDatabaseName:Ts.TelemetryDatabase.Sql
```

### Allow functions to access database

```sql
USE [{DATABASE_NAME}]
CREATE USER [{IDENTITY}] FROM EXTERNAL PROVIDER;
EXEC sp_addrolemember 'db_datareader', [{IDENTITY}];
EXEC sp_addrolemember 'db_datawriter', [{IDENTITY}];

GRANT EXECUTE ON SCHEMA ::dbo TO [{IDENTITY}]
```

### Create certificates

- Add to Key Vault
    - Secret: __Client--CertificatePassword__
    - Secret: __Certificate--RootCaPassword__
- Generate a root certificate
    - /src/misc/generate-root-cert.ps1    
- Optional: Generate a client certificates to Key Vault
    - /src/misc/generate-client-cert.ps1

### Configure IoT Hub

- Add root __Telemetry Stash Root CA__ PEM certificate to IoT Hub  
    - Enable: _Set certificate status to verified on upload_
- Create device  
    - Device ID: Must match the client certificate CN, eg: Client-Dev1
    - Authentication type: _X.509 CA signed_
    
```bash

## Ways of working

### Pull requests
```bash
 @coderabbitai full review: Conducts a full review from scratch, covering all files again.
 @coderabbitai summary: Regenerates the summary of the PR.
 @coderabbitai configuration: Displays the current CodeRabbit configuration for the repository.
```

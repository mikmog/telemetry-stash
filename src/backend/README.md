# TelemetryStash, Backend

## Testing

See [TelemetryStash, Database](Database/README.md) about running integration tests.

## Infrastructure as code (IaC)

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

## Github Actions Workflows
The IaC creates user assigned identity **id-deploy-ts-_{{environment}}_** intended to be used by GitHub Actions workflows.

To authorize the workflows to access Azure resources
1. Manually add a Federated credential to the user assigned identity **id-deploy-ts-_{{environment}}_** Azure.
following [Step 4 Create a federated credential](https://github.com/Azure/functions-action?tab=readme-ov-file#use-oidc-recommended)
> :bulb: Note. If the action fails. The generated 'Subject identifier' might need to be edited to match the Github expected subject.

2. In Github repository settings. Add environment variables following [Step 1 Create these variables in your repository](https://github.com/Azure/functions-action?tab=readme-ov-file#use-oidc-recommended)

## Deploy SQL database

A SQL database with [RepoDb](https://github.com/mikependon/RepoDB) ORM is used. The database is created and updated via Ts.TelemetryDatabase.Sql project.

### Publish database project

Publish the database project via Visual Studio Schema Compare or CLI.

```bash
dotnet tool install -g microsoft.sqlpackage
```

```bash
./SqlPackage /Action:Publish /SourceFile:bin/Ts.TelemetryDatabase.Sql.dacpac /TargetServerName:{SQL_SERVER} /TargetDatabaseName:Ts.TelemetryDatabase.Sql
```

### Allow functions to access database

```sql
USE [{DATABASE_NAME}]
CREATE USER [{IDENTITY}] FROM EXTERNAL PROVIDER;
EXEC sp_addrolemember 'db_datareader', [{IDENTITY}];
ALTER ROLE [db_execute_procedure_role] ADD MEMBER [{IDENTITY}]

```

### Create certificates

- Add to Key Vault
    - Secret: __Client--CertificatePassword__
    - Secret: __Certificate--RootCaPassword__
- Generate a root certificate
    - /src/misc/generate-root-cert.ps1    
- Optional: Generate a client certificates to Key Vault
+ Optional: Generate client certificates to Key Vault
    - /src/misc/generate-client-cert.ps1

### Configure IoT Hub

- Add root __Telemetry Stash Root CA__ PEM certificate to IoT Hub  
    - Enable: _Set certificate status to verified on upload_
- Create device  
    - Device ID: Must match the client certificate CN, eg: Client-Dev1
    - Authentication type: _X.509 CA signed_
    
## Ways of working

### Pull requests
```bash
 @coderabbitai full review: Conducts a full review from scratch, covering all files again.
 @coderabbitai summary: Regenerates the summary of the PR.
 @coderabbitai configuration: Displays the current CodeRabbit configuration for the repository.
```

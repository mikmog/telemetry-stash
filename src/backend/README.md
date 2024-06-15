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

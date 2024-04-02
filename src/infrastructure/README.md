# TelemetryStash

## Best Practices Abbreviation for Azure resources
https://learn.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/resource-abbreviations

## To deploy to a new resource group

1. Sign in to Azure with the az cli and set your subscription
```
shell
az login
```

```shell
az account set --subscription xxx
```

2. Create the resource group in the correct location
```
shell
az group create --name rg-telemetrystash-dev --location swedencentral
```

3. Then run the az cli command to deploy to that resourcegroup, make sure to update name of resource group, environment and the sql admin password accordingly:
```
shell
az deployment group create --name deploy_test --resource-group telemetry-stash-dev --template-file main.bicep --parameters ts.dev.parameters --what-if
az deployment group create --resource-group rg-telemetrystash-dev --parameters dev.bicepparam
```

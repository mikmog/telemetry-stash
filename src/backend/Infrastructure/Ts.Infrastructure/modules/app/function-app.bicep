// ********************************************************************
// LINUX FLEX FUNCTION APP SERVICE
// https://learn.microsoft.com/en-us/azure/templates/microsoft.web/sites?pivots=deployment-language-bicep
// https://learn.microsoft.com/en-us/azure/templates/microsoft.storage/storageaccounts/blobservices?pivots=deployment-language-bicep
// ********************************************************************

import { applicationParams, functionParams, getResourceName }  from '../../parameter-types.bicep'

param applicationParameters applicationParams
param functionParameters functionParams
param appServicePlanId string
param appStorageName string
param appInsightsConnectionString string

var functionAppName = getResourceName({ resourceAbbr: 'func' }, applicationParameters)

resource appStorage 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: appStorageName
}

resource appStorageBlob 'Microsoft.Storage/storageAccounts/blobServices@2025-06-01' = {
  parent: appStorage
  name: 'default'
  resource deploymentContainer 'containers' = {
      name: functionAppName
      properties: {
        publicAccess: 'None'
      }
    }
}

resource functionApp 'Microsoft.Web/sites@2024-04-01' = {
  name: functionAppName
  location: applicationParameters.location
  tags: applicationParameters.tags
  kind: 'functionapp,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlanId
    httpsOnly: true
    siteConfig: {
      minTlsVersion: '1.2'
    }
    functionAppConfig: {   
      deployment: {
        storage: {
          type: 'blobcontainer'
          value: '${appStorage.properties.primaryEndpoints.blob}${functionAppName}'
          authentication: {
            type: 'SystemAssignedIdentity'
          }
        }
      }
      scaleAndConcurrency: {
        maximumInstanceCount: 100
        instanceMemoryMB: 512
      }
      runtime: {
        name: 'dotnet-isolated'
        version: functionParameters.runtimeVersion
      }
    }
  }
  resource configAppSettings 'config' = {
    name: 'appsettings'
    properties: {
      AZURE_FUNCTIONS_ENVIRONMENT: applicationParameters.environment
      APPLICATIONINSIGHTS_CONNECTION_STRING: appInsightsConnectionString
      AzureWebJobsStorage: 'DefaultEndpointsProtocol=https;AccountName=${appStorage.name};AccountKey=${appStorage.listKeys().keys[0].value}'
    }
  }
}

output identityPrincipalId string = functionApp.identity.principalId
output functionsAppId string = functionApp.id
output functionsAppName string = functionApp.name

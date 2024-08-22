// ********************************************************************
// LINUX FUNCTION APP SERVICE
// https://learn.microsoft.com/en-us/azure/templates/microsoft.web/sites?pivots=deployment-language-bicep
// ********************************************************************

import { applicationParams, functionParams, getResourceName }  from '../../parameter-types.bicep'

param applicationParameters applicationParams
param functionParameters functionParams
param appServicePlanId string
param appStorageName string
param appInsightsConnectionString string
param appInsightsInstrumentationKey string

resource appStorage 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: appStorageName
}

resource functionApp 'Microsoft.Web/sites@2022-09-01' = {
  name: getResourceName({ resourceAbbr: 'func' }, applicationParameters)
  location: applicationParameters.location
  tags: applicationParameters.tags
  kind: 'functionapp,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlanId
    siteConfig: {
      linuxFxVersion: functionParameters.linuxFxVersion
      use32BitWorkerProcess: false
      publicNetworkAccess: 'Enabled'
      alwaysOn: false
      ipSecurityRestrictions: [
        {
          ipAddress: 'Any'
          action: 'Allow'
        }
      ]
      scmIpSecurityRestrictions: [
        {
          ipAddress: 'Any'
          action: 'Allow'
        }
      ]
      appSettings: [
        {
          name: 'AZURE_FUNCTIONS_ENVIRONMENT'
          value: applicationParameters.environment
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsightsConnectionString
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsightsInstrumentationKey
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${appStorage.name};AccountKey=${appStorage.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${appStorage.name};AccountKey=${appStorage.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: 'func-${applicationParameters.appName}-${applicationParameters.envAbbr}'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED'
          value: '1'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
      ]
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
    }
    httpsOnly: true
  }
}

output identityPrincipalId string = functionApp.identity.principalId
output functionsAppId string = functionApp.id
output functionsAppName string = functionApp.name

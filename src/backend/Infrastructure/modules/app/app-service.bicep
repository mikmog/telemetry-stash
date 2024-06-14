// ********************************************************************
// LINUX APP SERVICE
// https://learn.microsoft.com/en-us/azure/templates/microsoft.web/sites?pivots=deployment-language-bicep
// ********************************************************************

import { applicationParams, appServiceParams, getResourceName }  from '../../parameters.bicep'

param applicationParameters applicationParams
param appServiceParameters appServiceParams
param appServicePlanId string
param appInsightsName string

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

resource webApp 'Microsoft.Web/sites@2022-09-01' = {
  name: getResourceName({ resourceAbbr: 'app', resourceName: appServiceParameters.resourceName }, applicationParameters)
  location: applicationParameters.location
  tags: applicationParameters.tags
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlanId
    siteConfig: {
      linuxFxVersion: appServiceParameters.linuxFxVersion
      publicNetworkAccess: 'Enabled'
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
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
      ]
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
    }
    httpsOnly: true
  }
}

output identityPrincipalId string = webApp.identity.principalId

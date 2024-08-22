// ********************************************************************
// LINUX APP SERVICE
// https://learn.microsoft.com/en-us/azure/templates/microsoft.web/sites?pivots=deployment-language-bicep
// ********************************************************************

import { applicationParams, appServiceParams, getResourceName }  from '../../parameter-types.bicep'

param applicationParameters applicationParams
param appServiceParameters appServiceParams
param appServicePlanId string
param appInsightsConnectionString string
param appInsightsInstrumentationKey string

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

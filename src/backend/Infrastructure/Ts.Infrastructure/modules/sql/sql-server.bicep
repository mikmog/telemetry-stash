// ********************************************************************
// SQL SERVER 
// https://learn.microsoft.com/en-us/azure/templates/microsoft.sql/2021-11-01/servers?pivots=deployment-language-bicep
// https://learn.microsoft.com/en-us/azure/templates/microsoft.sql/servers/firewallrules?pivots=deployment-language-bicep
// ********************************************************************

import { applicationParams, sqlParams, getResourceName }  from '../../parameter-types.bicep'
param applicationParameters applicationParams
param sqlParameters sqlParams

resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
  name: getResourceName({ resourceAbbr: 'sql' }, applicationParameters)
  location: applicationParameters.location
  tags: applicationParameters.tags
  properties: {
    minimalTlsVersion: '1.2'
    administrators: {
      administratorType: 'ActiveDirectory'
      login: sqlParameters.server.adminLoginName
      azureADOnlyAuthentication: true
      principalType: 'Group'
      sid: sqlParameters.server.adminGroupSid
      tenantId: subscription().tenantId 
    }
    publicNetworkAccess: 'Enabled'
    restrictOutboundNetworkAccess: 'Disabled'
  }
}

// Allow Azure Service and Resources to access this server
resource allowAllWindowsAzureIps 'Microsoft.Sql/servers/firewallRules@2022-05-01-preview' = {
  name: 'AllowAllWindowsAzureIps'
  parent: sqlServer
  properties: {
    endIpAddress: '0.0.0.0'
    startIpAddress: '0.0.0.0'
  }
}

output sqlServerName string = sqlServer.name

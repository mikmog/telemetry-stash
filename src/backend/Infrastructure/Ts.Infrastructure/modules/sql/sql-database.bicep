// ********************************************************************
// SQL SERVER DATABASE
// https://learn.microsoft.com/en-us/azure/templates/microsoft.sql/servers/databases?pivots=deployment-language-bicep
// ********************************************************************

import { applicationParams, sqlParams, getResourceName }  from '../../parameter-types.bicep'
param applicationParameters applicationParams
param sqlParameters sqlParams
param sqlServerName string

resource sqlServer 'Microsoft.Sql/servers@2021-11-01' existing = {
  name: sqlServerName
}

resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2021-11-01' = {
  name: getResourceName({ resourceAbbr: 'sqldb' }, applicationParameters)
  location: applicationParameters.location
  parent: sqlServer
  tags: applicationParameters.tags
   sku: {
    name: sqlParameters.sku.name
    tier: sqlParameters.sku.tier
    capacity: sqlParameters.sku.capacity
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: sqlParameters.sku.maxSizeBytes
    zoneRedundant: false
    readScale: 'Disabled'
    requestedBackupStorageRedundancy: 'Local'
    isLedgerOn: false
  }
}

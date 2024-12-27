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

var vCoreFreeTier = sqlParameters.sku.name == 'GP_S_Gen5' ? {
  minCapacity: '0.5'
  useFreeLimit: true
  freeLimitExhaustionBehavior: 'AutoPause'
} : {}

resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2023-05-01-preview' = {
  name: getResourceName({ resourceAbbr: 'sqldb' }, applicationParameters)
  location: applicationParameters.location
  parent: sqlServer
  tags: applicationParameters.tags
   sku: {
    name: sqlParameters.sku.name
    tier: sqlParameters.sku.tier
    family: sqlParameters.sku.family
    capacity: sqlParameters.sku.capacity
  }
  properties: union( {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: sqlParameters.sku.maxSizeBytes
    zoneRedundant: false
    readScale: 'Disabled'
    requestedBackupStorageRedundancy: 'Local'
    isLedgerOn: false
  }, vCoreFreeTier)
}

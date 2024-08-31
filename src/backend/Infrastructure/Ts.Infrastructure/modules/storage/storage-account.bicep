// ********************************************************************
// STORAGE ACCOUNT
// https://learn.microsoft.com/en-us/azure/templates/microsoft.storage/allversions
// ********************************************************************

import { applicationParams, getResourceName }  from '../../parameter-types.bicep'
param applicationParameters applicationParams
param resourceName string

resource appStorage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: replace(getResourceName({ resourceAbbr: 'st', resourceName: resourceName }, applicationParameters), '-', '')
  location: applicationParameters.location
  tags: applicationParameters.tags
  kind: 'StorageV2'
  identity: {
    type: 'None'
  }
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
  }
}

output appStorageName string = appStorage.name

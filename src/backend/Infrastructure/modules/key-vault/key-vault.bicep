// ********************************************************************
// KEY VAULT
// https://learn.microsoft.com/en-us/azure/templates/microsoft.keyvault/vaults?pivots=deployment-language-bicep
// ********************************************************************

import { applicationParams, getResourceName }  from '../../parameters.bicep'
param applicationParameters applicationParams

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: getResourceName({ resourceAbbr: 'kv' }, applicationParameters)
  location: applicationParameters.location
  tags: applicationParameters.tags
  properties: {
    tenantId: subscription().tenantId    
    enableRbacAuthorization: true
    publicNetworkAccess: 'enabled'
    sku: {
      family: 'A'
      name: 'standard'
    }
    enableSoftDelete: true
    softDeleteRetentionInDays: 30
  }
}

output keyVaultName string = keyVault.name

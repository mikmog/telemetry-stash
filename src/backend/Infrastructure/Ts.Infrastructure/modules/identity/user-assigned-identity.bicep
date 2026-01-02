// ********************************************************************
// USER ASSIGNED IDENTITY
// https://learn.microsoft.com/en-us/azure/templates/microsoft.managedidentity/userassignedidentities?pivots=deployment-language-bicep
// ********************************************************************

import { applicationParams, getResourceName }  from '../../parameter-types.bicep'

param applicationParameters applicationParams
param resourceName string

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
    name: getResourceName({ resourceAbbr: 'id', resourceName: resourceName }, applicationParameters)
    location: applicationParameters.location
    tags: applicationParameters.tags
}

output clientId string = identity.properties.clientId
output identityId string = identity.id
output principalId string = identity.properties.principalId

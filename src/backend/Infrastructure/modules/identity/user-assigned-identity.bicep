// ********************************************************************
// USER ASSIGNED IDENTITY
// https://learn.microsoft.com/en-us/azure/templates/microsoft.managedidentity/2023-01-31/userassignedidentities?pivots=deployment-language-bicep
// ********************************************************************

import { applicationParams, getResourceName }  from '../../parameters.bicep'
param applicationParameters applicationParams

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
    name: getResourceName({ resourceAbbr: 'id' }, applicationParameters)
    location: applicationParameters.location
    tags: applicationParameters.tags
}

output identityId string = identity.id
output identityName string = identity.name
output identityClientId string = identity.properties.clientId

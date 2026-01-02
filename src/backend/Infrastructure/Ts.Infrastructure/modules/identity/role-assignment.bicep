// ********************************************************************
// USER ASSIGNED IDENTITY ROLES ASSIGNMENT
// https://learn.microsoft.com/en-us/azure/templates/microsoft.authorization/roleassignments?pivots=deployment-language-bicep
// ********************************************************************

param principalId string
param identityId string
param roleAssignments string[]

resource assignRoles 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for assignment in roleAssignments: {
  name: guid(identityId, principalId, resourceGroup().id, azureRoleDefinitionIds[assignment])
  scope: resourceGroup() // scope defined at module level
  properties: {
    principalId: principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions',  azureRoleDefinitionIds[assignment!])
  }
}]

// https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles
var azureRoleDefinitionIds = {
  Contributor: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
}

// ********************************************************************
// STORAGE ROLES ASSIGNMENT
// https://learn.microsoft.com/en-us/azure/templates/microsoft.authorization/2022-04-01/roleassignments?pivots=deployment-language-bicep
// ********************************************************************

import { roleAssignment }  from '../../parameter-types.bicep'
param storageName string
param roleAssignments roleAssignment[]

resource storage 'Microsoft.Storage/storageAccounts@2021-09-01' existing = {
  name: storageName
}

resource assignRoles 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for assignment in roleAssignments: {
  name: guid(storage.id, assignment.principalId, storageRoleDefinitions[assignment.roleDefinition])
  scope: storage
  properties: {
    principalId: assignment.principalId
    principalType: assignment.principalType
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', storageRoleDefinitions[assignment.roleDefinition])
  }
}]

// https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#internet-of-things
var storageRoleDefinitions = {
    StorageBlobDataOwner:      'b7e6dc6d-f1e8-4753-8033-0f276bb0955b'
}

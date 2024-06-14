// ********************************************************************
// KEY VAULT ROLES ASSIGNMENT
// https://learn.microsoft.com/en-us/azure/templates/microsoft.authorization/2022-04-01/roleassignments?pivots=deployment-language-bicep
// ********************************************************************

import { roleAssignment }  from '../../parameters.bicep'
param roleAssignments roleAssignment[]
param keyVaultName string

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

resource assignRoles 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for assignment in roleAssignments: {
  name: guid(keyVault.id, assignment.principalId, keyVaultRoleDefinitions[assignment.roleDefinition])
  scope: keyVault
  properties: {
    principalId: assignment.principalId
    principalType: assignment.principalType
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions',  keyVaultRoleDefinitions[assignment.roleDefinition])
  }
}]

// https://learn.microsoft.com/en-us/azure/key-vault/general/rbac-guide?tabs=azure-cli#azure-built-in-roles-for-key-vault-data-plane-operations
var keyVaultRoleDefinitions = {
  Administrator:                '00482a5a-887f-4fb3-b363-3b7fe8e74483'
  CertificateOfficer:           'a4417e6f-fecd-4de8-b567-7b0420556985'
  Contributor:                  'f25e0fa2-a7c8-4377-a976-54943a77a395'
  CryptoOfficer:                '14b46e9e-c2b7-41b4-b07b-48a6ebf60603'
  CryptoServiceEncryptionUser:  'e147488a-f6f5-4113-8e2d-b22465e65bf6'
  CryptoUser:                   '12338af0-0e69-4776-bea7-57ae8d297424'
  Reader:                       '21090545-7ca7-4776-b22c-e363652d74d2'
  SecretsOfficer:               'b86a8fe4-44ce-4948-aee5-eccb2c155cd7'
  SecretsUser:                  '4633458b-17de-408a-b874-0445c86b69e6'
}

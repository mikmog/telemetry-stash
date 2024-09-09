// ********************************************************************
// IOT HUB ROLES ASSIGNMENT
// https://learn.microsoft.com/en-us/azure/templates/microsoft.authorization/2022-04-01/roleassignments?pivots=deployment-language-bicep
// ********************************************************************

import { roleAssignment }  from '../../parameter-types.bicep'
param roleAssignments roleAssignment[]
param iotHubName string

resource iotHub 'Microsoft.Devices/IotHubs@2023-06-30' existing = {
  name: iotHubName
}

resource assignRoles 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for assignment in roleAssignments: {
  name: guid(iotHub.id, assignment.principalId, iotHubRoleDefinitions[assignment.roleDefinition])
  scope: iotHub
  properties: {
    principalId: assignment.principalId
    principalType: assignment.principalType
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', iotHubRoleDefinitions[assignment.roleDefinition])
  }
}]

// https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#internet-of-things
var iotHubRoleDefinitions = {
    IoTHubDataContributor:      '4fc6c259-987e-4a07-842e-c321cc9d413f'
    IoTHubDataReader:           'b447c946-2db7-41ec-983d-d8bf3b1c77e3'
    IoTHubRegistryContributor:  '4ea46cd5-c1b2-4a8e-910b-273211f9ce47'
    IoTHubTwinContributor:      '494bdba2-168f-4f31-a0a1-191d2f7c028c'
}

﻿// ********************************************************************
// PARAMETER TYPE DEFINITIONS
// ********************************************************************

@export()
func getResourceName(nameParam name, parameters applicationParams) string => '${nameParam.resourceAbbr}-${empty(nameParam.?resourceName) ? '' : '${nameParam.resourceName}-'}${parameters.appName}-${parameters.envAbbr}'
type name = {
  resourceAbbr: string
  resourceName: string?
}

@export()
type applicationParams = {
  appName: string
  envAbbr: 'dev' | 'develop' | 'prod'
  environment: 'Development' | 'Production'
  location: 'swedencentral'
  tags: object
}

@export()
type appParams = {
  api: appServiceParams
  functions: functionParams
}

@export()
type appServiceParams = {
  sku: { 
    name: 'F1'
    tier: 'Free'
    size: 'F1'
    family: 'F'
    capacity: 1
  } | {
    name: 'B1'
    tier: 'Basic'
    size: 'B1'
    family: 'B'
    capacity: 1
  }
  linuxFxVersion: ('DOTNET-ISOLATED|8.0')
  resourceName: string
}

// @export()
// type skuParams = {
//   sku: { 
//     name: 'F1'
//     tier: 'Free'
//     size: 'F1'
//     family: 'F'
//     capacity: 1
//   } | {
//     name: 'B1'
//     tier: 'Basic'
//     size: 'B1'
//     family: 'B'
//     capacity: 1
//   }
// }

@export()
type functionParams = {
  sku: { 
    name: 'Y1'
    tier: 'Dynamic'
    size: 'Y1'
    family: 'Y'
    capacity: 0
  } | {
    name: 'B1'
    tier: 'Basic'
    size: 'B1'
    family: 'B'
    capacity: 1
  }
  linuxFxVersion: ('DOTNET-ISOLATED|8.0')
  resourceName: string
}

@export()
type iotHubParams = {
  sku: { 
    name: 'B1'
    capacity: 1
    partitionCount: 4
  } | {
    name: 'F1'
    capacity: 1
    partitionCount: 2
  }
  
  retentionTimeInDays: 1 | 7 | 30
  location: 'westeurope' | 'swedencentral' | 'norwayeast'
  roleAssignments: roleAssignment[]
}

@export()
type keyVaultParams = {
  roleAssignments: roleAssignment[]
}

@export()
type monitorParams = {
  retentionInDays: 30 | 90 | 180
  dailyQuotaGb: 1 | 10 | 100
}

@export()
type roleAssignment = {
  principalId: string
  principalType: 'Device' | 'ForeignGroup' | 'Group' | 'ServicePrincipal' | 'User'
  roleDefinition: string // See role-assigment.bicep for definitions
}

@export()
type sqlParams = {
  resourceName: string?
  sku: { 
    name: 'Basic'
    tier: 'Basic'
    capacity: 5
    maxSizeBytes: 2147483648
  } | {
    name: 'Standard'
    tier: 'Standard'
    capacity: 10
    maxSizeBytes: 268435456000
  }
  server: {
    adminLoginName: string
    adminGroupSid: string
  }
}

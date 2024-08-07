// ********************************************************************
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
  envAbbr: 'dev' | 'prod'
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
    name: 'F1' | 'B1'
    tier: 'Free' | 'Basic'
    size: 'F1' | 'B1'
    family: 'F' | 'B'
    capacity: 1
  }
  linuxFxVersion: ('DOTNET-ISOLATED|8.0')
  resourceName: string
}

@export()
type functionParams = {
  sku: { 
    name: 'Y1' | 'B1'
    tier: 'Dynamic' | 'Basic'
    size: 'Y1' | 'B1'
    family: 'Y' | 'B'
    capacity: 0 | 1
  }
  linuxFxVersion: ('DOTNET-ISOLATED|8.0')
  resourceName: string
}

@export()
type iotHubParams = {
  sku: { 
    name: 'B1'
    capacity: 1
  }
  
  retentionTimeInDays: 1 | 7 | 30
  partitionCount: 4
  location: 'westeurope' | 'swedencentral'
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
  roleDefinition: 'Administrator' | 'CertificateOfficer' | 'Contributor' | 'CryptoOfficer' | 'CryptoServiceEncryptionUser' | 'CryptoUser' | 'Reader' | 'SecretsOfficer' | 'SecretsUser'
}

@export()
type sqlParams = {
  sku: { 
    name: 'Basic' | 'Standard'
    tier: 'Basic' | 'Standard'
    capacity: 5 | 10
  }
  maxSizeBytes: 2147483648 | 268435456000
  server: {
    adminLoginName: string
    adminGroupSid: string
  }
}

using './main.bicep'
import { defaultTagDefinitions, userDefinitions }  from './definitions.bicep'

// Application
param applicationParameters = {
  appName: 'ts'
  environment: 'Production'
  envAbbr: 'prod'
  location: 'swedencentral'
  tags: union(defaultTagDefinitions, { Environment: 'Production' })
}

// Application Insights
param monitorParameters = {
  dailyQuotaGb: 1
  retentionInDays: 30
}

// API + Functions
param appParameters = {
  api: {
    sku: {
      name: 'F1'
      tier: 'Free'
      size: 'F1'
      family: 'F'
      capacity: 1
    }
    linuxFxVersion: 'DOTNET-ISOLATED|8.0'
    resourceName: 'api'
  }
  functions: {
    sku: {
      name: 'Y1'
      tier: 'Dynamic'
      size: 'Y1'
      family: 'Y'
      capacity: 0
    }
    linuxFxVersion: 'DOTNET-ISOLATED|8.0'
    resourceName: 'func'
  }
}

// Key Vault Role Assignment
param keyVaultParameters = {
  roleAssignments: [
    {
      principalId: userDefinitions.Administrator_Prod.principalId
      principalType: 'Group'
      roleDefinition: 'Administrator'
    }
]}

// Database
param sqlParameters = {
  sku: {
    name: 'Basic'
    tier: 'Basic'
    capacity: 5
    maxSizeBytes: 2147483648
  }
  server: {
    adminLoginName: userDefinitions.Administrator_Prod.name
    adminGroupSid: userDefinitions.Administrator_Prod.principalId
  }
}

// Iot Hub
param iotHubParameters = {
    sku: {
      name: 'B1'
      capacity: 1
      partitionCount: 4
    }
    location: 'westeurope'
    retentionTimeInDays: 1
    roleAssignments: []
 }

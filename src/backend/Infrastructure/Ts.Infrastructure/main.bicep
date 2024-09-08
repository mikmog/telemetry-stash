// ********************************************************************
// MAIN.BICEP
// https://learn.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/resource-abbreviations
// ********************************************************************

import { applicationParams, appParams, iotHubParams, keyVaultParams, monitorParams, sqlParams, getResourceName }  from './parameter-types.bicep'

targetScope = 'resourceGroup'

param applicationParameters applicationParams
param appParameters appParams
param iotHubParameters iotHubParams
param keyVaultParameters keyVaultParams
param monitorParameters monitorParams
param sqlParameters sqlParams

// Key Vault
module keyVaultModule 'modules/key-vault/key-vault.bicep' = {
  name: getResourceName({ resourceAbbr: 'kv' }, applicationParameters)
  params: { applicationParameters: applicationParameters }
}

// Monitor - Workspace
module logWorkspaceModule 'modules/monitor/log-workspace.bicep' = {
  name: getResourceName({ resourceAbbr: 'log' }, applicationParameters)
  params: { 
    applicationParameters: applicationParameters
    monitorParameters: monitorParameters
  }
}

// Monitor - Application Insights
module appInsightsModule 'modules/monitor/application-insights.bicep' = {
  name: getResourceName({ resourceAbbr: 'appi' }, applicationParameters)
  params: {
    applicationParameters: applicationParameters
    logAnalyticsWorkspaceId: logWorkspaceModule.outputs.logAnalyticsWorkspaceId
  }
}

// App - Functions Service Plan
module functionsAppServicePlanModule 'modules/app/app-service-plan.bicep' = {
  name: getResourceName({ resourceAbbr: 'asp', resourceName: appParameters.functions.resourceName }, applicationParameters)
  params: { 
    applicationParameters: applicationParameters
    skuParameters: appParameters.functions.sku
    resourceName: appParameters.functions.resourceName
  }
}

// Storage Account
module functionsStorageModule 'modules/storage/storage-account.bicep' = {
  name: replace(getResourceName({ resourceAbbr: 'st', resourceName: 'func' }, applicationParameters), '-', '')
  params: { 
    applicationParameters: applicationParameters
    resourceName: 'func'
  }
}

// Function - App
module functionAppModule 'modules/app/function-app.bicep' = {
  name: getResourceName({ resourceAbbr: 'func' }, applicationParameters)
  params: {
    applicationParameters: applicationParameters
    functionParameters: appParameters.functions
    appServicePlanId: functionsAppServicePlanModule.outputs.appServicePlanId
    appStorageName: functionsStorageModule.outputs.appStorageName
    appInsightsConnectionString: appInsightsModule.outputs.appInsightsConnectionString
    appInsightsInstrumentationKey: appInsightsModule.outputs.appInsightsInstrumentationKey
  }
}

// App - API Service Plan
module apiAppServicePlanModule 'modules/app/app-service-plan.bicep' = {
  name: getResourceName({ resourceAbbr: 'asp', resourceName: appParameters.api.resourceName }, applicationParameters)
  params: { 
    applicationParameters: applicationParameters
    skuParameters: appParameters.api.sku
    resourceName: appParameters.api.resourceName
  }
}

// App service - API
module apiModule 'modules/app/app-service.bicep' = {
  name: getResourceName({ resourceAbbr: 'app' }, applicationParameters)
  params: {
    applicationParameters: applicationParameters
    appServiceParameters: appParameters.api
    appServicePlanId: apiAppServicePlanModule.outputs.appServicePlanId
    appInsightsConnectionString: appInsightsModule.outputs.appInsightsConnectionString
    appInsightsInstrumentationKey: appInsightsModule.outputs.appInsightsInstrumentationKey
  }
}

// SQL - Server
module sqlServerModule 'modules/sql/sql-server.bicep' = {
  name: getResourceName({ resourceAbbr: 'sql' }, applicationParameters)
  params: { 
    applicationParameters: applicationParameters
    sqlParameters: sqlParameters
  }
}

// SQL - Database
module sqlDatabaseModule 'modules/sql/sql-database.bicep' = {
  name: getResourceName({ resourceAbbr: 'sqldb' }, applicationParameters)
  params: {
    applicationParameters: applicationParameters
    sqlParameters: sqlParameters
    sqlServerName: sqlServerModule.outputs.sqlServerName
  }
}

// Iot Hub
module iotHubModule 'modules/iot-hub/iot-hub.bicep' = {
  name: getResourceName({ resourceAbbr: 'iot-egst' }, applicationParameters)
  params: { 
    applicationParameters: applicationParameters
    iotHubParameters: iotHubParameters
    functionsAppName: functionAppModule.outputs.functionsAppName
  }
}

// Iot Hub - Role Assignments
module iotHubRoleAssignmentModule 'modules/iot-hub/role-assignment.bicep' = {
  name: getResourceName({ resourceAbbr: 'ras', resourceName: 'iot' }, applicationParameters)
  params: { 
    iotHubName: iotHubModule.outputs.iotHubName
    roleAssignments: concat([
      {
        principalId: functionAppModule.outputs.identityPrincipalId
        principalType: 'ServicePrincipal'
        roleDefinition: 'IoTHubDataReader'
      }
    ], iotHubParameters.roleAssignments)
  }
}

// Key Vault - Role Assignments
module keyVaultRoleAssignmentModule 'modules/key-vault/role-assignment.bicep' = {
  name: getResourceName({ resourceAbbr: 'ras', resourceName: 'kv' }, applicationParameters)
  params: { 
    keyVaultName: keyVaultModule.outputs.keyVaultName
    roleAssignments: concat([
      {
        principalId: functionAppModule.outputs.identityPrincipalId
        principalType: 'ServicePrincipal'
        roleDefinition: 'SecretsUser'
      }
      {
        principalId: apiModule.outputs.identityPrincipalId
        principalType: 'ServicePrincipal'
        roleDefinition: 'SecretsUser'
      }
    ], keyVaultParameters.roleAssignments)
  }
}

module lockModule 'modules/lock/delete-lock.bicep' = {
  name: getResourceName({ resourceAbbr: 'lock' }, applicationParameters)
  params: { applicationParameters: applicationParameters }
}

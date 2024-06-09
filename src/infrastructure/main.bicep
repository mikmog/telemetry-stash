targetScope = 'resourceGroup'

@allowed(['dev', 'prod'])
param env string

var environmentMap = {
  prod: 'Production'
  dev: 'Development'
}
var environment = environmentMap[env]

@description('The location in which the resources should be deployed.')
param location string = resourceGroup().location
param appName string
param commonTags object = {}
var envTag = {
    Environment : environment
}


param sqlAdminGroupName string = 'Telemetry Stash Owners'
param sqlAdminGroupId string = 'c8d44721-8c00-4371-acf2-190811d54469'

// ********************************************************************
// USER-ASSIGNED IDENTITY
// ********************************************************************
resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
    name: 'id-${appName}-${env}'
    location: location
    tags: union(commonTags, envTag)
}

// ********************************************************************
// KEY VAULT
// ********************************************************************
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: 'kv1-${appName}-${env}'
  location: location
  tags: union(commonTags, envTag)
  properties: {
    tenantId: subscription().tenantId    
    enableRbacAuthorization: true
    publicNetworkAccess: 'enabled'
    sku: {
      family: 'A'
      name: 'standard'
    }
    enableSoftDelete: true
    softDeleteRetentionInDays: 30
  }
}

// ********************************************************************
// LOG ANALYTICS WORKSPACE
// ********************************************************************
resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: 'log-${appName}-${env}'
  location: location
  tags: union(commonTags, envTag)
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 90
    workspaceCapping: {
      dailyQuotaGb: 1
    }
  }
}

// ********************************************************************
// SQL SERVER
// ********************************************************************
param sqlServerExist bool = true

resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
  name: 'sql-${appName}-${env}'
  location: location
  tags: union(commonTags, envTag)
  properties: sqlServerExist ? {
    minimalTlsVersion: '1.2'
    administrators: {
      administratorType: 'ActiveDirectory'
      login: sqlAdminGroupName
      azureADOnlyAuthentication: true
      principalType: 'Group'
      sid: sqlAdminGroupId
      tenantId: subscription().tenantId 
    }
    publicNetworkAccess: 'Enabled'
    restrictOutboundNetworkAccess: 'Disabled'
  } : {
    minimalTlsVersion: '1.2'
    administrators: {
      administratorType: 'ActiveDirectory'
      login: sqlAdminGroupName
      azureADOnlyAuthentication: true
      principalType: 'Group'
      sid: sqlAdminGroupId
      tenantId: subscription().tenantId 
    }
    administratorLogin: 'dbadmin'
    #administratorLoginPassword: ''
    publicNetworkAccess: 'Enabled'
    restrictOutboundNetworkAccess: 'Disabled'  
  }
}

// resource sqlAzureAdOnly 'Microsoft.Sql/servers/azureADOnlyAuthentications@2022-05-01-preview' = {
//   name: 'Default'
//   parent: sqlServer
//   properties: {
//     azureADOnlyAuthentication: true
//   }
// }

// Allow Azure Service and Resources to access this server
resource allowAllWindowsAzureIps 'Microsoft.Sql/servers/firewallRules@2022-05-01-preview' = {
  name: 'AllowAllWindowsAzureIps'
  parent: sqlServer
  properties: {
    endIpAddress: '0.0.0.0'
    startIpAddress: '0.0.0.0'
  }
}

// ********************************************************************
// SQL SERVER DATABASE
// ********************************************************************
var tierMaxSizeBytes = {
  Basic: 2147483648
  Standard: 268435456000
}

param sqldbSkuName string = 'Standard'
param sqldbSkuTier string = 'Standard'
param sqldbSkuCapacity int = 10

resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2021-11-01' = {
  name: 'sqldb-${appName}-${env}'
  location: location
  parent: sqlServer
  tags: union(commonTags, envTag)
   sku: {
    name: sqldbSkuName
    tier: sqldbSkuTier
    capacity: sqldbSkuCapacity
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: tierMaxSizeBytes[sqldbSkuTier]
    zoneRedundant: false
    readScale: 'Disabled'
    requestedBackupStorageRedundancy: 'Local'
    isLedgerOn: false
  }
}


// ********************************************************************
// FUNCTIONS APP SERVICE PLAN
// ********************************************************************
resource appStorage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: 'st${appName}${env}'
  location: location
  tags: union(commonTags, envTag)
  kind: 'StorageV2'
  identity: {
    type: 'None'
  }
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
  }
  // properties:{
  //   allowBlobPublicAccess: false
  //   allowSharedKeyAccess: false
  //   networkAcls:{
  //      bypass: 'AzureServices'
  //      defaultAction: 'Deny'
  //      resourceAccessRules:[{
  //        resourceId: iotHub.id
  //        tenantId: tenant().tenantId
  //      }]
  //   }
  // }
}

resource functionApplicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: 'appi-func-${appName}-${env}'
  location: location
  tags: union(commonTags, envTag)
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
    WorkspaceResourceId: logAnalyticsWorkspace.id
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' =  {
    name: 'asp-func-${appName}-${env}'
    location: location
    tags: union(commonTags, envTag)
    sku: {
        name: 'Y1'
        tier: 'Dynamic'
    }
}

resource functionApp 'Microsoft.Web/sites@2021-03-01' = {
  name: 'func-${appName}-${env}'
  location: location
  tags: union(commonTags, envTag)
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      netFrameworkVersion: 'v8.0'
      use32BitWorkerProcess: false
      //http20Enabled: true
      publicNetworkAccess: 'Enabled'
      ipSecurityRestrictions: [
        {
          ipAddress: 'Any'
          action: 'Allow'
        }
      ]
      scmIpSecurityRestrictions: [
        {
          ipAddress: 'Any'
          action: 'Allow'
        }
      ]
      appSettings: [
        {
          name: 'AZURE_FUNCTIONS_ENVIRONMENT'
          value: environment
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: functionApplicationInsights.properties.ConnectionString
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${appStorage.name};AccountKey=${appStorage.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${appStorage.name};AccountKey=${appStorage.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: 'func-${appName}-${env}'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED'
          value: '1'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
      ]
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
    }
    httpsOnly: true
  }
}


// ********************************************************************
// IOT HUB
// ********************************************************************
// https://veldify.com/2023/03/06/setting-up-a-iot-hub-with-bicep/

param iotHubLocation string = 'westeurope'

resource iotHub 'Microsoft.Devices/IotHubs@2023-06-30' = {
  name: 'iot-${appName}-${env}'
  location: iotHubLocation
  tags: union(commonTags, envTag)
  sku: {
    name: 'B1'
    capacity: 1
  }
    properties: {
    eventHubEndpoints: {
      events: {
        retentionTimeInDays: 1
        partitionCount: 4
      }
    }
    routing: {
        endpoints: {
        }
      // routes: [
      //   {
      //     name: 'RoutingToEventGrid'
      //     source: 'DeviceMessages'
      //     condition: 'true'
      //     endpointNames: [
      //       'eventgrid'
      //     ]
      //     isEnabled: true
      //   }
      // ]
      fallbackRoute: {
        name: '$fallback'
        source: 'DeviceMessages'
        condition: 'true'
        endpointNames: [
          'events'
        ]
        isEnabled: true
      }
    }    
  }
}

resource telemetryReceivedTopic 'Microsoft.EventGrid/systemTopics@2022-06-15' = {
  name: 'egst-telemetryreceived-${appName}-${env}'
  location: iotHubLocation
  tags: union(commonTags, envTag)
  properties: {
    source: iotHub.id
    topicType: 'Microsoft.Devices.IoTHubs'
  }
}

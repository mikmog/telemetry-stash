﻿using './main.bicep'
import { defaultTagDefinitions, userDefinitions }  from './definitions.bicep'

// Application
param applicationParameters = {
  appName: 'ts'
  environment: 'Development'
  envAbbr: 'develop'
  location: 'swedencentral'
  tags: union(defaultTagDefinitions, { Environment: 'Development' })
}

// Application Insights
param monitorParameters = {
  dailyQuotaGb: 1
  retentionInDays: 30
}

param appParameters = {
  api: {
    sku: {
      name: 'F1'
      tier: 'Free'
      size: 'F1'
      family: 'F'
      capacity: 1
    }
    linuxFxVersion: 'DOTNET-ISOLATED|9.0'
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
    linuxFxVersion: 'DOTNET-ISOLATED|9.0'
    resourceName: 'func'
  }
}

// Key Vault Role Assignment
param keyVaultParameters = {
  roleAssignments: [
    {
      principalId: userDefinitions.Administrator_NonProd.principalId
      principalType: 'Group'
      roleDefinition: 'Administrator'
    }
]}

// Database
param sqlParameters = {
  sku: {
    name: 'GP_S_Gen5'
    tier: 'GeneralPurpose'
    family: 'Gen5'
    capacity: 1
    maxSizeBytes: 34359738368
  }
  server: {
    adminLoginName: userDefinitions.Administrator_NonProd.name
    adminGroupSid: userDefinitions.Administrator_NonProd.principalId
  }
}

// Iot Hub
param iotHubParameters = {
    sku: {
      name: 'F1'
      capacity: 1
      partitionCount: 2
    }
    location: 'westeurope'
    retentionTimeInDays: 1
    roleAssignments: [
    {
      principalId: userDefinitions.Administrator_NonProd.principalId
      principalType: 'Group'
      roleDefinition: 'IoTHubDataReader'
    }
  ]}

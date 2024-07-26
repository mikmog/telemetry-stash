// ********************************************************************
// LOG ANALYTICS WORKSPACE
// https://learn.microsoft.com/en-us/azure/templates/microsoft.operationalinsights/allversions
// ********************************************************************

import { applicationParams, monitorParams, getResourceName }  from '../../parameters.bicep'
param applicationParameters applicationParams
param monitorParameters monitorParams

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: getResourceName({ resourceAbbr: 'log' }, applicationParameters)
  location: applicationParameters.location
  tags: applicationParameters.tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: monitorParameters.retentionInDays
    workspaceCapping: {
      dailyQuotaGb: monitorParameters.dailyQuotaGb
    }
  }
}

output logAnalyticsWorkspaceId string = logAnalyticsWorkspace.id

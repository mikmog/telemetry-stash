// ********************************************************************
// APPLICATION INSIGHTS
// https://learn.microsoft.com/en-us/azure/templates/microsoft.insights/allversions
// ********************************************************************

import { applicationParams, monitorParams, getResourceName }  from '../../parameters.bicep'
param applicationParameters applicationParams
param logAnalyticsWorkspaceId string

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: getResourceName({ resourceAbbr: 'appi' }, applicationParameters)
  location: applicationParameters.location
  tags: applicationParameters.tags
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
    WorkspaceResourceId: logAnalyticsWorkspaceId
  }
}

output appInsightsName string = applicationInsights.name

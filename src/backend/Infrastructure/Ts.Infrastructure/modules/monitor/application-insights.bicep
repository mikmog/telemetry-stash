// ********************************************************************
// APPLICATION INSIGHTS
// https://learn.microsoft.com/en-us/azure/templates/microsoft.insights/components?pivots=deployment-language-bicep
// ********************************************************************

import { applicationParams, getResourceName }  from '../../parameter-types.bicep'
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
    Flow_Type: 'Bluefield'
    WorkspaceResourceId: logAnalyticsWorkspaceId
  }
}

output appInsightsConnectionString string = applicationInsights.properties.ConnectionString
output appInsightsInstrumentationKey string = applicationInsights.properties.InstrumentationKey

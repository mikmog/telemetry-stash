// ********************************************************************
// IOT HUB
// Create an IoT Hub with an Event Grid System Topic and an Event Grid Subscription to an Azure Function
// https://learn.microsoft.com/en-us/azure/templates/microsoft.devices/2023-06-30/iothubs?pivots=deployment-language-bicep
// https://learn.microsoft.com/sv-se/azure/templates/microsoft.eventgrid/2022-06-15/systemtopics?pivots=deployment-language-bicep
// https://learn.microsoft.com/en-us/azure/templates/microsoft.eventgrid/2023-12-15-preview/systemtopics/eventsubscriptions?pivots=deployment-language-bicep
// ********************************************************************

import { applicationParams, iotHubParams, getResourceName }  from '../../parameter-types.bicep'
param applicationParameters applicationParams
param iotHubParameters iotHubParams
param functionsAppName string

var functionName = 'TelemetryTrigger'
var eventSubscriptionName = 'TelemetryRecived'

resource iotHub 'Microsoft.Devices/IotHubs@2023-06-30' = {
  name: getResourceName({ resourceAbbr: 'iot' }, applicationParameters)
  location: iotHubParameters.location
  tags: applicationParameters.tags
  sku: {
    name: iotHubParameters.sku.name
    capacity: iotHubParameters.sku.capacity
  }
    properties: {
    eventHubEndpoints: {
      events: {
        retentionTimeInDays: iotHubParameters.retentionTimeInDays
        partitionCount: iotHubParameters.sku.partitionCount
      }
    }
    routing: {
        endpoints: {
        }
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
  name: getResourceName({ resourceAbbr: 'egst', resourceName: 'telemetry' }, applicationParameters)
  location: iotHubParameters.location
  tags: applicationParameters.tags
  properties: {
    source: iotHub.id
    topicType: 'Microsoft.Devices.IoTHubs'
  }
}

resource functionApp 'Microsoft.Web/sites@2022-09-01' existing = {
  name: functionsAppName
}

resource eventSubscription 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2023-12-15-preview' = {
  name: eventSubscriptionName
  parent: telemetryReceivedTopic
  properties: {
    destination: {
      endpointType: 'AzureFunction'
      properties: {
        maxEventsPerBatch: 1
        preferredBatchSizeInKilobytes: 64
        resourceId: '${functionApp.id}/functions/${functionName}'
      }
    }
    eventDeliverySchema: 'CloudEventSchemaV1_0'
    filter: {
      includedEventTypes: [
        'Microsoft.Devices.DeviceTelemetry'
      ]
    }
    retryPolicy: {
      eventTimeToLiveInMinutes: 1440
      maxDeliveryAttempts: 30
    }
  }
}

output iotHubName string = iotHub.name

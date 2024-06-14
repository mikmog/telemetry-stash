// ********************************************************************
// IOT HUB
// https://learn.microsoft.com/en-us/azure/templates/microsoft.devices/iothubs?pivots=deployment-language-bicep
// ********************************************************************

import { applicationParams, iotHubParams, getResourceName }  from '../../parameters.bicep'
param applicationParameters applicationParams
param iotHubParameters iotHubParams

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
        partitionCount: iotHubParameters.partitionCount
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
  name: getResourceName({ resourceAbbr: 'egst', resourceName: 'telemetry' }, applicationParameters)
  location: iotHubParameters.location
  tags: applicationParameters.tags
  properties: {
    source: iotHub.id
    topicType: 'Microsoft.Devices.IoTHubs'
  }
}

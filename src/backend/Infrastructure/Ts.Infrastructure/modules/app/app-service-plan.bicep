// ********************************************************************
// LINUX APP SERVICE PLAN
// https://learn.microsoft.com/en-us/azure/templates/microsoft.web/serverfarms?pivots=deployment-language-bicep
// ********************************************************************

import { applicationParams, getResourceName }  from '../../parameter-types.bicep'
param applicationParameters applicationParams
param skuParameters skuParams 
param resourceName string

type skuParams = {
    name: string
    tier: string
    size: string
    family: string
    capacity: int
}

resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' =  {
    name: getResourceName({ resourceAbbr: 'asp', resourceName: resourceName }, applicationParameters)
    location: applicationParameters.location
    tags: applicationParameters.tags
    sku: {
        name: skuParameters.name
        tier: skuParameters.tier
        size: skuParameters.size
        family: skuParameters.family
        capacity: skuParameters.capacity
    }
    properties: {
        reserved: true
    }
}

output appServicePlanId string = appServicePlan.id

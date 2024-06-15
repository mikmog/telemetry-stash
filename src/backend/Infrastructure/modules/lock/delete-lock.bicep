// ********************************************************************
// DELETE LOCK
// ********************************************************************

import { applicationParams, getResourceName }  from '../../parameters.bicep'
param applicationParameters applicationParams

 resource resourceDeleteLock 'Microsoft.Authorization/locks@2016-09-01' = {
  name: getResourceName({ resourceAbbr: 'lock' }, applicationParameters)
  properties: {
    level: 'CanNotDelete'
    notes: 'Prevent delete resources in resource group'
  }
}

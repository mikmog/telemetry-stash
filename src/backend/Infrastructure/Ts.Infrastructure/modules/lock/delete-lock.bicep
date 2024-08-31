// ********************************************************************
// DELETE LOCK
// ********************************************************************

import { applicationParams, getResourceName }  from '../../parameter-types.bicep'
param applicationParameters applicationParams

 resource resourceDeleteLock 'Microsoft.Authorization/locks@2016-09-01' = {
  name: getResourceName({ resourceAbbr: 'lock' }, applicationParameters)
  properties: {
    level: 'CanNotDelete'
    notes: 'Prevent delete resources in resource group'
  }
}

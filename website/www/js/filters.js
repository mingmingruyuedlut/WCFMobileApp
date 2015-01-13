angular.module('interactive.filters', [])

.filter('filterByService', function() {
  var serviceMapping = {
    'cloud & managed services': {
      label: 'CLOUD & MANAGED SERVICES',
      icon: 'icon-cloud_managed_service'
    },
    'data centre': {
      label: 'DATA CENTRE',
      icon: 'icon-data_centre'
    },
    'business continuity': {
      label: 'BUSINESS CONTINUITY',
      icon: 'icon-business_continuity'
    },
    'hardware maintenance': {
      label: 'HARDWARE MAINTENANCE',
      icon: 'icon-hardware_maintenance'
    }
  };

  return function(serviceName, returnKey) {
    return (serviceName && returnKey) ? serviceMapping[serviceName][returnKey] : '';
  };
})

.filter('emailName', function() {
  return function(input) {
    return input.split('@')[0];
  };
});
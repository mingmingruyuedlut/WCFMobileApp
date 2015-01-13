angular.module('interactive', [
  'ionic',
  'ngResource',
  'ngCookies',
  'interactive.controllers',
  'interactive.directives',
  'interactive.filters',
  'interactive.services'
  ])

.run(function($ionicPlatform) {
  $ionicPlatform.ready(function() {
    // Hide the accessory bar by default (remove this to show the accessory bar above the keyboard
    // for form inputs)
    if(window.cordova && window.cordova.plugins.Keyboard) {
      cordova.plugins.Keyboard.hideKeyboardAccessoryBar(true);
    }
    if(window.StatusBar) {
      StatusBar.styleDefault();
    }
  });
})

.run(function($rootScope, $cookieStore, AuthService, $state) {
  $rootScope.$on('$stateChangeStart', function(event, toState, toParams, fromState, fromParams) {
    var token = $cookieStore.get('token');

    if (toState.data && toState.data.auth) {
      event.preventDefault();

      if (!token) {
        $rootScope.$broadcast('event:auth-loginRequired');

      } else {
        AuthService
          .authorise()
          .then(function(response) {
            if (response.data.IsTokenExist && !response.data.IsTokenExpired) {
              $state.go(toState.name, toParams, { notify: false }).then(function() {
                // [ui-router known issue](https://github.com/angular-ui/ui-router/issues/1399)
                $rootScope.$broadcast('$stateChangeSuccess', toState, toParams, fromState, fromParams);
              });

            } else {
              $rootScope.$broadcast('event:auth-loginRequired');
            }
          });
      }
    }
  });

  $rootScope.$on('event:auth-loginRequired', function() {
    $state.go('app.login');
  });
})

.config(function($stateProvider, $urlRouterProvider, $httpProvider) {
  // Custom HTTP interceptor
  $httpProvider.interceptors.push('InteractiveInterceptor');

  $stateProvider
    .state('app', {
      url: '/app',
      abstract: true,
      templateUrl: 'templates/menu.html',
      controller: 'ApplicationController'
    })

    // Log incident
    .state('app.logIncident', {
      url: '/log',
      templateUrl: 'templates/log-incident.html',
      controller: 'LogIncidentController',
      resolve: {
        incidentsCount: ['Incidents', function(Incidents) {
          return Incidents.getOpenIncidentsCount();
        }],
        historyIncidentsCount: ['Incidents', function(Incidents) {
          return Incidents.getHistoryIncidentsCount();
        }]
      },
      data: {
        auth: true
      }
    })
    .state('app.logIncidentResult', {
      url: '/log/:incidentId',
      templateUrl: 'templates/log-incident-result.html',
      controller: 'ResultController',
      data: {
        auth: true
      }
    })

    // Incidents list
    .state('app.incidents', {
      url: '/view',
      templateUrl: 'templates/incidents.html',
      controller: 'IncidentsController',
      resolve: {
        incidentsCount: ['Incidents', function(Incidents) {
          return Incidents.getOpenIncidentsCount();
        }],
        historyIncidentsCount: ['Incidents', function(Incidents) {
          return Incidents.getHistoryIncidentsCount();
        }]
      },
      data: {
        auth: true
      }
    })
    .state('app.viewSingleIncident', {
      url: '/view/:incidentId',
      templateUrl: 'templates/incident.html',
      controller: 'IncidentController',
      data: {
        auth: true
      }
    })
    .state('app.history', {
      url: '/history',
      templateUrl: 'templates/incidents.html',
      controller: 'IncidentsHistoryController',
      resolve: {
        incidentsCount: ['Incidents', function(Incidents) {
          return Incidents.getOpenIncidentsCount();
        }],
        historyIncidentsCount: ['Incidents', function(Incidents) {
          return Incidents.getHistoryIncidentsCount();
        }]
      },
      data: {
        auth: true
      }
    })
    .state('app.viewSingleHistoryIncident', {
      url: '/history/:incidentId',
      templateUrl: 'templates/incident.html',
      controller: 'IncidentController',
      data: {
        auth: true
      }
    })

    // Registration
    .state('app.register', {
      url: '/register',
      templateUrl: 'templates/register.html',
      controller: 'RegisterController'
    })
    .state('app.registerDetails', {
      url: '/register/details',
      templateUrl: 'templates/register-details.html',
      controller: 'RegisterDetailsController'
    })
    .state('app.registerResult', {
      url: '/register/result',
      templateUrl: 'templates/register-result.html',
      controller: 'RegisterResultController'
    })

    // Password
    .state('app.setNewPassword', {
      url: '/set-new-password',
      templateUrl: 'templates/set-new-password.html',
      controller: 'CreatePasswordController'
    })
    .state('app.forgotPassword', {
      url: '/forgot-password',
      templateUrl: 'templates/forgot-password.html',
      controller: 'ForgotPasswordController'
    })
    .state('app.forgotPasswordError', {
      url: '/forgot-password/error',
      templateUrl: 'templates/forgot-password-error.html'
    })
    .state('app.resetPassword', {
      url: '/reset-password',
      templateUrl: 'templates/reset-password.html',
      controller: 'ResetPasswordController'
    })

    .state('app.login', {
      url: '/login',
      templateUrl: 'templates/login.html',
      controller: 'LoginController'
    })

    // If none of the above states are matched, use this as the fallback
    // https://github.com/angular-ui/ui-router/wiki/URL-Routing
    .state('otherwise', {
      url: '*path',
      template: '',
      controller: ['$state', function($state) {
        $state.go('app.incidents');
      }]
    });
})

.value('AppGlobalStates', {
  selectedIncidentsStatus: '',
  hideLogo: 0,
  bootstrap: false,
  currentIncidents: [],
  historyIncidents: []
});

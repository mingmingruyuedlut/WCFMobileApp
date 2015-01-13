angular.module('interactive.services', [])
/** 
 * @name timeago
 * @description Calculate the time distance then transform into wrods.
 */
.factory('timeago', function() {
  /** 
   * @description Parse a ISO8601 format date into a Javascript Date object.
   * @param {String} ISO8601 format date, e.g.: 20014-10-17T09:24:17Z
   */
  function parse(time) {
    var s = time.replace(/^\s*|\s*$/g, '');

    s = s.replace(/\.\d+/, '');
    s = s.replace(/-/, '/').replace(/-/, '/');
    s = s.replace(/T/, ' ').replace(/Z/,' UTC');
    s = s.replace(/([\+\-]\d\d)\:?(\d\d)/, ' $1$2');
    s = s.replace(/([\+\-]\d\d)$/, ' $100');

    return new Date(s);
  }

  /**
   * @description Calculate the time distance then transform into wrods.
   * @param {Date} Javascript date object, `new Date()`.
   * @return {String} Distance in words, e.g.: '4 months'.
   */
  function transformIntoWords(date) {
    var distance = new Date().getTime() - date.getTime(),
      seconds = Math.abs(distance) / 1000,
      minutes = seconds / 60,
      hours = minutes / 60,
      days = hours / 24,
      years = days / 365,
      strings = {
        suffix : ' ago',
        seconds: 'less than a minute',
        minute : 'about a minute',
        minutes: '%d minutes',
        hour   : 'about an hour',
        hours  : 'about %d hours',
        day    : 'a day',
        days   : '%d days',
        month  : 'about a month',
        months : '%d months',
        year   : 'about a year',
        years  : '%d years'
      };

    function substitute(template, number) {
      return template.replace(/%d/i, number) + strings.suffix;
    }

    var words = 
      seconds < 45  && substitute(strings.seconds, Math.round(seconds)) ||
      seconds < 90  && substitute(strings.minute, 1) ||
      minutes < 45  && substitute(strings.minutes, Math.round(minutes)) ||
      minutes < 90  && substitute(strings.hour, 1) ||
      hours   < 24  && substitute(strings.hours, Math.round(hours)) ||
      hours   < 42  && substitute(strings.day, 1) ||
      days    < 30  && substitute(strings.days, Math.round(days)) ||
      days    < 45  && substitute(strings.month, 1) ||
      days    < 365 && substitute(strings.months, Math.round(days / 30)) ||
      years   < 1.5 && substitute(strings.year, 1) ||
      substitute(strings.years, Math.round(years));

    return words;
  }

  return function(time) {
    if(time.date) {
      return transformIntoWords(parse(time.date));
    } else if (time.timestamp) {
      return transformIntoWords(new Date(parseInt(time.timestamp, 10)));
    } else {
      return '';
    }
  };
})

.factory('InteractiveUtils', ['AppGlobalStates', '$ionicLoading', '$ionicPopup',
  function(AppGlobalStates, $ionicLoading, $ionicPopup) {
    var Utils = {};

    Utils.showLoading = function(msg) {
      msg = msg || 'Loading...';

      $ionicLoading.show({
        template: msg
      });
    };

    Utils.hideLoading = function() {
      $ionicLoading.hide();
    };

    Utils.showAlert = function(args) {
      var title = args.title || 'Warning';

      if (args.template) {
        return $ionicPopup.alert({
          title: title,
          template: args.template
        });
      }
    };

    return Utils;
  }
])


/** 
 * @name Incidents
 * @description A data store, keep data and pull/push with backend.
 * @param {Object} Configuration object.
 * @return {DataStore} A data store instance. 
 */
.factory('Incidents', [
  '$q', '$cacheFactory', '$http', 'AppGlobalStates', 
  function($q, $cacheFactory, $http, AppGlobalStates) {
    var Incidents = {},
      cache = $cacheFactory('interactive'),
      fetchedRemote = false,
      fetchedRemoteHistory = false,
      fetchedIncidentsCount = false,
      fetchedHistoryIncidentsCount = false;

    Incidents.query = function() {
      return _fetch({
        url: '/Services/WCFService/OperateIncidentWCFService.svc/v1/getopenincidents',
        cacheId: 'incidents'
      });
    };

    Incidents.queryHistory = function() {
      return _fetch({
        url: '/Services/WCFService/OperateIncidentWCFService.svc/v1/gethistoryincidents',
        cacheId: 'incidentsHistory',
        isHistory: true
      });
    };

    Incidents.add = function(args) {
      var severityMapping = {
        'P0': {
          label: 'Not sure'
        },
        'P1': {
          label: 'P1 - Urgent'
        },
        'P2': {
          label: 'P2 - Normal'
        },
        'P3': {
          label: 'P3 - Minor'
        },
        'P4': {
          label: 'P4 - Low'
        }
      };

      var newCase = {
        'Description': 'A nice technican will contact you shortly',
        'Status': 'Open',
        'Severity': {
          'Label': args.severity.label,
          'Value': args.severity.value
        },
        'Technician': {
          'Avatar': 'technician',
          'Id': '1',
          'Name': 'Gennady Vaisman',
          'Rating': 5,
          'Title': 'SSE'
        },
        'TimeStamp': new Date().getTime()
      };

      return $http.post('/Services/WCFService/OperateIncidentWCFService.svc/v1/createincident', newCase)
        .then(function(response) {
          var incident = response.data;

          if (!cache.get('incidents')) {
            cache.put('incidents', []);
          }

          cache.get('incidents').unshift(incident);
          AppGlobalStates.incidentsCount += 1;

          return incident.IdInFP;
        });
    };

    Incidents.get = function(IdInFP) {
      var deferred = $q.defer(),
        _incident;

      if (cache.get('incidents')) {
        angular.forEach(cache.get('incidents'), function(incident) {
          if (incident.IdInFP == IdInFP) {
            _incident = incident;
          }
        });

        if (_incident) {
          deferred.resolve(_incident);
          return deferred.promise;
        }
      }

      return $http.post('/Services/WCFService/OperateIncidentWCFService.svc/v1/incidentdetail/' + IdInFP)
        .then(function(response) {
           cache.get('incidents').push(response.data);

           return response.data;
        });
    };

    Incidents.share = function(args) {
      return $http.post('/Services/WCFService/OperateIncidentWCFService.svc/v1/shareincident/' + args.id, args.coworkers);
    };

    Incidents.getSharedUsers = function(incidentId) {
      return $http.post('/Services/WCFService/OperateIncidentWCFService.svc/v1/getsharedusers/' + incidentId);
    };

    Incidents.getOpenIncidentsCount = function() {
      if (!fetchedIncidentsCount) {
        return $http.post('/Services/WCFService/OperateIncidentWCFService.svc/v1/getopenincidentscount')
          .then(function(response) {
            AppGlobalStates.incidentsCount = parseInt(response.data, 10);
            fetchedIncidentsCount = true;
          });

      } else {
        return AppGlobalStates.incidentsCount;
      }
    };

    Incidents.getHistoryIncidentsCount = function() {
      if (!fetchedHistoryIncidentsCount) {
        return $http.post('/Services/WCFService/OperateIncidentWCFService.svc/v1/gethistoryincidentscount')
          .then(function(response) {
            AppGlobalStates.historyIncidentsCount = parseInt(response.data, 10);
            fetchedHistoryIncidentsCount = true;
          });

      } else {
        return AppGlobalStates.historyIncidentsCount;
      }
    }

    Incidents.clear  = function() {
      cache.remove('incidents');
      cache.remove('incidentsHistory');
      fetchedRemote = false;
      fetchedRemoteHistory = false;
      fetchedIncidentsCount = false;
      fetchedHistoryIncidentsCount = false;
    };

    function _fetch(args) {
      var deferred;

      if ((!args.isHistory && !fetchedRemote) || (args.isHistory && !fetchedRemoteHistory)) {
        return $http.post(args.url)
          .then(function(response) {
            cache.put(args.cacheId, response.data);
            if(args.isHistory) {
              fetchedRemoteHistory = true;
            } else {
              fetchedRemote = true;
            }

            return response.data;
          });

      } else {
        deferred = $q.defer();
        deferred.resolve(cache.get(args.cacheId));

        return deferred.promise;
      }
    }

    return Incidents;
  }
])

.factory('AuthService', [
  '$rootScope', '$http', '$cookieStore', '$q', 
  function($rootScope, $http, $cookieStore, $q) {
    return {
      authorise: function(token) {
        return $http.post('/Services/WCFService/RegistrationWCFService.svc/v1/checktoken', null, {
          'headers': {
            'Authorization': token ? 'Bearer ' + token : ''
          }
        });
      },

      registerCheck: function(args) {
        return $http.post('/Services/WCFService/RegistrationWCFService.svc/v1/checkuser', { 'Email': args });
      },

      register: function(args) {
        return $http.post('/Services/WCFService/OperateIncidentWCFService.svc/v1/registuser', args);
      },

      setPassword: function(args) {
        return $http.post('/Services/WCFService/RegistrationWCFService.svc/v1/createpassword', {
          'Password': args.Password
        }, {
          'headers': {
            'Authorization': args.token ? 'Bearer ' + args.token : ''
          }
        });
      },

      forgotPassword: function(args) {
        return $http.post('/Services/WCFService/RegistrationWCFService.svc/v1/forgetpassword', args);
      },

      resetPassword: function(args) {
        return $http.post('/Services/WCFService/RegistrationWCFService.svc/v1/updatepassword', {
          'Password': args.Password
        }, {
          'headers': {
            'Authorization': args.token ? 'Bearer ' + args.token : ''
          }
        });
      },

      getCoworkers: function(incidentId) {
        return $http.post('/Services/WCFService/OperateIncidentWCFService.svc/v1/getuserstoshare/' + incidentId);
      },

      getUserDetail: function() {
        return $http.post('/Services/WCFService/RegistrationWCFService.svc/v1/getuserdetail');
      },

      login: function(args) {
        return $http.post('/Services/WCFService/LoginWCFService.svc/v1/login', args)
          .then(function(response) {
            if (response.data.IsEmailExist && response.data.IsPwdValid) {
              $cookieStore.put('token', response.data.Token);
              $cookieStore.put('contactName', response.data.ContactName);
              $cookieStore.put('contactNumber', response.data.ContactNumber);

            } else if (!response.data.IsEmailExist) {
              return $q.reject('Email not exist.');

            } else if (response.data.IsEmailExist && !response.data.IsPwdValid) {
              return $q.reject('Incorrect password.');
            }
          });
      },

      logout: function() {
        return $http.post('/Services/WCFService/LoginWCFService.svc/v1/logout');
      }
    };
  }
])

.factory('InteractiveInterceptor', [
  '$rootScope', '$q', '$cookieStore', '$injector', 
  function($rootScope, $q, $cookieStore, $injector) {
    return {
      request: function(config) {
        var $state = $injector.get('$state');

        config.headers = config.headers || {};
        config.toState = $state.current;
        config.toParams = $state.params;

        if ($cookieStore.get('token') && !config.headers.Authorization) {
          config.headers.Authorization = 'Bearer ' + $cookieStore.get('token');
        }

        return config;
      },

      /**
       * $http interceptor.
       * On 401 response (without 'ignoreAuthModule' option) stores the request
       * and broadcasts 'event:auth-loginRequired'.
       */
      responseError: function(rejection) {
        var $state = $injector.get('$state');

        if (!rejection.config.ignoreAuthModule) {
          switch (rejection.status) {
            case 401:
              var deferred = $q.defer();
              httpBuffer.append(rejection.config, deferred);
              if ($state.current.name == 'app.setNewPassword' || $state.current.name == 'app.resetPassword') {
                $rootScope.$broadcast('event:auth-setPasswordTokenInvalid', rejection);

              } else {
                $rootScope.$broadcast('event:auth-loginRequired');
              }
              return deferred.promise;
          }
        }
        // otherwise, default behaviour
        return $q.reject(rejection);
      }
    };
}])

.factory('User', function() {
  var profile = {};

  return {
    get: function() {
      return profile;
    },
    set: function(data) {
      return angular.extend(profile, data);
    },
    remove: function() {
      profile = {};
    }
  };
})

.factory('interactiveSplash', [
  '$document', '$timeout',
  function($document, $timeout) {
    var el = angular.element('<div class="splash active"></div>');
    var backdropHolds = 0;

    $document[0].body.appendChild(el[0]);

    return {
      retain: retain,
      release: release
    };

    function retain() {
      if ((++backdropHolds) === 1) {
        el.addClass('visible');
        ionic.requestAnimationFrame(function() {
          backdropHolds && el.addClass('active');
        });
      }
    }

    function release() {
      if ((--backdropHolds) === 0) {
        el.removeClass('active');
        $timeout(function() {
          !backdropHolds && el.removeClass('visible');
        }, 400, false);
      }
    }
  }
]);
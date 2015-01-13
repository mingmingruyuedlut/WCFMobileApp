angular.module('interactive.controllers', [])

.controller('ApplicationController', [
  '$scope', 'AppGlobalStates', '$ionicPopup', 'InteractiveUtils', 'AuthService', '$cookieStore', 'Incidents', '$state', 
  function($scope, AppGlobalStates, $ionicPopup, InteractiveUtils, AuthService, $cookieStore, Incidents, $state) {
    AppGlobalStates.hideLogo = 0;
    $scope.data = {
      AppGlobalStates: AppGlobalStates,
      $state: $state
    };

    $scope.logout = function() {
      $ionicPopup.confirm({
        title: 'Log out',
        template: 'Are you sure you want to log out?'

      }).then(function(res) {
        if (res) {
          InteractiveUtils.showLoading('Logging out...');
          AuthService
            .logout()
            .then(function() {
              InteractiveUtils.hideLoading();
              Incidents.clear();
              $cookieStore.remove('token');
              $cookieStore.remove('contactName');
              $cookieStore.remove('contactNumber');
              $state.go('app.login');
            });
        }
      });
    };
  }
])

.controller('LogIncidentController', [
  '$scope', '$state', 'Incidents', 'AppGlobalStates', 'InteractiveUtils', 'AuthService', '$cookieStore', 
  function($scope, $state, Incidents, AppGlobalStates, InteractiveUtils, AuthService, $cookieStore) {
    AppGlobalStates.hideLogo = 1;
    $scope.data = {
      contactName: $cookieStore.get('contactName'),
      contactNumber: $cookieStore.get('contactNumber'),
      logIncidentForm: {},
      severityOptions: [{
        label: 'Not sure',
        value: 'P0'
      }, {
        label: 'P1 - Urgent',
        value: 'P1'
      }, {
        label: 'P2 - Normal',
        value: 'P2'
      }, {
        label: 'P3 - Minor',
        value: 'P3'
      }, {
        label: 'P4 - Low',
        value: 'P4'
      }]
    };

    $scope.data.severity = $scope.data.severityOptions[0];

    $scope.doCancel = function() {
      if ($state.previous && $state.previous.name) {
        $state.go($state.previous.name);

      } else {
        $state.go('app.incidents');
      }
    };

    $scope.doSubmit = function() {
      InteractiveUtils.showLoading('Sending...');

      if(!$scope.data.logIncidentForm.$error.required) {
        Incidents.add({
          severity: $scope.data.severity

        }).then(function(IdInFP) {
          InteractiveUtils.hideLoading();
          $state.go('app.logIncidentResult', {
            incidentId: IdInFP
          });
        });

      } else {
        InteractiveUtils.hideLoading();
        InteractiveUtils.showAlert({
          title: 'Error',
          template: 'Input all required fields!'
        });
      }
    };
  }
])

.controller('ResultController', [
  '$scope', '$stateParams', 'Incidents', 'AppGlobalStates', 
  function($scope, $stateParams, Incidents, AppGlobalStates) {
    AppGlobalStates.hideLogo = 1;

    Incidents.get($stateParams.incidentId)
      .then(function(data) {
        $scope.incident = data;
      });
  }
])

.controller('IncidentsController', [
  '$scope', '$state', 'Incidents', 'AppGlobalStates', 'InteractiveUtils', 
  function($scope, $state, Incidents, AppGlobalStates, InteractiveUtils) {
    AppGlobalStates.hideLogo = 1;
    $scope.data = {
      title: 'Current Incidents',
      noIncidentMsg: 'You currently have no open incidents, please click the button below to create an incident'
    };

    InteractiveUtils.showLoading();

    Incidents
      .query()
      .then(function(incidents) {
        $scope.incidents = incidents;
        AppGlobalStates.currentIncidents = incidents;
        InteractiveUtils.hideLoading();

      }, function() {
        InteractiveUtils.hideLoading();
        InteractiveUtils.showAlert({
          title: 'Error',
          template: 'Error occured.'
        });
      });

    $scope.gotoIncident = function(incident) {
      $state.go('app.viewSingleIncident', {
        incidentId: incident.Id
      });
    };

    $scope.logIncident = function() {
      $state.go('app.logIncident');
    };
  }
])

.controller('IncidentsHistoryController', [
  '$scope', '$state', 'Incidents', 'AppGlobalStates', 'InteractiveUtils', 
  function($scope, $state, Incidents, AppGlobalStates, InteractiveUtils) {
    AppGlobalStates.hideLogo = 1;
    $scope.data = {
      isViewHistory: 1,
      title: 'View History',
      noIncidentMsg : 'There are currently no viewable incidents, this may be because you have not created an incident, or an incident has not been created in the last 30 days.'
    };

    InteractiveUtils.showLoading();

    Incidents
      .queryHistory()
      .then(function(incidents) {
        $scope.incidents = incidents;
        AppGlobalStates.historyIncidents = incidents;
        InteractiveUtils.hideLoading();

      }, function() {
        InteractiveUtils.hideLoading();
        InteractiveUtils.showAlert({
          title: 'Error',
          template: 'Error occured.'
        });
      });

    $scope.gotoIncident = function(incident) {
      $state.go('app.viewSingleHistoryIncident', {
        incidentId: incident.Id
      });
    };

    $scope.logIncident = function() {
      $state.go('app.logIncident');
    };
  }
])

.controller('IncidentController', [
  '$scope', '$stateParams', '$ionicSlideBoxDelegate', 'Incidents', '$timeout', 'AppGlobalStates', 'InteractiveUtils', '$ionicPopup', 'AuthService', '$state', 
  function($scope, $stateParams, $ionicSlideBoxDelegate, Incidents, $timeout, AppGlobalStates, InteractiveUtils, $ionicPopup, AuthService, $state) {
    var promise;

    AppGlobalStates.hideLogo = 1;
    $scope.data = {
      AppGlobalStates: AppGlobalStates,
      InteractiveUtils: InteractiveUtils,
      currentIndex: 0,
      isShared: false,
      isHistory: false
    };

    InteractiveUtils.showLoading();

    if ($state.current.name == 'app.viewSingleIncident') {
      promise = Incidents.query();

    } else if ($state.current.name == 'app.viewSingleHistoryIncident') {
      promise = Incidents.queryHistory();
      $scope.data.isHistory = true;
    }

    promise.then(function(incidents) {
      var idx = 0,
        count;

      $scope.incidents = incidents;
      $ionicSlideBoxDelegate.update();
      InteractiveUtils.hideLoading();

      if (angular.isArray(incidents)) {
          count = incidents.length;

          if (count === 0) {
            return;
          }

          for (var i = 0; i < count; i++) {
            if (incidents[i].Id == $stateParams.incidentId) {
              $scope.incident = incidents[i];
              idx = i;
            }
          }

          $scope.data.currentIndex = idx + 1;
          $ionicSlideBoxDelegate.update();
          $timeout(function() {
            $ionicSlideBoxDelegate.slide(idx);
          }, 0);
        }
    });

    $scope.onSlideChange = function(index) {
      $scope.data.currentIndex = index + 1;
      $scope.incident = $scope.incidents[$scope.data.currentIndex - 1];
    };

    $scope.showSharePopup = function() {
      AuthService.getCoworkers($scope.incident.Id)
        .then(function(response) {
          $scope.data.coworkers = response.data;

          $ionicPopup.show({
            templateUrl: './templates/share-incident.html',
            title: 'Share to',
            cssClass: 'popup-share-incident',
            scope: $scope,
            buttons: [
              { text: 'Cancel' },
              {
                text: 'Share',
                type: 'button-position',
                onTap: function() {
                  Incidents.share({
                    id: $scope.incident.Id,
                    coworkers: $scope.data.coworkers

                  }).then(function() {
                    var count = 0;
                    angular.forEach($scope.data.coworkers, function(coworker) {
                      if (coworker.IsShared) {
                        count += 1;
                      }
                    });
                    $scope.incident.ShareCount = count;

                  }, function() {
                    InteractiveUtils.showAlert({
                      title: 'Error',
                      template: 'Error occured.'
                    });
                  });
                }
              }
            ]
          });
        });
    };

    $scope.getSharedUsers = function() {
      if ($scope.incident.ShareCount < 1) {
        return;
      }

      var incidentId = $scope.incident.Id;
      InteractiveUtils.showLoading();
      Incidents.getSharedUsers(incidentId)
        .then(function(response) {
          InteractiveUtils.hideLoading();
          $scope.data.sharedUsers = response.data;
          $ionicPopup.show({
            templateUrl: './templates/shared-users.html',
            title: 'Shared with:',
            cssClass: 'popup-share-incident',
            scope: $scope,
            buttons: [{
              text: 'Done'
            }]
          });

        }, function() {
          InteractiveUtils.hideLoading();
          InteractiveUtils.showAlert({
            title: 'Error',
            template: 'Error occured. Please try again.'
          });
        });
    };
  }
])

.controller('RegisterController', [
  '$scope', 'AuthService', 'User', 'InteractiveUtils', '$state', 'AppGlobalStates', 
  function($scope, AuthService, User, InteractiveUtils, $state, AppGlobalStates) {
    AppGlobalStates.hideLogo = 0;
    $scope.user = {
      email: ''
    };

    $scope.doSubmit = function() {
      if ($scope.user.email) {
        User.set({
          email: $scope.user.email
        });

        InteractiveUtils.showLoading('Sending...');
        AuthService
          .registerCheck($scope.user.email)
          .then(function(response) {
            InteractiveUtils.hideLoading();
            if (response.data.IsWaitingFPResponse) {
              InteractiveUtils.showAlert({
                title: 'Warning',
                template: 'Your email has already been registered(FP). Please wait.'
              }).then(function() {
                $state.go('app.login');
              });

            } else {
              if (response.data.IsExistInFP) {
                $scope.user.email = '';

                if (response.data.IsExistInMDB) {
                  InteractiveUtils.showAlert({
                    title: 'Warning',
                    template: 'Your email has already been registered.'
                  }).then(function() {
                    $state.go('app.login');
                  });

                } else {
                  $state.go('app.registerResult');
                }

              } else {
                $state.go('app.registerDetails');
              }
            }

          }, function() {
            InteractiveUtils.hideLoading();
            InteractiveUtils.showAlert({
              title: 'Error',
              template: 'Error occured. Please try again.'
            });
          });

      } else {
        InteractiveUtils.showAlert({
          title: 'Error',
          template: 'Please enter a valid username.'
        });
      }
    };

    $scope.goLogin = function() {
      $state.go('app.login');
    };
  }
])

.controller('RegisterDetailsController', [
  '$scope', 'User', 'AuthService', 'InteractiveUtils', '$state', 'AppGlobalStates', 
  function($scope, User, AuthService, InteractiveUtils, $state, AppGlobalStates) {
    AppGlobalStates.hideLogo = 0;
    $scope.data = {
      user: User.get()
    };

    $scope.goBack = function() {
      $state.go('app.register');
    };

    $scope.doSubmit = function() {
      if ($scope.data.registrationForm.$valid) {
        InteractiveUtils.showLoading('Sending...');
        AuthService
          .register({
            'Email': $scope.data.user.email,
            'Name' : $scope.data.user.name,
            'Phone': $scope.data.user.phone,
            'Customer' : $scope.data.user.customer
          })
          .then(function() {
            InteractiveUtils.hideLoading();
            $scope.data.user.email = '';
            $scope.data.user.name = '';
            $scope.data.user.phone = '';
            $scope.data.user.customer = '';
            $state.go('app.registerResult');

          }, function() {
            InteractiveUtils.hideLoading();
            InteractiveUtils.showAlert({
              title: 'Error',
              template: 'Error occured. Please try again.'
            });
          });

      } else {
        InteractiveUtils.showAlert({
          title: 'Error',
          template: 'Please input all the required fields.'
        });
      }
    };
  }
])

.controller('CreatePasswordController', [
  '$scope', '$location', '$cookieStore', 'AuthService', 'InteractiveUtils', '$state', 'AppGlobalStates', 
  function($scope, $location, $cookieStore, AuthService, InteractiveUtils, $state, AppGlobalStates) {
    var token = $location.search().token;

    AppGlobalStates.hideLogo = 0;
    $scope.data = {};

    if (token) {
      InteractiveUtils.showLoading();
      AuthService
        .authorise(token)
        .then(function(response) {
          InteractiveUtils.hideLoading();
          if (!response.data.IsTokenExist) {
            InteractiveUtils.showAlert({
              title: 'Error',
              template: 'Invalid token.'
            }).then(function() {
              $state.go('app.register');
            });

          } else if (response.data.IsTokenExist && response.data.IsTokenExpired) {
            InteractiveUtils.showAlert({
              title: 'Error',
              template: 'Token expired.'

            }).then(function() {
              $state.go('app.register');
            });

          } else {
            $scope.data.email = response.data.Email;
          }

        }, function() {
          InteractiveUtils.hideLoading();
          InteractiveUtils.showAlert({
            title: 'Error',
            template: 'Error occured.'
          });
        });
    }

    $scope.doSubmit = function() {
      if ($scope.data.password && $scope.data.password == $scope.data.passwordConfirm) {
        InteractiveUtils.showLoading();
        AuthService
          .setPassword({
            'Password': PBKDF2($scope.data.password, 'FABRIC.COM.SALT', 128 / 8, { iterations: 1000 }),
            'token': token
          })
          .then(function() {
            InteractiveUtils.hideLoading();
            $scope.data.password = '';
            $scope.data.passwordConfirm = '';
            $state.go('app.registerResult');

          }, function() {
            InteractiveUtils.hideLoading();
            InteractiveUtils.showAlert({
              title: 'Error',
              template: 'Error occured.'
            });
          });

      } else {
        InteractiveUtils.showAlert({
          title: 'Error',
          template: 'Please check your password.'
        });
      }
    };

    $scope.$on('event:auth-setPasswordTokenInvalid', function() {
      InteractiveUtils.hideLoading();
      InteractiveUtils.showAlert({
        title: 'Expired',
        template: 'Please register again.'
      });
      $state.go('app.register');
    });
}])

.controller('ForgotPasswordController', [
  '$scope', 'AuthService', 'InteractiveUtils', '$state', 'AppGlobalStates', 
  function($scope, AuthService, InteractiveUtils, $state, AppGlobalStates) {
    AppGlobalStates.hideLogo = 0;
    $scope.data = {};

    $scope.doSubmit = function() {
      if ($scope.data.resetPasswordForm.$valid) {
        InteractiveUtils.showLoading();
        AuthService
          .forgotPassword({
            'Email': $scope.data.email
          })
          .then(function(response) {
            InteractiveUtils.hideLoading();
            if (!response.data.IsExistInMDB) {
              $state.go('app.forgotPasswordError');

            } else {
              InteractiveUtils.showAlert({
                title: 'Success',
                template: 'An email will be sent to you. Please check your emails.'

              }).then(function() {
                $state.go('app.login');
              });
            }

          }, function() {
            InteractiveUtils.hideLoading();
            InteractiveUtils.showAlert({
              title: 'Error',
              template: 'Error occured. Please try again.'
            });
          });
      }
    };

    $scope.goBack = function() {
      $state.go('app.login');
    };
  }
])

.controller('ResetPasswordController', [
  '$scope', 'AuthService', 'InteractiveUtils', '$location', '$cookieStore', '$state', 'AppGlobalStates', 
  function($scope, AuthService, InteractiveUtils, $location, $cookieStore, $state, AppGlobalStates) {
    var token = $location.search().token;

    AppGlobalStates.hideLogo = 0;
    $scope.data = {};

    if (token) {
      InteractiveUtils.showLoading();
      AuthService
        .authorise(token)
        .then(function(response) {
          InteractiveUtils.hideLoading();
          if (!response.data.IsTokenExist) {
            InteractiveUtils.showAlert({
              title: 'Error',
              template: 'Invalid token.'
            }).then(function() {
              $state.go('app.resetPassword');
            });

          } else if (response.data.IsTokenExist && response.data.IsTokenExpired) {
            InteractiveUtils.showAlert({
              title: 'Error',
              template: 'Token expired.'

            }).then(function() {
              $state.go('app.resetPassword');
            });

          } else {
            $scope.data.email = response.data.Email;
          }

        }, function() {
          InteractiveUtils.hideLoading();
          InteractiveUtils.showAlert({
            title: 'Error',
            template: 'Error occured.'
          });
        });
    }

    $scope.doSubmit = function() {
      if ($scope.data.password !== '' && $scope.data.password === $scope.data.passwordConfirm) {
        InteractiveUtils.showLoading();
        AuthService
          .resetPassword({
            'Password': PBKDF2($scope.data.password, 'FABRIC.COM.SALT', 128 / 8, { iterations: 1000 }),
            'token': token
          })
          .then(function() {
            InteractiveUtils.hideLoading();
            $scope.data.password = '';
            $scope.data.passwordConfirm = '';
            $state.go('app.login');

          }, function() {
            InteractiveUtils.hideLoading();
            InteractiveUtils.showAlert({
              title: 'Error',
              template: 'Error occured.'
            });
          });

      } else {
        InteractiveUtils.showAlert({
          title: 'Error',
          template: 'Please check your password.'
        });
      }
    };

    $scope.$on('event:auth-setPasswordTokenInvalid', function() {
      InteractiveUtils.hideLoading();
      InteractiveUtils.showAlert({
        title: 'Expired',
        template: 'Please register again.'
      });
      $state.go('app.register');
    });
  }
])

.controller('RegisterResultController', [
  '$scope', '$state', 'AppGlobalStates', 
  function($scope, $state, AppGlobalStates) {
    AppGlobalStates.hideLogo = 0;
    $scope.data = {
      fromRegisterCheck: false,
      fromRegisterDetails: false,
      fromSetPassword: false,
      backState: 'app.login',
      buttonText: 'Login'
    };

    if ($state.previous) {
      if ($state.previous.name == 'app.register') {
        $scope.data.fromRegisterCheck = true;
        $scope.data.fromRegisterDetails = false;
        $scope.data.fromSetPassword = false;
      } else if ($state.previous.name == 'app.registerDetails') {
        $scope.data.fromRegisterCheck = false;
        $scope.data.fromRegisterDetails = true;
        $scope.data.fromSetPassword = false;
      } else if ($state.previous.name == 'app.setNewPassword') {
        $scope.data.fromRegisterCheck = false;
        $scope.data.fromRegisterDetails = false;
        $scope.data.fromSetPassword = true;
      } else {
        $scope.data.fromRegisterCheck = true;
        $scope.data.fromRegisterDetails = false;
        $scope.data.fromSetPassword = false;
      }
    } else {
      $scope.data.fromRegisterCheck = true;
      $scope.data.fromRegisterDetails = false;
    }

    $scope.goBack = function() {
      $state.go($scope.data.backState);
    };
  }
])

.controller('LoginController', [
  '$scope', 'AppGlobalStates', 'AuthService', 'InteractiveUtils', '$state', '$timeout', 'interactiveSplash', 
  function($scope, AppGlobalStates, AuthService, InteractiveUtils, $state, $timeout, interactiveSplash) {
    AppGlobalStates.hideLogo = 0;
    $scope.data = {};

    if (!AppGlobalStates.bootstrap && !$state.previous) {
      interactiveSplash.retain();
      $timeout(function() {
        AppGlobalStates.bootstrap = true;
        interactiveSplash.release();
      }, 2000);
    }

    $scope.doLogin = function() {
      if ($scope.data.loginForm.$valid) {
        InteractiveUtils.showLoading('Waiting...');
        AuthService
          .login({
            'Email': $scope.data.email,
            'Password': PBKDF2($scope.data.password, 'FABRIC.COM.SALT', 128 / 8, { iterations: 1000 })
          })
          .then(function() {
            InteractiveUtils.hideLoading();
            $state.go('app.incidents');

          }, function(rejection) {
            InteractiveUtils.hideLoading();
            if (angular.isString(rejection)) {
              InteractiveUtils.showAlert({
                title: 'Error',
                template: 'Incorrect password'
              });

            } else {
              InteractiveUtils.showAlert({
                title: 'Error',
                template: 'Error occured.'
              });
            }
          });

      } else {
        InteractiveUtils.showAlert({
          title: 'Error',
          template: 'Please input all the required fields.'
        });
      }
    };

    $scope.doRegister = function() {
      $state.go('app.register');
    };
}]);

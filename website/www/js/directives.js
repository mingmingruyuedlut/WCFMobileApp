angular.module('interactive.directives', [])

.directive('interactiveTimeago', ['timeago', function(timeago) {
  return {
    restrict: 'EA',
    template: '<span>{{timeago}}</span>',
    replace: true,
    scope: {
      timestamp: '@',  // e.g., 1403429923163
      date: '@'  //  e.g., 2014-10-17T09:24:17Z
    },
    link: function($scope) {
      $scope.$watch('timestamp', function() {
        $scope.timeago = timeago({
          timestamp: $scope.timestamp,
          date: $scope.date
        });
      });
    }
  };
}])

.directive('interactiveNavBackButton', [
  '$state', '$rootScope', 
  function($state, $rootScope) {
    $rootScope.$on('$stateChangeSuccess', function(event, toState, toParams, fromState) {
      $state.previous = fromState;
    });

    return {
      restrict: 'E',
      require: '^ionNavBar',
      compile: function(tElement) {
        tElement.addClass('button back-button ng-hide');

        return function($scope, $element) {
          $scope.$watch(function() {
            return $state.current.name;

          }, function(currentState) {
            if (currentState == 'app.viewSingleIncident') {
              $element.removeClass('ng-hide');

              $element.one('click', function() {
                $state.go('app.incidents');
              });

            } else if (currentState == 'app.viewSingleHistoryIncident') {
              $element.removeClass('ng-hide');

              $element.one('click', function() {
                $state.go('app.history');
              });

            } else {
              $element.addClass('ng-hide');
            }
          });
        };
      }
    };
  }
]);

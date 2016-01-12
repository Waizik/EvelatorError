'use strict';
// zde se definuje presmerovani

// zase definice modulu (default v app.js)
angular.module('default')
        .config(['$routeProvider', function($routeProvider) {
            /*
            $routeProvider.when('/:controller/:action/:actionId', {
                    templateUrl: function(params) {
                        return 'modules/views/' + params.controller + '/' + params.action + '.html';
                    },
                    controller: 'IndexController'
                });
                $routeProvider.when('/:controller/:action', {
                    templateUrl: function(params) {
                        return 'modules/views/' + params.controller + '/' + params.action + '.html';
                    },
                    controller: 'IndexController'
                });
                $routeProvider.when('/:controller', {
                    redirectTo: function(params) {
                        return '/' + params.controller + '/';
                    }
                });
                */
                $routeProvider.when('/', {
                    templateUrl: function(params) {
                        return 'view.html';
                    },
                    controller: 'ViewController'
                });
                $routeProvider.otherwise({
                    templateUrl: function(params) {
                        return 'view.html';
                    },
                    controller: 'ViewController'
                });
            
                //$routeProvider.otherwise({redirectTo: 'index'});
            }]);
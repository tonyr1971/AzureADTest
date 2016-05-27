/// <reference path="lib/angular/angular.js" />

var locationApp = angular.module('locationApp', ['AdalAngular']);

locationApp.config(['$httpProvider', 'adalAuthenticationServiceProvider',
    function ($httpProvider, adalProvider) {
        var endpoints = {
            'https://localhost:44302/': 'http://tabsteamoutlook.onmicrosoft.com/TestApp'
        };

        adalProvider.init({
            instance: 'https://login.microsoftonline.com/',
            tenant: 'tabsteamoutlook.onmicrosoft.com',
            clientId: '86e8eb64-97bc-41b8-bc61-0dd72a213df2',
            endpoints: endpoints
        }, $httpProvider)
    }]);

var locationController = locationApp.controller("locationController", ['$scope', '$http', 'adalAuthenticationService',
    function ($scope, $http, adalService) {
        $scope.getLocation = function () {
            $http.get("https://localhost:44302/api/location?cityName=Omaha").success(function (location) {
                $scope.city = location;
            });
        }

        $scope.logout = function () {
            adalService.logOut();
        }

        $scope.login = function () {
            adalService.login();
        }
    }]);

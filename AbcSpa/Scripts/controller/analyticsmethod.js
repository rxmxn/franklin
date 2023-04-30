"use strict";

angular.module("abcadmin")
    .controller("AnalyticsMethodController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller',
        function ($scope, $route, $location, $http, $cookies, $controller) {
            
            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------
            
            var filters = function (page)
            {
                return {
                    page: (page === null || page === undefined) ? $scope.currentPage : page,
                    pageSize: $scope.pageSize,
                    searchGeneral: $scope.searchGeneral,
                	alta: $scope.alta,
                	baja: $scope.baja
                }
            };


            var setActivationObjects = function (eObj, act) {
                return { id: eObj.Id, active: act }
            };
			
            var saveObject = function (promise) {
                var analyticsMethodObj = {
                    Id: promise.Id,
                    Name: promise.Name,
                    Active: promise.Active,
                    Description: promise.Description
                };
                return { analyticsMethod: analyticsMethodObj }
            }

            var editElement = function (element) {
                return {
                    template: "editDialog",
                    controller: "NewAnalyticsMethodController",
                    className: "ngdialog-theme-default",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }

            $scope.mainFunction(
                "icon mif-calculator2", "Name",
                "/analyticsmethod/refreshlist", filters,
                "/analyticsmethod/saveactivestatus", setActivationObjects,
                "/analyticsmethod/saveanalyticsmethod", saveObject,
                editElement,
				"analyticsmethod"	// to get the access level
                );

            // AnalyticsMethod specifics
            $scope.SearchGeneral = true;
            $scope.FilterActive = true;
            $scope.boxHeaderTitleActive = "Técnicas Analíticas activas";
            $scope.boxHeaderTitleDeactive = "Técnicas Analíticas de baja";

            $scope.panels = [
                {
                    index: 0,
                    name: 'Filtros',
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: 'Tabla de Técnicas Analíticas',
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewAnalyticsMethodController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog",
        function($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog) {
            
            var errorDelay = 5000;

            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nueva técnica analítica" : "Editar técnica analítica " + $scope.ngDialogData.dData.Name;

            // TODO: modificar para que ademas de mostrar la notificacion ocurra alguna sennalizacion en el input
            $scope.acceptDialog = function () {
                var error = false;

                if ($scope.ngDialogData.dData.Name === undefined) {
                    notification.error({
                        message: "Nombre de la técnica analítica indefinido",
                        delay: errorDelay
                    });
                    error = true;
                }
                if (error === true)
                    return;

                $scope.confirm({
                    Id: $scope.ngDialogData.dData.Id != null ?
                        $scope.ngDialogData.dData.Id : 0,
                    Name: $scope.ngDialogData.dData.Name,
                    Active: $scope.ngDialogData.dData.Active,
                    Description: $scope.ngDialogData.dData.Description
                });

            }
        }
        
    ])
;
"use strict";

angular.module("abcadmin")
    .controller("StatusController",
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
                var statusObj = {
                    Id: promise.Id,
                    Tipo: promise.Tipo,
                    Active: promise.Active,
                    Description: promise.Description
                };
                return { status: statusObj }
            }

            var editElement = function (element) {
                return {
                    template: "editDialog",
                    controller: "NewStatusController",
                    className: "ngdialog-theme-default",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }

            $scope.mainFunction(
                "icon mif-bookmarks", "Name",
                "/status/refreshlist", filters,
                "/status/saveactivestatus", setActivationObjects,
                "/status/savestatus", saveObject,
                editElement,
				"status"	// to get the access level
                );

            // Status specifics
            $scope.SearchGeneral = true;
            $scope.FilterActive = true;
            // $scope.boxHeaderTitleActive = "Status activos";
            // $scope.boxHeaderTitleDeactive = "Status de baja";
			
            $scope.panels = [
                {
                    index: 0,
                    name: 'Filtros',
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: 'Tabla de Status',
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewStatusController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog",
        function($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog) {
            
            var errorDelay = 5000;

            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nuevo Status" : "Editar Status" + $scope.ngDialogData.dData.Tipo;

            $scope.acceptDialog = function () {
                var error = false;

                if ($scope.ngDialogData.dData.Tipo === undefined) {
                    notification.error({
                        message: "Nombre del Status indefinido",
                        delay: errorDelay
                    });
                    error = true;
                }
                if (error === true)
                    return;

                $scope.confirm({
                    Id: $scope.ngDialogData.dData.Id != null ?
                        $scope.ngDialogData.dData.Id : 0,
                    Tipo: $scope.ngDialogData.dData.Tipo,
                    Active: $scope.ngDialogData.dData.Active,
                    Description: $scope.ngDialogData.dData.Description
                });

            }
        }
        
    ])
;
"use strict";

angular.module("abcadmin")
    .controller("CentroCostoController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller',
        function($scope, $route, $location, $http, $cookies, $controller) {
            
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
                var centrocostoObj = {
                    Id: promise.Id,
                    Number: promise.Number,
                    Active: promise.Active,
                    Description: promise.Description,
                    Tipo: promise.Tipo
                };
                return { centrocosto: centrocostoObj }
            }

            var editElement = function (element) {
                return {
                    template: "editDialog",
                    controller: "NewCentroCostoController",
                    className: "ngdialog-theme-default",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }

            $scope.mainFunction(
                "icon mif-creative-commons", "Number",
                "/centrocosto/refreshlist", filters,
                "/centrocosto/saveactivestatus", setActivationObjects,
                "/centrocosto/savecentrocosto", saveObject,
                editElement,
				"analyticsarea"
                );

            // CentroCosto specifics
            $scope.SearchGeneral = true;
            $scope.FilterActive = true;
            //$scope.boxHeaderTitleActive = "Centros de Costo activos";
            //$scope.boxHeaderTitleDeactive = "Centros de Costo de baja";

            $scope.panels = [
                {
                    index: 0,
                    name: 'Filtros',
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: 'Tabla de Centros de Costo',
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewCentroCostoController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog", "generalFactory",
        function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog, generalFactory) {
            
        	var errorDelay = 5000;


            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nuevo Centro de Costo" : "Editar Centro de Costo "  + $scope.ngDialogData.dData.Number;

            // TODO: modificar para que ademas de mostrar la notificacion ocurra alguna sennalizacion en el input
            $scope.acceptDialog = function () {
                var error = false;

                if ($scope.ngDialogData.dData.Number === undefined) {
                    notification.error({
                    	message: "Número del Centro de Costo indefinido",
                        delay: errorDelay
                    });
                    error = true;
                }
                if ($scope.ngDialogData.dData.Tipo === undefined) {
                    notification.error({
                    	message: "Tipo de Centro de Costo indefinido",
                        delay: errorDelay
                    });
                    error = true;
                }
                if (error === true)
                    return;

                $scope.confirm({
                    Id: $scope.ngDialogData.dData.Id != null ?
                        $scope.ngDialogData.dData.Id : 0,
                    Number: $scope.ngDialogData.dData.Number,
                    Active: $scope.ngDialogData.dData.Active,
                    Description: $scope.ngDialogData.dData.Description,
                    Tipo: $scope.ngDialogData.dData.Tipo
                });

            }
        }
        
    ])
;
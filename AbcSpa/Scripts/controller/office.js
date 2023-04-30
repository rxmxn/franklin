"use strict";

angular.module("abcadmin")
    .controller("OfficeController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', 'Notification',
        function($scope, $route, $location, $http, $cookies, $controller, notification) {
            
            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------
            
            var filters = function (page)
            {
                return {
                    page: (page === null || page === undefined) ? $scope.currentPage : page,
                    pageSize: $scope.pageSize,
                    searchGeneral: $scope.searchGeneral,
                    bLine: $scope.bLine,
                	alta: $scope.alta,
                	baja: $scope.baja
                }
            };


            var setActivationObjects = function (eObj, act) {
                return { id: eObj.Id, active: act }
            };

            var saveObject = function (promise) {
                var officeObj = {
                    Id: promise.Id,
                    Name: promise.Name,
                    Active: promise.Active,
                    Description: promise.Description,
                    Market: promise.Market
                };
                return { office: officeObj }
            }

            function getFilters() {
                $http.post("/sucursal/getFilters")
                    .success(function (response) {
                        if (response.success) {
                            $scope.BusinessLineList = response.marketList;

                        } else {
                            notification.error({
                                message: "Error en la conexión con el servidor.",
                                delay: 5000
                            });
                            $scope.initSaving = false;
                        }
                    })
                    .error(function () {
                        notification.error({
                            message: "Se obtuvo un error del servidor. Ver detalles.",
                            delay: 5000
                        });
                        $scope.initSaving = false;
                    });
            }

            getFilters();

            var editElement = function (element) {
                return {
                    template: "editDialog",
                    controller: "NewOfficeController",
                    className: "ngdialog-theme-default",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }

            $scope.mainFunction(
                "icon mif-cabinet", "Name",
                "/office/refreshlist", filters,
                "/office/saveactivestatus", setActivationObjects,
                "/office/saveoffice", saveObject,
                editElement,
                "region"
                );

            // Office specifics
            $scope.SearchGeneral = true;
            $scope.FilterActive = true;
            $scope.BusinessLineFilterActive = true;
            $scope.boxHeaderTitleActive = "Empresas activas";
            $scope.boxHeaderTitleDeactive = "Empresas de baja";

            $scope.panels = [
                {
                    index: 0,
                    name: 'Filtros',
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: 'Tabla Empresas',
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewOfficeController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog",
        function($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog) {
            
            var errorDelay = 5000;

            function getAllMarkets(activated) {
                $http.post('/office/getofficemarket', { active: activated })
                .success(function (response) {
                    if (response.success === true) {
                        $scope.markets = response.elements;
                    } else {
                        notification.error({
                            message: 'Error al recuperar datos.', delay: errorDelay
                        });
                    }
                })
                .error(function () {
                    notification.error({
                        message: 'Error en la conexión con el servidor.', delay: errorDelay
                    });
                });
            }
            getAllMarkets(true);

            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nueva empresa" : "Editar empresa " + $scope.ngDialogData.dData.Name;

            // TODO: modificar para que ademas de mostrar la notificacion ocurra alguna sennalizacion en el input
            $scope.acceptDialog = function () {
                var error = false;

                if ($scope.ngDialogData.dData.Name === undefined) {
                    notification.error({
                        message: "Nombre de la empresa indefinido",
                        delay: errorDelay
                    });
                    error = true;
                }
                if ($scope.ngDialogData.dData.Market === undefined) {
                    notification.error({
                        message: 'Mercado indefinido',
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
                    Description: $scope.ngDialogData.dData.Description,
                    Market: $scope.ngDialogData.dData.Market
                });

            }
        }
        
    ])
;
"use strict";

angular.module("abcadmin")
    .controller("BaseMatrixController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', "Notification", "ngDialog", 
        function ($scope, $route, $location, $http, $cookies, $controller, notification, ngDialog) {
            
            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------

            
            var filters = function ()
            {
                return {
                    searchGeneral: $scope.searchGeneral,
                    marketId: $scope.marketId,
                	alta: $scope.alta,
                	baja: $scope.baja
                }
            };
			
            var setActivationObjects = function (eObj, act) {
                return { id: eObj.Id, active: act }
            };

            var saveObject = function (promise) {
                var basematrixObj = {
                    Id: promise.Id,
                    Name: promise.Name,
                    Active: promise.Active,
                    Description: promise.Description,
                    Mercado: promise.market
                };
                return { basematrix: basematrixObj }
            }

            function getFilters() {
                $http.post("/basematrix/getFilters")
                    .success(function (response) {
                        if (response.success) {
                            $scope.marketList = response.marketList;

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

            $scope.editElement = function (element) {

                var tElement = {};
                angular.extend(tElement, element);

                $http.post("/basematrix/getmarkets")
                    .success(function(response) {
                        if (response.success) {
                           
                            ngDialog.openConfirm({
                                template: "editDialog",
                                controller: "NewBaseMatrixController",
                                className: "ngdialog-theme-default",
                                preCloseCallback: "preCloseCallbackOnScope",
                                data: { dData: tElement, markets: response.elements },
                                closeByEscape: true
                            })
                                .then(function (promise) {
                                    //Accepting
                                    $scope.save(promise);
                                }, function (reason) {
                                    //Rejecting
                                });

                        } else {
                            notification.error({
                                message: "Error al recuperar los datos. " + response.err,
                                delay: 5000
                        });
                        }
                    })
                    .error(function() {
                        notification.error({
                            message: "Error en la conexión con el servidor.",
                            delay: 5000
                        });

                    });
            }

            $scope.mainFunction(
                "icon mif-windy2", "Name",
                "/basematrix/refreshlist", filters,
                "/basematrix/saveactivestatus", setActivationObjects,
                "/basematrix/savebasematrix", saveObject,
                null,
				"basematrix"	// to get the access level
                );

            // Residue specifics
            $scope.SearchGeneral = true;
            $scope.FilterGeneral = true;
            $scope.MarketFilterActive = true;
            $scope.FilterActive = true;
            // $scope.boxHeaderTitleActive = "Grupos de Matrices Activos";
            // $scope.boxHeaderTitleDeactive = "Grupos de Matrices de baja";

            $scope.panels = [
                {
                    index: 0,
                    name: "Filtros",
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: "Tabla de Grupos de Matrices",
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewBaseMatrixController",
    [
        "$scope", "$routeParams", "$location", "$http", "$controller", "SecurityFact", "Notification", "ngDialog",
        function($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog) {
            
            var errorDelay = 5000;

            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nuevo Grupo de Matrices" : "Editar Grupo de Matrices " + $scope.ngDialogData.dData.Name;

            // TODO: modificar para que ademas de mostrar la notificacion ocurra alguna sennalizacion en el input
            $scope.acceptDialog = function () {
                var error = false;

                if ($scope.ngDialogData.dData.Name === undefined) {
                    notification.error({
                    	message: "Nombre del Grupo de Matrices indefinido",
                        delay: errorDelay
                    });
                    error = true;
                }
                if ($scope.ngDialogData.dData.Mercado===undefined) {
                    notification.error({
                    	message: "Debe seleccionar un mercado para el Grupo de Matrices que está creando",
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
                    market: $scope.ngDialogData.dData.Mercado
                });

            }
        }
        
    ])
;
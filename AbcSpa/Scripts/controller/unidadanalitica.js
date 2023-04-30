"use strict";

angular.module("abcadmin")
    .controller("UnidadAnaliticaController",
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
                    annalistKey: $scope.annalistKey,
                    alta: $scope.alta,
                    baja: $scope.baja
                }
            };

            var setActivationObjects = function (eObj, act) {
                return { id: eObj.Id, active: act }
            };

            var getFilters = function () {
                $http.post("/unidadanalitica/getFilters")
                    .success(function (response) {
                        if (response.success) {
                            $scope.annalistList = response.annalistKeyList;
                            
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

            var saveObject = function (promise) {
                var unidadanaliticaObj = {
                    Id: promise.Id,
                    Key: promise.Key,
                    Active: promise.Active,
                    Description: promise.Description,
                    AnnalistKeys: promise.AnnalistKeys
                };
                return { unidadanalitica: unidadanaliticaObj }
            }

            var editElement = function (element) {
                return {
                    template: "editDialog",
                    controller: "NewUnidadAnaliticaController",
                    className: "ngdialog-theme-default ngdialog-width700",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }

            $scope.mainFunction(
                "icon mif-command", "Key",
                "/unidadanalitica/refreshlist", filters,
                "/unidadanalitica/saveactivestatus", setActivationObjects,
                "/unidadanalitica/saveunidadanalitica", saveObject,
                editElement,
				"analyticsarea"
                );

            // UnidadAnalitica specifics
            $scope.SearchGeneral = true;
            $scope.FilterActive = true;
            $scope.annalistKeyFilter = true;
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
                    name: 'Tabla de Unidades Analíticas',
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewUnidadAnaliticaController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog", "generalFactory",
        function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog, generalFactory) {
            
        	var errorDelay = 5000;
			
            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nueva Unidad Analítica" : "Editar Unidad Analítica " + $scope.ngDialogData.dData.Key;

            var id = $scope.ngDialogData.dData ? $scope.ngDialogData.dData.Id : 0;
            function getAnnalistKeys() {
                $http.post('/annalistkey/GetAnnalistKeys', { page: 1, pageSize: 1000, id: id })
				.success(function (response) {
				    if (response.success === true) {
				        $scope.annalistkeyList = response.elements;

				        if ($scope.ngDialogData.dData.Id !== undefined)
				            for (var i = 0; i < $scope.ngDialogData.dData.AnnalistKeys.length; i++) {
				                for (var j = 0; j < $scope.annalistkeyList.length; j++) {
				                    if ($scope.annalistkeyList[j].checked) {
				                        continue;
				                    }
				                    if ($scope.annalistkeyList[j].Id === $scope.ngDialogData.dData.AnnalistKeys[i].Id) {
				                        $scope.annalistkeyList[j].checked = true;
				                        break;
				                    }
				                    else
				                        $scope.annalistkeyList[j].checked = false;
				                }
				            }
				    } else {
				        notification.error({
				            message: "Error al recuperar elementos.",
				            delay: errorDelay
				        });
				    }
				})
				.error(function () {
				    notification.error({
				        message: "Error al conectar con el servidor.",
				        delay: errorDelay
				    });
				});
            }

            getAnnalistKeys();

            

            // TODO: modificar para que ademas de mostrar la notificacion ocurra alguna sennalizacion en el input
            $scope.acceptDialog = function () {
                var error = false;
                var annalistkeys = new Array();
                for (var i = 0; i < $scope.annalistkeyList.length; i++) {
                    if (!$scope.annalistkeyList[i].checked) {
                        continue;
                    }
                    annalistkeys.push({ Id: $scope.annalistkeyList[i].Id });

                }

                if ($scope.ngDialogData.dData.Key === undefined) {
                    notification.error({
                    	message: "Número de la Unidad Analítica indefinido",
                        delay: errorDelay
                    });
                    error = true;
                }
                if (error === true)
                    return;

                $scope.confirm({
                    Id: $scope.ngDialogData.dData.Id != null ?
                        $scope.ngDialogData.dData.Id : 0,
                    Key: $scope.ngDialogData.dData.Key,
                    Active: $scope.ngDialogData.dData.Active,
                    Description: $scope.ngDialogData.dData.Description,
                    AnnalistKeys: annalistkeys//$scope.ngDialogData.dData.AnnalistKey?{Id:$scope.ngDialogData.dData.AnnalistKey.Id}:null
                });

            }
        }
        
    ])
;
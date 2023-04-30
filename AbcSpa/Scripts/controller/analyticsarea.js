"use strict";

angular.module("abcadmin")
    .controller("AnalyticsAreaController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller','Notification',
        function ($scope, $route, $location, $http, $cookies, $controller, notification) {

            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------

            var filters = function (page) {
                return {
                    page: (page === null || page === undefined) ? $scope.currentPage : page,
                    pageSize: $scope.pageSize,
                    alta: $scope.alta,
                    baja: $scope.baja,
                    searchGeneral: $scope.searchGeneral,
                    CeCId: $scope.CeCId,
                    sucursalId: $scope.sucursalId,
                    tServId: $scope.tServId,
                    uAnaliticaId: $scope.uAnaliticaId
                }
            };

            var setActivationObjects = function (eObj, act) {
                return { id: eObj.Id, active: act }
            };

            var saveObject = function (promise) {
                var analyticsareaObj = {
                    Id: promise.Id,
                    Key: promise.Key,
                    Active: promise.Active,
                    Description: promise.Description,
                    CentroCosto: promise.CentroCosto,
                    Sucursal: promise.Sucursal,
                    TipoServicio: promise.TipoServicio,
                    UnidadesAnaliticas: promise.UnidadesAnaliticas
                };
                return { analyticsarea: analyticsareaObj }
            }

            function getFilters() {
                $("#loadingSpinners").show();
                $http.post("/analyticsarea/getFilters")
                    .success(function (response) {
                        if (response.success) {
                           
                            $scope.tServList = response.tServList;
                            $scope.uAnaliticaList = response.uAnaliticasList;
                            $scope.CeCList = response.CeCList;
                            $scope.sucursalList = response.sucList;

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
                    controller: "NewAnalyticsAreaController",
                    className: "ngdialog-theme-default ngdialog-width1000",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }

            $scope.mainFunction(
                "icon mif-broadcast", "Key",
                "/analyticsarea/refreshlist", filters,
                "/analyticsarea/saveactivestatus", setActivationObjects,
                "/analyticsarea/saveanalyticsarea", saveObject,
                editElement,
				"analyticsarea"
                );

            // AnalyticsArea specifics
            $scope.SearchGeneral = true;
            $scope.CeCFilter = true;
            $scope.SucursalFilterActive = true;
            $scope.TipoServFilter = true;
            $scope.unidadAnaliticaFilter = true;
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
                    name: 'Tabla de Áreas Analíticas',
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewAnalyticsAreaController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog", "generalFactory",
        function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog, generalFactory) {

            var errorDelay = 5000;

            function getAllSucursales(activated) {
                $http.post('/analyticsarea/GetAnalyticsAreaSucursal', { active: activated })
                .success(function (response) {
                    if (response.success === true) {
                        $scope.sucursales = response.elements;
                    } else {
                        Notification.error({
                            message: 'Error al recuperar datos.', delay: errorDelay
                        });
                    }
                })
                .error(function () {
                    Notification.error({
                        message: 'Error en la conexión con el servidor.', delay: errorDelay
                    });
                });
            }
            getAllSucursales(true);

            function getTiposServicio() {
                generalFactory.getAll('/tiposervicio/RefreshList')
					.then(function (response) {
					    if (response.success) {
					        $scope.tiposServicio = response.elements;
					    } else {
					        notification.error({
					            message: "Error al recuperar tipos de servicio.",
					            delay: errorDelay
					        });
					    }
					});
            }
            getTiposServicio();

            function getCentrosCosto() {
                generalFactory.getAll('/centrocosto/getcentroscosto')
					.then(function (response) {
					    if (response.success) {
					        $scope.centrosCosto = response.elements;
					    } else {
					        notification.error({
					            message: "Error al recuperar centros de costo.",
					            delay: errorDelay
					        });
					    }
					});
            }
            getCentrosCosto();

            function getAllUnidadesAnaliticas() {
                $http.post("/unidadanalitica/GetUnidadesAnaliticas")
                .success(function (response) {
                    if (response.success === true) {
                        $scope.unidadesAnaliticas = response.elements;

                        for (var j = 0; j < $scope.unidadesAnaliticas.length; j++) {
                            $scope.unidadesAnaliticas[j].activated = false;
                            if ($scope.unidadesAnaliticas[j].AreaAnalitica && $scope.unidadesAnaliticas[j].AreaAnalitica.Id !== $scope.ngDialogData.dData.Id) {
                                $scope.unidadesAnaliticas.splice(j, 1);
                                j--;
                                continue;
                            }
                            if ($scope.ngDialogData.dData.Id !== undefined)
                                for (var i = 0; i < $scope.ngDialogData.dData.UnidadesAnaliticas.length; i++) {
                                    if ($scope.ngDialogData.dData.UnidadesAnaliticas[i].Id === $scope.unidadesAnaliticas[j].Id) {
                                        $scope.unidadesAnaliticas[j].activated = true;
                                        break;
                                    }
                                }
                        }
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

            getAllUnidadesAnaliticas();

            $scope.selectUnidadAnalitica = function (unit) {
                if (unit.activated === 'undefined')
                    unit.activated = null;
                unit.activated = !unit.activated;
            }

            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nueva Área Analítica" : "Editar Área Analítica " + $scope.ngDialogData.dData.Key;

            // TODO: modificar para que ademas de mostrar la notificacion ocurra alguna sennalizacion en el input
            $scope.acceptDialog = function () {
                var error = false;

                if ($scope.ngDialogData.dData.Key === undefined) {
                    notification.error({
                        message: "Clave del área analítica indefinida",
                        delay: errorDelay
                    });
                    error = true;
                }
                if ($scope.ngDialogData.dData.CentroCosto === undefined) {
                    notification.error({
                        message: "Centro de Costo indefinido",
                        delay: errorDelay
                    });
                    error = true;
                }
                if (error === true)
                    return;

                var uaList = new Array();
                for (var i = 0; i < $scope.unidadesAnaliticas.length; i++) {
                    if ($scope.unidadesAnaliticas[i].activated) {
                        uaList.push({ Id: $scope.unidadesAnaliticas[i].Id });
                    }
                }

                $scope.confirm({
                    Id: $scope.ngDialogData.dData.Id != null ?
                        $scope.ngDialogData.dData.Id : 0,
                    Key: $scope.ngDialogData.dData.Key,
                    Active: $scope.ngDialogData.dData.Active,
                    Description: $scope.ngDialogData.dData.Description,
                    CentroCosto: $scope.ngDialogData.dData.CentroCosto,
                    Sucursal: $scope.ngDialogData.dData.Sucursal,
                    TipoServicio: $scope.ngDialogData.dData.TipoServicio,
                    UnidadesAnaliticas: uaList
                });

            }
        }

    ])
;
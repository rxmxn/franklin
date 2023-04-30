"use strict";

angular.module("abcadmin")
    .controller("RamaController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', "Notification",
        function($scope, $route, $location, $http, $cookies, $controller, notification) {
            
            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------
            
            var filters = function ()
            {
                return {
                    searchGeneral: $scope.searchGeneral,
                	alta: $scope.alta,
                	baja: $scope.baja,
                	searchMatrix: $scope.searchMatrix,
                	institutionId: $scope.institutionId
                }
            };

            var setActivationObjects = function (eObj, act) {
                return { id: eObj.Id, active: act }
            };

            var saveObject = function (promise) {
                var ramaObj = {
                    Id: promise.Id,
                    Name: promise.Name,
                    Active: promise.Active,
                    Description: promise.Description,
                    Matrixes: promise.Matrixes,
                    Enterprises: promise.Enterprises
                };
                return { rama: ramaObj }
            }

            function getFilters() {
                $http.post("/rama/getFilters")
                    .success(function (response) {
                        if (response.success) {
                            $scope.matrixList = response.matrixList;
                            $scope.InstList = response.InstitutionList;
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
                    controller: "NewRamaController",
                    className: "ngdialog-theme-default ngdialog-width1000",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }

            $scope.mainFunction(
                "icon mif-florist", "Key",
                "/rama/refreshlist", filters,
                "/rama/saveactivestatus", setActivationObjects,
                "/rama/saverama", saveObject,
                editElement,
				"rama"
                );

            // Rama specifics
            $scope.SearchGeneral = true;
            $scope.FilterActive = true;
            $scope.MatrixFilterActive = true;
            $scope.InstitutionFilterActive = true;
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
                    name: 'Tabla de Ramas',
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewRamaController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog", "generalFactory",
        function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog, generalFactory) {
            
        	var errorDelay = 5000;

        	function getAllMatrixes() {
        		$http.post('/matrix/GetMatrixes')
                .success(function (response) {
                	if (response.success === true) {
                		$scope.matrixes = response.elements;

                        for (var j = 0; j < $scope.matrixes.length; j++) {
                            $scope.matrixes[j].activated = false;
                            if ($scope.ngDialogData.dData.Id !== undefined)
                                for (var i = 0; i < $scope.ngDialogData.dData.Matrixes.length; i++) {
                                    if ($scope.ngDialogData.dData.Matrixes[i].Id === $scope.matrixes[j].Id) {
                                        $scope.matrixes[j].activated = true;
                                        break;
                                    }
                                }
                        }
                	} else {
                		Notification.error({
                			message: 'Error al recuperar matrices.', delay: errorDelay
                		});
                	}
                })
                .error(function () {
                	Notification.error({
                		message: 'Error en la conexión con el servidor.', delay: errorDelay
                	});
                });
        	}
        	getAllMatrixes();

            $scope.selectMatrix = function (mtrx) {
                if (mtrx.activated === 'undefined')
                    mtrx.activated = null;
                mtrx.activated = !mtrx.activated;
            }

        	function getAllEnterprises() {
        		generalFactory.getAll('/enterprise/GetAllEnterprises')
					.then(function (response) {
						if (response.success) {
							$scope.enterprises = response.elements;

                            for (var j = 0; j < $scope.enterprises.length; j++) {
                                $scope.enterprises[j].activated = false;
                                if ($scope.ngDialogData.dData.Id !== undefined)
                                    for (var i = 0; i < $scope.ngDialogData.dData.Enterprises.length; i++) {
                                        if ($scope.ngDialogData.dData.Enterprises[i].Id === $scope.enterprises[j].Id) {
                                            $scope.enterprises[j].activated = true;
                                            break;
                                        }
                                    }
                            }
						} else {
							notification.error({
								message: "Error al recuperar instituciones.",
								delay: errorDelay
							});
						}
					});
        	}
        	getAllEnterprises();

        	$scope.selectEnterprise = function (ent) {
        		if (ent.activated === 'undefined')
        			ent.activated = null;
        		ent.activated = !ent.activated;
        	}

            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nueva Rama" : "Editar Rama " + $scope.ngDialogData.dData.Name;

            // TODO: modificar para que ademas de mostrar la notificacion ocurra alguna sennalizacion en el input
            $scope.acceptDialog = function () {
                if ($scope.ngDialogData.dData.Name === undefined) {
                    notification.error({
                        message: "Nombre de la rama indefinido",
                        delay: errorDelay
                    });
                    return;
                }    

                var matrixList = new Array();
                for (var i = 0; i < $scope.matrixes.length; i++) {
                	if ($scope.matrixes[i].activated) {
		                matrixList.push({ Id: $scope.matrixes[i].Id });
	                }
                }

                var enterpriseList = new Array();
                for (i = 0; i < $scope.enterprises.length; i++) {
                    if ($scope.enterprises[i].activated) {
                        enterpriseList.push({ Id: $scope.enterprises[i].Id });
                    }
                }

                $scope.confirm({
                    Id: $scope.ngDialogData.dData.Id != null ?
                        $scope.ngDialogData.dData.Id : 0,
                    Name: $scope.ngDialogData.dData.Name,
                    Active: $scope.ngDialogData.dData.Active,
                    Description: $scope.ngDialogData.dData.Description,
                    Matrixes: matrixList,
                    Enterprises: enterpriseList                	
                });

            }
        }
        
    ])
;
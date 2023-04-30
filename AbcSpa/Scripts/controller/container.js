"use strict";

angular.module("abcadmin")
    .controller("ContainerController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', "SecurityFact", "Notification",
        function ($scope, $route, $location, $http, $cookies, $controller, securityFact, notification) {
            
            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------
            
            var filters = function (page)
            {
                return {
                    page: (page === null || page === undefined) ? $scope.currentPage : page,
                    pageSize: $scope.pageSize,
                	alta: $scope.alta,
                	baja: $scope.baja,
                	searchGeneral: $scope.searchGeneral,
                    materialId: $scope.materialId

                }
            };


            var setActivationObjects = function (eObj, act) {
                return { id: eObj.Id, active: act }
            };
			
            var saveObject = function (promise) {
                var containerObj = {
                    Id: promise.Id,
                    Name: promise.Name,
                    Active: promise.Active,
                    Description: promise.Description,
					Capacity: promise.Capacity,
                    Material: promise.Material
                };
                return { container: containerObj }
            }

            function getFilters() {
                $("#loadingSpinners").show();
                $http.post("/container/getFilters")
                    .success(function (response) {
                        if (response.success) {
                            $scope.materialList = response.materialList;
                           
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
                    controller: "NewContainerController",
                    className: "ngdialog-theme-default",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }

            $scope.mainFunction(
                "icon mif-gift", "Name",
                "/container/refreshlist", filters,
                "/container/saveactivestatus", setActivationObjects,
                "/container/savecontainer", saveObject,
                editElement,
				"container"	// to get the access level
                );

            // Container specifics
            $scope.SearchGeneral = true;
            $scope.materialFilter = true;
            $scope.FilterActive = true;

            $scope.boxHeaderTitleActive = "Envases activos";
            $scope.boxHeaderTitleDeactive = "Envases de baja";

            $scope.panels = [
                {
                    index: 0,
                    name: 'Filtros',
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: 'Tabla Envases',
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewContainerController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog",
        function($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog) {
            
        	var errorDelay = 5000;

        	function getAllMaterials(activated) {
        		$http.post('/container/getcontainermaterial', { active: activated })
                .success(function (response) {
                	if (response.success === true) {
                		$scope.materials = response.elements;
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
        	getAllMaterials(true);

            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nuevo envase" : "Editar envase " + $scope.ngDialogData.dData.Name;

            // TODO: modificar para que ademas de mostrar la notificacion ocurra alguna sennalizacion en el input
            $scope.acceptDialog = function () {
                var error = false;

                if ($scope.ngDialogData.dData.Name === undefined) {
                    notification.error({
                        message: "Nombre del envase indefinido",
                        delay: errorDelay
                    });
                    error = true;
                }
                if ($scope.ngDialogData.dData.Material === undefined) {
                    notification.error({
                        message: "Material del envase indefinido",
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
                    Capacity: $scope.ngDialogData.dData.Capacity,
                    Material: $scope.ngDialogData.dData.Material
                });

            }
        }
        
    ])
;
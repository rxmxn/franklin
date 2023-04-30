"use strict";

angular.module("abcadmin")
    .controller("RegionController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', 'Notification',
        function ($scope, $route, $location, $http, $cookies, $controller, notification) {

        	// This controller groups similar functionalities of different controllers.
        	$controller('GeneralController', { $scope: $scope, $http: $http });
        	//-------------------------------------------------------------------------

        	var filters = function (page) {
        	    return {
        	        page: (page === null || page === undefined) ? $scope.currentPage : page,
        	        pageSize: $scope.pageSize,
        	        searchGeneral: $scope.searchGeneral,
        	        bLine: $scope.bLine,
        	        OfficeId:$scope.OfficeId,
        			alta: $scope.alta,
        			baja: $scope.baja
        		}
        	};


        	var setActivationObjects = function (eObj, act) {
        		return { id: eObj.Id, active: act }
        	};

        	function getFilters() {
        	    $http.post("/sucursal/getFilters")
                    .success(function (response) {
                        if (response.success) {
                            $scope.BusinessLineList = response.marketList;
                            $scope.OfficeList = response.officeList;
                            

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
        		var regionObj = {
        			Id: promise.Id,
        			Name: promise.Name,
        			Active: promise.Active,
        			Description: promise.Description,
        			Key: promise.Key
        		};
				
        		var offices = [];
        		for (var i = 0; i < promise.Offices.length; i++) {
        			if (promise.Offices[i].checked === true) {
        				offices.push(promise.Offices[i].Id);
			        }
        		}

        		return { region: regionObj, offices: offices }
        	}

        	var editElement = function (element) {
        		return {
        			template: "editDialog",
        			controller: "NewRegionController",
        			className: "ngdialog-theme-default",
        			preCloseCallback: "preCloseCallbackOnScope",
        			data: { dData: element },
        			closeByEscape: true
        		}
        	}

        	$scope.mainFunction(
                "icon mif-map", "Name",
                "/region/refreshlist", filters,
                "/region/saveactivestatus", setActivationObjects,
                "/region/saveregion", saveObject,
                editElement,
                "region"
                );

        	// Region specifics
        	$scope.SearchGeneral = true;
        	$scope.BusinessLineFilterActive = true;
            $scope.OfficeFilterActive=true;
        	$scope.FilterActive = true;
        	$scope.boxHeaderTitleActive = "Regiones activas";
        	$scope.boxHeaderTitleDeactive = "Regiones de baja";

        	$scope.panels = [
                {
                	index: 0,
                	name: 'Filtros',
                	state: false,
                	size: 3
                },
                {
                	index: 1,
                	name: 'Tabla Regiones',
                	state: true,
                	size: 9
                }
        	];
        }
    ])
    .controller("NewRegionController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog",
        function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog) {

        	var errorDelay = 5000;

        	function getAllOffices() {
        		$http.post('/region/GetRegionOffices')
                .success(function (response) {
                	if (response.success === true) {
                		$scope.offices = response.elements;

                		if ($scope.ngDialogData.dData.Id !== undefined)
                			for (var i = 0; i < $scope.ngDialogData.dData.Offices.length; i++) {
                				for (var j = i; j < $scope.offices.length; j++) {
                					if ($scope.offices[j].checked) {
                						continue;
                					}
                					if ($scope.offices[j].Id === $scope.ngDialogData.dData.Offices[i].Id) {
                						$scope.offices[j].checked = true;
                						break;
                					}
                					else
                						$scope.offices[j].checked = false;
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

        	getAllOffices();

        	$scope.actionChecked = function (office) {
        		office.checked = !office.checked;
        	}

        	$scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nueva región" : "Editar región " + $scope.ngDialogData.dData.Name;

        	// TODO: modificar para que ademas de mostrar la notificacion ocurra alguna sennalizacion en el input
        	$scope.acceptDialog = function () {
        		var error = false;

        		if ($scope.ngDialogData.dData.Name === undefined) {
        			notification.error({
        				message: "Nombre de la región indefinido",
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
        			Key: $scope.ngDialogData.dData.Key,
        			Offices: $scope.offices
        		});

        	}
        }

    ])
;
"use strict";

angular.module("abcadmin")
    .controller("BaseParamController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller','Notification', 'ngDialog',
        function ($scope, $route, $location, $http, $cookies, $controller, notification, ngDialog) {

            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------

            var filters = function (page) {
                return {
                    page: (page === null || page === undefined) ? $scope.currentPage : page,
                    pageSize: $scope.pageSize,
                    alta: $scope.alta,
                    baja: $scope.baja,
                    CQ1Id: $scope.CQ1Id,
                    CQ2Id: $scope.CQ2Id,
                    CQ3Id: $scope.CQ3Id,
                    searchGeneral: $scope.searchGeneral
                }
            };

            var setActivationObjects = function (eObj, act) {
                return { id: eObj.Id, active: act }
            };
			
            var saveObject = function (promise) {

                var listaCp = new Array();
                for (var i in promise.Units) {
                    if (promise.Units.hasOwnProperty(i)) {
                        if (promise.Units[i].activated)
                            listaCp.push(
                                promise.Units[i]
                            );
                    }
                }

                var baseParamObj = {
                    Id: promise.Id,
                    Name: promise.Name,
                    Active: promise.Active,
                    Description: promise.Description,
                    PrintInResultReport: promise.PrintInResultReport,
                    Units: listaCp,
                    Matrixes: promise.Matrixes,
                    ClasificacionQuimica1: promise.ClasificacionQuimica1,
                    ClasificacionQuimica2: promise.ClasificacionQuimica2,
                    ClasificacionQuimica3: promise.ClasificacionQuimica3
                };
                return { baseparam: baseParamObj }
            }

            var editElement = function (element) {
                return {
                    template: "editDialog",
                    controller: "NewBaseParamController",
                    className: "ngdialog-theme-default ngdialog-width1000",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }


            function getFilters() {
                $("#loadingSpinners").show();
                $http.post("/param/getFilters")
                    .success(function (response) {
                        if (response.success) {
                            $scope.CQ1List = response.cq1;
                            $scope.CQ2List = response.cq2;
                            $scope.CQ3List = response.cq3;
                           
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

            $scope.mainFunction(
                "icon mif-air", "Name",
                "/baseparam/refreshlist", filters,
                "/baseparam/saveactivestatus", setActivationObjects,
                "/baseparam/savebaseparam", saveObject,
                editElement,
				"baseparam"
                );

            // BaseParam specifics
            $scope.SearchGeneral = true;
            $scope.FilterActive = true;
            $scope.CQ1Filter = true;
            $scope.CQ2Filter = true;
            $scope.CQ3Filter = true;
            $scope.boxHeaderTitleActive = "Parámetros Base activos";
            $scope.boxHeaderTitleDeactive = "Parámetros Base de baja";

            $scope.panels = [
                {
                    index: 0,
                    name: 'Filtros',
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: 'Tabla Parámetros Base',
                    state: true,
                    size: 9
                }
            ];

            $scope.ShowMatrixesDlg = function (data) {
                $http.post("/baseparam/getMatrixes", { Id: data })
                 .success(function (response) {
                    if (response.success) {
                        ngDialog.openConfirm({
                            template: "Shared/ShowMatrixesDlg",
                            controller: "dlgController",
                            className: "ngdialog-theme-default ngdialog-width1000",
                            preCloseCallback: "preCloseCallbackOnScope",
                            data: { dData: response.Matrixes },
                            closeByEscape: true
                        })
                       .then(function (promise) {
                        //Accepting
                       }, function (reason) {
                        //Rejecting
                       });
                    }
                    $scope.isLoading = false;
                 })
                 .error(function () {
                    notification.error({
                        message: "Error en la conexión con el servidor.", delay: 5000
                    });
                    $scope.isLoading = false;
                 });
            }   
			
        }
    ])

    .controller("NewBaseParamController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog",
        function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog) {

            var errorDelay = 5000;
            //$scope.countunit = 0;
            //$scope.unitList = $scope.ngDialogData.units;
            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nuevo parámetro base" : "Editar parámetro base " + $scope.ngDialogData.dData.Name;
        	// $scope.db.indb = false;

            $scope.ngDialogData.BaseMatrix = {};

            function getMatrixes() {
                $http.post("/baseparam/getbasematrix", { active: true })
                    .success(function (response) {
                        if (response.success === true) {
                            $scope.baseMatrixList = response.baseMatrix;

                            if ($scope.ngDialogData.dData.Matrixes) {
                                var j = 0;
                                for (var i = 0; i < $scope.ngDialogData.dData.Matrixes.length; i++) {
                                    for (; j < $scope.baseMatrixList.length; j++) {
                                        if ($scope.ngDialogData.dData.Matrixes[i].BaseMatrix.Id === $scope.baseMatrixList[j].Id) {
                                            $scope.ngDialogData.BaseMatrix = $scope.baseMatrixList[j];
                                            j = $scope.baseMatrixList.length;
                                            break;
                                        }
                                    }
                                    for (var k = 0; k < $scope.ngDialogData.BaseMatrix.Matrixes.length; k++) {
                                        if ($scope.ngDialogData.BaseMatrix.Matrixes[k].Id === $scope.ngDialogData.dData.Matrixes[i].Id) {
                                            $scope.ngDialogData.BaseMatrix.Matrixes[k].check = true;
                                            break;
                                        }
                                    }
                                }

                            }
                        } else {
                            notification.error({
                                message: "Error al recuperar datos.",
                                delay: errorDelay
                            });
                        }
                    })
                    .error(function () {
                        notification.error({
                            message: "Error en la conexión con el servidor.",
                            delay: errorDelay
                        });
                    });
            }
            getMatrixes();

            $scope.changeBaseMtrx = function () {
                if ($scope.ngDialogData.BaseMatrix !== null) {
                    $scope.mtrxList = $scope.ngDialogData.BaseMatrix.Matrixes;
                    return;
                }
            }

            $scope.mtrxChecked = function (mtrx) {
                if (mtrx.check) {
                    for (var i = 0; i < $scope.ngDialogData.dData.Matrixes.length; i++) {
                        if (mtrx.Id === $scope.ngDialogData.dData.Matrixes[i].Id) {
                            $scope.ngDialogData.dData.Matrixes.splice(i, 1);
                            break;
                        }
                    }
                    return;
                }
                if ($scope.ngDialogData.dData.Matrixes === undefined)
                    $scope.ngDialogData.dData.Matrixes = [];
                $scope.ngDialogData.dData.Matrixes.push({ Id: mtrx.Id });
                return;
            }

            function getAllmeasureUnits() {
                $http.post("/baseparam/getunits", { activeOption: true })
                .success(function (response) {
                    if (response.success === true) {
                        $scope.unitList = response.elements;

                        for (var j = 0; j < $scope.unitList.length; j++) {
                        	$scope.unitList[j].activated = false;
                        	if ($scope.ngDialogData.dData.Id !== undefined)
                        		for (var i = 0; i < $scope.ngDialogData.dData.Units.length; i++) {
			                        if ($scope.ngDialogData.dData.Units[i].Id === $scope.unitList[j].Id) {
				                        $scope.unitList[j].activated = true;
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

            getAllmeasureUnits();

            $scope.selectUnit = function (unit) {
            	if (unit.activated === 'undefined')
            		unit.activated = null;
            	unit.activated = !unit.activated;
            }

	        var getClasificacionesQuimicas = function() {
	        	$http.post("/clasificacionquimica/GetClasificacionQuimica", {level: 1})
                .success(function (response) {
                	if (response.success === true) {
                		$scope.clasificacionesquimicas1 = response.elements;
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

	        	$http.post("/clasificacionquimica/GetClasificacionQuimica", { level: 2 })
                .success(function (response) {
                	if (response.success === true) {
                		$scope.clasificacionesquimicas2 = response.elements;
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

	        	$http.post("/clasificacionquimica/GetClasificacionQuimica", { level: 3 })
                .success(function (response) {
                	if (response.success === true) {
                		$scope.clasificacionesquimicas3 = response.elements;
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
	        };

	        getClasificacionesQuimicas();

            ////-----------------------------------------------------------------------------
            // TODO: modificar para que ademas de mostrar la notificacion ocurra alguna sennalizacion en el input
            $scope.acceptDialog = function () {
                var error = false;

                if ($scope.ngDialogData.dData.Name === undefined) {
                    notification.error({
                        message: "Nombre del parámetro base indefinido",
                        delay: errorDelay
                    });
                    return;
                }
	            var unitSelected = false;
                for (var i = 0; i < $scope.unitList.length; i++) {
                	if ($scope.unitList[i].activated) {
		                unitSelected = true;
		                break;
	                }
	            }
                if (!unitSelected) {
                	notification.error({
                		message: "Debe seleccionar una unidad de medida",
                		delay: errorDelay
                	});
                	return;
	            }

                if (!$scope.ngDialogData.dData.Matrixes || $scope.ngDialogData.dData.Matrixes.length === 0) {                    
                    notification.error({
                        message: "Debe introducir matrices al parámetro base",
                        delay: errorDelay
                    });
                    return;                    
                }

                $scope.confirm({
                    Id: $scope.ngDialogData.dData.Id != null ?
                        $scope.ngDialogData.dData.Id : 0,
                    Name: $scope.ngDialogData.dData.Name,
                    Active: $scope.ngDialogData.dData.Active,
                    Description: $scope.ngDialogData.dData.Description,
                    PrintInResultReport: $scope.ngDialogData.dData.PrintInResultReport,
                    Units: $scope.unitList,
                    Matrixes: $scope.ngDialogData.dData.Matrixes,
                    ClasificacionQuimica1: $scope.ngDialogData.dData.ClasificacionQuimica1,
                    ClasificacionQuimica2: $scope.ngDialogData.dData.ClasificacionQuimica2,
                    ClasificacionQuimica3: $scope.ngDialogData.dData.ClasificacionQuimica3
                });

            }

        }

    ])
;
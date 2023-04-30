"use strict";

angular.module("abcadmin")
    .controller("AckController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', 'RecService', 'Notification',
        function ($scope, $route, $location, $http, $cookies, $controller, recService, notification) {
            
            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------

            $scope.recVigentes = true;
            $scope.recExpirados = true;

            var filters = function (page)
            {
                return {
                    page: (page === null || page === undefined) ? $scope.currentPage : page,
                    pageSize: $scope.pageSize,
                    alta: $scope.alta,
                    baja: $scope.baja,
                    searchGeneral: $scope.searchGeneral,
                    ackKey: $scope.ackKey,
                    institutionId: $scope.institutionId,
                    recVigentes: $scope.recVigentes,
                    recExpirados: $scope.recExpirados
                }
            };
			
            var setActivationObjects = function (eObj, act) {
                return { id: eObj.Id, active: act }
            };

            var saveObject = function (promise) {
                var ackObj = {
                    Id: promise.Id,
                    Name: promise.Name,
                    Active: promise.Active,
                    Description: promise.Description,
                    Key: promise.Key,
                    Enterprise: promise.Enterprise,
					//Estado: promise.Estado
                    Alcance: promise.Alcance,
                    Accion: promise.Accion,
                    Pdf: promise.Pdf,
                    AlertaDias: promise.AlertaDias
                };
                return { ack: ackObj, fromDate: promise.VigenciaInicial, untilDate: promise.VigenciaFinal }
            }

            var getFilters = function () {
                $http.post("/ack/getFilters")
                    .success(function (response) {
                        if (response.success) {
                            $scope.ackKeyList = response.ackKeyList;
                            $scope.InstList = response.institutionList;
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
                    controller: "NewAckController",
                    className: "ngdialog-theme-default ngdialog-width1000",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }

            $scope.mainFunction(
                "icon mif-clipboard", "Name",
                "/ack/refreshlist", filters,
                "/ack/saveactivestatus", setActivationObjects,
                "/ack/saveack", saveObject,
                editElement,
				"ack"
                );

            // Ack specifics
            $scope.SearchGeneral = true;
            $scope.FilterActive = true;
            $scope.FilterExpiredAck = true;
            $scope.InstitutionFilterActive = true;
            $scope.ackKeyFilter = true;
            //$scope.boxHeaderTitleActive = "Reconocimientos activos";
            //$scope.boxHeaderTitleDeactive = "Reconocimientos de baja";

            $scope.panels = [
                {
                    index: 0,
                    name: 'Filtros',
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: 'Tabla Reconocimientos',
                    state: true,
                    size: 9
                }
            ];

            $scope.viewAckParams = function (ackId, ackName, ackKey) {
            	recService.viewAckParams(ackId, ackName, ackKey);
            }

            $scope.getPdf = function(filePath){
                window.open(filePath, "newtab");
            }
        }
    ])
    .controller("NewAckController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog", "ImageUploadService",
        function($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog, imageUploadService) {
            
        	var errorDelay = 5000;

	        $scope.anyDisabled = false;

        	function getAllEnterprises(active) {
        		$http.post('/enterprise/GetEnterprises', {activeOption: active})
                .success(function (response) {
                	if (response.success === true) {
                		$scope.enterprises = response.elements;

                		if ($scope.ngDialogData.dData.Enterprise)                			
            				for (var j = 0; j < $scope.enterprises.length; j++) {
            					if ($scope.enterprises[j].Id === $scope.ngDialogData.dData.Enterprise.Id) {
            						$scope.enterprises[j].selected = true;
            						$scope.enterprises[j].disable = $scope.enterprises[j].RecOtorg.length > 0;
            						$scope.anyDisabled = true;
            						break;
            					}
            					else
            						$scope.enterprises[j].selected = false;
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

        	getAllEnterprises(true);

        	$scope.entActive = function (ent) {
        		//ent.selected = !ent.selected;
                for (var i = 0; i < $scope.enterprises.length; i++) {
                    if ($scope.enterprises[i].selected === true) {
                        $scope.enterprises[i].selected = false;
                        break;
                    }
                }
                ent.selected = true;
                //modified by adiel
        	};

            $scope.selectPdfforUpload = function (file) {
                $scope.cleanPdf();
                imageUploadService.FileToUpload(file).then(function (response) {
                    if (response.success) {
                        $scope.ngDialogData.dData.Pdf = response.fileName;
                    } else {
                        notification.error({
                            message: "Error al recuperar fichero.",
                            delay: 5000
                        });
                    }
                });
            };

            //-------------------------------------------------------
            $scope.cleanPdf = function () {
                //var nombre = $scope.newEmployee.Photo.split('/');
                //console.log($scope.ngDialogData.dData.Photo);
                //$http.post('/user/removeimage', {name: nombre[nombre.length-1]});
                $scope.ngDialogData.dData.Pdf = null;
            };

            function getAllActions(active) {
                $http.post('/actions/getallactions', { active: active })
                .success(function (response) {
                    if (response.success === true) {
                        $scope.actions = response.elements;
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

            getAllActions(true);

            function getAllAlcances(active) {
                $http.post('/alcance/GetAllAlcances', { activeOption: active })
                .success(function (response) {
                    if (response.success === true) {
                        $scope.alcances = response.elements;
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

            getAllAlcances(true);

            function getAllRamas(active) {
                $http.post('/rama/GetRamas', { active: active })
                .success(function (response) {
                    if (response.success === true) {
                        $scope.ramas = response.elements;
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

            getAllRamas(true);

            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nuevo reconocimiento" : "Editar reconocimiento " + $scope.ngDialogData.dData.Name;

            $scope.acceptDialog = function () {
                if ($scope.ngDialogData.dData.Name === undefined) {
                    notification.error({
                        message: "Nombre del reconocimiento indefinido",
                        delay: errorDelay
                    });
                    return;
                }

                for (var i = 0; i < $scope.enterprises.length; i++) {
                    if ($scope.enterprises[i].selected) {
                        if (!$scope.enterprises[i].Rama) {
                            notification.error({
                                message: "Seleccione una Rama",
                                delay: errorDelay
                            });
                            return;
                        }
                        $scope.ngDialogData.dData.Enterprise = { Id: $scope.enterprises[i].Id, Rama: { Id: $scope.enterprises[i].Rama.Id } };
                    }
                }

                if (!$scope.ngDialogData.dData.Enterprise) {
                    notification.error({
                        message: "Seleccione una Institución",
                        delay: errorDelay
                    });
                    return;
                }
                // if (!$scope.ngDialogData.dData.Alcance) {
                //     notification.error({
                //         message: "Seleccione el Alcance del Reconocimiento",
                //         delay: errorDelay
                //     });
                //     return;
                // }
                // if (!$scope.ngDialogData.dData.Accion) {
                //     notification.error({
                //         message: "Seleccione el Tipo de Reconocimiento",
                //         delay: errorDelay
                //     });
                //     return;
                // }
                
               $scope.confirm({
                    Id: $scope.ngDialogData.dData.Id != null ?
                        $scope.ngDialogData.dData.Id : 0,
                    Name: $scope.ngDialogData.dData.Name,
                    Active: $scope.ngDialogData.dData.Active,
                    Description: $scope.ngDialogData.dData.Description,
                    Key: $scope.ngDialogData.dData.Key,
                    VigenciaInicial: $scope.ngDialogData.dData.VigenciaInicial,
                    VigenciaFinal: $scope.ngDialogData.dData.VigenciaFinal,
                    Enterprise: $scope.ngDialogData.dData.Enterprise,
                    //Estado: $scope.ngDialogData.dData.Estado
                    Alcance: $scope.ngDialogData.dData.Alcance,
                    Accion: $scope.ngDialogData.dData.Accion,
                    Pdf: $scope.ngDialogData.dData.Pdf,
                    AlertaDias: $scope.ngDialogData.dData.AlertaDias
                });

            }
        }
        
    ])
;
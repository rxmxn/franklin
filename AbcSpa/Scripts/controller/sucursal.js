"use strict";

angular.module("abcadmin")
    .controller("SucursalController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', "ngDialog", "Notification",
        function ($scope, $route, $location, $http, $cookies, $controller, ngDialog, notification) {

        	// This controller groups similar functionalities of different controllers.
        	$controller('GeneralController', { $scope: $scope, $http: $http });
        	//-------------------------------------------------------------------------

        	$scope.vende = true;
        	$scope.realiza = true;

        	var filters = function (page) {
        		return {
        			page: (page === null || page === undefined) ? $scope.currentPage : page,
        			pageSize: $scope.pageSize,
        			searchGeneral: $scope.searchGeneral,
        			bLine: $scope.bLine,
        			regionId: $scope.regionId,
        			OfficeId: $scope.OfficeId,
        			ackId: $scope.ackId,
        			AnAreaId: $scope.AnAreaId,
        			alta: $scope.alta,
        			baja: $scope.baja,
        			vende: $scope.vende,
        			realiza: $scope.realiza
        		}
        	};

        	var setActivationObjects = function (eObj, act) {
        		return { id: eObj.Id, active: act }
        	};
            function getFilters () {
        	    $http.post("/sucursal/getFilters")
                    .success(function (response) {
                        if (response.success) {
                            $scope.regionList = response.regionList;
                            $scope.BusinessLineList = response.marketList;
                            $scope.OfficeList = response.officeList;
                            $scope.AnAreaList = response.annAreaList;
                           
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
        		var sucursalObj = {
        			Id: promise.Id,
        			Name: promise.Name,
        			Active: promise.Active,
        			Description: promise.Description,
        			Key: promise.Key,
        			AnalyticsArea: promise.AnalyticsArea,
        			Vende: promise.Vende,
        			Realiza: promise.Realiza,
        			SucursalAutolab: promise.SucursalAutolab,
        			SucursalIntelesis: promise.SucursalIntelesis,
                    Offices: promise.Offices
        		};
        		return { sucursal: sucursalObj, regionId: promise.Region.Id }
        	}

        	var editElement = function (element) {
        		return {
        			template: "editDialog",
        			controller: "NewSucursalController",
        			className: "ngdialog-theme-default ngdialog-width700",
        			preCloseCallback: "preCloseCallbackOnScope",
        			data: { dData: element },
        			closeByEscape: true
        		}
        	}

        	$scope.mainFunction(
                "icon mif-library", "Name",
                "/sucursal/refreshlist", filters,
                "/sucursal/saveactivestatus", setActivationObjects,
                "/sucursal/savesucursal", saveObject,
                editElement,
				"region"
                );

        	// Sucursal specifics
        	$scope.SearchGeneral = true;
        	$scope.FilterActive = true;
        	$scope.BusinessLineFilterActive = true;
        	$scope.RegionFilterActive = true;
        	//$scope.AckFilter = true;
        	$scope.OfficeFilterActive = true;
            $scope.AnAreaFilter = true;
        	//$scope.boxHeaderTitleActive = "Instalaciones activas";
        	//$scope.boxHeaderTitleDeactive = "Instalaciones de baja";
        	$scope.SucursalFilters = true;

        	$scope.panels = [
                {
                	index: 0,
                	name: 'Filtros',
                	state: false,
                	size: 3
                },
                {
                	index: 1,
                	name: "Tabla de Instalaciones",
                	state: true,
                	size: 9
                }
        	];

        	$scope.addRec = function (element) {
        		var tElement = {};
        		angular.extend(tElement, element);

        		ngDialog.openConfirm({
        			template: "Shared/RecOtorgados",
        			controller: "AddRecController",
        			className: "ngdialog-theme-default ngdialog-width700",
        			preCloseCallback: "preCloseCallbackOnScope",
        			data: { dData: tElement },
        			closeByEscape: true
        		})
					.then(function (promise) {
						//Accepting
						$scope.RefreshList();
					}, function (reason) {
						//Rejecting
					});
        	};
        }
    ])
    .controller("NewSucursalController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog", "ImageUploadService",
        function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog, imageUploadService) {

        	var errorDelay = 5000;
        	$scope.isLoading = false;
        	function getAllRegions(activated) {
        		$http.post('/region/getallregions', { active: activated })
				.success(function (response) {
					if (response.success === true) {
						$scope.regions = response.elements;
						if ($scope.ngDialogData.dData.Region !== undefined) {
							for (var i = 0; i < $scope.regions.length; i++) {
								if ($scope.regions[i].Id === $scope.ngDialogData.dData.Region.Id) {
									$scope.offices = $scope.regions[i].Offices;

                                    if ($scope.ngDialogData.dData.Id !== undefined)
                                        for (var i = 0; i < $scope.ngDialogData.dData.Offices.length; i++) {
                                            for (var j = 0; j < $scope.offices.length; j++) {
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
        	getAllRegions(true);

            $scope.officeChecked = function (o) {
                o.checked = !o.checked;
            }

        	// coger todos los centros de costo de la sucursal seleccionada y los que no
        	// esten asignados a ninguna sucursal.
        	function getAllAnalyticsArea(active) {
        		var id = $scope.ngDialogData.dData.Id === undefined ? 0 : $scope.ngDialogData.dData.Id;
        		$http.post('/sucursal/GetAnalyticsArea', { sucursalId: id, activeOption: active })
                .success(function (response) {
                	if (response.success === true) {
                		$scope.analyticsareas = response.elements;

                		if ($scope.ngDialogData.dData.Id !== undefined)
                			for (var i = 0; i < $scope.ngDialogData.dData.AnalyticsArea.length; i++) {
                				for (var j = 0; j < $scope.analyticsareas.length; j++) {
                					if ($scope.analyticsareas[j].checked) {
                						continue;
                					}
                					if ($scope.analyticsareas[j].Id === $scope.ngDialogData.dData.AnalyticsArea[i].Id) {
                						$scope.analyticsareas[j].checked = true;
                						break;
                					}
                					else
                						$scope.analyticsareas[j].checked = false;
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

        	getAllAnalyticsArea(true);

        	$scope.aaChecked = function (aa) {
        		aa.checked = !aa.checked;
        	}

        	$scope.dialogTitle = $scope.ngDialogData.dData.Id === undefined ?
                "Nueva instalación" : "Editar instalación " + $scope.ngDialogData.dData.Name;

        	$scope.acceptDialog = function () {
        		var error = false;

        		if ($scope.ngDialogData.dData.Name === undefined) {
        			notification.error({
        				message: "Nombre de la instalación indefinido",
        				delay: errorDelay
        			});
        			error = true;
        		}

        		if ($scope.isIndb) {
        			notification.error({
        				message: "La clave ya existe en base de datos, por favor cámbiela",
        				delay: errorDelay
        			});
        			error = true;
        		}

        		if (($scope.ngDialogData.dData.Vende === false || $scope.ngDialogData.dData.Vende === undefined)
					&& ($scope.ngDialogData.dData.Realiza === false || $scope.ngDialogData.dData.Realiza === undefined)) {
        			notification.error({
        				message: "La instalación no vende ni realiza. Por favor, seleccione al menos una de las dos opciones.",
        				delay: errorDelay
        			});
        			error = true;
        		}
        		if (error === true)
        			return;

        		var analytics = new Array();
        		for (var i = 0; i < $scope.analyticsareas.length; i++) {
        			if ($scope.analyticsareas[i].checked === true) {
        				analytics.push({
        					'Id': $scope.analyticsareas[i].Id
        				});
        			}
        		}

                var officesList = new Array();
                for (var i = 0; i < $scope.offices.length; i++) {
                    if ($scope.offices[i].checked === true) {
                        officesList.push({
                            'Id': $scope.offices[i].Id
                        });
                    }
                }

        		$scope.confirm({
        			Id: $scope.ngDialogData.dData.Id != null ?
                        $scope.ngDialogData.dData.Id : 0,
        			Name: $scope.ngDialogData.dData.Name,
        			Active: $scope.ngDialogData.dData.Active,
        			Description: $scope.ngDialogData.dData.Description,
        			Key: $scope.ngDialogData.dData.Key,
        			Offices: officesList,
        			Region: $scope.ngDialogData.dData.Region,
        			AnalyticsArea: analytics,
        			Vende: $scope.ngDialogData.dData.Vende,
        			Realiza: $scope.ngDialogData.dData.Realiza,
        			SucursalAutolab: $scope.ngDialogData.dData.SucursalAutolab,
        			SucursalIntelesis: $scope.ngDialogData.dData.SucursalIntelesis
        		});

        	}
        }

    ])

.directive('sucursalSelectDirective', [function () {
	return {
		link: function (scope) {
			// nota: ya probe que estas cosas se pueden hacer mejor poniendo un ng-change en el comboBox
			scope.$watch('ngDialogData.dData.Region.Id', function () {
				if (scope.ngDialogData.dData.Region !== undefined && scope.regions !== undefined) {

					if (scope.ngDialogData.dData.Region.Id === null) {
						scope.offices = [];
					} else {
						for (var i = 0; i < scope.regions.length; i++) {
							if (scope.regions[i].Id === scope.ngDialogData.dData.Region.Id) {
								scope.offices = scope.regions[i].Offices;
								break;
							}
						}
					}
				}
			});

		}
	};
}])

.controller("AddReconocimientosController",
    [
        "$scope", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog", "RecService",
        function ($scope, $location, $http, $controller, securityFact, notification, ngDialog, recService) {

        	var errorDelay = 5000;

        	$scope.dialogTitle = "Adicionar Reconocimiento";
        	var saveDialogTitle = "Adicionar Reconocimiento";

        	function copy(o) {
        		var output, v, key;
        		output = Array.isArray(o) ? [] : {};
        		for (key in o) {
        			v = o[key];
        			output[key] = (typeof v === "object") ? copy(v) : v;
        		}
        		return output;
        	}

        	$scope.nivelAdquirido = "";
        	var selectedAck = null;

        	$scope.acks = copy($scope.ngDialogData.acks);
        	$scope.enterprises = copy($scope.ngDialogData.enterprises);

        	$scope.selectionArray = new Array();
        	for (var j = 0; j < $scope.acks.length; j++) {
        		//$scope.selectionArray.push({ Ack: { Id: $scope.acks[j].Id, Name: $scope.acks[j].Name } });
        		
    			$scope.selectionArray.push({
    				Ack: { Id: $scope.acks[j].Id, Name: $scope.acks[j].Name },
    				Ent: { 
                        Id: $scope.acks[j].Enterprise.Id, 
                        Name: $scope.acks[j].Enterprise.Name,
                        Tipo: $scope.acks[j].Enterprise.Tipo
                    }
    			});
        		
        	}

        	var savedSelectionArray = copy($scope.selectionArray);

        	$scope.selectionChanged = function (sa) {
        		selectedAck = sa;
        	}

        	$scope.ackChanged = function (ack) {
        		if (ack !== null && ack !== undefined) {
        			//if ($scope.nivelAdquirido === "" || $scope.nivelAdquirido === "0")
        			$scope.enterprises.length = 0;
                    $scope.enterprises.push(ack.Enterprise);
        			$scope.ack = ack;
        		} else {
        			//if ($scope.nivelAdquirido === "" || $scope.nivelAdquirido === "0")
        			$scope.enterprises = copy($scope.ngDialogData.enterprises);
        			$scope.ack = null;
        		}

        		$scope.selectionArray.length = 0;	// borrando todos los elementos del array
        		if (ack !== null && ack !== undefined) {
        			for (var l = 0; l < savedSelectionArray.length; l++) {
        				if ($scope.nivelAdquirido === "0") {
        					if (savedSelectionArray[l].Ack.Id === ack.Id && savedSelectionArray[l].Ent.Tipo === 0) {
        						$scope.selectionArray.push(savedSelectionArray[l]);
        					}
        				} else if ($scope.nivelAdquirido === "1") {
        					if (savedSelectionArray[l].Ack.Id === ack.Id && savedSelectionArray[l].Ent.Tipo === 1) {
        						$scope.selectionArray.push(savedSelectionArray[l]);
        					}
        				} else {
        					if (savedSelectionArray[l].Ack.Id === ack.Id) {
        						$scope.selectionArray.push(savedSelectionArray[l]);
        					}
        				}
        			}
        		} else {
        			for (l = 0; l < savedSelectionArray.length; l++) {
        				if ($scope.nivelAdquirido === "0") {
        					if (savedSelectionArray[l].Ent.Tipo === 0) {
        						$scope.selectionArray.push(savedSelectionArray[l]);
        					}
        				} else if ($scope.nivelAdquirido === "1") {
        					if (savedSelectionArray[l].Ent.Tipo === 1) {
        						$scope.selectionArray.push(savedSelectionArray[l]);
        					}
        				} else {
        					$scope.selectionArray.push(savedSelectionArray[l]);
        				}
        			}
        		}

        		selectedAck = null;
        		$scope.enterprise = "";
        	}

        	$scope.entChanged = function (ent) {
        		$scope.selectionArray.length = 0;	// borrando todos los elementos del array
        		if (ent !== null && ent !== undefined) {
        			for (var l = 0; l < savedSelectionArray.length; l++) {
        				if (savedSelectionArray[l].Ent.Id === ent.Id) {
        					$scope.selectionArray.push(savedSelectionArray[l]);
        				}
        			}
        			if (selectedAck !== null &&
						ent.Id !== selectedAck.Ent.Id) {
        				selectedAck = null;
        			}
        		} else {
        			for (l = 0; l < savedSelectionArray.length; l++) {
        				if ($scope.ack !== null) {
        					if ($scope.nivelAdquirido === "0") {
        						if (savedSelectionArray[l].Ack.Id === $scope.ack.Id && savedSelectionArray[l].Ent.Tipo === 0) {
        							$scope.selectionArray.push(savedSelectionArray[l]);
        						}
        					} else if ($scope.nivelAdquirido === "1") {
        						if (savedSelectionArray[l].Ack.Id === $scope.ack.Id && savedSelectionArray[l].Ent.Tipo === 1) {
        							$scope.selectionArray.push(savedSelectionArray[l]);
        						}
        					} else {
        						if (savedSelectionArray[l].Ack.Id === $scope.ack.Id) {
        							$scope.selectionArray.push(savedSelectionArray[l]);
        						}
        					}
        				} else {
        					if ($scope.nivelAdquirido === "0") {
        						if (savedSelectionArray[l].Ent.Tipo === 0) {
        							$scope.selectionArray.push(savedSelectionArray[l]);
        						}
        					} else if ($scope.nivelAdquirido === "1") {
        						if (savedSelectionArray[l].Ent.Tipo === 1) {
        							$scope.selectionArray.push(savedSelectionArray[l]);
        						}
        					} else {
        						$scope.selectionArray.push(savedSelectionArray[l]);
        					}
        				}
        			}
        		}
        	}

        	$scope.levelChanged = function (level) {
        		$scope.nivelAdquirido = level;
        		if (level === "0" || level === "1") {

        			$scope.selectionArray.length = 0;	// borrando todos los elementos del array
        			selectedAck = null;
        			$scope.enterprises.length = 0;
        			$scope.ack = null;

        			// if (level === "1") {
        			// 	for (var l = 0; l < savedSelectionArray.length; l++) {
        			// 		if (level === "1" && savedSelectionArray[l].Ent.Tipo === 0) {
        			// 			$scope.selectionArray.push(savedSelectionArray[l]);
        			// 		}
        			// 	}
        			// } else {
    				//$scope.enterprises = $scope.ngDialogData.enterprises;

    				$scope.acks.length = 0;
    				for (var i = 0; i < $scope.ngDialogData.acks.length; i++) {
    					// if ($scope.ngDialogData.acks[i].Enterprises.length > 0) {
    					// 	$scope.acks.push($scope.ngDialogData.acks[i]);
    					// }
                        if (level === "0" && $scope.ngDialogData.acks[i].Enterprise.Tipo === 0) {
                            $scope.acks.push($scope.ngDialogData.acks[i]);
                        }
                        if (level === "1" && $scope.ngDialogData.acks[i].Enterprise.Tipo === 1) {
                            $scope.acks.push($scope.ngDialogData.acks[i]);
                        }
    				}

                    for (i = 0; i < $scope.ngDialogData.enterprises.length; i++) {
                        if (level === "0" && $scope.ngDialogData.enterprises[i].Tipo === 0) {
                            $scope.enterprises.push($scope.ngDialogData.enterprises[i]);
                        }
                        if (level === "1" && $scope.ngDialogData.enterprises[i].Tipo === 1) {
                            $scope.enterprises.push($scope.ngDialogData.enterprises[i]);
                        }
                    }

    				for (var l = 0; l < savedSelectionArray.length; l++) {
    					if (level === "0" && savedSelectionArray[l].Ent.Tipo === 0) {
    						$scope.selectionArray.push(savedSelectionArray[l]);
    					}
                        if (level === "1" && savedSelectionArray[l].Ent.Tipo === 1) {
                            $scope.selectionArray.push(savedSelectionArray[l]);
                        }
    				}
        			//}
        		} else {
        			$scope.selectionArray.length = 0;	// borrando todos los elementos del array
        			selectedAck = null;
        			$scope.selectionArray = copy(savedSelectionArray);
        			$scope.acks = copy($scope.ngDialogData.acks);
        			$scope.enterprises = copy($scope.ngDialogData.enterprises);
        		}
        	}

        	$scope.currentStep = null;

        	var getAnnalists = function () {
        		$http.post('/sucursal/GetAnnalistToAddRec', { sucursalId: $scope.ngDialogData.sucursal.Id })
					.success(function (response) {
						if (response.success === true) {
							$scope.annalists = response.elements;

							for (var l = 0; l < $scope.annalists.length; l++) {
								for (var m = 0; m < $scope.annalists[l].RecAdqs.length; m++) {
									for (var n = 0; n < $scope.annalists[l].RecAdqs[m].RecOtorgs.length; n++) {
										if ($scope.annalists[l].RecAdqs[m].RecOtorgs[n].Ack.Id === selectedAck.Ack.Id &&
											(!$scope.annalists[l].RecAdqs[m].RecOtorgs[n].Enterprise ||
											$scope.annalists[l].RecAdqs[m].RecOtorgs[n].Enterprise.Id === selectedAck.Ent.Id) &&
											$scope.annalists[l].RecAdqs[m].RecOtorgs[n].Sucursal.Id === $scope.ngDialogData.sucursal.Id) {
											$scope.annalists[l].tipoSignatario = $scope.annalists[l].RecAdqs[m].TipoSignatario.Id;

											for (var o = 0; o < $scope.annalists[l].RecAdqs[m].RecOtorgs[n].Params.length; o++) {
												for (var p = 0; p < $scope.annalists[l].Params.length; p++) {
													if ($scope.annalists[l].RecAdqs[m].RecOtorgs[n].Params[o].Id === $scope.annalists[l].Params[p].Id) {
														$scope.annalists[l].Params.splice(p, 1);
														break;
													}
												}
											}
											break;
										}
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

        	$scope.annalistToAck = new Array();

        	$scope.tipoSigDisabled = true;

        	$scope.nextPage = function () {
        		if ($scope.currentStep === "page1") {
        			if (selectedAck === null) {
        				selectedAck = $scope.selectionArray[0];
        			}
        			$scope.tipoSigDisabled = selectedAck.Ent.Tipo === 1;

        			getAnnalists();
        			$scope.dialogTitle = saveDialogTitle;
        			$scope.dialogTitle += ": " + selectedAck.Ack.Name +
						"/" + selectedAck.Ent.Name + "(" + (selectedAck.Ent.Tipo === 0 
                            ? 'Externa' : 'Interna') + ")";
        		}
        		else if ($scope.currentStep === "page2") {
        			$scope.annalistToAck.length = 0;
                    if ($scope.annalists) {
                        for (var l = 0; l < $scope.annalists.length; l++) {
                            if ($scope.annalists[l].checked === true) {
                                $scope.annalistToAck.push({
                                    NivelAdquirido: selectedAck.Ent.Tipo,
                                    TipoSignatario: {
                                        Id: $scope.annalists[l].tipoSignatario
                                    },
                                    Annalist: {
                                        Id: $scope.annalists[l].Id,
                                        Name: $scope.annalists[l].Name,
                                        Photo: $scope.annalists[l].Photo,
                                        Gender: $scope.annalists[l].Gender,
                                        Params: $scope.annalists[l].Params
                                    }
                                });
                            }
                        }
                    }        			
        		}
        	}

        	var getTiposSig = function () {
        		$http.post('/sucursal/GetTiposSignatario')
					.success(function (response) {
						if (response.success === true) {
							$scope.tiposSig = response.elements;
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
        	getTiposSig();

        	$scope.viewRecAdqs = function (an) {
        		recService.viewRecAdqs(an);
        	}

        	$scope.checkAnnalist = function (an) {
        		an.checked = true;
        	}

        	$scope.annalistChecked = function (an) {
        		an.checked = !an.checked;
        		if (an.checked === false) {
        			an.tipoSignatario = "";
        		}
        	}

        	$scope.paramChecked = function (p) {
        		p.checked = !p.checked;
        	}

        	$scope.acceptDialog = function () {
        		var error = false;

                if (!$scope.ngDialogData.officeId) {
                    notification.error({
                        message: "Seleccione una Empresa",
                        delay: errorDelay
                    });
                    return;
                }

        		for (var n = 0; n < $scope.annalistToAck.length; n++) {
        			if (($scope.annalistToAck[n].TipoSignatario.Id === undefined ||
						$scope.annalistToAck[n].TipoSignatario.Id === null) &&
						$scope.tipoSigDisabled === false) {
        				notification.error({
        					message: "Seleccione un tipo de Signatario para todos los analistas reconocidos",
        					delay: errorDelay
        				});
        				error = true;
        				break;
        			}
        		}

        		if (error === true)
        			return;

        		for (var l = 0; l < $scope.annalistToAck.length; l++) {
        			for (var m = 0; m < $scope.annalistToAck[l].Annalist.Params.length; m++) {
        				if ($scope.annalistToAck[l].Annalist.Params[m].checked !== true) {
        					$scope.annalistToAck[l].Annalist.Params.splice(m, 1);
        					m--;
        				}
        			}
        		}

        		if ($scope.annalistToAck.length > 0) {
        			$scope.confirm({
        				Enterprise: { Id: selectedAck.Ent.Id },
        				Ack: { Id: selectedAck.Ack.Id },
        				Sucursal: { Id: $scope.ngDialogData.sucursal.Id },
                        Office: {Id: $scope.ngDialogData.officeId},
        				RecAdqs: $scope.annalistToAck
        			});
        		} else {
        			$scope.closeThisDialog();
        		}
        	}
        }

    ])

.controller("AddRecController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog", "ImageUploadService", "RecService",
        function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog, imageUploadService, recService) {

        	var errorDelay = 5000;
        	$scope.isLoading = false;

        	function getSucursalRecOtorg() {
        		$scope.isLoading = true;
        		var id = $scope.ngDialogData.dData.Id === undefined ? 0 : $scope.ngDialogData.dData.Id;
        		$http.post('/sucursal/GetSucursalRecOtorg', { sucursalId: id })
                .success(function (response) {
                	if (response.success === true) {
                		$scope.rOtorgs = response.elements;
                		$scope.rOtorgsTotal = response.total;
                		$scope.isLoading = false;
                	} else {
                		notification.error({
                			message: 'Error al recuperar datos.', delay: errorDelay
                		});
                		$scope.isLoading = false;
                	}
                })
                .error(function () {
                	notification.error({
                		message: 'Error en la conexión con el servidor.', delay: errorDelay
                	});
                	$scope.isLoading = false;
                });
        	}

        	getSucursalRecOtorg();

        	$scope.viewParams = function (aName, roId, ackName, entName) {
        		recService.viewParams(aName, roId, ackName, entName);
        	}

        	$scope.addRec = function (suc) {
                $("#loadingSpinners").show();
        		$http.post("/sucursal/getelementstoaddrec", { sucId: suc.Id })
					.success(function (response) {
						if (response.success) {
							ngDialog.openConfirm({
								template: "Shared/AddReconocimientos",
								controller: "AddReconocimientosController",
								className: "ngdialog-theme-default ngdialog-width1000",
								preCloseCallback: "preCloseCallbackOnScope",
								data: {
									sucursal: suc,
									acks: response.acks,
									enterprises: response.enterprises,
                                    offices: response.offices
								},
								closeByEscape: true
							})
							.then(function (promise) {
								//Accepting
								$http.post("/sucursal/AddReconocimiento",
									{
										recOtorg: {
											Enterprise: promise.Enterprise,
											Ack: promise.Ack,
											Sucursal: promise.Sucursal,
                                            Office: promise.Office
										},
										recAdqs: promise.RecAdqs
									})
								.success(function (recresponse) {
									if (recresponse.success) {
										getSucursalRecOtorg();	// refresh
										notification.success({
											message: "Reconocimientos guardados correctamente.",
											delay: 3000
										});
									} else {
										notification.error({
											message: "Error en la conexión con el servidor.",
											delay: errorDelay
										});
									}
								})
								.error(function () {
									notification.error({
										message: "Se obtuvo un error del servidor. Ver detalles.",
										delay: errorDelay
									});
								});
							}, function (reason) {
								//Rejecting
							});
						}
						$scope.isLoading = false;
					})
				 .error(function () {
				 	notification.error({
				 		message: "Error en la conexión con el servidor.", delay: errorDelay
				 	});
				 	$scope.isLoading = false;
				 });
        	}

        	$scope.acceptDialog = function () {
        		$scope.confirm();
        	}
        }

    ])
;
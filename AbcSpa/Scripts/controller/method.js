"use strict";

angular.module("abcadmin")
    .controller("MethodController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', "Notification", "ngDialog",
        function ($scope, $route, $location, $http, $cookies, $controller, notification, ngDialog) {

        	// This controller groups similar functionalities of different controllers.
        	$controller('GeneralController', { $scope: $scope, $http: $http });
        	//-------------------------------------------------------------------------

        	var filters = function (page) {
        		return {
        			page: (page === null || page === undefined) ? $scope.currentPage : page,
        			pageSize: $scope.pageSize,
        			searchGeneral: $scope.searchGeneral,
        			// envaseId: $scope.envaseId,
        			// preserverId: $scope.preserverId,
        			alta: $scope.alta,
        			baja: $scope.baja
        		}
        	};

        	var setActivationObjects = function (eObj, act) {
        		return { id: eObj.Id, active: act }
        	};

        	var saveObject = function (promise) {
        		var methodObj = {
        			Id: promise.Id,
        			Name: promise.Name,
        			Active: promise.Active,
        			Description: promise.Description,
        			// RequiredVolume: promise.RequiredVolume,
        			// MinimumVolume: promise.MinimumVolume,
        			// ReportLimit:promise.ReportLimit,
        			// DeliverTime: promise.DeliverTime,
        			// DetectionLimit: promise.DetectionLimit,
        			// CuantificationLimit: promise.CuantificationLimit,
        			// MaxTimeBeforeAnalysis: promise.MaxTimeBeforeAnalysis,
        			// LabDeliverTime: promise.LabDeliverTime,
        			// ReportTime: promise.ReportTime,
        			// Container: promise.Container,
        			// Preserver: promise.Preserver,
        			//Matrixes: promise.Matrixes,
        			Norms: promise.Norms,
        			EntradaEnVigor: promise.EntradaEnVigor,
        			Estado: promise.Estado,
        			TiposServicios: promise.TiposServicios
        		};

        		return { method: methodObj }
        	}

        	var editElement = function (element) {
        		return {
        			template: "editDialog",
        			controller: "NewMethodController",
        			className: "ngdialog-theme-default ngdialog-width1000",
        			preCloseCallback: "preCloseCallbackOnScope",
        			data: { dData: element },
        			closeByEscape: true
        		}
        	}

        	$scope.mainFunction(
                "icon mif-list", "Name",
                "/method/refreshlist", filters,
                "/method/saveactivestatus", setActivationObjects,
                "/method/savemethod", saveObject,
                editElement,
				"method"	// to get the access level
                );

        	// function getFilters() {
        	//     $("#loadingSpinners").show();
        	//     $http.post("/method/getFilters")
        	//         .success(function (response) {
        	//             if (response.success) {
        	//                 $scope.envaseList = response.envaseList;
        	//                 $scope.preserverList = response.preserverList;
        	//                 //$scope.residueList = response.residueList;

        	//             } else {
        	//                 notification.error({
        	//                     message: "Error en la conexión con el servidor.",
        	//                     delay: 5000
        	//                 });
        	//                 $scope.initSaving = false;
        	//             }
        	//         })
        	//         .error(function () {
        	//             notification.error({
        	//                 message: "Se obtuvo un error del servidor. Ver detalles.",
        	//                 delay: 5000
        	//             });
        	//             $scope.initSaving = false;
        	//         });
        	// }

        	//getFilters();

        	// Method specifics
        	$scope.SearchGeneral = true;
        	// $scope.EnvaseFilter = true;
        	// $scope.preserverFilter = true;
        	//$scope.residueFilter = true;
        	$scope.FilterActive = true;
        	$scope.SearchGeneral = true;
        	// $scope.boxHeaderTitleActive = "Métodos activos";
        	// $scope.boxHeaderTitleDeactive = "Métodos de baja";

        	$scope.panels = [
                {
                	index: 0,
                	name: 'Filtros',
                	state: false,
                	size: 3
                },
                {
                	index: 1,
                	name: 'Tabla Métodos',
                	state: true,
                	size: 9
                }
        	];

        	// $scope.ShowMatrixesDlg = function (data) {
        	//     $http.post("/method/getMatrixes", { Id: data })
        	//      .success(function (response) {
        	//         if (response.success) {
        	//             ngDialog.openConfirm({
        	//                 template: "Shared/ShowMatrixesDlg",
        	//                 controller: "dlgController",
        	//                 className: "ngdialog-theme-default ngdialog-width1000",
        	//                 preCloseCallback: "preCloseCallbackOnScope",
        	//                 data: { dData: response.Matrixes },
        	//                 closeByEscape: true
        	//             })
        	//            .then(function (promise) {
        	//             //Accepting
        	//            }, function (reason) {
        	//             //Rejecting
        	//            });
        	//         }
        	//         $scope.isLoading = false;
        	//      })
        	//      .error(function () {
        	//         notification.error({
        	//             message: "Error en la conexión con el servidor.", delay: 5000
        	//         });
        	//         $scope.isLoading = false;
        	//      });
        	// }   

        	$scope.showNormInfo = function (norm) {
        		$http.post("/norm/getnorminfo", { Id: norm.Id })
                 .success(function (response) {
                 	if (response.success) {
                 		ngDialog.openConfirm({
                 			template: "Shared/ShowNormInfo",
                 			controller: "dlgController",
                 			className: "ngdialog-theme-default ngdialog-width1000",
                 			preCloseCallback: "preCloseCallbackOnScope",
                 			data: { dData: response.norm },
                 			closeByEscape: true
                 		})
					   .then(function (promise) {
					   	//Accepting
					   }, function (reason) {
					   	//Rejecting
					   });
                 	}
                 	// $scope.isLoading = false;
                 })
                 .error(function () {
                 	notification.error({
                 		message: "Error en la conexión con el servidor.", delay: 5000
                 	});
                 	//  $scope.isLoading = false;
                 });
        	}

        }
    ])
    .controller("NewMethodController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog", "generalFactory",
        function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog, generalFactory) {

        	var errorDelay = 5000;

        	//$scope.ngDialogData.BaseMatrix = {};

        	// Checking Column List
        	$scope.columnList = securityFact.getColumnList();

        	if ($scope.columnList === undefined) {
        		securityFact.subscribe($scope, function updateCLparam() {
        			$scope.columnList = securityFact.getColumnList();
        		});
        	}

        	// function getMatrixes() {
        	//     $http.post("/method/getbasematrix", { active: true })
        	//         .success(function (response) {
        	//             if (response.success === true) {
        	//                 $scope.baseMatrixList = response.baseMatrix;

        	//                 if ($scope.ngDialogData.dData.Matrixes) {
        	//                     var j = 0;
        	//                     for (var i = 0; i < $scope.ngDialogData.dData.Matrixes.length; i++) {
        	//                         for (; j < $scope.baseMatrixList.length; j++) {
        	//                             if ($scope.ngDialogData.dData.Matrixes[i].BaseMatrix.Id === $scope.baseMatrixList[j].Id) {
        	//                                 $scope.ngDialogData.BaseMatrix = $scope.baseMatrixList[j];
        	//                                 j = $scope.baseMatrixList.length;
        	//                                 break;
        	//                             }
        	//                         }
        	//                         for (var k = 0; k < $scope.ngDialogData.BaseMatrix.Matrixes.length; k++) {
        	//                             if ($scope.ngDialogData.BaseMatrix.Matrixes[k].Id === $scope.ngDialogData.dData.Matrixes[i].Id) {
        	//                                 $scope.ngDialogData.BaseMatrix.Matrixes[k].check = true;
        	//                                 break;
        	//                             }
        	//                         }
        	//                     }

        	//                 }
        	//             } else {
        	//                 notification.error({
        	//                     message: "Error al recuperar datos.",
        	//                     delay: errorDelay
        	//                 });
        	//             }
        	//         })
        	//         .error(function () {
        	//             notification.error({
        	//                 message: "Error en la conexión con el servidor.",
        	//                 delay: errorDelay
        	//             });
        	//         });
        	// }
        	// getMatrixes();

        	// $scope.changeBaseMtrx = function () {
        	//     if ($scope.ngDialogData.BaseMatrix !== null) {
        	//         $scope.mtrxList = $scope.ngDialogData.BaseMatrix.Matrixes;
        	//         return;
        	//     }
        	//     $scope.ngDialogData.BaseMatrix.Matrixes = [];
        	//     return;
        	// }

        	// $scope.mtrxChecked = function (mtrx) {
        	//     if (mtrx.check) {
        	//         for (var i = 0; i < $scope.ngDialogData.dData.Matrixes.length; i++) {
        	//             if (mtrx.Id === $scope.ngDialogData.dData.Matrixes[i].Id) {
        	//                 $scope.ngDialogData.dData.Matrixes.splice(i, 1);
        	//                 break;
        	//             }
        	//         }
        	//         return;
        	//     }
        	//     if ($scope.ngDialogData.dData.Matrixes === undefined)
        	//         $scope.ngDialogData.dData.Matrixes = [];
        	//     $scope.ngDialogData.dData.Matrixes.push({ Id: mtrx.Id });
        	//     return;
        	// }

        	// function getAllContainers(activated) {
        	//     $http.post('/method/getmethodcontainer', { active: activated })
        	//     .success(function (response) {
        	//         if (response.success === true) {
        	//             $scope.containers = response.elements;
        	//         } else {
        	//             Notification.error({
        	//                 message: 'Error al recuperar datos.', delay: errorDelay
        	//             });
        	//         }
        	//     })
        	//     .error(function () {
        	//         Notification.error({
        	//             message: 'Error en la conexión con el servidor.', delay: errorDelay
        	//         });
        	//     });
        	// }
        	// getAllContainers(true);

        	// function getAllPreservers(activated) {
        	//     $http.post('/method/getmethodpreserver', { active: activated })
        	//     .success(function (response) {
        	//         if (response.success === true) {
        	//             $scope.preservers = response.elements;
        	//         } else {
        	//             Notification.error({
        	//                 message: 'Error al recuperar datos.', delay: errorDelay
        	//             });
        	//         }
        	//     })
        	//     .error(function () {
        	//         Notification.error({
        	//             message: 'Error en la conexión con el servidor.', delay: errorDelay
        	//         });
        	//     });
        	// }
        	// getAllPreservers(true);

        	function getNorms() {
        		generalFactory.getAll('/norm/refreshList', { page: 1, pageSize: 100 }).then(function (response) {
        			if (response.success) {
        				$scope.norms = response.elements;

        				if ($scope.ngDialogData.dData.Id !== undefined)
        					for (var i = 0; i < $scope.ngDialogData.dData.Norms.length; i++) {
        						for (var j = 0; j < $scope.norms.length; j++) {
        							if ($scope.norms[j].checked) {
        								continue;
        							}
        							if ($scope.norms[j].Id === $scope.ngDialogData.dData.Norms[i].Id) {
        								$scope.norms[j].checked = true;
        								break;
        							}
        							else
        								$scope.norms[j].checked = false;
        						}
        					}

        			} else {
        				notification.error({
        					message: "Error al recuperar las normas.",
        					delay: 5000
        				});
        			}
        		});
        	}

        	getNorms();

        	function getTiposServicios() {
        		generalFactory.getAll('/tiposervicio/refreshList', { page: 1, pageSize: 100 }).then(function (response) {
        			if (response.success) {
        				$scope.tiposervicioList = response.elements;

        				if ($scope.ngDialogData.dData.Id !== undefined)
        					for (var i = 0; i < $scope.ngDialogData.dData.TiposServicios.length; i++) {
        						for (var j = 0; j < $scope.tiposervicioList.length; j++) {
        							if ($scope.tiposervicioList[j].checked) {
        								continue;
        							}
        							if ($scope.tiposervicioList[j].Id === $scope.ngDialogData.dData.TiposServicios[i].Id) {
        								$scope.tiposervicioList[j].checked = true;
        								break;
        							}
        							else
        								$scope.tiposervicioList[j].checked = false;
        						}
        					}

        			} else {
        				notification.error({
        					message: "Error al recuperar los Tipos de Servicios.",
        					delay: 5000
        				});
        			}
        		});
        	}

        	getTiposServicios();

        	function getStatus() {
        		$http.post('/status/refreshList', { page: 1, pageSize: 100 })
				.success(function (response) {
					if (response.success === true) {
						$scope.statusList = response.elements;
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

        	getStatus();

        	$scope.normChecked = function (a) {
        		a.checked = !a.checked;
        	}

        	$scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nuevo método" : "Editar método " + $scope.ngDialogData.dData.Name;

        	$scope.acceptDialog = function () {
        		var error = false;
        		var normList = new Array();
        		for (var i = 0; i < $scope.norms.length; i++) {
        			if (!$scope.norms[i].checked) {
        				continue;
        			}
        			normList.push({ Id: $scope.norms[i].Id });
        		}

        		var tipoServiciosList = new Array();
        		for (i = 0; i < $scope.tiposervicioList.length; i++) {
        			if (!$scope.tiposervicioList[i].checked) {
        				continue;
        			}
        			tipoServiciosList.push({ Id: $scope.tiposervicioList[i].Id });
        		}

        		if ($scope.ngDialogData.dData.Name === undefined) {
        			notification.error({
        				message: "Nombre del método indefinido",
        				delay: errorDelay
        			});
        			return;
        		}

        		//if (($scope.ngDialogData.dData.RequiredVolume !== undefined) && ($scope.ngDialogData.dData.RequiredVolume < 0)) {
        		//	notification.error({
        		//		message: "El volúmen requerido debe ser positivo",
        		//		delay: errorDelay
        		//	});
        		//	error = true;
        		//}
        		//if (($scope.ngDialogData.dData.MinimumVolume !== undefined) && ($scope.ngDialogData.dData.MinimumVolume < 0)) {
        		//	notification.error({
        		//		message: "El volúmen mínimo debe ser positivo",
        		//		delay: errorDelay
        		//	});
        		//	error = true;
        		//}
        		//if (($scope.ngDialogData.dData.DeliverTime !== undefined) && ($scope.ngDialogData.dData.DeliverTime < 0)) {
        		//	notification.error({
        		//		message: "El tiempo de entrega debe ser positivo",
        		//		delay: errorDelay
        		//	});
        		//	error = true;
        		//}
        		//if (($scope.ngDialogData.dData.ReportTime !== undefined) && ($scope.ngDialogData.dData.ReportTime < 0)) {
        		//	notification.error({
        		//		message: "El tiempo de reporte debe ser positivo",
        		//		delay: errorDelay
        		//	});
        		//	error = true;
        		//}
        		//if (($scope.ngDialogData.dData.AnalysisTime !== undefined) && ($scope.ngDialogData.dData.AnalysisTime < 0)) {
        		//	notification.error({
        		//		message: "El tiempo de análisis debe ser positivo",
        		//		delay: errorDelay
        		//	});
        		//	error = true;
        		//}
        		//if (($scope.ngDialogData.dData.MaxTimeBeforeAnalysis !== undefined) && ($scope.ngDialogData.dData.MaxTimeBeforeAnalysis < 0)) {
        		//	notification.error({
        		//		message: "El tiempo máximo previo al análisis debe ser positivo",
        		//		delay: errorDelay
        		//	});
        		//	error = true;
        		//}
        		//if (($scope.ngDialogData.dData.LabDeliverTime !== undefined) && ($scope.ngDialogData.dData.LabDeliverTime < 0)) {
        		//	notification.error({
        		//		message: "El tiempo de entrega al laboratorio debe ser positivo",
        		//		delay: errorDelay
        		//	});
        		//	error = true;
        		//}
        		//if ($scope.ngDialogData.dData.DetectionLimit !== undefined) {
        		//	if (($scope.ngDialogData.dData.DetectionLimit.Value !== undefined) && ($scope.ngDialogData.dData.DetectionLimit.Value < 0)) {
        		//		notification.error({
        		//			message: "El límite de detección debe ser positivo",
        		//			delay: errorDelay
        		//		});
        		//		error = true;
        		//	}
        		//	if (($scope.ngDialogData.dData.DetectionLimit.Decimals !== undefined) && ($scope.ngDialogData.dData.DetectionLimit.Decimals < 0)) {
        		//		notification.error({
        		//			message: "Los decimales del límite de detección deben ser positivos",
        		//			delay: errorDelay
        		//		});
        		//		error = true;
        		//	}
        		//}

        		//if ($scope.ngDialogData.dData.CuantificationLimit !== undefined) {
        		//	if (($scope.ngDialogData.dData.CuantificationLimit.Value !== undefined) && ($scope.ngDialogData.dData.CuantificationLimit.Value < 0)) {
        		//		notification.error({
        		//			message: "El límite de cuantificación debe ser positivo",
        		//			delay: errorDelay
        		//		});
        		//		error = true;
        		//	}
        		//	if (($scope.ngDialogData.dData.CuantificationLimit.Decimals !== undefined) && ($scope.ngDialogData.dData.CuantificationLimit.Decimals < 0)) {
        		//		notification.error({
        		//			message: "Los decimales del límite de cuantificación deben ser positivos",
        		//			delay: errorDelay
        		//		});
        		//		error = true;
        		//	}
        		//}

        		//if ($scope.ngDialogData.dData.Uncertainty !== undefined) {
        		//	if (($scope.ngDialogData.dData.Uncertainty.Decimals !== undefined) && ($scope.ngDialogData.dData.Uncertainty.Decimals < 0)) {
        		//		notification.error({
        		//			message: "Los decimales de incertidumbre deben ser positivos",
        		//			delay: errorDelay
        		//		});
        		//		error = true;
        		//	}
        		//}

        		//    if (!$scope.ngDialogData.dData.Matrixes || $scope.ngDialogData.dData.Matrixes.length === 0) {                    
        		//        notification.error({
        		//            message: "Debe introducir matrices al Método",
        		//            delay: errorDelay
        		//        });
        		//        error = true;                    
        		// }

        		// if (error === true)
        		//     return;

        		$scope.confirm({
        			Id: $scope.ngDialogData.dData.Id != null ? $scope.ngDialogData.dData.Id : 0,
        			Name: $scope.ngDialogData.dData.Name,
        			Active: $scope.ngDialogData.dData.Active,
        			Description: $scope.ngDialogData.dData.Description,
        			//RequiredVolume: $scope.ngDialogData.dData.RequiredVolume,
        			//MinimumVolume: $scope.ngDialogData.dData.MinimumVolume,
        			//InternetPublish: $scope.ngDialogData.dData.InternetPublish,
        			//DeliverTime: $scope.ngDialogData.dData.DeliverTime,
        			//DetectionLimit: $scope.ngDialogData.dData.DetectionLimit,
        			//CuantificationLimit: $scope.ngDialogData.dData.CuantificationLimit,
        			//ReportLimit: $scope.ngDialogData.dData.ReportLimit,
        			//MaxTimeBeforeAnalysis: $scope.ngDialogData.dData.MaxTimeBeforeAnalysis,
        			//LabDeliverTime: $scope.ngDialogData.dData.LabDeliverTime,
        			//ReportTime: $scope.ngDialogData.dData.ReportTime,
        			//AnalysisTime: $scope.ngDialogData.dData.AnalysisTime,
        			//Container: $scope.ngDialogData.dData.Container,
        			//Preserver: $scope.ngDialogData.dData.Preserver,
        			//Matrixes: $scope.ngDialogData.dData.Matrixes,
        			Norms: normList,
        			EntradaEnVigor: $scope.ngDialogData.dData.EntradaEnVigor,
        			Estado: $scope.ngDialogData.dData.Estado ? { Id: $scope.ngDialogData.dData.Estado.Id } : null,
        			TiposServicios: tipoServiciosList
        		});

        	}
        }

    ])
;
"use strict";
//************************ojo agregar al prototipo de los arreglos****************
function copy(o) {
    var output, v, key;
    output = Array.isArray(o) ? [] : {};
    for (key in o) {
        v = o[key];
        output[key] = (typeof v === "object") ? copy(v) : v;
    }
    return output;
}
//*******************************************************************************
angular.module("abcadmin")
    .controller("GroupController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', "dataProvider", "ngDialog", "Notification", "counterFactory", "$window", "SecurityFact", "generalFactory",
        function ($scope, $route, $location, $http, $cookies, $controller, dataProvider, ngDialog, notification, counterFactory, $window, securityFact, generalFactory) {

            $scope.isFullscreen = false;
            var errorDelay = 5000;

            $scope.spcSI = true;	//debe estar por defecto en false. Se puso en true pq si no no sale ningun elemento.
            $scope.spcNO = true;

            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------
            $scope.iframeHeight = $window.innerHeight;

            //-----------función q se llama para mostrar un div en fullscreen----------------
            $scope.toggleFullScreen = function () {

                $scope.isFullscreen = !$scope.isFullscreen;
                if ($scope.accessLevel === 1) {
                    if ($scope.isFullscreen)
                        $scope.col_defs.splice($scope.col_defs.length - 3, 3);
                    else {
                        $scope.col_defs.push({
                            field: "edit",
                            displayName: "Editar",
                            levels: [1, 2],
                            width: "50px",
                            cellTemplate: "<a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Editar'><span class='icon mif-pencil' style='color: #1b6eae; font-size: 1rem;'></span></a>",
                            //"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
                            cellTemplateScope: {
                                click: function (data) { // this works too: $scope.someMethod;
                                    data.d = false;
                                    $scope.editElement(data);
                                }
                            }
                        });

                        $scope.col_defs.push({
                            field: "duplicate",
                            displayName: "Duplicar",
                            levels: [1, 2],
                            width: "50px",
                            cellTemplate: "<a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Duplicar'><span class='icon mif-floppy-disk' style='color: #1b6eae; font-size: 1rem;'></span></a>",
                            //"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
                            cellTemplateScope: {
                                click: function (data) { // this works too: $scope.someMethod;
                                    data.d = true;
                                    $scope.editElement(data);
                                }
                            }
                        });

                        $scope.col_defs.push({
                            field: "",
                            displayName: "Dar baja/alta",
                            levels: [1],
                            cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title=\"{{row.branch.Active ? 'Dar baja' : 'Dar alta'}}\"><span ng-class=\"[row.branch.Active ? 'fa fa-thumbs-down' : 'fa fa-thumbs-up']\" ng-style=\"{'color': row.branch.Active ?'red':'green'}\" style='font-size:24px;'></span></a></div>",
                            cellTemplateScope: {
                                click: function (data) {
                                    $scope.setActivation(data, !data.Active);
                                }
                            },
                            width: "50px"
                        });
                    }
                }
            }
            //---------------------------------------------------------------------------------

            var filters = function (page) {
                return {
                    page: (page === null || page === undefined) ? $scope.currentPage : page,
                    pageSize: $scope.pageSize,
                    alta: $scope.alta,
                    baja: $scope.baja,
                    spcSi: $scope.spcSI,
                    spcNo: $scope.spcNO,
                    searchGeneral: $scope.searchGenericKey,
                    searchDescription: $scope.searchGenericDescription,
                    sucId: $scope.sucursalId,
                    bLine: $scope.bLine,
                    tServId: $scope.tServId,
                    CQ1Id: $scope.CQ1Id,
                    CQ2Id: $scope.CQ2Id,
                    CQ3Id: $scope.CQ3Id
                }
            };
            var address = function (elementType) {
                switch (elementType) {
                    case "parameter":
                        return "/param/saveparam";
                    case "group":
                        return "/group/savegroup";

                    default:
                        return "";
                }
            }

            var setActivationObjects = function (eObj, act) {
                return { id: eObj.Id, active: act }
            };

            var saveObject = function (promise) {
                if (promise.elemType === "group") {
                	return generalFactory.getGroupObject(promise);
                } else {
                	return generalFactory.getParamObject(promise);
                }
            }

            var save = function (promise) {
                $scope.initSaving = true;

                $http.post(address(promise.elemType), saveObject(promise))
                    .success(function (response) {
                        if (response.success) {
                            notification.success({
                                message: "Datos guardados correctamente.",
                                delay: 3000
                            });
                            $scope.initSaving = false;
                            $scope.RefreshList();
                            counterFactory.getAllCounts(true);
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

            var getFilters = function () {
                $http.post("/group/getFilters")
                    .success(function (response) {
                        if (response.success) {
                            $scope.sucursalList = response.sucList;
                            $scope.BusinessLineList = response.marketList;
                            $scope.tServList = response.tipoServicioList;
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
                "icon mif-stack", "Name",
                "/group/refreshlist", filters,
                "/group/saveactivestatus", setActivationObjects,
                "", null,
                null,
                "group"
            );
            
            //------------------------------to configure tree object----------------------------------------------------------------

            $scope.rowActions = [];

           
            $scope.my_tree = {};
            $scope.tableWidht = 150;
            $scope.expanding_property = {
                field: "Name",
                displayName: "Clave Específica",
                sortable: true,
                filterable: true,
                width: "150px"
            };

            $scope.col_defs = new Array();
            $scope.col_defs.push({
                field: "elemType",
                displayName: "Tipo de artículo",
                levels: [1, 2],
                cellTemplate: "<div>{{(row.branch[col.field]==='group')?'Grupo':'Parámetro'}}</div>",
                width: "150px"
            });

            $scope.col_defs.push({
                field: "Description",
                //levels: [1, 2],
                displayName: "Descripción Específica",
                width: "170px"
                //cellTemplate: "<div>{{row.branch[col.field]}}</div>"
            });

        	// Este no esta en CAT PARAMETROS, pero creo que es un elemento importante para grupos.
            //$scope.col_defs.push({
            //	field: "CantidadMuestreos",
            //	displayName: "Cantidad de Muestreos",
            //	levels: [2],
            //	cellTemplate: "<div>{{row.branch[col.field]}}</div>",
            //	tooltip: "Cantidad de Muestreos (Muestreos Compuestos)",
            //	width: "150px"
            //});

            $scope.col_defs.push({
                field: "Matrixes",
                displayName: "Matriz",
                levels: [1, 2],
                cellTemplate: "<div><span style='cursor:pointer; font-size:1.2rem' ng-click='cellTemplateScope.click(row.branch)' class='icon mif-windy4'></span></div>",
                cellTemplateScope: {
                    click: function (data) {
                        var pAddress = data.elemType === "group" ? "/group/getMatrixes" : "/param/getMatrixes";
                        $http.post(pAddress, { Id: data.Id })
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
                },
                width: "50px"

            });

            $scope.col_defs.push({
                field: "Matrixes",
                displayName: "Línea de Negocio", //mercado
                levels: [1, 2],
                cellTemplate: "<div  class='text-uppercase'>{{row.branch[col.field][0].BaseMatrix.Mercado}}</div>",
                width: "150px"
            });

            $scope.col_defs.push({
                field: "TipoServicio",
                displayName: "Tipo de Servicio",
                levels: [1, 2],
                cellTemplate: "<div>{{row.branch[col.field].Name?row.branch[col.field].Name:'N/A'}}</div>",
                tooltip: "Tipo de Servicio",
                width: "150px"
            });

        	// Linea (Intelisis)

            $scope.col_defs.push({
            	field: "ClasificacionQuimica1",
            	displayName: "Clasificación Química (Nivel I)",
                cellTemplate: "<div>{{row.branch[col.field].Name?row.branch[col.field].Name:'N/A'}}</div>",
            	levels: [1, 2],
            	width: "100px",
            	tooltip: "Clasificación Química (Nivel I)"
            });

            $scope.col_defs.push({
            	field: "ClasificacionQuimica2",
            	displayName: "Clasificación Química (Nivel II)",
                cellTemplate: "<div>{{row.branch[col.field].Name?row.branch[col.field].Name:'N/A'}}</div>",
            	levels: [1, 2],
            	width: "100px",
            	tooltip: "Clasificación Química (Nivel II)"
            });

            $scope.col_defs.push({
            	field: "ClasificacionQuimica3",
            	displayName: "Clasificación Química (Nivel III)",
                cellTemplate: "<div>{{row.branch[col.field].Name?row.branch[col.field].Name:'N/A'}}</div>",
            	levels: [1, 2],
            	width: "100px",
            	tooltip: "Clasificación Química (Nivel III)"
            });

            $scope.col_defs.push({
                field: "CentroCosto",
                displayName: "Centro de costo",
                tooltip: "Centro de costo",
                levels: [1, 2],
                cellTemplate: "<div style='text-align: center;'>{{row.branch[col.field].Number}}</div>",
                width: "80px"
            });
			
            $scope.col_defs.push({
                field: "Precio",
                displayName: "Precio de Lista",
                levels: [1, 2],
                cellTemplate: "<div style='text-align: center;'>{{row.branch[col.field].Value}}({{row.branch[col.field].Currency.Name}})</div>",
                //"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
                width: "70px"
            });

            $scope.col_defs.push({
                field: "Active",
                displayName: "Estatus",
                levels: [1, 2],
                cellTemplate: "<div style='text-align: center;'>{{row.branch[col.field] === false ? 'BAJA' : 'ALTA'}}</div>",
                width: "50px"
            });
			
            //$scope.col_defs.push({
            //    field: "MaxPermitedLimit",
            //    displayName: "LMP",
            //    tooltip: "Límite Máximo Permitido",
            //    levels: [1, 2, 3],
            //    cellTemplate: "<div style='text-align: center;'><span style='cursor:pointer; font-size:1.2rem' ng-if=\"row.branch.elemType=='parameter'\" ng-click='cellTemplateScope.click(row.branch)' class='mif-thermometer2 icon'></span></div>",
            //    cellTemplateScope: {
            //        click: function (data) {

            //            $http.post("/param/getallmaxpermitedlimit", { Id: data.Id })
            //             .success(function (response) {
            //                 if (response.success) {

            //                     ngDialog.openConfirm({
            //                         template: "Shared/ShowLMP",
            //                         controller: "SetLMPDialogController",
            //                         className: "ngdialog-theme-default",
            //                         preCloseCallback: "preCloseCallbackOnScope",
            //                         data: { dData: response.LMP, paramKey: data.ParamUniquekey },
            //                         closeByEscape: true
            //                     })
            //                .then(function (promise) {
            //                    //Accepting

            //                }, function (reason) {
            //                    //Rejecting

            //                });

            //                 }
            //                 $scope.isLoading = false;
            //             })
            //     .error(function () {
            //         notification.error({
            //             message: "Error en la conexión con el servidor.", delay: 5000
            //         });
            //         $scope.isLoading = false;
            //     });

            //        }
            //    },
            //    width: "50px"
        	//});

            $scope.col_defs.push({
            	field: "DecimalesReporte",
            	displayName: "Decimales Reporte",
            	tooltip: "Decimales para Reporte",
            	levels: [1, 2],
            	cellTemplate: "<div style='text-align: center;'>{{row.branch[col.field]===null?'N/A':row.branch[col.field]}}</div>",
            	width: "90px"

            });

            $scope.col_defs.push({
                field: "MostrarLista",
                displayName: "Mostrar Comp.",
                levels: [1, 2],
                cellTemplate: "<div style='text-align: center;'>{{row.branch[col.field] === true ? 'SI' : row.branch[col.field] === undefined ? '' : 'NO'}}</div>",
                tooltip: "Mostrar Lista de Componentes del Grupo",
                width: "50px"
            });

            $scope.col_defs.push({
                field: "DispParam",
                displayName: "DPGrupo",
                cellTemplate: "<div style='text-align: center;'>{{(row.branch[col.field]===null)?'N/A':row.branch[col.field].Name}}</div>",
                levels: [2],
                tooltip: "Disparador de los Parámetros del Grupo",
                width: "100px"
            });
			
            $scope.col_defs.push({
                field: "PublishInAutolab",
                displayName: "Solo para cotizar",
                levels: [1, 2],
                cellTemplate: "<div style='text-align: center;'>{{row.branch[col.field] === false ? 'Si' : 'No'}}</div>",
                tooltip: "Solo para cotizar",
                width: "90px"
            });

            // $scope.col_defs.push({
            //    field: "PerWeekCapacity",
            //    displayName: "CS",
            //    tooltip: "Capacidad Instalada/Semana (Máxima)",
            //    levels: [1, 2, 3],
            //    cellTemplate: "<div ng-show='row.branch[col.field] !== undefined'>{{row.branch[col.field] === null ? '-' : row.branch[col.field]}}</div>"
            // });

            // $scope.col_defs.push({
            //    field: "PerTurnCapacity",
            //    displayName: "CT",
            //    tooltip: "Capacidad Instalada/Semana (Trabajo Combinado)",
            //    levels: [1, 2, 3],
            //    cellTemplate: "<div ng-show='row.branch[col.field] !== undefined'>{{row.branch[col.field] === null ? '-' : row.branch[col.field]}}</div>"
            //});

            $scope.col_defs.push({
            	field: "Week",
            	displayName: "Lun",
            	width: "50px",
            	levels: [1, 2],
            	cellTemplate: "<div>{{row.branch[col.field].Monday === true ? 'SI' : row.branch[col.field].Monday === undefined ? '' : 'NO'}}</div>",
            	tooltip: "Se programa el Lunes"
            });

            $scope.col_defs.push({
            	field: "Week",
            	displayName: "Mar",
            	levels: [1, 2],
            	width: "50px",
            	cellTemplate: "<div>{{row.branch[col.field].Tuesday === true ? 'SI' : row.branch[col.field].Monday === undefined ? '' : 'NO'}}</div>",
            	tooltip: "Se programa el Martes"
            });

            $scope.col_defs.push({
            	field: "Week",
            	displayName: "Mie",
            	levels: [1, 2],
            	width: "50px",
            	cellTemplate: "<div>{{row.branch[col.field].Wednesday === true ? 'SI' : row.branch[col.field].Monday === undefined ? '' : 'NO'}}</div>",
            	tooltip: "Se programa el Miércoles"
            });

            $scope.col_defs.push({
            	field: "Week",
            	displayName: "Jue",
            	levels: [1, 2],
            	width: "50px",
            	cellTemplate: "<div>{{row.branch[col.field].Thursday === true ? 'SI' : row.branch[col.field].Monday === undefined ? '' : 'NO'}}</div>",
            	tooltip: "Se programa el Jueves"
            });

            $scope.col_defs.push({
            	field: "Week",
            	displayName: "Vie",
            	levels: [1, 2],
            	width: "50px",
            	cellTemplate: "<div>{{row.branch[col.field].Friday === true ? 'SI' : row.branch[col.field].Monday === undefined ? '' : 'NO'}}</div>",
            	tooltip: "Se programa el Viernes"
            });

            $scope.col_defs.push({
            	field: "Week",
            	displayName: "Sáb",
            	levels: [1, 2],
            	width: "50px",
            	cellTemplate: "<div>{{row.branch[col.field].Saturday === true ? 'SI' : row.branch[col.field].Monday === undefined ? '' : 'NO'}}</div>",
            	tooltip: "Se programa el Sábado"
            });

            $scope.col_defs.push({
            	field: "Week",
            	displayName: "Dom",
            	levels: [1, 2],
            	width: "50px",
            	cellTemplate: "<div>{{row.branch[col.field].Sunday === true ? 'SI' : row.branch[col.field].Monday === undefined ? '' : 'NO'}}</div>",
            	tooltip: "Se programa el Domingo"
            });

            $scope.col_defs.push({
                field: "SellSeparated",
                displayName: "SVS",
                levels: [1, 2],
                cellTemplate: "<div ng-style=\"{'background-color': (row.branch[col.field] === true) ? 'transparent' : 'red'}\">{{row.branch[col.field] === true ? 'Si' : 'No vender por separado'}}</div>",
                tooltip: "Se vende por separado",
                width: "150px"
            });

            $scope.col_defs.push({
                field: "CuentaEstadistica",
                displayName: "CPE",
                levels: [1, 2],
                cellTemplate: "<div style='text-align: center;' >{{row.branch[col.field] === true ? 'SI' : 'NO'}}</div>",
                tooltip: "Cuenta para Estadística",
                width: "50px"
            });

            //$scope.col_defs.push({
            //    field: "ParamPrintResults",
            //    displayName: "Imp. result",
            //    levels: [2],
            //    width: "50px",
            //    cellTemplate: "<div>{{row.branch[col.field].Yes === true ? 'SI' : 'NO'}}</div>"
            //});
            $scope.col_defs.push({
                field: "ImpResultado",
                displayName: "Imp. result",
                levels: [2],
                width: "50px",
                cellTemplate: "<div>{{row.branch[col.field].Yes === true ? 'SI' : 'NO'}}</div>"
            });

            $scope.col_defs.push({
            	field: "Sucursal",
            	displayName: "Sucursal/Instalación",
            	levels: [1],
            	width: "150px",
            	tooltip: "Sucursal/Instalación del Grupo",
                cellTemplate: "<div>{{(row.branch.elemType==='group' && row.branch[col.field].Name!=undefined)?row.branch[col.field].Name + '/' + row.branch[col.field].Region.Name:'N/A'}}</div>"
            });

            for (var j = 0; j < $scope.col_defs.length; j++) {
            	if ($scope.col_defs[j]["width"] === undefined) {
            		$scope.tableWidht += 100;
            		continue;
            	}
            	$scope.tableWidht += 1 * $scope.col_defs[j]["width"].slice(0, $scope.col_defs[j]["width"].length - 2);
            }

            var printAccess = function () {
	            if ($scope.accessLevel === 1) {
	            	$scope.col_defs.push({
	            		field: "edit",
	            		displayName: "Editar",
	            		levels: [1, 2],
	            		cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Editar'><span class='icon mif-pencil' style='color: #1b6eae; font-size: 1rem;'></span></a></div>",
	            		//"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
	            		cellTemplateScope: {
	            			click: function (data) { // this works too: $scope.someMethod;
	            				data.d = false;
	            				$scope.editElement(data);
	            			}
	            		},
	            		width: "50px"
	            	});
		            $scope.tableWidht += 50;

	            	$scope.col_defs.push({
	            		field: "duplicate",
	            		displayName: "Duplicar",
	            		levels: [1, 2],
	            		width: "50px",
	            		cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Duplicar'><span class='icon mif-floppy-disk' style='color: #1b6eae; font-size: 1rem;'></span></a></div>",
	            		//"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
	            		cellTemplateScope: {
	            			click: function (data) { // this works too: $scope.someMethod;
	            				data.d = true;
	            				$scope.editElement(data);
	            			}
	            		}
	            	});
	            	$scope.tableWidht += 50;

	            	$scope.col_defs.push({
	            		field: "",
	            		displayName: "Dar baja/alta",
	            		levels: [1],
	            		width: "50px",
	            		cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title=\"{{row.branch.Active ? 'Dar baja' : 'Dar alta'}}\"><span ng-class=\"[row.branch.Active ? 'fa fa-thumbs-down' : 'fa fa-thumbs-up']\" ng-style=\"{'color': row.branch.Active ?'red':'green'}\" style='font-size:24px;'></span></a></div>",
	            		cellTemplateScope: {
	            			click: function (data) {
	            				$scope.setActivation(data, !data.Active);
	            			}
	            		}
	            	});
	            	$scope.tableWidht += 50;
	            }

            }
			
            if ($scope.accessLevel === null) {
            	securityFact.subscribe($scope, function updateAccessLgroup() {
            		printAccess();
            	});
            }
            else
            	printAccess();
            
            $scope.editElement = function (data) {
                var tElement = {};
                angular.extend(tElement, data);
                $("#loadingSpinners").show();

                var id = (data === null) ? 0 : data.Id;
                var actionpost = (data != null && data.elemType === "parameter") ? "/param/GetRootPrintResults" : "/group/getparamsforgroup";
                var objPost = (data != null && data.elemType === "parameter") ? { id: id } : { id: id, page: 1, pageSize: 10, searchGeneral: "" };
                $http.post(actionpost, objPost)
                    .success(function (response) {
                        if (response.success) {
                            if (data != null && data.elemType === "group") {
                                var params = response.paramList;
                                for (var i = 0; i < params.length; i++) {
                                    if (params[i].ParamPrintResults == null)
                                        params[i].ParamPrintResults = { Id: 0, Yes: false, }
                                    for (var j = 0; j < data.children.length; j++) {
                                        if (params[i].Id === data.children[j].Id && params[i].elemType === data.children[j].elemType) {
                                            params[i].check = true;
                                            break;
                                        }
                                    }
                                }
                            } else if (data != null && data.elemType === "parameter") {
	                            response.paramPrintResult === null
		                            ? tElement.ParamPrintResults = { Id: 0, Yes: false }
		                            : tElement.ParamPrintResults =
		                            {
			                            Id: response.paramPrintResult.Id,
			                            Yes: response.paramPrintResult.Yes
		                            };
                                tElement.DecimalesReporte = response.decimalReport;
                            } else {
                                var params = response.paramList;
                                for (var i = 0; i < params.length; i++) {
                                    params[i].ParamPrintResults = { Id: 0, Yes: false }
                                }
                            }

                            var template = (id === 0 || data.elemType === "group") ? "Shared/EditGroup" : "Shared/EditParam";
                            var dialogController = (id === 0 || data.elemType === "group") ? "NewGroupController" : "NewParamController";
							
                            ngDialog.openConfirm({
                                template: template,
                                controller: dialogController,
                                className: "ngdialog-theme-default ngdialog-width2000",//{{!data.elemType || data.elemType === 'group'?'ngdialog-theme-default ngdialog-width2000':'ngdialog-theme-default ngdialog-width1000'}}
                                preCloseCallback: "preCloseCallbackOnScope",
                                data: {
                                	dData: tElement,
                                	paramList: params,
                                    maxpermitedlimitList: response.PermitedLimits,
                                    total: response.total
                                },
                                closeByEscape: true
                            })
                                .then(function (promise) {
                                    //Accepting
                                    save(promise);
                                    getFilters();
                                }, function (reason) {
                                    //Rejecting
                                });
                        } else {
                            notification.error({
                                message: "Error al recuperar los datos. " + response.err,
                                delay: 5000
                            });
                        }
                        $scope.isLoading = false;
                    })
                    .error(function () {
                        notification.error({
                            message: "Error en la conexión con el servidor.",
                            delay: errorDelay
                        });
                        $scope.isLoading = false;
                    });
            }
			
            // Group specifics
           // $scope.SearchGeneral = true;
            $scope.FilterActive = true;
            $scope.SucursalFilterActive = true;
            $scope.ParamFilterActive = true;
            $scope.BusinessLineFilterActive = true;
            $scope.TipoServFilter = true;
            $scope.CQ1Filter = true;
            $scope.CQ2Filter = true;
            $scope.CQ3Filter = true;
            $scope.boxHeaderTitleActive = "Grupos activos";
            $scope.boxHeaderTitleDeactive = "Grupos de baja";
            $scope.FilterSoloParaCotizar = true;
            //$scope.boxHeaderTitleActive = "Grupos activos";
            //$scope.boxHeaderTitleDeactive = "Grupos de baja";

            $scope.panels = [
                {
                    index: 0,
                    name: "FILTROS",
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: "TABLA DE GRUPOS",
                    state: true,
                    size: 9
                }
            ];
        }
    ])
     .controller("dlgController", ["$scope", function ($scope) {

         $scope.dialogTitle = "Matrices";

         $scope.acceptDialog = function () {

             $scope.confirm({

             });
         }
     }
     ])
    .controller("NewGroupController",
    [
        "$scope", "$routeParams", "$location", "$http", "$controller", "SecurityFact", "Notification", "ngDialog", "generalFactory",
        function ($scope,  $routeParams, $location, $http, $controller, securityFact, notification, ngDialog, generalFactory) {

            var errorDelay = 5000;
            var searchParamKey = "";
            $scope.panelHeight = 390;
            $scope.isFullscreen = false;
            //-----------función q se llama para mostrar un div en fullscreen----------------
            $scope.toggleFullScreen = function () {
               
                $scope.isFullscreen = !$scope.isFullscreen;
                $scope.panelHeight = $scope.isFullscreen ? 600 :390;
             }
            //---------------------------------------------------------------------------------


            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                 "Nuevo Grupo" : "Editar Grupo " + $scope.ngDialogData.dData.Name;
            var paramList = $scope.ngDialogData.paramList;
            $scope.paramList = paramList;
            $scope.pageTotal = $scope.ngDialogData.total;
            $scope.pageSize = 10;
            $scope.currentPage = 1;
            $scope.my_tree = {};
            $scope.tableWidht = 150;

            if ($scope.ngDialogData.dData.children)
                $scope.ngDialogData.dData.Parameters = copy($scope.ngDialogData.dData.children);
			
            $scope.expanding_property = {
                field: "Name",
                displayName: "Clave Específica",
                sortable: true,
                filterable: true,
                width: "150px"
            };

            $scope.col_defs = new Array();

            //$scope.col_defs.push({
            //    field: "ParamPrintResults",
            //    displayName: "Imp. result",
            //    levels: [1],
            //    width: "50px",
            //    cellTemplate: "<input type='checkbox' ng-click='cellTemplateScope.click(row.branch)' data-ng-model='row.branch[col.field].Yes'>",
            //    cellTemplateScope: {
            //        click: function (data) { // this works too: $scope.someMethod;
            //            if (data.check) {
            //                for (var i = 0; i < $scope.ngDialogData.dData.Parameters.length; i++) {
            //                    if ($scope.ngDialogData.dData.Parameters[i].Id === data.Id) {
            //                        if ($scope.ngDialogData.dData.Parameters[i].ParamPrintResults) {
            //                            $scope.ngDialogData.dData.Parameters[i].ParamPrintResults.Yes = !data.ParamPrintResults.Yes;
            //                            break;
            //                        } 
            //                    }
            //                }
                         
            //            } else {
            //                data.check = true;
            //                $scope.ngDialogData.dData.Parameters.push({ Id: data.Id, Name: data.Name, elemType: data.elemType, ParamPrintResults: { Id: 0, Yes: true } });
            //            }
            //        }
            //    }
            //});

            $scope.col_defs.push({
                field: "Description",
                displayName: "Descripción Específica",
                width: "170px"
            });

            $scope.col_defs.push({
                field: "GenericDescription",
                displayName: "Descripción Genérica",
                width: "150px"
            });

            $scope.col_defs.push({
                field: "Matrixes",
                displayName: "Matriz",
                levels: [1],
                cellTemplate: "<div><span style='cursor:pointer; font-size:1.2rem' ng-click='cellTemplateScope.click(row.branch)' class='icon mif-windy4'></span></div>",
                cellTemplateScope: {
                    click: function (data) {
                        $http.post("/param/getMatrixes", { Id: data.Id })
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
                },
                width: "50px"

            });

            $scope.col_defs.push({
                field: "Matrixes",
                displayName: "Línea de Negocio", //mercado
                levels: [1],
                cellTemplate: "<div  class='text-uppercase'>{{row.branch[col.field][0].BaseMatrix.Mercado}}</div>",
                width: "150px"
            });

            $scope.col_defs.push({
                field: "TipoServicio",
                displayName: "Tipo de Servicio",
                levels: [1],
                cellTemplate: "<div>{{row.branch[col.field].Name}}</div>",
                tooltip: "Tipo de Servicio",
                width: "150px"
            });

            $scope.col_defs.push({
                field: "CentroCosto",
                displayName: "Centro de costo",
                tooltip: "Centro de costo",
                levels: [1],
                cellTemplate: "<div style='text-align: center;'>{{row.branch[col.field].AnalyticsAreaKey}}</div>",
                width: "80px"
            });

            $scope.col_defs.push({
                field: "Precio",
                displayName: "Precio de Lista",
                levels: [1],
                cellTemplate: "<div style='text-align: center;'>{{row.branch[col.field].Value}}({{row.branch[col.field].Currency.Name}})</div>",
                //"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
                width: "70px"
            });

            $scope.col_defs.push({
                field: "Active",
                displayName: "Estatus",
                levels: [1],
                cellTemplate: "<div style='text-align: center;'>{{row.branch[col.field] === false ? 'BAJA' : 'ALTA'}}</div>",
                width: "50px"
            });

            $scope.col_defs.push({
                field: "MaxPermitedLimit",
                displayName: "LMP",
                tooltip: "Límite Máximo Permitido",
                levels: [1],
                cellTemplate: "<div style='text-align: center;'><span style='cursor:pointer; font-size:1.2rem' ng-if=\"row.branch.elemType=='parameter'\" ng-click='cellTemplateScope.click(row.branch)' class='mif-thermometer2 icon'></span></div>",
                cellTemplateScope: {
                    click: function (data) {

                        $http.post("/param/getallmaxpermitedlimit", { Id: data.Id })
                         .success(function (response) {
                             if (response.success) {

                                 ngDialog.openConfirm({
                                     template: "Shared/ShowLMP",
                                     controller: "SetLMPDialogController",
                                     className: "ngdialog-theme-default",
                                     preCloseCallback: "preCloseCallbackOnScope",
                                     data: { dData: response.LMP, paramKey: data.ParamUniquekey },
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
                },
                width: "50px"
            });

            $scope.col_defs.push({
                field: "PublishInAutolab",
                displayName: "Solo para cotizar",
                levels: [1],
                cellTemplate: "<div style='text-align: center;'>{{row.branch[col.field] === false ? 'Si' : 'No'}}</div>",
                tooltip: "Solo para cotizar",
                width: "90px"
            });

            $scope.col_defs.push({
                field: "SellSeparated",
                displayName: "SVS",
                levels: [1],
                cellTemplate: "<div ng-style=\"{'background-color': (row.branch[col.field] === true) ? 'transparent' : 'red'}\">{{row.branch[col.field] === true ? 'Si' : 'No se vende por separado'}}</div>",
                tooltip: "Se vende por separado",
                width: "50px"
            });

            $scope.col_defs.push({
                field: "CuentaEstadistica",
                displayName: "CPE",
                levels: [1],
                cellTemplate: "<div style='text-align: center;' >{{row.branch[col.field] === true ? 'SI' : 'NO'}}</div>",
                tooltip: "Cuenta para Estadística",
                width: "50px"
            });


            // cellTemplate: "<div>{{row.branch[col.field].Yes==undefined || row.branch[col.field].Yes==false ? 'NO': 'SI'}}</div>"

            if ($scope.ngDialogData.dData.ParamPrintResults == undefined)
                $scope.ngDialogData.dData.ParamPrintResults = [];

            if ($scope.ngDialogData.dData.children)
                $scope.ngDialogData.dData.Parameters = copy($scope.ngDialogData.dData.children);
            else
                $scope.ngDialogData.dData.Parameters = [];


            $scope.rowActions = [{
                levels: [1],
                //icon: "icon-plus  glyphicon glyphicon-plus  fa fa-plus",
                cellTemplate: "<input style='margin:0' ng-click='cellTemplateScope.click(row.branch)' ng-model='row.branch.check' type='checkbox'>",
                cellTemplateScope: {
                    click: function (data) { // this works too: $scope.someMethod;
                        if (data.check) {
                            for (var i = 0; i < $scope.ngDialogData.dData.Parameters.length; i++)
                                if ($scope.ngDialogData.dData.Parameters[i].Id === data.Id) {
                                    $scope.ngDialogData.dData.Parameters.splice(i, 1);
                                    data.ParamPrintResults.Yes = false;
                                    break;
                                }
                        }
                        else
                            $scope.ngDialogData.dData.Parameters.push({ Id: data.Id, Name: data.Name, elemType: data.elemType, Description: data.Description });
                    }
                }
            }];

            for (var j = 0; j < $scope.col_defs.length; j++) {
                if ($scope.col_defs[j]["width"] === undefined) {
                    $scope.tableWidht += 100;
                    continue;
                }
                $scope.tableWidht += 1 * $scope.col_defs[j]["width"].slice(0, $scope.col_defs[j]["width"].length - 2);
            }

            function getTiposServicio() {
                generalFactory.getAll('/tiposervicio/RefreshList', { activeOption: true })
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
            $scope.searchfunction = function (searchGeneral) {
                searchParamKey = searchGeneral;
                paramList = [];
                $scope.getpage(1, $scope.pageSize);
            }

            $scope.getpage = function (page, pageSize) {

                var id = ($scope.ngDialogData.dData.Id) ? $scope.ngDialogData.dData.Id : 0;

                if (!paramList[(page - 1) * pageSize] || searchParamKey) {
                    
                    $scope.isLoading = true;
                    $http.post("/group/getparamsforgroup", { id: id, page: page, pageSize: pageSize, searchGeneral: searchParamKey })
                    .success(function (response) {
                        if (response.success) {
                            var params = response.paramList;
                            for (var i = 0; i < params.length; i++) {
                                if (params[i].ParamPrintResults == null)
                                    params[i].ParamPrintResults = { Id: 0, Yes: false }
                                if ($scope.ngDialogData.dData.children)
                                    for (var j = 0; j < $scope.ngDialogData.dData.children.length; j++) {
                                        if (params[i].Id === $scope.ngDialogData.dData.children[j].Id) {
                                            params[i].check = true;
                                            break;
                                        }
                                    }
                            }
                            
                            for (i = 0; i < params.length ; i++)
                                paramList[(page - 1) * pageSize + i] = copy(params[i]);

                            $scope.pageTotal = response.total;

                            var end = (page - 1) * pageSize + $scope.pageSize;
                            $scope.paramList = paramList.slice((page - 1) * pageSize, end);

                        } else {
                            notification.error({
                                message: "Error al recuperar los datos. " + response.err,
                                delay: 5000
                            });
                        }
                        $scope.isLoading = false;
                    })
                        .error(function () {
                            notification.error({
                                message: "Error en la conexión con el servidor.",
                                delay: errorDelay
                            });
                            $scope.isLoading = false;
                        });
                } else {
                    var end = (page - 1) * pageSize + $scope.pageSize;
                    $scope.paramList = paramList.slice((page - 1) * pageSize, end);
                }
            }

            //$scope.getpage(1, $scope.pageSize);
            $scope.getComplexKeys = function (cs) {
                cs.keys = "";
                for (var i = 0; i < cs.CantidadMuestreos; i++) {
                    cs.keys += cs.Param.ParamUniquekey + "-" + (i + 1) + ((i < cs.CantidadMuestreos-1) ? ", " : "");
                }
                return;

            }
            function getMatrixes() {
                $http.post("/param/getbasematrix", { active: true })
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

            var getClasificacionesQuimicas = function () {
                $http.post("/group/GetClasificacionesQuimicas", { level: 1 })
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

                $http.post("/group/GetClasificacionesQuimicas", { level: 2 })
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

                $http.post("/group/GetClasificacionesQuimicas", { level: 3 })
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

            $scope.currentStep = null;
            $scope.complexSamplings = new Array();

            $scope.nextPage = function () {
            	if ($scope.currentStep === "page2") {
            		$scope.complexSamplings.length = 0;
                    for (var l = 0; l < $scope.ngDialogData.dData.Parameters.length; l++) {
            			var flag = false;
            			if ($scope.ngDialogData.dData.ComplexSamplings !== undefined) {
            				for (var i = 0; i < $scope.ngDialogData.dData.ComplexSamplings.length; i++) {
                                if ($scope.ngDialogData.dData.Parameters[l].Id === $scope.ngDialogData.dData.ComplexSamplings[i].Param.Id) {
                                    //continue;
                                    var newKeys = "";
                                    for (var k = 0; k < $scope.ngDialogData.dData.ComplexSamplings[i].CantidadMuestreos; k++) {
                                        newKeys += $scope.ngDialogData.dData.Parameters[l].Name + "-" + (k + 1) + ((k < $scope.ngDialogData.dData.ComplexSamplings[i].CantidadMuestreos - 1) ? ", " : "");
                                    }
            						$scope.complexSamplings.push({
            							Id: $scope.ngDialogData.dData.ComplexSamplings[i].Id,
            							Param: {
                                            Id: $scope.ngDialogData.dData.Parameters[l].Id,
                                            ParamUniquekey: $scope.ngDialogData.dData.Parameters[l].Name,
                                            Description: $scope.ngDialogData.dData.Parameters[l].Description
							            },
            							CantidadMuestreos: $scope.ngDialogData.dData.ComplexSamplings[i].CantidadMuestreos,
            							check: true,
            							keys: newKeys

            						});
            						flag = true;
            						break;
            					}
            				}
            			}
                        if (!flag) {
                            //if ($scope.ngDialogData.dData.Parameters) {

            				$scope.complexSamplings.push({
            					Param: {
                                    Id: $scope.ngDialogData.dData.Parameters[l].Id,
                                    ParamUniquekey: $scope.ngDialogData.dData.Parameters[l].Name,
                                    Description: $scope.ngDialogData.dData.Parameters[l].Description
            					},
            					CantidadMuestreos: 0,
            					check: false
            				});
            			}
            		}
                } else if ($scope.currentStep === "page4" && $scope.ngDialogData.BaseMatrix) {
            		
            		$http.post('/group/GetSucursales', { bMatrixId: $scope.ngDialogData.BaseMatrix.Id })
					.success(function (response) {
						if (response.success === true) {
							$scope.sucursales = response.elements;
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
            }

            $scope.csChecked = function (cs) {
            	cs.check = !cs.check;
            }

            $scope.acceptDialog = function () {
            	var error = false;

                $scope.ngDialogData.dData.ParamPrintResults = [];
                
                for (var i = 0; i < $scope.ngDialogData.dData.Parameters.length; i++)
                    if ($scope.ngDialogData.dData.Parameters[i].ParamPrintResults && $scope.ngDialogData.dData.Parameters[i].ParamPrintResults.Yes) {
                        $scope.ngDialogData.dData.ParamPrintResults.push({
                            Id: $scope.ngDialogData.dData.Parameters[i].ParamPrintResults.Id,
                            Yes: $scope.ngDialogData.dData.Parameters[i].ParamPrintResults.Yes,
                            Parameter: { Id: $scope.ngDialogData.dData.Parameters[i].Id }
                        });
                    }

                if ($scope.ngDialogData.dData.Name === undefined) {
                    notification.error({
                        message: "Nombre del grupo indefinido",
                        delay: errorDelay
                    });
                    error = true;
                }

                if ($scope.ngDialogData.dData.Parameters.length <= 0) {
                    notification.error({
                        message: "Debe adicionar parámetros al grupo",
                        delay: errorDelay
                    });
                    error = true;
                }

                if ($scope.ngDialogData.dData.Sucursal === null) {
                	notification.error({
                		message: "Instalación/Sucursal indefinida",
                		delay: errorDelay
                	});
                	error = true;
                }

				for (j = 0; j < $scope.complexSamplings.length; j++) {
        			if ($scope.complexSamplings[j].CantidadMuestreos <= 0 &&
						$scope.complexSamplings[j].check === true) {
        				notification.error({
        					message: "La cantidad de muestreos debe ser estrictamente positiva",
        					delay: errorDelay
        				});
        				error = true;
        				break;
        			}
        		}

                if (error === true)
                	return;

                for (j = 0; j < $scope.complexSamplings.length; j++) {
                	if ($scope.complexSamplings[j].check === false) {
                		$scope.complexSamplings.splice(j, 1);
                		j--;
                	}
                }

                var packlist = [];
                if ($scope.ngDialogData.dData.Packages)
                    for (i = 0; i < $scope.ngDialogData.dData.Packages.length; i++)
                        packlist.push({ Id: $scope.ngDialogData.dData.Packages[i].Id });

                var paramlist = [];
                if ($scope.ngDialogData.dData.Parameters)
                    for (i = 0; i < $scope.ngDialogData.dData.Parameters.length; i++)
                        paramlist.push({ Id: $scope.ngDialogData.dData.Parameters[i].Id });

                $scope.confirm({
                    Id: ($scope.ngDialogData.dData.Id != null && !$scope.ngDialogData.dData.d) ?
                        $scope.ngDialogData.dData.Id : 0,
                    Name: $scope.ngDialogData.dData.Name,
                    Active: $scope.ngDialogData.dData.Active,
                    MaxPermitedLimit: $scope.ngDialogData.dData.MaxPermitedLimit,
                    Description: $scope.ngDialogData.dData.Description,
                    Parameters: paramlist,
                    Packages: packlist,
                    elemType: "group",
                    ParamPrintResults: $scope.ngDialogData.dData.ParamPrintResults,
                    Matrixes: $scope.ngDialogData.dData.Matrixes,
                    TipoServicio: $scope.ngDialogData.dData.TipoServicio,
                    DispParamId: ($scope.ngDialogData.dData.DispParam) ? $scope.ngDialogData.dData.DispParam.Id : 0,
                    SellSeparated: $scope.ngDialogData.dData.SellSeparated,
                    CuentaEstadistica: $scope.ngDialogData.dData.CuentaEstadistica,
                    MostrarLista: $scope.ngDialogData.dData.MostrarLista,
                    DecimalesReporte: $scope.ngDialogData.dData.DecimalesReporte,
                    Week: $scope.ngDialogData.dData.Week,
                    ComplexSamplings: $scope.complexSamplings,
                    ClasificacionQuimica1: $scope.ngDialogData.dData.ClasificacionQuimica1,
                    ClasificacionQuimica2: $scope.ngDialogData.dData.ClasificacionQuimica2,
                    ClasificacionQuimica3: $scope.ngDialogData.dData.ClasificacionQuimica3,
                    Sucursal: $scope.ngDialogData.dData.Sucursal
                });
            }
        }
    ]);
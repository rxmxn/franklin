"use strict";

angular.module("abcadmin")
    .controller("ParamController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', "SecurityFact", "$window", "generalFactory", "Notification", "ngDialog",
        function ($scope, $route, $location, $http, $cookies, $controller, securityFact, $window, generalFactory, notification, ngDialog) {
        	$scope.isFullscreen = false;
        	$scope.isFullscreen1 = false;
        	$scope.iframeHeight = $window.innerHeight;
        	var errorDelay = 5000;
        	
        	$scope.spcSI = true;	//debe estar por defecto en false. Se puso en true pq si no no sale ningun elemento.
        	$scope.spcNO = true;

        	// This controller groups similar functionalities of different controllers.
        	$controller('GeneralController', { $scope: $scope, $http: $http });
        	//-------------------------------------------------------------------------

        	//-----------función q se llama para mostrar un div en fullscreen----------------
        	$scope.toggleFullScreen = function (index) {
		        if (index === 1) {
		        	$scope.isFullscreen1 = !$scope.isFullscreen1;
		        } else {
        		$scope.isFullscreen = !$scope.isFullscreen;
        		if ($scope.accessLevel === 1) {
        			if ($scope.isFullscreen)
        				$scope.col_defs.splice($scope.col_defs.length - 4, 4);
        			else {
        				$scope.col_defs.push({
        					field: "ccChange",
        					displayName: "Cambiar CC",
        					tooltip: "Cambiar Centro de Costo",
        					levels: [1],
        					width: "100px",
        					cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Cambiar Centro de Costo'><span class='icon mif-broadcast' style='color: #1b6eae; font-size: 1rem;'></span></a></div>",
        					cellTemplateScope: {
        						click: function (data) { // this works too: $scope.someMethod;
        							$scope.ccChange(data);
        						}
        					}
        				});

        				$scope.col_defs.push({
        					field: "edit",
        					displayName: "Editar",
        					levels: [2],
        					width: "50px",
        					cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Editar'><span class='icon mif-pencil' style='color: #1b6eae; font-size: 1rem;'></span></a></div>",
        					cellTemplateScope: {
        						click: function (data) { // this works too: $scope.someMethod;
        							data.d = false;
        							$scope.edit(data);
        						}
        					}
        				});

        				$scope.col_defs.push({
        					field: "duplicate",
        					displayName: "Duplicar",
        					levels: [2],
        					width: "50px",
        					cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Duplicar'><span class='icon mif-floppy-disk' style='color: #1b6eae; font-size: 1rem;'></span></a></div>",
        					cellTemplateScope: {
        						click: function (data) { // this works too: $scope.someMethod;
        							data.d = true;
        							$scope.edit(data);
        						}
        					}
        				});

        				$scope.col_defs.push({
        					field: "",
        						displayName: "Dar baja/alta",
        					levels: [2],
        					width: "50px",
        						cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title=\"{{row.branch.Active ? 'Dar baja' : 'Dar alta'}}\"><span ng-class=\"[row.branch.Active ? 'fa fa-thumbs-down' : 'fa fa-thumbs-up']\" ng-style=\"{'color': row.branch.Active ?'red':'green'}\" style='font-size:24px;'></span></a></div>",
        					cellTemplateScope: {
        						click: function (data) {
        							$scope.setActivation(data, !data.Active);
        						}
        					}
        				});
        			}
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
        			searchGeneral: $scope.searchGeneral,
                    searchSpecificKey: $scope.searchSpecificKey,
                    searchGenericKey: $scope.searchGenericKey,
                    // searchGenericDescription: $scope.searchGenericDescription,
                    searchDescription: $scope.searchGenericDescription,
                    baseparamfiltered: $scope.baseparamfiltered,
                    //sucId: $scope.sucursalId,
                    bLine: $scope.bLine,
                    tServId: $scope.tServId,
                    CQ1Id: $scope.CQ1Id,
                    CQ2Id: $scope.CQ2Id,
                    CQ3Id: $scope.CQ3Id,
                    sucReId: $scope.sucReId,
                    sucVenId: $scope.sucVenId,
                    ramaId: $scope.ramaId,
                    CeCId: $scope.CeCId,
                    AnalyticMethodId: $scope.AnalyticMethodId,
                    ackId: $scope.ackId,
                    signatariosId: $scope.signatariosId,
                    EIDASId: $scope.EIDASId,
                    AnAreaId: $scope.AnAreaId,
                    AnnalistKeyId: $scope.AnnalistKeyId,
                    envaseId: $scope.envaseId,
                    preserverId: $scope.preserverId
        		}
        	};

          
            function getFilters() {
                $("#loadingSpinners").show();
                $http.post("/param/getFilters")
                    .success(function (response) {
                        if (response.success) {
                            $scope.BusinessLineList = response.marketList;
                            $scope.tServList = response.tServList;
                            $scope.CQ1List = response.cq1;
                            $scope.CQ2List = response.cq2;
                            $scope.CQ3List = response.cq3;
                            $scope.sucVenList = response.sucVenList;
                            $scope.sucReList = response.sucRealList;
                            $scope.ramaList = response.ramaList;
                            $scope.CeCList = response.CeCList;
                            $scope.AnalyticMethodList = response.AnalyliticMethodList;
                            $scope.ackList = response.AckList;
                            $scope.AnAreaList = response.AnAreaList;
                            $scope.signatariosList = response.signatariosList;
                            $scope.EIDASList = response.EIDASList;
                            $scope.envaseList = response.envaseList;
                            $scope.preserverList = response.preserverList;
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

        	var setActivationObjects = function (eObj, act) {
        		return { id: eObj.Id, active: act }
        	};

        	var saveObject = function (promise) {
        		return generalFactory.getParamObject(promise);
        			}

        	var editElement = function (element) {

        		return {
        			template: "Shared/EditParam",
        			controller: "NewParamController",
        			className: "ngdialog-theme-default ngdialog-width2000",
        			preCloseCallback: "preCloseCallbackOnScope",
        			data: { dData: element },
        			closeByEscape: true
        		}
        	}

        	$scope.RefreshList = function (page) {
        		$scope.isLoading = true;
                $("#loadingSpinners").show();
        		$http.post("/param/refreshlist", filters(page))
				.success(function (response) {
					if (response.success) {
						$scope.elementsList = response.elements;
						$scope.pageTotal = response.total;
						$scope.totalParam = response.totalParam;
						$scope.RefreshListMore(1);
                        
					} else {
						notification.error({
							message: "Error al recuperar los datos. " + response.err, delay: errorDelay
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
        	};

        	$scope.mainFunction(
                "icon mif-lab", "BaseParam.Name",
                //"/param/refreshlist", filters,
				null, null,
                "/param/saveactivestatus", setActivationObjects,
                "/param/saveparam", saveObject,
                editElement,
				"param"
                );

        	// Param specifics
            $scope.SearchGeneral = true;
            $scope.ParamFilterActive = true;
            $scope.genericKeyFilterActive = true;
        	$scope.FilterActive = true;
            // $scope.SucursalFilterActive = true;
        	$scope.FilterSoloParaCotizar = true;
            $scope.BusinessLineFilterActive = true;
            $scope.TipoServFilter = true;
            $scope.CQ1Filter = true;
            $scope.CQ2Filter = true;
            $scope.CQ3Filter = true;
            $scope.SucRealizaFilter = true;
            $scope.SucVendeFilter = true;
            $scope.RamaFilter = true;
            $scope.CeCFilter = true;
            $scope.AnalyticMethodFilter = true;
            $scope.AckFilter = true;
            $scope.SignatariosFilter = true;
            $scope.EIDASFilter = true;
            $scope.AnAreaFilter = true;
            $scope.EnvaseFilter = true;
            $scope.preserverFilter = true;
           // $scope.AnnalistFilter = true;

        	//$scope.boxHeaderTitleActive = "Parámetros activos";
        	//$scope.boxHeaderTitleDeactive = "Parámetros de baja";

        	$scope.panels = [
                {
                	index: 0,
                	name: 'Filtros',
                	state: false
                },
                {
                	index: 1,
                	name: "Tabla de Parámetros",
                	state: true
                }//, TODO: Descomentariar cuando se arregle lo de audit
				//{
				//	index: 2,
				//	name: "Historial",
				//	state: false
				//}
        	];

        	$scope.rowActions = [];

        	$scope.tableWidht = 150;
        	$scope.my_tree = {};

        	$scope.expanding_property = {
        		field: "Name",
        		displayName: "Clave Específica",
        		sortable: true,
        		filterable: true,
        		width: "250px"
        	};

        	$scope.col_defs = new Array();

        	//$scope.col_defs.push({
        	//    field: "elemType",
        	//    displayName: "Tipo de artículo",
        	//    levels: [1, 2],
        	//    cellTemplate: "<div class='text-uppercase'>{{(row.branch[col.field]==='group')?'Grupo':'Parámetro'}}</div>",
        	//    width: "150px"
        	//});

        	$scope.col_defs.push({
        		field: "Description",
        		//levels: [1, 2],
        		displayName: "Descripción Específica",
        		width: "250px"
        	});

        	$scope.col_defs.push({
        		field: "Metodo",
        		displayName: "Método Analítico/Muestreo",
        		levels: [2],
        		cellTemplate: "<div>{{row.branch[col.field].Name}}</div>",
        		tooltip: "Método",
        		width: "250px"
        	});

        	//$scope.col_defs.push({
        	//	field: "GenericKey",
        	//	displayName: "Clave Genérica",
        	//	width: "150px"
        	//});

        	//$scope.col_defs.push({
        	//	field: "GenericDescription",
        	//	displayName: "Descripción Genérica",
        	//	width: "150px"
        	//});

        	$scope.col_defs.push({
        		field: "Matrixes",
        		displayName: "Matriz",
        		levels: [2],
        		cellTemplate: "<div><a href='' ng-click='cellTemplateScope.click(row.branch)'>{{row.branch[col.field][0].Name}}</a></div>",
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
        		width: "150px"

        	});

        	$scope.col_defs.push({
        		field: "Matrixes",
        		displayName: "Línea de Negocio", //mercado
        		levels: [2],
        		cellTemplate: "<div class='text-uppercase'>{{row.branch[col.field][0].BaseMatrix.Mercado}}</div>",
        		width: "150px"
        	});

        	$scope.col_defs.push({
        		field: "TipoServicio",
        		displayName: "Tipo de Servicio",
        		levels: [2],
        		cellTemplate: "<div class='text-uppercase'>{{row.branch[col.field].Name}}</div>",
        		tooltip: "Tipo de Servicio",
        		width: "200px"
        	});

        	$scope.col_defs.push({
        		field: "ClasificacionQuimica1",
        		displayName: "Clasificación Química (Nivel I)",
                cellTemplate: "<div>{{row.branch[col.field].Name===undefined?'N/A':row.branch[col.field].Name}}</div>",
        		levels: [2],
        		width: "200px",
        		tooltip: "Clasificación Química (Nivel I)"
        	});

        	$scope.col_defs.push({
        		field: "ClasificacionQuimica2",
        		displayName: "Clasificación Química (Nivel II)",
                cellTemplate: "<div>{{row.branch[col.field].Name===undefined?'N/A':row.branch[col.field].Name}}</div>",
        		levels: [2],
        		width: "200px",
        		tooltip: "Clasificación Química (Nivel II)"
        	});

        	$scope.col_defs.push({
        		field: "ClasificacionQuimica3",
        		displayName: "Clasificación Química (Nivel III)",
                cellTemplate: "<div>{{row.branch[col.field].Name===undefined?'N/A':row.branch[col.field].Name}}</div>",
        		levels: [2],
        		width: "200px",
        		tooltip: "Clasificación Química (Nivel III)"
        	});

        	$scope.col_defs.push({
        		field: "SucursalVende",
        		displayName: "Sucursal/Instalación donde se vende",
        		levels: [2],
        		width: "150px",
        		cellTemplate: "<div>{{row.branch[col.field].Name}}</div>",

        	});

        	$scope.col_defs.push({
        		field: "SucursalRealiza",
        		levels: [2],
        		displayName: "Sucursal/Instalación donde se realiza",
        		cellTemplate: "<div>{{row.branch[col.field].Name}}</div>",
        		width: "150px"
        	});

        	$scope.col_defs.push({
        		field: "Rama",
        		displayName: "Rama",
        		cellTemplate: "<div>{{row.branch[col.field].Name}}</div>",
        		levels: [2],
        		width: "50px"
        	});

        	$scope.col_defs.push({
        		field: "CentroCosto",
        		displayName: "Centro de costo",
        		tooltip: "Centro de costo",
        		levels: [2],
        		cellTemplate: "<div style='text-align: center;'>{{row.branch[col.field].Number}}</div>",
        		width: "120px"
        	});

        	//function printColumnList() {

        	$scope.col_defs.push({
        		field: "Precio",
        		displayName: "Precio de Lista",
        		levels: [2],
        		cellTemplate: "<div style='text-align: center;'>{{row.branch[col.field].Value}}({{row.branch[col.field].Currency.Name}})</div>",
        		//"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
        		width: "120px"
        	});

        	$scope.col_defs.push({
        		field: "Active",
        		displayName: "Estatus",
        		levels: [2],
        		cellTemplate: "<div style='text-align: center;'>{{row.branch[col.field] === false ? 'BAJA' : 'ALTA'}}</div>",
        		width: "50px"
        	});

        	$scope.col_defs.push({
                field: "InternetPublish",
                displayName: "PI",
                levels: [2],
                cellTemplate: "<div>{{row.branch[col.field] === true ? 'Si' : 'No'}}</div>",
                width: "50px",
                tooltip: "Publicar en Internet"
            });

        	$scope.col_defs.push({
                field: "AnalyticsMethod",
        		displayName: "Técnica Analítica",
        		levels: [2],
                cellTemplate: "<div>{{row.branch[col.field].Name}}</div>",
        		width: "150px"
        	});

        	//$scope.col_defs.push({
        	//    field: "",
        	//    displayName: "LDM",
        	//    tooltip: "Límite de Detección del Método",
        	//    width: "50px"

        	//});

        	$scope.col_defs.push({
        		field: "MaxPermitedLimit",
        		displayName: "LMP",
        		tooltip: "Límite Máximo Permitido",
        		levels: [2],
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

        	//$scope.col_defs.push({
        	//    field: "",
        	//    displayName: "Decimales LDM",
        	//    tooltip: "Decimales para el Límite de Detección del Método",
        	//    width: "90px"

        	//});

        	//$scope.col_defs.push({
        	//    field: "",
        	//    displayName: "LPC",
        	//    tooltip: "Límite Práctico de Cuantificación",
        	//    width: "70px"

        	//});

        	//$scope.col_defs.push({
        	//    field: "",
        	//    displayName: "Decimales LPC ",
        	//    tooltip: "Decimales para el LPC",
        	//    width: "70px"

        	//});

        	$scope.col_defs.push({
        		field: "Unit",
        		displayName: "Unidades para Reporte",
        		levels: [2],
        		cellTemplate: "<div style='text-align: center;'>{{row.branch[col.field].Name}}</div>",
        		width: "200px"
        	});

        	//$scope.col_defs.push({
        	//    field: "",
        	//    displayName: "Fórmula de Conversión",
        	//    tooltip: "Fórmula de conversión por unidad",
        	//    width: "90px"

        	//});

        	$scope.col_defs.push({
        		field: "DecimalesReporte",
        		displayName: "Decimales para Reporte",
        		tooltip: "Decimales para Reporte",
        		levels: [2],
        		cellTemplate: "<div>{{row.branch[col.field]===null?'N/A':row.branch[col.field]}}</div>",
        		width: "200px"

        	});

            $scope.col_defs.push({
                field: "AnalysisTime",
                displayName: "TPA en días nat.",
                tooltip: "Tiempo para el Análisis (días naturales)",
                levels: [2],
                cellTemplate: "<div>{{row.branch[col.field] === null ? 'NA' : row.branch[col.field]}}</div>",
                width: "150px"

            });

        	//$scope.col_defs.push({
        	//    field: "",
        	//    displayName: "TMPA (días)",
        	//    tooltip: "Tiempo Máximo Previo al Análisis en días",
        	//    width: "80px"

        	//});

        	//$scope.col_defs.push({
        	//    field: "",
        	//    displayName: "Tipo de Envase",
        	//    tooltip: "Tipo de Envase",
        	//    width: "80px"

        	//});

        	//$scope.col_defs.push({
        	//    field: "",
        	//    displayName: "Volumen Mínimo (mL)",
        	//    tooltip: "Volumen Mínimo",
        	//    width: "80px"

        	//});

        	//$scope.col_defs.push({
        	//    field: "",
        	//    displayName: "Volumen Deseado (mL)",
        	//    tooltip: "Volumen Deseado",
        	//    width: "80px"

        	//});

        	//$scope.col_defs.push({
        	//    field: "",
        	//    displayName: "Tipo de Preservador",
        	//    tooltip: "Tipo de Preservador",
        	//    width: "80px"

        	//});

            $scope.col_defs.push({
                field: "Formula",
                displayName: "Fórmula de Cálculo",
                width: "300px",
                //levels: [1, 2, 3],
                //cellTemplate: "<div>{{row.branch[col.field]}}</div>",
                tooltip: "Fórmula de Cálculo"
            });

            $scope.col_defs.push({
                field: "QcObj",
                displayName: "Genera Muestra QC",
                width: "150px",
                levels: [1, 2, 3],
                cellTemplate: "<div ng-show='row.branch[col.field]'>{{row.branch[col.field].HasQc ? 'Si' : 'No'}}</div>",
                tooltip: "Genera Muestra QC"
            });

            $scope.col_defs.push({
                field: "QcObj",
                displayName: "LIE",
                width: "50px",
                levels: [1, 2, 3],
                cellTemplate: "<div style='text-align: center;' ng-show='row.branch[col.field]'>{{row.branch[col.field].HasQc ? row.branch[col.field].LowerLimit : 'N/A'}}</div>",
                tooltip: "Límite Inferior de Exactitud"
            });

            $scope.col_defs.push({
                field: "QcObj",
                displayName: "LSE",
                width: "50px",
                levels: [1, 2, 3],
                cellTemplate: "<div style='text-align: center;' ng-show='row.branch[col.field]'>{{row.branch[col.field].HasQc ? row.branch[col.field].UpperLimit : 'N/A'}}</div>",
                tooltip: "Límite Superior de Exactitud"
            });

        	$scope.col_defs.push({
        		field: "RecOtorgs",
        		width: "150px",
        		displayName: "Reconocimientos",
        		levels: [1, 2, 3],
        		tooltip: "Reconocimientos otorgados al Parámetro",
        		cellTemplate: "<span ng-repeat='ro in row.branch[col.field]' ng-style=\"{'color': ro.Expired ? '#CCC7C7' : 'black'}\">{{ro.Key}}{{row.branch[col.field][row.branch[col.field].length -1] !== ro ? ',' : ''}} </span>"
        	});

            $scope.col_defs.push({
                field: "Uncertainty",
                displayName: "Incertidumbre",
                width: "50px",
                levels: [1, 2],
                cellTemplate: "<div ng-show='row.branch[col.field]'>{{!row.branch[col.field] || row.branch[col.field].Value === null ? 'N/A' : row.branch[col.field].Value.toFixed(row.branch[col.field].Decimals)}}</div>",
                tooltip: "Incertidumbre"
            });

        	$scope.col_defs.push({
        		field: "Signatarios",
        		displayName: "Signatarios",
        		width: "150px",
        		levels: [2],
        		tooltip: "Signatarios",
        		cellTemplate: "<span ng-repeat='sig in row.branch[col.field]'>{{sig}}{{row.branch[col.field][row.branch[col.field].length -1] !== sig ? ',' : ''}} </span>"
        	});

        	$scope.col_defs.push({
        		field: "Eidas",
        		displayName: "EIDAS",
        		width: "150px",
        		levels: [2],
        		tooltip: "EIDAS",
        		cellTemplate: "<span ng-repeat='eidas in row.branch[col.field]'>{{eidas}}{{row.branch[col.field][row.branch[col.field].length -1] !== eidas ? ',' : ''}} </span>"
        	});

        	//$scope.col_defs.push({
        	//    field: "",
        	//    displayName: "Tiempo de entrega (días hábiles)",
        	//    tooltip: "Tiempo de entrega al Cliente (días hábiles)",
        	//    width: "150px"


        	//});

        	$scope.col_defs.push({
        		field: "ResiduoPeligroso",
        		displayName: "Residuo Peligroso",
        		tooltip: "Residuo Peligroso",
        		levels: [2],
        		cellTemplate: "<div>{{row.branch[col.field]===true?'Si':'No'}}</div>",
        		width: "80px"
        	});

            $scope.col_defs.push({
                field: "Residue",
                displayName: "Tipo de Desecho",
                tooltip: "Tipo de Desecho",
                levels: [2],
                cellTemplate: "<div>{{row.branch[col.field].Name}}</div>",
                width: "120px"
        	});

        	$scope.col_defs.push({
        		field: "UnidadAnalitica",
        		displayName: "Área Analítica",
        		levels: [2],
        		cellTemplate: "<div>{{row.branch[col.field].AreaAnalitica}}</div>",
        		width: "80px"
        	});

        	$scope.col_defs.push({
        		field: "UnidadAnalitica",
        		displayName: "Clave Analista Asig.",
        		levels: [2],
        		tooltip: "Clave de Analista Asignado.",
        		cellTemplate: "<div>{{row.branch[col.field].AnnalistKey}}</div>",
        		width: "100px"
        	});

        	$scope.col_defs.push({
        		field: "ReportaCliente",
        		displayName: "Se Reporta al Cliente",
        		cellTemplate: "<div style='text-align: center;'>{{row.branch[col.field]===true?'SI':'NO'}}</div>",
        		levels: [2],
        		tooltip: "Se Reporta al Cliente",
        		width: "100px"
        	});

        	$scope.col_defs.push({
        		field: "PublishInAutolab",
        		displayName: "Solo para cotizar",
        		levels: [2],
        		cellTemplate: "<div style='text-align: center;'>{{row.branch[col.field] === false ? 'Si' : 'No'}}</div>",
        		tooltip: "Solo para cotizar",
        		width: "90px"
        	});

        	$scope.col_defs.push({
        		field: "PerWeekCapacity",
        		displayName: "CS",
        		tooltip: "Capacidad Instalada/Semana (Máxima)",
        		levels: [2],
        		cellTemplate: "<div ng-show='row.branch[col.field] !== undefined'>{{row.branch[col.field] === null ? '-' : row.branch[col.field]}}</div>",
        		width: "80px"
        	});

        	$scope.col_defs.push({
        		field: "PerTurnCapacity",
        		displayName: "CT",
        		tooltip: "Capacidad Instalada/Semana (Trabajo Combinado)",
        		levels: [2],
        		cellTemplate: "<div ng-show='row.branch[col.field] !== undefined'>{{row.branch[col.field] === null ? '-' : row.branch[col.field]}}</div>",
        		width: "80px"
        	});

        	//$scope.col_defs.push({
        	//    field: "",
        	//    displayName: "TA (días hábiles)",
        	//    // cellTemplate: "<ul style='margin-left: 0; padding-left: 1rem;'><li ng-repeat='an in row.branch[col.field]'>{{an.Name}}</li></ul>",
        	//    //  levels: [1, 2],
        	//    tooltip: "Tiempo para el Analista en días hábiles",
        	//    width: "80px"
        	//});

        	$scope.col_defs.push({
        		field: "AutolabAssignedAreaName",
        		displayName: "NAAA",
        		tooltip: "Nombre de Área Autolab Asignada",
        		width: "70px"
        	});

        	$scope.col_defs.push({
        		field: "GenericKeyForStatistic",
        		displayName: "CGE",
        		tooltip: "Clave genérica para estadísticas",
        		width: "70px"
        	});

        	$scope.col_defs.push({
        		field: "ParamPrintResults",
        		displayName: "Imp. result.",
        		tooltip: "Imprimir en los resultados",
        		levels: [2],
        		cellTemplate: "<div>{{row.branch[col.field].Yes===true ? 'SI': 'NO'}}</div>",
        		width: "50px"
        	});

        	$scope.col_defs.push({
                field: "RequiredVolume",
                displayName: "VR",
                width: "50px",
                levels: [2],
                cellTemplate: "<div>{{row.branch[col.field] === null ? 'NA' : row.branch[col.field]}}</div>",
                tooltip: "Volúmen Requerido"
            });

            $scope.col_defs.push({
                field: "MinimumVolume",
                displayName: "VM",
                width: "50px",
                levels: [2],
                cellTemplate: "<div>{{row.branch[col.field] === null ? 'NA' : row.branch[col.field]}}</div>",
                tooltip: "Volúmen Mínimo"
            });

            $scope.col_defs.push({
                field: "DeliverTime",
                displayName: "TEC en días háb.",
                width: "150px",
                levels: [2],
                cellTemplate: "<div>{{row.branch[col.field] === null ? 'NA' : row.branch[col.field]}}</div>",
                tooltip: "Tiempo de Entrega al Cliente (días hábiles)"
            });

            $scope.col_defs.push({
                field: "ReportTime",
                displayName: "TEA en días háb.",
                width: "150px",
                levels: [2],
                cellTemplate: "<div>{{row.branch[col.field] === null ? 'NA' : row.branch[col.field]}}</div>",
                tooltip: "Tiempo de Entrega para el Analista (días hábiles)"
            });

            $scope.col_defs.push({
                field: "MaxTimeBeforeAnalysis",
                displayName: "TMPA en días nat.",
                width: "150px",
                levels: [2],
                cellTemplate: "<div>{{row.branch[col.field] === null ? 'NA' : row.branch[col.field]}}</div>",
                tooltip: "Tiempo Máximo Previo al Análisis (días naturales)"
            });

            $scope.col_defs.push({
                field: "LabDeliverTime",
                displayName: "TEL en días háb.",
                width: "150px",
                levels: [2],
                cellTemplate: "<div>{{row.branch[col.field] === null ? 'NA' : row.branch[col.field]}}</div>",
                tooltip: "Tiempo de Entrega al Laboratorio (días hábiles)"
            });

            $scope.col_defs.push({
                field: "Container",
                displayName: "Tipo de Envase",
                width: "150px",
                levels: [2],
                cellTemplate: "<div>{{row.branch[col.field].Name}}</div>",
                tooltip: "Tipo de Envase"
            });

            $scope.col_defs.push({
                field: "Preserver",
                displayName: "Tipo de Preservador",
                width: "170px",
                levels: [2],
                cellTemplate: "<div>{{row.branch[col.field].Name}}</div>",
                tooltip: "Tipo de Preservador"
            });

            $scope.col_defs.push({
                field: "DetectionLimit",
                displayName: "LDM",
                width: "50px",
                levels: [2],
                cellTemplate: "<div ng-show='row.branch[col.field]'>{{row.branch[col.field]===null || row.branch[col.field].Value === null ? 'N/A' : row.branch[col.field].Value.toFixed(row.branch[col.field].Decimals)}}</div>",
                tooltip: "Límite de Detección del Método"
            });

            $scope.col_defs.push({
                field: "DetectionLimit",
                displayName: "Decimales para el LDM",
                width: "100px",
                levels: [2],
                cellTemplate: "<div ng-show='row.branch[col.field]'>{{row.branch[col.field].Decimals}}</div>",
                tooltip: "Decimales para el Límite de Detección del Método"
            });

            $scope.col_defs.push({
                field: "CuantificationLimit",
                displayName: "LPC",
                width: "50px",
                levels: [2],
                cellTemplate: "<div ng-show='row.branch[col.field]'>{{!row.branch[col.field] || row.branch[col.field].Value === null ? 'N/A' : row.branch[col.field].Value.toFixed(row.branch[col.field].Decimals)}}</div>",
                tooltip: "Límite Práctico de Cuantificación"
            });

            $scope.col_defs.push({
                field: "CuantificationLimit",
                displayName: "Decimales para el LPC",
                width: "100px",
                levels: [2],
                cellTemplate: "<div ng-show='row.branch[col.field]'>{{row.branch[col.field].Decimals}}</div>",
                tooltip: "Decimales para el LPC"
            });

            $scope.col_defs.push({
                field: "ReportLimit",
                displayName: "LR",
                width: "50px",
                levels: [2],
                cellTemplate: "<div ng-show='row.branch[col.field]'>{{!row.branch[col.field] || row.branch[col.field].Value === null ? 'N/A' : row.branch[col.field].Value}}</div>",
                tooltip: "Límite de Reporte"
            });

        	$scope.col_defs.push({
        		field: "Week",
        		displayName: "Lun",
        		levels: [2],
        		cellTemplate: "<div>{{row.branch[col.field].Monday === true ? 'SI' : 'NO'}}</div>",
        		tooltip: "Se programa el Lunes"
        	});

        	$scope.col_defs.push({
        		field: "Week",
        		displayName: "Mar",
        		levels: [2],
        		cellTemplate: "<div>{{row.branch[col.field].Tuesday === true ? 'SI' : 'NO'}}</div>",
        		tooltip: "Se programa el Martes"
        	});

        	$scope.col_defs.push({
        		field: "Week",
        		displayName: "Mie",
        		levels: [2],
        		cellTemplate: "<div>{{row.branch[col.field].Wednesday === true ? 'SI' : 'NO'}}</div>",
        		tooltip: "Se programa el Miércoles"
        	});

        	$scope.col_defs.push({
        		field: "Week",
        		displayName: "Jue",
        		levels: [2],
        		cellTemplate: "<div>{{row.branch[col.field].Thursday === true ? 'SI' : 'NO'}}</div>",
        		tooltip: "Se programa el Jueves"
        	});

        	$scope.col_defs.push({
        		field: "Week",
        		displayName: "Vie",
        		levels: [2],
        		cellTemplate: "<div>{{row.branch[col.field].Friday === true ? 'SI' : 'NO'}}</div>",
        		tooltip: "Se programa el Viernes"
        	});

        	$scope.col_defs.push({
        		field: "Week",
        		displayName: "Sáb",
        		levels: [2],
        		cellTemplate: "<div>{{row.branch[col.field].Saturday === true ? 'SI' : 'NO'}}</div>",
        		tooltip: "Se programa el Sábado"
        	});

        	$scope.col_defs.push({
        		field: "Week",
        		displayName: "Dom",
        		levels: [2],
        		cellTemplate: "<div>{{row.branch[col.field].Sunday === true ? 'SI' : 'NO'}}</div>",
        		tooltip: "Se programa el Domingo"
        	});

        	$scope.col_defs.push({
        		field: "SellSeparated",
        		displayName: "SVS",
        		levels: [2],
        		cellTemplate: "<div ng-style=\"{'background-color': (row.branch[col.field] === true) ? 'transparent' : 'red'}\">{{row.branch[col.field] === true ? 'Si' : 'No  se vende por separado'}}</div>",
        		tooltip: "Se vende por separado",
        		width: "150px"
        	});

        	$scope.col_defs.push({
        		field: "CuentaEstadistica",
        		displayName: "CPE",
        		levels: [2],
        		cellTemplate: "<div style='text-align: center;' >{{row.branch[col.field] === true ? 'SI' : 'NO'}}</div>",
        		tooltip: "Cuenta para Estadística",
        		width: "50px"
        	});

        	$scope.col_defs.push({
        		field: "ccChange",
        		displayName: "Cambiar CC",
        		tooltip: "Cambiar Centro de Costo",
        		levels: [1],
        		width: "100px",
        		cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Cambiar Centro de Costo'><span class='icon mif-broadcast' style='color: #1b6eae; font-size: 1rem;'></span></a></div>",
        		cellTemplateScope: {
        			click: function (data) { // this works too: $scope.someMethod;
        				$scope.ccChange(data);
        			}
        		}
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
        				levels: [2],
        				cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Editar'><span class='icon mif-pencil' style='color: #1b6eae; font-size: 1rem;'></span></a></div>",
        				//"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
        				cellTemplateScope: {
        					click: function (data) { // this works too: $scope.someMethod;
        						data.d = false;
        						$scope.edit(data);
                                
        					}
        				},
        				width: "50px"
        			});
			        $scope.tableWidht += 50;

        			$scope.col_defs.push({
        				field: "duplicate",
        				displayName: "Duplicar",
        				levels: [2],
        				width: "50px",
        				cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Duplicar'><span class='icon mif-floppy-disk' style='color: #1b6eae; font-size: 1rem;'></span></a></div>",
        				//"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
        				cellTemplateScope: {
        					click: function (data) { // this works too: $scope.someMethod;
        						data.d = true;
        						$scope.edit(data);
        					}
        				}
        			});
        			$scope.tableWidht += 50;

        			$scope.col_defs.push({
        				field: "",
        				displayName: "Dar baja/alta",
        				levels: [2],
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
        		securityFact.subscribe($scope, function updateALparameter() {
        			printAccess();
        		});
        	}
        	else
        		printAccess();

        	$scope.baseParamFilter = true;

        	function getBaseParam() {
        		generalFactory.getAll('/param/getparambase', { active: true }).then(function (response) {
        			if (response.success) {
        				$scope.baseparamsfilter = response.elements;
        			} else {
        				notification.error({
        					message: "Error al recuperar parámetros base.",
        					delay: errorDelay
        				});
        			}
        		});
        	}

        	getBaseParam();

        	$scope.bpFilter = function () {
        		$scope.initbpFilter = true;
        		$scope.RefreshList();
        		$scope.initbpFilter = false;
        	};
        	$scope.bpFilterRemove = function () {
        		$scope.initbpFilter = true;
        		$scope.baseparamfiltered = 0;
        		$scope.RefreshList();
        		$scope.initbpFilter = false;
        	};

        	var ccChangeDialog = function (element) {
        		return {
        			template: "Shared/ChangeCC",
        			controller: "ChangeCCController",
        			className: "ngdialog-theme-default ngdialog-width1000",
        			preCloseCallback: "preCloseCallbackOnScope",
        			data: { dData: element },
        			closeByEscape: true
        		}
        	}

        	var saveChange = function (promise) {
        		generalFactory.save("/param/SaveCcChange", {
        			baseParam: promise.BaseParam,
        			centroCosto: promise.CentroCosto,
        			prms: promise.Params
        		}).then(function (response) {
        			if (response.success) {
        				$scope.RefreshList();
        			}
        		});

        	}

        	$scope.ccChange = function (data) {
        		generalFactory.edit(ccChangeDialog, data, saveChange);
        	}

        	// History
        	$scope.currentPageMore = 1;
        	$scope.pageSizeMore = 10;

        	$scope.searchGeneralMore = "";
        	$scope.searchIpMore = "";
        	$scope.viewAdded = true;
        	$scope.viewModified = true;

        	var filtersMore = function (page) {
        		return {
        			page: (page === null || page === undefined) ? $scope.currentPageMore : page,
        			pageSize: $scope.pageSizeMore,
        			tableName: "ParamSet",
        			fromDate: $scope.fromDateMore === undefined ? null : $scope.fromDateMore,
        			untilDate: $scope.untilDateMore === undefined ? null : $scope.untilDateMore,
        			searchGeneral: $scope.searchGeneralMore,
        			searchIp: $scope.searchIpMore,
        			viewAdded: $scope.viewAdded,
        			viewModified: $scope.viewModified
        		}
        	};

        	$scope.elementsListMore = null;
        	$scope.isLoadingMore = false;

        	$scope.RefreshListMore = function (page) {
        		$scope.isLoadingMore = true;
        		generalFactory.RefreshList("/audit/RefreshAuditInfo", filtersMore, page)
                    .then(function (response) {
                        if (response.success) {
                        	$scope.elementsListMore = response.elements;
                        	$scope.pageTotalMore = response.total;
                        	$scope.isLoadingMore = false;
                        } else {
                        	notification.error({
                        		message: "Error al recuperar los Elementos",
                        		delay: 5000
                        	});
                        	$scope.isLoadingMore = false;
                        }
                    });
        	}

        	$scope.initDateFilterMore = false;
        	$scope.dateFilterMore = function () {
        		$scope.initDateFilterMore = true;
        		$scope.RefreshListMore();
        		$scope.initDateFilterMore = false;
        	};
        	$scope.dateFilterRemoveMore = function () {
        		$scope.initDateFilterMore = true;
        		$scope.untilDateMore = "";
        		$scope.fromDateMore = "";
        		$scope.RefreshListMore();
        		$scope.initDateFilterMore = false;
        	};

        }
    ])

    .controller("NewParamController",
    [
        "$scope", "$stateParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "generalFactory", "ngDialog", "RecService",
        function ($scope, $stateParams, $location, $http, $controller, securityFact, notification, generalFactory, ngDialog, recService) {

        	var errorDelay = 5000;
        	//var currentId = $stateParams.id;
        	$scope.ngDialogData.vSucursalList = [];
        	$scope.ngDialogData.rSucursalList = [];

        	//$scope.ngDialogData.BaseMatrix = {};

        	$scope.isLoading = false;
        	var page = 1;
        	$scope.pageSize = 10;

        	// Checking Column List
        	$scope.columnList = securityFact.getColumnList();

        	if ($scope.columnList === undefined) {
        		securityFact.subscribe($scope, function updatenewCLparam() {
        			$scope.columnList = securityFact.getColumnList();
        		});
        	}

        	//   $scope.ngDialogData.dData.ParamRoutes = [];
        	if ($scope.ngDialogData.dData.ParamPrintResults == undefined)
        		$scope.ngDialogData.dData.ParamPrintResults = { Id: 0, Yes: false }

            $scope.paramKeys = new Array();
            $scope.paramKeysChecked = {};

			//TODO: REVISAR FORMULA!!!
            var getParamKeys = function (){
               $http.post("/param/GetParamKeys")
               .success(function (response) {
                   if (response.success) {                                               
                       for (var i = 0; i < response.elements.length; i++) {
                           // if (response.elements[i].CantidadMuestreos > 0) {
                           //     for (var j = 1; j <= response.elements[i].CantidadMuestreos; j++) {
                           //         $scope.paramKeys.push(
                           //             {
                           //                 key: response.elements[i].ParamUniquekey + "-" + j,
                           //                 peso: 0  
                           //             }                                        
                           //         );
                           //     }
                           // } else {
                           $scope.paramKeys.push(
                               {
                                   key: response.elements[i].ParamUniquekey,
                                   peso: 0  
                               }                                      
                           );
                           //}                            
                       }

                       if ($scope.ngDialogData.dData.Formula) {
                           var elemsFormula = $scope.ngDialogData.dData.Formula.split(" ");
                       } 

                        for (i = 0; i < $scope.paramKeys.length; i++) {
                            $scope.paramKeysChecked[$scope.paramKeys[i].key] = false;
                            if (elemsFormula && elemsFormula.length > 0) {
                                for (var k = 0; k < elemsFormula.length; k++) {
                                    if ($scope.ngDialogData.dData.TipoFormula == '2') {
                                        var promPond = elemsFormula[k].split("*");

                                        if (promPond[0] === $scope.paramKeys[i].key) {
                                            $scope.paramKeysChecked[$scope.paramKeys[i].key] = true;
                                            $scope.paramKeys[i].peso = 1*promPond[1];   // 1* para convertir el string a int
                                            break;    
                                        }
                                    } else {
                                        if (elemsFormula[k] === $scope.paramKeys[i].key) {
                                            $scope.paramKeysChecked[$scope.paramKeys[i].key] = true;
                                            break;    
                                        } 
                                    }                                                                       
                                }
                            }
                        }
                   } else {
                       notification.error({
                           message: "Error al recuperar el listado de parámetros. " + response.err, delay: errorDelay
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
            getParamKeys();
            //hasta aqui formula

            $scope.paramChecked = function(p){
                if ($scope.paramKeysChecked[p]) {
                    //delete $scope.paramKeysChecked[p];
                    $scope.paramKeysChecked[p] = false;
                } else {
                    $scope.paramKeysChecked[p] = true;
                }               
                $scope.tipoFormulaChanged($scope.ngDialogData.dData.TipoFormula); 
            }

            $scope.tipoFormulaChanged = function (tipo){
                $scope.ngDialogData.dData.Formula = "";
                if (tipo == "0") {
                    $scope.ngDialogData.dData.Formula = "sqrt ";
                    var formulaTemp = "( ";
                    var count = 0;
                    for (var i = 0; i < $scope.paramKeys.length; i++) {
                        if ($scope.paramKeysChecked[$scope.paramKeys[i].key]) {
                            if (formulaTemp !== "( ") {
                               formulaTemp += " * "; 
                            }
                            formulaTemp += $scope.paramKeys[i].key;
                            count += 1;
                        }
                    }
                    formulaTemp += " )";
                    $scope.ngDialogData.dData.Formula += count + formulaTemp;
                }
                else if (tipo == "1") {
                    $scope.ngDialogData.dData.Formula = "( ";
                    var count = 0;
                    for (var i = 0; i < $scope.paramKeys.length; i++) {
                        if ($scope.paramKeysChecked[$scope.paramKeys[i].key]) {
                            if ($scope.ngDialogData.dData.Formula !== "( ") {
                               $scope.ngDialogData.dData.Formula += " + "; 
                            }
                            $scope.ngDialogData.dData.Formula += $scope.paramKeys[i].key;
                            count += 1;
                        }
                    }
                    $scope.ngDialogData.dData.Formula += " ) / " + count;
                }
                else if (tipo == "2") {
                    $scope.ngDialogData.dData.Formula = "( ";
                    var count = 0;
                    for (var i = 0; i < $scope.paramKeys.length; i++) {
                        if ($scope.paramKeysChecked[$scope.paramKeys[i].key]) {
                            if ($scope.ngDialogData.dData.Formula !== "( ") {
                               $scope.ngDialogData.dData.Formula += " + "; 
                            }
                            $scope.ngDialogData.dData.Formula += $scope.paramKeys[i].key + '*' + $scope.paramKeys[i].peso;
                            count += 1;
                        }
                    }
                    $scope.ngDialogData.dData.Formula += " ) / " + count;
                }
            }

        	//$scope.changeBaseMtrx = function () {
        	//	if ($scope.ngDialogData.BaseMatrix !== null) {
        	//		$scope.mtrxList = $scope.ngDialogData.BaseMatrix.Matrixes;
        	//		return;
        	//	}
        	//	$scope.ngDialogData.BaseMatrix.Matrixes = [];
        	//	return;
        	//}
			
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

                // Si tiene mas de 1 elemento entonces no hace falta pedir sucursales, 
                //ya que son las mismas pq es la misma Matriz Base
                if ($scope.ngDialogData.dData.Matrixes.length === 1)
                    getSucursales();

        		return;
        	}

        	$scope.viewRecAdqs = function (an) {
        		recService.viewRecAdqs(an);
        	}

        	function getMaxPermitedLimits(page, pageSize) {
        		$scope.isLoading = true;
        		$http.post("/maxpermitedlimit/refreshlist", { page: page, pageSize: pageSize, activeOption: true })
				.success(function (response) {
					if (response.success) {
						$scope.elementsList = response.elements;
						if ($scope.ngDialogData.dData.ParamRoutes != undefined)
							for (var i = 0; i < $scope.ngDialogData.dData.ParamRoutes.length; i++)
								for (var j = 0; j < $scope.elementsList.length; j++) {
									if ($scope.elementsList[j].Id === $scope.ngDialogData.dData.ParamRoutes[i].MaxPermitedLimit.Id) {
										$scope.elementsList[j].check = true;
										break;
									}
								}
					} else {
						notification.error({
							message: "Error al recuperar el listado de Límte máximo permitido. " + response.err, delay: errorDelay
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
        	getMaxPermitedLimits(page, $scope.pageSize);

        	function getCurrency() {
        		generalFactory.getAll('/param/getparamcurrency', { active: true }).then(function (response) {
        			if (response.success) {
        				$scope.currencies = response.elements;
        			} else {
        				notification.error({
        					message: "Error al recuperar monedas.",
        					delay: errorDelay
        				});
        			}
        		});
        	}
        	getCurrency();

        	function getBaseParam() {
        		generalFactory.getAll('/param/getparambase', { active: true }).then(function (response) {
        			if (response.success) {
        				$scope.baseparams = response.elements;
        				if ($scope.ngDialogData.dData.BaseParam !== undefined) {
        					for (var i = 0; i < $scope.baseparams.length; i++) {
        						if ($scope.baseparams[i].Id === $scope.ngDialogData.dData.BaseParam.Id) {
        							$scope.units = $scope.baseparams[i].Units;
                                    $scope.matrixes = $scope.baseparams[i].Matrixes;

                                    if ($scope.ngDialogData.dData.Matrixes) {
                                        for (var i = 0; i < $scope.ngDialogData.dData.Matrixes.length; i++) {
                                            for (var j = 0; j < $scope.matrixes.length; j++) {
                                                if ($scope.ngDialogData.dData.Matrixes[i].Id === $scope.matrixes[j].Id) {
                                                    $scope.matrixes[j].check = true;
        							break;
        						}
        					}
        				}
                                    }
        							break;
        						}
        					}
        				}
        			} else {
        				notification.error({
        					message: "Error al recuperar parámetros base.",
        					delay: errorDelay
        				});
        			}
        		});
        	}
        	getBaseParam();

        	function getCentrosCosto() {
        		generalFactory.getAll('/centrocosto/GetCentrosCosto').then(function (response) {
        			if (response.success) {
        				$scope.centroscosto = response.elements;
        			} else {
        				notification.error({
        					message: "Error al recuperar centros de costo.",
        					delay: errorDelay
        				});
        			}
        		});
        	}
        	getCentrosCosto();

        	// function getMethods() {
        	// 	generalFactory.getAll('/param/GetBaseParamMatrixes', { active: true }).then(function (response) {
        	// 		if (response.success) {
        	// 			$scope.methods = response.elements;

        	// 			// if ($scope.ngDialogData.dData.Id !== undefined)
        	// 			// 	for (var j = 0; j < $scope.methods.length; j++) {
        	// 			// 		if ($scope.methods[j].Id === $scope.ngDialogData.dData.Metodo.Id) {
        	// 			// 			for (var i = 0; i < $scope.methods[j].Matrixes.length; i++) {
        	// 			// 				if ($scope.ngDialogData.dData.Matrix && $scope.methods[j].Matrixes[i].Id === $scope.ngDialogData.dData.Matrix.Id) {
        	// 			// 					$scope.methods[j].Matrixes[i].selected = true;
								 //    //     } else {
        	// 			// 					$scope.methods[j].Matrixes[i].selected = false;
								 //    //     }
        	// 			// 			}
        	// 			// 			$scope.matrixes = $scope.methods[j].Matrixes;
							  //    //    break;
						   //    //   }
        	// 			// 	}
        					
        	// 		} else {
        	// 			notification.error({
        	// 				message: "Error al recuperar métodos.",
        	// 				delay: errorDelay
        	// 			});
        	// 		}
        	// 	});
        	// }
        	// getMethods();

	        // $scope.matrixes = new Array();
	        // $scope.MetodoChanged = function (metodoId) {
		       //  //$scope.matrixes.length = 0;
		       //  for (var i = 0; i < $scope.methods.length; i++) {
			      //   if ($scope.methods[i].Id === metodoId) {
				     //    $scope.matrixes = $scope.methods[i].Matrixes;
			      //   }
		       //  }
	        // }

	        function getSucursales() {
	        	$http.post("/param/getsucursales", { mtrxId: $scope.ngDialogData.dData.Matrixes[0].Id })
        		.success(function (response) {
        			if (response.success === true) {
        				$scope.vSucursalList = response.vSucursalList;
        				$scope.rSucursalList = response.rSucursalList;
        			}
        		});
	        }

            if ($scope.ngDialogData.dData.Matrixes && $scope.ngDialogData.dData.Matrixes.length > 0) 
	        	getSucursales();

	        // $scope.matrixSelected = function (m) {
	        // 	for (var i = 0; i < $scope.matrixes.length; i++) {
	        // 		if ($scope.matrixes[i].selected === true) {
	        // 			$scope.matrixes[i].selected = false;
	        // 			break;
	        // 		}
	        // 	}
	        // 	m.selected = true;
         //        $scope.ngDialogData.dData.Matrix = m;
	        // 	getSucursales();
	        // };

         //    if ($scope.ngDialogData.dData.Matrix) 
	        //    getSucursales();

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

        	function getAnnalists() {
                //generalFactory.getAll('/param/getannalist', { active: true }).then(function (response) {
                generalFactory.getAll("/annalist/GetAnnalist", { activeOption: true }).then(function (response) {
        			if (response.success) {
        				$scope.annalists = response.elements;

        				if ($scope.ngDialogData.dData.Id !== undefined)
        					for (var i = 0; i < $scope.ngDialogData.dData.Annalists.length; i++) {
        						for (var j = 0; j < $scope.annalists.length; j++) {
        							if ($scope.annalists[j].checked) {
        								continue;
        							}
        							if ($scope.annalists[j].Id === $scope.ngDialogData.dData.Annalists[i].Id) {
        								$scope.annalists[j].checked = true;
        								break;
        							}
        							else
        								$scope.annalists[j].checked = false;
        						}
        					}

        			} else {
        				notification.error({
        					message: "Error al recuperar analistas.",
        					delay: 5000
        				});
        			}
        		});
        	}
        	getAnnalists();

        	$scope.annalistChecked = function (a) {
        		a.checked = !a.checked;
        	}

            function getAllResidues(activated) {
                $http.post('/param/getparamresidue', { active: activated })
                .success(function (response) {
                    if (response.success === true) {
                        $scope.residues = response.elements;
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
            getAllResidues(true);

            function getAllContainers(activated) {
                $http.post('/param/getparamcontainer', { active: activated })
                .success(function (response) {
                    if (response.success === true) {
                        $scope.containers = response.elements;
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
            getAllContainers(true);

            function getAllPreservers(activated) {
                $http.post('/param/getparampreserver', { active: activated })
                .success(function (response) {
                    if (response.success === true) {
                        $scope.preservers = response.elements;
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
            getAllPreservers(true);

        	$scope.sucRealizaChange = function () {
        		$http.post("/sucursal/GetAnalyticsArea",
			        {
			            sucursalId: $scope.ngDialogData.dData.SucursalRealiza.Id?$scope.ngDialogData.dData.SucursalRealiza.Id:0,
			        	activeOption: true
			        })
        	        .success(function (response) {
        	            if (response.success === true) {
                     		$scope.analyticsareas = response.elements;

                            if ($scope.ngDialogData.dData.CentroCosto !== undefined) 
                            {
                              for (var i = 0; i < $scope.analyticsareas.length; i++) {
                                    $scope.analyticsareas[i].selected = false;
                                    if ($scope.analyticsareas[i].CentroCosto.Id === $scope.ngDialogData.dData.CentroCosto.Id) {
                                        $scope.analyticsareas[i].selected = true;
                                        $scope.unidadesAnaliticas = $scope.analyticsareas[i].UnidadesAnaliticas;
                                        for (var k = 0; k < $scope.unidadesAnaliticas.length; k++) {
                                            if ($scope.ngDialogData.dData.AnnalistKey && $scope.unidadesAnaliticas[k].Id === $scope.ngDialogData.dData.UnidadAnalitica.Id) {
                                                $scope.unidadesAnaliticas[k].AnnalistKeyId = $scope.ngDialogData.dData.AnnalistKey.Id;
                                            }
                                           
                                        }
                                    }
                                }

                                for (i = 0; i < $scope.unidadesAnaliticas.length; i++) {
                                    $scope.unidadesAnaliticas[i].selected = false;
                                    if ($scope.ngDialogData.dData.UnidadAnalitica !== null &&
                                        $scope.unidadesAnaliticas[i].Id === $scope.ngDialogData.dData.UnidadAnalitica.Id) {
                                        $scope.unidadesAnaliticas[i].selected = true;
                                        $scope.unidadesAnaliticas[i].AnnalistKeyId = $scope.ngDialogData.dData.AnnalistKey.Id?$scope.ngDialogData.dData.AnnalistKey.Id:0;
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

            if ($scope.ngDialogData.dData.SucursalRealiza) 
        	   $scope.sucRealizaChange();


            $scope.akChange=function(ak) {
                $scope.ngDialogData.dData.AnnalistKey = { Id: ak};
            }


        	$scope.aaChange = function (aa) {
        		for (var i = 0; i < $scope.analyticsareas.length; i++) {
        			if ($scope.analyticsareas[i].selected === true) {
        				$scope.analyticsareas[i].selected = false;
        				break;
        			}
        		}
        		aa.selected = true;
        		$scope.ngDialogData.dData.CentroCosto = aa.CentroCosto;

        		$scope.unidadesAnaliticas = aa.UnidadesAnaliticas;

        		for (i = 0; i < $scope.unidadesAnaliticas.length; i++) {
        			$scope.unidadesAnaliticas[i].selected = false;
        			if ($scope.ngDialogData.dData.UnidadAnalitica !== null &&
						$scope.unidadesAnaliticas[i].Id === $scope.ngDialogData.dData.UnidadAnalitica.Id) {
        				$scope.unidadesAnaliticas[i].selected = true;
        			}
        		}
	        }

        	$scope.uaChange = function (ua) {
        		for (var i = 0; i < $scope.unidadesAnaliticas.length; i++) {
        			if ($scope.unidadesAnaliticas[i].selected === true) {
        				$scope.unidadesAnaliticas[i].selected = false;
        				break;
        			}
        		}
        		ua.selected = true;
        		$scope.ngDialogData.dData.UnidadAnalitica = ua;
        		$http.post("/unidadanalitica/getannalistkey",
			        {
			            id:ua.Id
			        })
        	        .success(function (response) {
        	            if (response.success === true) {
        	                $scope.annalistkeylist = response.elements;

        	                

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

            var getAnalyticMethods = function () {
                $http.post("/analyticsmethod/GetAnalyticsMethods")
                     .success(function (response) {
                         if (response.success === true) {
                             $scope.analyticsmethods = response.elements;

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

            getAnalyticMethods();

       //  	function getMatrixes() {
       //  		$http.post("/param/GetMethodMatrixes", { active: true })
       //  	        .success(function (response) {
       //  	        	if (response.success === true) {
       //  	        		$scope.methodMatrixList = response.res;

       //  	        		//if ($scope.ngDialogData.dData.Matrixes) {
       //  	        		//	var j = 0;
       //  	        		//	for (var i = 0; i < $scope.ngDialogData.dData.Matrixes.length; i++) {
       //  	        		//		for (; j < $scope.baseMatrixList.length; j++) {
       //  	        		//			if ($scope.ngDialogData.dData.Matrixes[i].BaseMatrix.Id === $scope.baseMatrixList[j].Id) {
       //  	        		//				$scope.ngDialogData.BaseMatrix = $scope.baseMatrixList[j];
       //  	        		//				j = $scope.baseMatrixList.length;
       //  	        		//				break;
       //  	        		//			}
       //  	        		//		}
       //  	        		//		for (var k = 0; k < $scope.ngDialogData.BaseMatrix.Matrixes.length; k++) {
       //  	        		//			if ($scope.ngDialogData.BaseMatrix.Matrixes[k].Id === $scope.ngDialogData.dData.Matrixes[i].Id) {
       //  	        		//				$scope.ngDialogData.BaseMatrix.Matrixes[k].check = true;
       //  	        		//				break;
       //  	        		//			}
       //  	        		//		}
       //  	        		//	}
							// //}
       //  	        	} else {
       //  	        		notification.error({
       //  	        			message: "Error al recuperar datos.",
       //  	        			delay: errorDelay
       //  	        		});
       //  	        	}
       //  	        })
       //  	        .error(function () {
       //  	        	notification.error({
       //  	        		message: "Error en la conexión con el servidor.",
       //  	        		delay: errorDelay
       //  	        	});
       //  	        });
       //  	}
       //  	getMatrixes();

        	//for tree configuration

        	$scope.rowActions = [{
        		levels: [1],
        		//icon: "icon-plus  glyphicon glyphicon-plus  fa fa-plus",
        		cellTemplate: "<input name= 'lpm' style='margin:0' ng-click='cellTemplateScope.click(row.branch)' ng-model='row.branch.check' type='checkbox'>",
        		cellTemplateScope: {
        			click: function (data) { // this works too: $scope.someMethod;

        				if (data.check === false || data.check === undefined) {
        					var dataArray = [data];
        					// data.check = !data.check;
        					ngDialog.openConfirm({
        						template: "editDialog",
        						controller: "SetLMPDialogController",
        						className: "ngdialog-theme-default",
        						preCloseCallback: "preCloseCallbackOnScope",
        						data: { dData: dataArray, DecimalsPoints: 8, col_defs: $scope.col_defs },
        						closeByEscape: true
        					})
                                .then(function (promise) {
                                	//Accepting
                                	if ($scope.ngDialogData.dData.ParamRoutes == undefined) {
                                		$scope.ngDialogData.dData.ParamRoutes = [
                                            {
                                            	MaxPermitedLimit: { Id: data.Id },
                                            	Value: promise.Value,
                                            	DecimalsPoints: promise.DecimalsPoints
                                            }
                                		];

                                	} else {
                                		$scope.ngDialogData.dData.ParamRoutes.push({
                                			MaxPermitedLimit: { Id: data.Id },
                                			Value: promise.Value,
                                			DecimalsPoints: promise.DecimalsPoints
                                		});
                                	}

                                }, function (reason) {
                                	//Rejecting
                                	data.check = false;
                                });
        				} else {
        					for (var i = 0; i < $scope.ngDialogData.dData.ParamRoutes.length; i++) {
        						if ($scope.ngDialogData.dData.ParamRoutes[i].MaxPermitedLimit.Id === data.Id) {
        							$scope.ngDialogData.dData.ParamRoutes.splice(i, 1);
        							break;
        						}
        					}
        				}
        			}
        		}
        	}];

        	$scope.tableWidht = 300;

        	$scope.my_tree = {};

        	$scope.expanding_property = {
        		field: "Name",
        		displayName: "Clave Específica",
        		sortable: true,
        		filterable: true,
        		width: "150px"
        	};

        	$scope.col_defs = new Array();
        	$scope.col_defs.push({
        		field: "Description",
        		displayName: "Descripción",
        		width: "300px"
        	});


        	$scope.col_defs.push({
        		field: "MaxPermitedLimit",
        		displayName: "LMP",
        		width: "50px",
        		tooltip: "Límite Máximo Permitido",
        		levels: [1, 2, 3, 4],
        		cellTemplate: "<div>{{row.branch[col.field].Value}}</div>"
        	});


        	$scope.col_defs.push({
        		field: "Precio",
        		displayName: "Precio",
        		levels: [1, 2, 3, 4],
        		width: "100px",
        		cellTemplate: "<div>{{row.branch[col.field]}}</div>"
        		//"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
        	});

        	$scope.col_defs.push({
        		field: "AutolabAssignedAreaName",
        		displayName: "NAAA",
        		width: "100px",
        		tooltip: "Nombre de Área Autolab Asignada"
        	});

        	$scope.col_defs.push({
        		field: "ParamUniquekey",
        		displayName: "CE",
        		width: "200px",
        		tooltip: "Clave Específica"
        	});

        	$scope.col_defs.push({
        		field: "CentroCosto",
        		displayName: "CeC",
        		tooltip: "Centro de Costo",
        		levels: [1, 2, 3, 4],
        		width: "300px"
        		// cellTemplate: "<div>{{row.branch[col.field]}}</div>"
        	});

        	$scope.col_defs.push({
        		field: "Unit",
        		displayName: "Unidades para Reporte",
        		levels: [1, 2, 3, 4],
        		width: "100px",
        		cellTemplate: "<div>{{row.branch[col.field]}}</div>"
        	});

        	for (var j = 0; j < $scope.col_defs.length; j++) {
        		if ($scope.col_defs[j]["width"] === undefined) {
        			$scope.tableWidht += 100;
        			continue;
        		}
        		$scope.tableWidht += 1 * $scope.col_defs[j]["width"].slice(0, $scope.col_defs[j]["width"].length - 2);
        	}

        	$scope.dialogTitle = $scope.ngDialogData.dData.Id === undefined ?
                "Nuevo parámetro" : "Editar parámetro " + $scope.ngDialogData.dData.ParamUniquekey;

        	function getRamas() {
        		generalFactory.getAll('/rama/GetRamas')
					.then(function (response) {
						if (response.success) {
							$scope.ramas = response.elements;
						} else {
							notification.error({
								message: "Error al recuperar ramas.",
								delay: errorDelay
							});
						}
					});
        	}
        	getRamas();

        	$scope.acceptDialog = function () {

        		if ($scope.ngDialogData.dData.BaseParam === undefined) {
        			notification.error({
        				message: "Parámetro Base indefinido",
        				delay: errorDelay
        			});
        			return;
        		}
        		//if ($scope.ngDialogData.dData.Metodo === undefined) {
        		//    notification.error({
        		//        message: "Método indefinido",
        		//        delay: errorDelay
        		//    });
        		//    return;
        		//}

        		if ($scope.ngDialogData.dData.Precio === undefined) {
        			notification.error({
        				message: "Precio indefinido",
        				delay: errorDelay
        			});
        			return;
        		}
        		else if ($scope.ngDialogData.dData.Precio.Currency === undefined) {
        			notification.error({
        				message: "Moneda indefinida",
        				delay: errorDelay
        			});
        			return;
        		}
        		if (($scope.ngDialogData.dData.Precio !== undefined) && ($scope.ngDialogData.dData.Precio < 0)) {
        			notification.error({
        				message: "El precio debe ser positivo",
        				delay: errorDelay
        			});
        			return;
        		}

        		if (($scope.ngDialogData.dData.MaxPermitedLimit !== undefined) && ($scope.ngDialogData.dData.MaxPermitedLimit < 0)) {
        			notification.error({
        				message: "El límite máximo permitido debe ser positivo",
        				delay: errorDelay
        			});
        			return;
        		}

        		if (($scope.ngDialogData.dData.PerTurnCapacity !== undefined) && ($scope.ngDialogData.dData.PerTurnCapacity < 0)) {
        			notification.error({
        				message: "La capacidad por turnos debe ser positivo",
        				delay: errorDelay
        			});
        			return;
        		}

        		if (($scope.ngDialogData.dData.PerWeekCapacity !== undefined) && ($scope.ngDialogData.dData.PerWeekCapacity < 0)) {
        			notification.error({
        				message: "La capacidad por semana debe ser positivo",
        				delay: errorDelay
        			});
        			return;
        		}

        		if ((($scope.ngDialogData.dData.SucursalRealiza === null) || ($scope.ngDialogData.dData.SucursalRealiza === undefined) || ($scope.ngDialogData.dData.SucursalRealiza.Id === null)) &&
					(($scope.ngDialogData.dData.SucursalVende === null) || ($scope.ngDialogData.dData.SucursalVende === undefined) || ($scope.ngDialogData.dData.SucursalVende.Id === null))) {
        			notification.error({
        				message: "Debe seleccionar una Instalación/Sucursal donde se realiza o donde se vende",
        				delay: errorDelay
        			});
        			return;
        		}

                if (($scope.ngDialogData.dData.RequiredVolume !== undefined) && ($scope.ngDialogData.dData.RequiredVolume < 0)) {
                    notification.error({
                        message: "El volúmen requerido debe ser positivo",
                        delay: errorDelay
                    });
                    return;
                }
                if (($scope.ngDialogData.dData.MinimumVolume !== undefined) && ($scope.ngDialogData.dData.MinimumVolume < 0)) {
                    notification.error({
                        message: "El volúmen mínimo debe ser positivo",
                        delay: errorDelay
                    });
                    return;
                }
                if (($scope.ngDialogData.dData.DeliverTime !== undefined) && ($scope.ngDialogData.dData.DeliverTime < 0)) {
                    notification.error({
                        message: "El tiempo de entrega debe ser positivo",
                        delay: errorDelay
                    });
                    return;
                }
                if (($scope.ngDialogData.dData.ReportTime !== undefined) && ($scope.ngDialogData.dData.ReportTime < 0)) {
                    notification.error({
                        message: "El tiempo de reporte debe ser positivo",
                        delay: errorDelay
                    });
                    return;
                }
                if (($scope.ngDialogData.dData.AnalysisTime !== undefined) && ($scope.ngDialogData.dData.AnalysisTime < 0)) {
                    notification.error({
                        message: "El tiempo de análisis debe ser positivo",
                        delay: errorDelay
                    });
                    return;
                }
                if (($scope.ngDialogData.dData.MaxTimeBeforeAnalysis !== undefined) && ($scope.ngDialogData.dData.MaxTimeBeforeAnalysis < 0)) {
                    notification.error({
                        message: "El tiempo máximo previo al análisis debe ser positivo",
                        delay: errorDelay
                    });
                    return;
                }
                if (($scope.ngDialogData.dData.LabDeliverTime !== undefined) && ($scope.ngDialogData.dData.LabDeliverTime < 0)) {
                    notification.error({
                        message: "El tiempo de entrega al laboratorio debe ser positivo",
                        delay: errorDelay
                    });
                    return;
                }
                if ($scope.ngDialogData.dData.DetectionLimit !== undefined) {
                    if (($scope.ngDialogData.dData.DetectionLimit.Value !== undefined) && ($scope.ngDialogData.dData.DetectionLimit.Value < 0)) {
                        notification.error({
                            message: "El límite de detección debe ser positivo",
                            delay: errorDelay
                        });
                        return;
                    }
                    if (($scope.ngDialogData.dData.DetectionLimit.Decimals !== undefined) && ($scope.ngDialogData.dData.DetectionLimit.Decimals < 0)) {
                        notification.error({
                            message: "Los decimales del límite de detección deben ser positivos",
                            delay: errorDelay
                        });
                        return;
                    }
                }

                if ($scope.ngDialogData.dData.CuantificationLimit !== undefined) {
                    if (($scope.ngDialogData.dData.CuantificationLimit.Value !== undefined) && ($scope.ngDialogData.dData.CuantificationLimit.Value < 0)) {
                        notification.error({
                            message: "El límite de cuantificación debe ser positivo",
                            delay: errorDelay
                        });
                        return;
                    }
                    if (($scope.ngDialogData.dData.CuantificationLimit.Decimals !== undefined) && ($scope.ngDialogData.dData.CuantificationLimit.Decimals < 0)) {
                        notification.error({
                            message: "Los decimales del límite de cuantificación deben ser positivos",
                            delay: errorDelay
                        });
                        return;
                    }
                }

                if ($scope.ngDialogData.dData.Uncertainty !== undefined) {
                    if (($scope.ngDialogData.dData.Uncertainty.Decimals !== undefined) && ($scope.ngDialogData.dData.Uncertainty.Decimals < 0)) {
                        notification.error({
                            message: "Los decimales de incertidumbre deben ser positivos",
                            delay: errorDelay
                        });
                        return;
        		}
                }
				
                var annalists = [];

                for (var i = 0; i < $scope.annalists.length; i++) {
                    if ($scope.annalists[i].checked) {
                        annalists.push({ Id: $scope.annalists[i].Id });
                    }
                }
               
                if ($scope.ngDialogData.dData.ParamRoutes && $scope.ngDialogData.dData.d)
                    for (i = 0; i < $scope.ngDialogData.dData.ParamRoutes.length; i++)
                        $scope.ngDialogData.dData.ParamRoutes[i].Id = 0;

                var packlist = [];
                if ($scope.ngDialogData.dData.Packages)
                    for (i = 0; i < $scope.ngDialogData.dData.Packages.length; i++)
                        packlist.push({ Id: $scope.ngDialogData.dData.Packages[i].Id });

                var grouplist = [];
                if ($scope.ngDialogData.dData.Groups)
                    for (i = 0; i < $scope.ngDialogData.dData.Groups.length; i++)
                        grouplist.push({ Id: $scope.ngDialogData.dData.Groups[i].Id });

        		$scope.confirm({
        			Id: $scope.ngDialogData.dData.Id != null && !$scope.ngDialogData.dData.d ?
                        $scope.ngDialogData.dData.Id : 0,
        			Description: $scope.ngDialogData.dData.Description,
        			Active: $scope.ngDialogData.dData.Active,
        			PerTurnCapacity: $scope.ngDialogData.dData.PerTurnCapacity,
        			PerWeekCapacity: $scope.ngDialogData.dData.PerWeekCapacity,
        			AutolabAssignedAreaName: $scope.ngDialogData.dData.AutolabAssignedAreaName,
        			ParamUniquekey: $scope.ngDialogData.dData.ParamUniquekey,
        			GenericKeyForStatistic: $scope.ngDialogData.dData.GenericKeyForStatistic,
        			Precio: $scope.ngDialogData.dData.Precio,
        			ParamPrintResults: [{
        				Id: $scope.ngDialogData.dData.ParamPrintResults.Id != undefined
                            ? $scope.ngDialogData.dData.ParamPrintResults.Id
                            : 0,
        				Yes: $scope.ngDialogData.dData.ParamPrintResults.Yes
        			}],
        			BaseParam: $scope.ngDialogData.dData.BaseParam,
        			Unit: $scope.ngDialogData.dData.Unit,
        			CentroCosto: $scope.ngDialogData.dData.CentroCosto,
                    Annalists: annalists,
        			Metodo: $scope.ngDialogData.dData.Metodo,
        			ParamRoutes: $scope.ngDialogData.dData.ParamRoutes,
        			Matrixes: $scope.ngDialogData.dData.Matrixes,
        			//GenericDescription: $scope.ngDialogData.dData.GenericDescription,
        			GenericKey: $scope.ngDialogData.dData.GenericKey,
        			Rama: $scope.ngDialogData.dData.Rama,
        			ResiduoPeligroso: $scope.ngDialogData.dData.ResiduoPeligroso,
        			ReportaCliente: $scope.ngDialogData.dData.ReportaCliente,
        			TipoServicio: $scope.ngDialogData.dData.TipoServicio,
        			SellSeparated: $scope.ngDialogData.dData.SellSeparated,
        			CuentaEstadistica: $scope.ngDialogData.dData.CuentaEstadistica,
        			SucursalVende: $scope.ngDialogData.dData.SucursalVende,
        			SucursalRealiza: $scope.ngDialogData.dData.SucursalRealiza,
        			DecimalesReporte: $scope.ngDialogData.dData.DecimalesReporte,
                    Groups: grouplist,
                    Packages: packlist,
        			Week: $scope.ngDialogData.dData.Week,
        			UnidadAnalitica: $scope.ngDialogData.dData.UnidadAnalitica,
                    AnalyticsMethod: $scope.ngDialogData.dData.AnalyticsMethod,
                    Uncertainty: $scope.ngDialogData.dData.Uncertainty,
                    QcObj: $scope.ngDialogData.dData.QcObj,
                    Formula: $scope.ngDialogData.dData.Formula,
                    Residue: $scope.ngDialogData.dData.Residue,
                    InternetPublish: $scope.ngDialogData.dData.InternetPublish,
                    TipoFormula: $scope.ngDialogData.dData.TipoFormula,
                    RequiredVolume: $scope.ngDialogData.dData.RequiredVolume,
                    MinimumVolume: $scope.ngDialogData.dData.MinimumVolume,
                    DeliverTime: $scope.ngDialogData.dData.DeliverTime,
                    DetectionLimit: $scope.ngDialogData.dData.DetectionLimit,
                    CuantificationLimit: $scope.ngDialogData.dData.CuantificationLimit,
                    ReportLimit: $scope.ngDialogData.dData.ReportLimit,
                    MaxTimeBeforeAnalysis: $scope.ngDialogData.dData.MaxTimeBeforeAnalysis,
                    LabDeliverTime: $scope.ngDialogData.dData.LabDeliverTime,
                    ReportTime: $scope.ngDialogData.dData.ReportTime,
                    AnalysisTime: $scope.ngDialogData.dData.AnalysisTime,
                    Container: $scope.ngDialogData.dData.Container,
                    Preserver: $scope.ngDialogData.dData.Preserver,
                    AnnalistKey: $scope.ngDialogData.dData.AnnalistKey? $scope.ngDialogData.dData.AnnalistKey :null,
        			elemType: "parameter"
        		});
        	}
        }
    ])

.controller("SetLMPDialogController", ["$scope", "Notification",
        function ($scope, notification) {

        	$scope.dialogTitle = "Límite Máximo Permitido";

        	$scope.expanding_property = {
        		field: "Name",
        		displayName: "Nombre",
        		sortable: true,
        		filterable: true,
        		width: "300px"
        	};
        	$scope.acceptDialog = function () {
        		if ($scope.ngDialogData.Value === undefined) {
        			notification.error({
        				message: "Introduzca el valor del Límite máximo permisible o cancele la operación",
        				delay: 5000
        			});
        			return;
        		}
        		if ($scope.ngDialogData.DecimalsPoints < 0 || $scope.ngDialogData.DecimalsPoints > 8) {
        			notification.error({
        				message: "Cantidad de cifras decimales para el límite fuera de rango. El valor debe estar entre 0-8",
        				delay: 5000
        			});
        			return;
        		}

        		$scope.confirm({
        			Value: $scope.ngDialogData.Value,
        			DecimalsPoints: $scope.ngDialogData.DecimalsPoints
        		});
        	}
        }])

.directive('editParamSelectDirective', [function () {
	return {
		link: function (scope) {
			scope.$watch('ngDialogData.dData.BaseParam.Id', function () {
				if (scope.ngDialogData.dData.BaseParam !== undefined && scope.baseparams !== undefined) {
					//console.log(scope.ngDialogData.dData.BaseParam.Id);
					if (scope.ngDialogData.dData.BaseParam.Id === null) {
						scope.units = [];
                        scope.matrixes = [];
					} else {
						for (var i = 0; i < scope.baseparams.length; i++) {
							if (scope.baseparams[i].Id === scope.ngDialogData.dData.BaseParam.Id) {
								scope.units = scope.baseparams[i].Units;
								scope.ngDialogData.dData.BaseParam.Name = scope.baseparams[i].Name;

								scope.matrixes = scope.baseparams[i].Matrixes;
								break;
							}
						}
					}
				}
			});

			scope.$watch('ngDialogData.dData.CentroCosto.Id', function () {
				if (scope.ngDialogData.dData.CentroCosto !== undefined && scope.centroscosto !== undefined) {
					//console.log(scope.ngDialogData.dData.CentroCosto.Id);
					for (var i = 0; i < scope.centroscosto.length; i++) {
						if (scope.centroscosto[i].Id === scope.ngDialogData.dData.CentroCosto.Id) {
							scope.ngDialogData.dData.CentroCosto.Name = scope.centroscosto[i].Name;
							scope.ngDialogData.dData.CentroCosto.Description = scope.centroscosto[i].Description;
							break;
						}
					}

				}
			});

			scope.$watch('ngDialogData.dData.Metodo.Id', function () {
				if (scope.ngDialogData.dData.Metodo !== undefined && scope.methods !== undefined) {
					//console.log(scope.ngDialogData.dData.CentroCosto.Id);
					for (var i = 0; i < scope.methods.length; i++) {
						if (scope.methods[i].Id === scope.ngDialogData.dData.Metodo.Id) {
							scope.ngDialogData.dData.Metodo.Name = scope.methods[i].Name;
							scope.ngDialogData.dData.Metodo.Description = scope.methods[i].Description;
							scope.ngDialogData.dData.Metodo.AckRoutes = scope.methods[i].AckRoutes;
							break;
						}
					}

				}
			});

			//scope.$watch('scope.ngDialogData.BaseMatrix.Id', function () {
			//    if (scope.ngDialogData.BaseMatrix !== undefined && scope.BaseMatrix.Matrixes !== undefined) {
			//        //console.log(scope.ngDialogData.dData.CentroCosto.Id);
			//        scope.mtrxList = scope.BaseMatrix.Matrixes;
			//        //for (var i = 0; i < scope.methods.length; i++) {
			//        //    if (scope.methods[i].Id === scope.ngDialogData.dData.Metodo.Id) {
			//        //        scope.ngDialogData.dData.Metodo.Name = scope.methods[i].Name;
			//        //        scope.ngDialogData.dData.Metodo.Description = scope.methods[i].Description;
			//        //        scope.ngDialogData.dData.Metodo.AckRoutes = scope.methods[i].AckRoutes;
			//        //        break;
			//        //    }
			//        //}

			//    }
			//});
    		}
    	};
    }])
    //.directive('chBasematrix', ["$http", "Notification", function ($http, notification) {
    //	return {
    //		link: function (scope) {
    //			scope.$watch('ngDialogData.BaseMatrix', function () {
    //				if (scope.ngDialogData.BaseMatrix.Id !== undefined) {
    //					//console.log("ok");
	//				    $http.post("/param/getsucursales", { mtrxId: scope.ngDialogData.BaseMatrix.Id })
	//					    .success(function(response) {
	//						    if (response.success === true) {
	//							    scope.ngDialogData.vSucursalList = response.vSucursalList;
	//							    scope.ngDialogData.rSucursalList = response.rSucursalList;
	//						    }
	//					    });
	//			    }
    //			});
    //		}
    //	};
    //}])

.controller("ChangeCCController",
    [
        "$scope", "$stateParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "generalFactory",
        function ($scope, $stateParams, $location, $http, $controller, securityFact, notification, generalFactory) {

        	var errorDelay = 5000;

        	function getAnalyticsArea() {
        		generalFactory.getAll('/param/getanalyticsarea', { active: true }).then(function (response) {
        			if (response.success) {
        				$scope.centroscosto = response.elements;
        			} else {
        				notification.error({
        					message: "Error al recuperar centros de costo.",
        					delay: errorDelay
        				});
        			}
        		});
        	}

        	getAnalyticsArea();

        	$scope.dialogTitle = "Cambiar Centro de Costo";

        	$scope.acceptDialog = function () {
        		var error = false;

        		if ($scope.centrocosto === undefined) {
        			notification.error({
        				message: "Debe seleccionar un Centro de Costo",
        				delay: errorDelay
        			});
        			return;
        		}

        		if (error === true)
        			return;

        		var prms = new Array();
        		for (var i = 0; i < $scope.ngDialogData.dData.children.length; i++) {
        			prms.push($scope.ngDialogData.dData.children[i].Id);
        		}

        		$scope.confirm({
        			BaseParam: $scope.ngDialogData.dData.Id != null ? $scope.ngDialogData.dData.Id : 0,
        			CentroCosto: $scope.centrocosto,
        			Params: prms
        		});
        	}
        }
    ])

;
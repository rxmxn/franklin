"use strict";

angular.module("abcadmin")
    .controller("SheetController",
	[
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', "dataProvider", "ngDialog", "Notification", "generalFactory", "$sce", "SecurityFact", "InfoService", "RecService",
        function ($scope, $route, $location, $http, $cookies, $controller, dataProvider, ngDialog, notification, generalFactory, $sce, SecurityFact, InfoService, recService) {

        	$scope.viewParameters = true;
        	$scope.viewGroups = true;
        	$scope.viewPackages = true;
        	$scope.taskbar = true;
        	$scope.filtering = false;
        	$scope.isFullscreen = false;
        	$scope.generatingPDFFile = false;
        	$scope.generatingEXCELFile = false;
        	$scope.tableWidth = 170;
        	//-----------función q se llama para mostrar un div en fullscreen----------------
        	$scope.toggleFullScreen = function () {

        		$scope.isFullscreen = !$scope.isFullscreen;

        	}; //---------------------------------------------------------------------------------

        	var errorDelay = 5000;
        	// This controller groups similar functionalities of different controllers.
        	$controller('GeneralController', { $scope: $scope, $http: $http });

        	//-------------------------------------------------------------------------
        	var filters = function (page) {
                $("#loadingSpinners").show();
        		return {
        			page: (page === null || page === undefined) ? $scope.currentPage : page,
        			pageSize: $scope.pageSize,
                    searchSpecificKey: $scope.specificKey,
                    specificDescription: $scope.specificDescription,
                    searchMethod: $scope.searchMethod,
                    searchGenericKey: $scope.searchGenericKey,
        			viewParameters: $scope.viewParameters,
        			viewGroups: $scope.viewGroups,
        			viewPackages: $scope.viewPackages
        		};
        	};

            $scope.getSpecificKeySug = function () {
                $http.post("/sheettable/getSpecificKeySug", { suggestSpecificKey: $scope.specificKey, specificDescription: $scope.specificDescription, searchMethod: $scope.searchMethod, searchGenericKey: $scope.searchGenericKey })
                      .success(function (response) {
                          if (response.success) {
                              $scope.genericKeyList = response.suggestedGenericKeyList;
                              $scope.methodList = response.suggestedMethods;
                              $scope.specificKeyList = response.suggestedSpecificKeys;
                              $scope.specificDescList = response.suggestedSpecificDescription;
                          }
                          //$scope.isLoading = false;
                      })
                 .error(function () {
                  notification.error({
                      message: "Error en la conexión con el servidor.", delay: 5000
                  });
                  $scope.isLoading = false;
                 });
            }

            $scope.ResetFilter=function() {
                $scope.specificKey = null;
                $scope.specificDescription= null;
                $scope.searchMethod= null;
                $scope.searchGenericKey = null;

                $scope.RefreshList(1);
            }

        	$scope.mainFunction(
                 "icon mif-power", "Name",
                 "/sheettable/refreshlist", filters,
                 null, null,
                 null, null,
                 null,
                 null
             );

        	//XXXNUEVOXXX
        	$scope.isExcelScope = true;
        	$scope.printingOptiones = [false, false, false, false, false, false, false, false, false];

        	// Recordar que cualquier cambio q se haga en esta función hay que hacerlo también en 
        	// la que se redefinio para cada uno de los paneles dinámicos.
        	$scope.printReportDialog = function (isExcel) {
        		if (isExcel)
        			$scope.generatingEXCELFile = true;
        		else
        			$scope.generatingPDFFile = true;

        		$http.get('/report/generatereport', {
        			params: {

        				viewParameters: $scope.viewParameters,
        				viewGroups: $scope.viewGroups,
        				viewPackages: $scope.viewPackages,
        				isExcel: isExcel

        			},
        			responseType: 'arraybuffer'
        		})
                       // }, { responseType: 'arraybuffer' })
                        .success(function (response) {

                        	// if(isExcel)
                        	//var file = new Blob([response], { type: type });
                        	//else 
                        	var file;

                        	if (isExcel)
                        		file = new Blob([response], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
                        	else
                        		file = new Blob([response], { type: 'application/pdf' }); //can be: 'application/zip' "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

                        	var fileURL = URL.createObjectURL(file);
                        	window.open(fileURL, "newtab");
                        	$scope.generatingPDFFile = false;
                        	$scope.generatingEXCELFile = false;
                        	// window.open(response, "newtab");
                        	//$scope.pdfContent = $sce.trustAsResourceUrl(fileURL);
                        })
                        .error(function () {
                        	notification.error({
                        		message: 'Error en la conexi&oacute;n con el servidor.',
                        		delay: 5000
                        	});
                        });

        	}
        	//}, function (reason) { });
        	//}
        	// Group specifics
        	$scope.FilterGeneral = true;
        	$scope.FilterActive = true;
        	$scope.boxHeaderTitleActive = "ELEMENTOS ACTIVOS";
        	//$scope.boxHeaderTitleDeactive = "Elementos de baja";

        	$scope.panels = [
                {
                	index: 0,
                	name: "FILTROS",
                	state: false
                },
                {
                	index: 1,
                	name: "TABLA PRINCIPAL",
                	state: true
                }
        	];

        	//to configure tree object
        	$scope.my_tree = {};

        	$scope.expanding_property = {
        		field: "Name",
        		displayName: "Clave Específica",
        		sortable: true,
        		filterable: true,
        		width: "200px"
        	};

        	$scope.col_defs = new Array();

        	$scope.col_defs.push({
        		field: "elemType",
        		displayName: "Tipo de artículo",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{row.branch[col.field] === 'package' ? 'Paquete' : row.branch[col.field]==='group' ? 'Grupo' : 'Parámetro'}}</div>",
        		width: "150px"
        	});

        	$scope.col_defs.push({
        		field: "Description",
        		levels: [1, 2, 3],
        		displayName: "Descripción Específica",
        		width: "250px",
        		cellTemplate: "<div>{{row.branch[col.field]}}</div>"
        	});

        	$scope.col_defs.push({
        		field: "Metodo",
        		displayName: "Método Analítico/Muestreo",
        		width: "250px",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{row.branch[col.field].Name}}</div>"
        	});

        	//$scope.col_defs.push({
        	//	field: "GenericKey",
        	//	displayName: "Clave Genérica",
        	//	levels: [1, 2, 3],
        	//	width: "200px",
        	//	cellTemplate: "<div>{{row.branch[col.field]}}</div>"
        	//});

        	//$scope.col_defs.push({
        	//	field: "GenericDescription",
        	//	displayName: "Descripción Genérica",
        	//	width: "250px",
        	//	levels: [1, 2, 3]
        	//});

        	$scope.col_defs.push({
        		field: "Matrixes",
        		displayName: "Matriz",
        		levels: [1, 2, 3],
        		//cellTemplate: "<div><span style='cursor:pointer; font-size:1.2rem' ng-click='cellTemplateScope.click(row.branch)' class='icon mif-windy4'></span></div>",
        		cellTemplate: "<div><a href='' ng-click='cellTemplateScope.click(row.branch)'>{{row.branch[col.field][0].Name}}</a></div>",
        		cellTemplateScope: {
        			click: function (data) {

        				var addr = (data.elemType === "package") ? "/package/getMatrixes" : (data.elemType === "group") ? "/group/getMatrixes" : "/param/getMatrixes";

        				$http.post(addr, { Id: data.Id })
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
        		width: "200px"
        	});

        	$scope.col_defs.push({
        		field: "Matrixes",
        		displayName: "Línea de Negocio", //mercado
        		levels: [1, 2, 3],
        		cellTemplate: "<div  class='text-uppercase'>{{row.branch[col.field][0].BaseMatrix.Mercado}}</div>",
        		width: "180px"
        	});

        	$scope.col_defs.push({
        		field: "TipoServicio",
        		displayName: "Tipo de Servicio",
        		levels: [1, 2, 3],
        		width: "200px",
        		cellTemplate: "<div>{{row.branch[col.field].Name}}</div>",
        		tooltip: "Tipo de Servicio"
        	});

            //TODO: Aqui va Linea (Intelesis). Hay que ver si es una propiedad de Param para agregarselo.

        	$scope.col_defs.push({
        		field: "ClasificacionQuimica1",
        		displayName: "Clasificación Química (Nivel I)",
        		cellTemplate: "<div>{{row.branch[col.field] !== undefined ? row.branch[col.field].Name : 'Paquete'}}</div>",
        		levels: [1, 2, 3],
        		width: "180px",
        		tooltip: "Clasificación Química (Nivel I)"
        	});

        	$scope.col_defs.push({
        		field: "ClasificacionQuimica2",
        		displayName: "Clasificación Química (Nivel II)",
        		cellTemplate: "<div>{{row.branch[col.field] !== undefined ? row.branch[col.field].Name : 'Paquete'}}</div>",
        		levels: [1, 2, 3],
        		width: "180px",
        		tooltip: "Clasificación Química (Nivel II)"
        	});

        	$scope.col_defs.push({
        		field: "ClasificacionQuimica3",
        		displayName: "Clasificación Química (Nivel III)",
        		cellTemplate: "<div>{{row.branch[col.field] !== undefined ? row.branch[col.field].Name : 'Paquete'}}</div>",
        		levels: [1, 2, 3],
        		width: "180px",
        		tooltip: "Clasificación Química (Nivel III)"
        	});
            
        	$scope.col_defs.push({
        		field: "SucursalRealiza",
        		displayName: "Sucursal/Instalación donde se realiza",
        		levels: [1, 2, 3],
        		width: "220px",
        		cellTemplate: "<div>{{row.branch[col.field].Name}}</div>"
        	});

            // No aparece en la nueva Estructura de Columnas de BD.
        	// $scope.col_defs.push({
        	// 	field: "SucursalVende",
        	// 	levels: [1, 2, 3],
        	// 	displayName: "Sucursal/Instalación donde se vende",
        	// 	cellTemplate: "<div>{{row.branch[col.field].Name}}</div>",
        	// 	width: "250px"
        	// });

        	$scope.col_defs.push({
        		field: "Rama",
        		displayName: "Rama",
        		levels: [1, 2, 3],
        		width: "100px",
        		cellTemplate: "<div>{{row.branch[col.field]}}</div>"
        	});

        	$scope.col_defs.push({
        		field: "CentroCosto",
        		displayName: "CeC",
        		tooltip: "Centro de Costos",
        		levels: [1, 2, 3],
        		width: "100px",
        		cellTemplate: "<div>{{row.branch[col.field].Number}}</div>"
        	});

        	$scope.col_defs.push({
        		field: "Precio",
        		displayName: "Precio Lista (MXN)",
                tooltip: "Precio Lista (MXN)",
        		width: "150px",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{row.branch[col.field].Value}}({{row.branch[col.field].Currency.Name}})</div>",
        		//"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
        	});

        	$scope.col_defs.push({
        		field: "Active",
        		displayName: "Estatus",
        		levels: [1, 2, 3],
        		width: "100px",
        		cellTemplate: "<div>{{row.branch[col.field] === false ? 'BAJA' : 'ALTA'}}</div>"
        	});

            $scope.col_defs.push({
             field: "InternetPublish",
             displayName: "PI",
             width: "50px",
             levels: [1, 2, 3],
             cellTemplate: "<div>{{row.branch[col.field] ? 'Si' : 'No'}}</div>",
             tooltip: "Publicar en Internet"
            });

        	$scope.col_defs.push({
        		field: "AnalyticsMethod",
        		displayName: "Técnica Analítica",
        		width: "275px",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{row.branch[col.field].Name}}</div>",
        		tooltip: "Técnica Analítica"
        	});

        	$scope.col_defs.push({
        		field: "DetectionLimit",
        		displayName: "LDM",
        		width: "50px",
        		levels: [1, 2, 3],
        		cellTemplate: "<div ng-show='row.branch[col.field]'>{{row.branch[col.field]===null || row.branch[col.field].Value === null ? 'N/A' : row.branch[col.field].Value.toFixed(row.branch[col.field].Decimals)}}</div>",
        		tooltip: "Límite de Detección del Método"
        	});

            $scope.col_defs.push({
                field: "DetectionLimit",
                displayName: "Decimales para el LDM",
                width: "100px",
                levels: [1, 2, 3],
                cellTemplate: "<div ng-show='row.branch[col.field]'>{{row.branch[col.field].Decimals}}</div>",
                tooltip: "Decimales para el Límite de Detección del Método"
            });

            $scope.col_defs.push({
                field: "CuantificationLimit",
                displayName: "LPC",
                width: "50px",
                levels: [1, 2, 3],
                cellTemplate: "<div ng-show='row.branch[col.field]'>{{!row.branch[col.field] || row.branch[col.field].Value === null ? 'N/A' : row.branch[col.field].Value.toFixed(row.branch[col.field].Decimals)}}</div>",
                tooltip: "Límite Práctico de Cuantificación"
            });

            $scope.col_defs.push({
                field: "CuantificationLimit",
                displayName: "Decimales para el LPC",
                width: "100px",
                levels: [1, 2, 3],
                cellTemplate: "<div ng-show='row.branch[col.field]'>{{row.branch[col.field].Decimals}}</div>",
                tooltip: "Decimales para el LPC"
            });

            $scope.col_defs.push({
                field: "Unit",
                displayName: "Unidades de Reporte",
                width: "100px",
                levels: [1, 2, 3],
                tooltip: "Unidades de Reporte"
            });

            $scope.col_defs.push({
                field: "DecimalesReporte",
                displayName: "Decimales para Reporte",
                tooltip: "Decimales para Reporte",
                levels: [1, 2, 3],
                cellTemplate: "<div>{{row.branch[col.field]===null?'N/A':row.branch[col.field]}}</div>",
                width: "120px"
            });

            $scope.col_defs.push({
                field: "MaxTimeBeforeAnalysis",
                displayName: "TMPA en días nat.",
                width: "100px",
                levels: [1, 2, 3],
                cellTemplate: "<div>{{row.branch[col.field] === null ? '-' : row.branch[col.field]}}</div>",
                tooltip: "Tiempo Máximo Previo al Análisis (días naturales)"
            });

            $scope.col_defs.push({
             field: "AnalysisTime",
             displayName: "TPA en días nat.",
             width: "100px",
             levels: [1, 2, 3],
             cellTemplate: "<div>{{row.branch[col.field].AnalysisTime === null ? '-' : row.branch[col.field].AnalysisTime}}</div>",
             tooltip: "Tiempo para el Análisis (días naturales)"
            });

        	$scope.col_defs.push({
        		field: "Container",
        		displayName: "Tipo de Envase",
        		width: "150px",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{row.branch[col.field]}}</div>",
        		tooltip: "Tipo de Envase"
        	});

        	$scope.col_defs.push({
        		field: "MinimumVolume",
        		displayName: "Volumen Mínimo (mL)",
        		width: "120px",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{!row.branch[col.field] || row.branch[col.field] === null ? 'N/A' : row.branch[col.field]}}</div>",
        		tooltip: "Volumen Mínimo (mL)"
        	});

        	$scope.col_defs.push({
        		field: "RequiredVolume",
        		displayName: "Volumen Deseado (mL)",
        		width: "120px",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{!row.branch[col.field] || row.branch[col.field] === null ? 'N/A' : row.branch[col.field]}}</div>",
        		tooltip: "Volumen Deseado (mL)"
        	});

        	$scope.col_defs.push({
        		field: "Preserver",
        		displayName: "Tipo de Preservador",
        		width: "170px",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{row.branch[col.field]}}</div>",
        		tooltip: "Tipo de Preservador"
        	});

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
        		width: "100px",
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
        		cellTemplate: "<a href='' ng-repeat='ro in row.branch[col.field]' ng-style=\"{'color': ro.Expired ? '#CCC7C7' : ''}\" ng-click='cellTemplateScope.click(ro.Key)'>{{ro.Key}}{{row.branch[col.field][row.branch[col.field].length -1] !== ro ? ',' : ''}} </a>",
                cellTemplateScope: {
                    click: function (data) {
                        $scope.viewAckInfo(data);
                    }
                }
        	});

        	$scope.col_defs.push({
        		field: "Uncertainty",
        		displayName: "Urel en %",
        		width: "80px",
        		levels: [1, 2, 3],
        		cellTemplate: "<div ng-show='row.branch[col.field]'>{{!row.branch[col.field] || row.branch[col.field].Value ===null ? 'N/A' : row.branch[col.field].Value.toFixed(row.branch[col.field].Decimals)}}</div>",
        		tooltip: "Incertidumbre"
        	});

        	$scope.col_defs.push({
        		field: "Signatarios",
        		displayName: "Signatarios",
        		width: "200px",
        		levels: [1, 2, 3],
        		tooltip: "Signatarios",
        		cellTemplate: "<a href='' ng-repeat='sig in row.branch[col.field]' ng-click='cellTemplateScope.click(sig)'>{{sig}}{{row.branch[col.field][row.branch[col.field].length -1] !== sig ? ',' : ''}} </a>",
        		cellTemplateScope: {
        			click: function (data) {
				        $scope.viewAnnalistInfo(data);
			        }
        		}
        	});

        	$scope.col_defs.push({
        		field: "Eidas",
        		displayName: "EIDAS",
        		width: "200px",
        		levels: [1, 2, 3],
        		tooltip: "EIDAS",
        		cellTemplate: "<a href='' ng-repeat='eidas in row.branch[col.field]' ng-click='cellTemplateScope.click(eidas)'>{{eidas}}{{row.branch[col.field][row.branch[col.field].length -1] !== eidas ? ',' : ''}} </a>",
        		cellTemplateScope: {
        			click: function (data) {
        				$scope.viewAnnalistInfo(data);
        			}
        		}
        	});

        	$scope.col_defs.push({
        		field: "DeliverTime",
        		displayName: "TEC en días háb.",
        		width: "100px",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{row.branch[col.field] === null ? '-' : row.branch[col.field]}}</div>",
        		tooltip: "Tiempo de entrega al Cliente (días hábiles)"
        	});

        	$scope.col_defs.push({
        		field: "ResiduoPeligroso",
        		displayName: "Residuo Peligroso",
        		width: "70px",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{row.branch[col.field] === true ? 'Si' : 'No'}}</div>",
        		tooltip: "Residuo Peligroso"
        	});

        	$scope.col_defs.push({
             field: "Residue",
             displayName: "Tipo de Desecho",
             width: "120px",
             levels: [1, 2, 3],
             cellTemplate: "<div>{{row.branch[col.field]}}</div>",
             tooltip: "Tipo de Desecho"
            });

        	$scope.col_defs.push({
        		field: "CentroCosto",
        		displayName: "Área Analítica",
        		width: "70px",
        		tooltip: "Área Analítica",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{row.branch[col.field].Key}}</div>"
        	});

        	$scope.col_defs.push({
        		field: "AnnalistKey",
        		displayName: "Clave de Analista Asignado",
        		tooltip: "Clave de Analista Asignado",
        		width: "150px"
        	});

        	$scope.col_defs.push({
        		field: "MostrarLista",
        		displayName: "Mostrar Lista de Componentes del Grupo",
        		levels: [1, 2],
        		width: "200px",
        		cellTemplate: "<div>{{row.branch[col.field] === true ? 'Si' : row.branch[col.field] === undefined ? '' : 'No'}}</div>",
        		tooltip: "Mostrar Lista de Componentes del Grupo"
        	});

        	$scope.col_defs.push({
        		field: "DispParam",
        		displayName: "Disparador de los Parámetros del Grupo",
        		cellTemplate: "<div>{{row.branch[col.field]===null?'N/A':row.branch[col.field].Name}}</div>",
        		levels: [1, 2, 3],
        		tooltip: "Disparador de los Parámetros del Grupo",
        		width: "200px"
        	});


        	$scope.col_defs.push({
        		field: "ReportaCliente",
        		displayName: "Se Reporta al Cliente",
        		width: "100px",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{row.branch[col.field] === true ? 'Si' : 'No'}}</div>",
        		tooltip: "Se Reporta al Cliente"
        	});

            // En la Estructura de Columnas de BD esta: Se publica en Autolab.
        	$scope.col_defs.push({
        		field: "PublishInAutolab",
        		displayName: "Solo para cotizar",
        		width: "100px",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{row.branch[col.field] === false ? 'Si' : 'No'}}</div>",
        		tooltip: "Solo para cotizar"
        	});

        	$scope.col_defs.push({
        		field: "PerWeekCapacity",
        		displayName: "Cap. Ins./Sem. (Máxima)",
        		width: "150px",
        		tooltip: "Capacidad Instalada/Semana (Máxima)",
        		levels: [1, 2, 3],
        		cellTemplate: "<div ng-show='row.branch[col.field]'>{{!row.branch[col.field] ? 'N/A' : row.branch[col.field]}}</div>"
        	});

        	$scope.col_defs.push({
        		field: "PerTurnCapacity",
        		displayName: "Cap. Ins./Sem. (Trabajo Combinado)",
        		width: "180px",
        		tooltip: "Capacidad Instalada/Semana (Trabajo Combinado)",
        		levels: [1, 2, 3],
        		cellTemplate: "<div ng-show='row.branch[col.field]'>{{!row.branch[col.field] ? 'N/A' : row.branch[col.field]}}</div>"
        	});

        	$scope.col_defs.push({
        		field: "ReportTime",
        		displayName: "TEA en días háb.",
        		width: "80px",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{row.branch[col.field] === null ? '-' : row.branch[col.field]}}</div>",
        		tooltip: "Tiempo de Entrega para el Analista (días hábiles)"
        	});

        	// NO APARECEN EN CAT PARAMETROS

            //      $scope.col_defs.push({
     //         field: "MaxPermitedLimit",
     //         displayName: "LMP",
     //         width: "50px",
     //         tooltip: "Límite Máximo Permitido",
     //         levels: [1, 2, 3],
     //         cellTemplate: "<div><span style='cursor:pointer; font-size:1.2rem' ng-if=\"row.branch.elemType=='parameter'\" ng-click='cellTemplateScope.click(row.branch)' class='mif-thermometer2 icon'></span></div>",
     //         cellTemplateScope: {
     //             click: function (data) {

     //                 $http.post("/param/getallmaxpermitedlimit", { Id: data.Id })
                    //      .success(function (response) {
                    //          if (response.success) {

                    //              ngDialog.openConfirm({
                    //                  template: "Shared/ShowLMP",
                    //                  controller: "SetLMPDialogController",
                    //                  className: "ngdialog-theme-default",
                    //                  preCloseCallback: "preCloseCallbackOnScope",
                    //                  data: { dData: response.LMP, paramKey: data.ParamUniquekey },
                    //                  closeByEscape: true
                    //              })
                    //          .then(function (promise) {
                    //              //Accepting

                    //          }, function (reason) {
                    //              //Rejecting

                    //          });


                    //          }
                    //          $scope.isLoading = false;
                    //      })
                    // .error(function () {
                    //  notification.error({
                    //      message: "Error en la conexión con el servidor.", delay: 5000
                    //  });
                    //  $scope.isLoading = false;
                    // });


     //             }
     //         }
     //     });

        	//$scope.col_defs.push({
        	//	field: "AutolabAssignedAreaName",
        	//	displayName: "NAAA",
        	//	width: "50px",
        	//	tooltip: "Nombre de Área Autolab Asignada"
        	//});
        	//$scope.tableWidth += 50;

        	//$scope.col_defs.push({
        	//	field: "GenericKeyForStatistic",
        	//	displayName: "CGE",
        	//	width: "70px",
        	//	tooltip: "Clave genérica para estadísticas"
        	//});

        	//$scope.col_defs.push({
        	//	field: "Metodo",
        	//	displayName: "TEL",
        	//	width: "50px",
        	//	levels: [1, 2, 3],
        	//	cellTemplate: "<div>{{row.branch[col.field].LabDeliverTime === null ? '-' : row.branch[col.field].LabDeliverTime}}</div>",
        	//	tooltip: "Tiempo Entrega al Laboratorio (días hábiles)"
        	//});

        	//$scope.col_defs.push({
        	//	field: "TipoServicio",
        	//	displayName: "TServ",
        	//	levels: [1, 2, 3],
        	//	cellTemplate: "<div>{{row.branch[col.field].Name}}</div>",
        	//	tooltip: "Tipo de Servicio"
        	//});
        	//$scope.tableWidth += 100;

        	$scope.col_defs.push({
        		field: "Week",
        		displayName: "Ln",
        		width: "50px",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{row.branch[col.field].Monday === true ? 'Si' : row.branch[col.field].Monday === undefined ? '' : 'No'}}</div>",
        		tooltip: "Se programa el Lunes"
        	});

        	$scope.col_defs.push({
        		field: "Week",
        		displayName: "Mt",
        		levels: [1, 2, 3],
        		width: "50px",
        		cellTemplate: "<div>{{row.branch[col.field].Tuesday === true ? 'Si' : row.branch[col.field].Monday === undefined ? '' : 'No'}}</div>",
        		tooltip: "Se programa el Martes"
        	});

        	$scope.col_defs.push({
        		field: "Week",
        		displayName: "Mc",
        		levels: [1, 2, 3],
        		width: "50px",
        		cellTemplate: "<div>{{row.branch[col.field].Wednesday === true ? 'Si' : row.branch[col.field].Monday === undefined ? '' : 'No'}}</div>",
        		tooltip: "Se programa el Miércoles"
        	});

        	$scope.col_defs.push({
        		field: "Week",
        		displayName: "Ju",
        		levels: [1, 2, 3],
        		width: "50px",
        		cellTemplate: "<div>{{row.branch[col.field].Thursday === true ? 'Si' : row.branch[col.field].Monday === undefined ? '' : 'No'}}</div>",
        		tooltip: "Se programa el Jueves"
        	});

        	$scope.col_defs.push({
        		field: "Week",
        		displayName: "Vi",
        		levels: [1, 2, 3],
        		width: "50px",
        		cellTemplate: "<div>{{row.branch[col.field].Friday === true ? 'Si' : row.branch[col.field].Monday === undefined ? '' : 'No'}}</div>",
        		tooltip: "Se programa el Viernes"
        	});

        	$scope.col_defs.push({
        		field: "Week",
        		displayName: "Sa",
        		levels: [1, 2, 3],
        		width: "50px",
        		cellTemplate: "<div>{{row.branch[col.field].Saturday === true ? 'Si' : row.branch[col.field].Monday === undefined ? '' : 'No'}}</div>",
        		tooltip: "Se programa el Sábado"
        	});

        	$scope.col_defs.push({
        		field: "Week",
        		displayName: "Dm",
        		levels: [1, 2, 3],
        		width: "50px",
        		cellTemplate: "<div>{{row.branch[col.field].Sunday === true ? 'Si' : row.branch[col.field].Monday === undefined ? '' : 'No'}}</div>",
        		tooltip: "Se programa el Domingo"
        	});

        	$scope.col_defs.push({
        		field: "SellSeparated",
        		displayName: "Se vende por separado",
        		width: "220px",
        		levels: [1, 2, 3],
        		cellTemplate: "<div ng-style=\"{'background-color': (row.branch[col.field] === true) ? 'transparent' : 'red'}\">{{row.branch[col.field] === true ? 'Si' : 'No se vende por separado'}}</div>",
        		tooltip: "Se vende por separado"
        	});

        	$scope.col_defs.push({
        		field: "CuentaEstadistica",
        		displayName: "Cuenta para Estadística",
        		width: "100px",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{row.branch[col.field] === true ? 'Si' : 'No'}}</div>",
        		tooltip: "Cuenta para Estadística"
        	});

        	for (var j = 0; j < $scope.col_defs.length; j++) {
        		if ($scope.col_defs[j]["width"] === undefined) {
        			$scope.tableWidth += 100;
        			continue;
        		}
        		$scope.tableWidth += 1 * $scope.col_defs[j]["width"].slice(0, $scope.col_defs[j]["width"].length - 2);
        	}
        	// };

        	//if ($scope.columnList === undefined) {
        	//	SecurityFact.subscribe($scope, function updateColumnList() {
        	//		//$scope.columnList = SecurityFact.getColumnList();
        	//		printColumnList();
        	//	});
        	//}
        	//else
        	//	printColumnList();

        	//$scope.my_tree_handler = function (branch) {
        	//	console.log("Eventos: ", branch);
        	//};

        	// Funciones para lanzar dialogos de algunos elementos
        	$scope.viewAnnalistInfo = function (a) {
        		InfoService.viewAnnalistInfo(a);
        	}

            $scope.viewAckInfo = function (a) {
                InfoService.viewAckInfo(a);
            }

        	// Filtros dinamicos
        	$scope.RecVR = true;

        	$scope.instalacion1 = [
				{
					index: 0,
					name: "Venden",
					selected: false
				},
				{
					index: 1,
					name: "Realizan",
					selected: false
				}
        	];

        	$scope.instalacion2 = [];
        	$scope.instalacion3 = [];

        	//var exampleDictionary = {"market": 0, "matrix": 0, "method": 0, "sucursal": 0, "family": 0};

        	//var deleteElemByIndex = function (index) {
        	//	for (var i = 0; i < $scope.filteredPanels.length; i++) {
        	//		if ($scope.filteredPanels[i].index === index) {
        	//			$scope.panels.splice($scope.filteredPanels[i].index, 1);
        	//			for (var j = $scope.filteredPanels[i].index; j < $scope.panels.length; j++) {
        	//				$scope.panels[j].index -= 1;
        	//			}
        	//			$scope.filteredPanels.splice(i, 1);
        	//			break;
        	//		}
        	//	}
        	//}

        	var addElem = function (element) {	//x, type
        		var elem = {
        			index: $scope.panels.length,
        			name: element.name,
        			state: true,
        			isFullscreen: false,
        			page_total: 0,
        			page_size: 10,
        			current_page: 1,
        			elementsList: null,
        			my_tree: {},
        			generatingEXCELFile: false,
        			generatingPDFFile: false,
        			isLoading: false,
        			viewParameters: true,
        			viewGroups: true,
        			viewPackages: true,
        			market: element["market"].Id,		//type === "market" ? x.Id : null,
        			matrix: element["matrix"].Id,		//type === "matrix" ? x.Id : null,
        			sucursal: element["sucursal"].Id,	//type === "sucursal" ? x.Id : null,
        			clasquim1: element["clasquim1"].Id,		//type === "clasquim1" ? x.Id : null,
        			clasquim2: element["clasquim2"].Id,		//type === "clasquim2" ? x.Id : null,
        			clasquim3: element["clasquim3"].Id,		//type === "clasquim3" ? x.Id : null,
        			RefreshList: function (page) {
        				elem.isLoading = true;
        				$http.post("/sheettable/refreshlist", {
        					page: (page === null || page === undefined) ? this.current_page : page,
        					pageSize: this.page_size,
        					viewParameters: this.viewParameters,
        					viewGroups: this.viewGroups,
        					viewPackages: this.viewPackages,
        					marketId: this.market === undefined || this.market === null ? 0 : this.market,
        					matrixId: this.matrix === undefined || this.matrix === null ? 0 : this.matrix,
        					sucursalId: this.sucursal === undefined || this.sucursal === null ? 0 : this.sucursal,
        					clasquim1Id: this.clasquim1 === undefined || this.clasquim1 === null ? 0 : this.clasquim1,
        					clasquim2Id: this.clasquim2 === undefined || this.clasquim2 === null ? 0 : this.clasquim2,
        					clasquim3Id: this.clasquim3 === undefined || this.clasquim3 === null ? 0 : this.clasquim3
        				})
						.success(function (response) {
							if (response.success) {
								elem.elementsList = response.elements;
								elem.page_total = response.total;
								//if (elem.page_total === 0) {
								//	deleteElemByIndex(elem.index);
								//}
								elem.isLoading = false;
							} else {
								notification.error({
									message: "Error al recuperar los Elementos",
									delay: 5000
								});
								elem.isLoading = false;
							}
						})
						.error(function () {
							notification.error({
								message: "Error en la conexión con el servidor.", delay: errorDelay
							});
							elem.isLoading = false;
						});
        			},
        			printReportDialog: function (isExcel) {
        				if (isExcel)
        					elem.generatingEXCELFile = true;
        				else
        					elem.generatingPDFFile = true;

        				$http.get('/report/generatereport', {
        					params: {
        						viewParameters: this.viewParameters,
        						viewGroups: this.viewGroups,
        						viewPackages: this.viewPackages,
        						marketId: this.market === undefined || this.market === null ? 0 : this.market,
        						matrixId: this.matrix === undefined || this.matrix === null ? 0 : this.matrix,
        						sucursalId: this.sucursal === undefined || this.sucursal === null ? 0 : this.sucursal,
        						clasquim1Id: this.clasquim1 === undefined || this.clasquim1 === null ? 0 : this.clasquim1,
        						clasquim2Id: this.clasquim2 === undefined || this.clasquim2 === null ? 0 : this.clasquim2,
        						clasquim3Id: this.clasquim3 === undefined || this.clasquim3 === null ? 0 : this.clasquim3,
        						isExcel: isExcel
        					},
        					responseType: 'arraybuffer'
        				})
						.success(function (response) {
							var file;
							if (isExcel)
								file = new Blob([response], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
							else
								file = new Blob([response], { type: 'application/pdf' }); //can be: 'application/zip' "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

							var fileURL = URL.createObjectURL(file);
							window.open(fileURL, "newtab");
							elem.generatingPDFFile = false;
							elem.generatingEXCELFile = false;
						})
						.error(function () {
							notification.error({
								message: 'Error en la conexi&oacute;n con el servidor.',
								delay: 5000
							});
						});
        			},
        			toggleFullScreen: function () {
        				this.isFullscreen = !this.isFullscreen;
        			}
        		};

        		$scope.filteredPanels.push(elem);
        		$scope.panels.push(elem);
        		$scope.filteredPanels[$scope.filteredPanels.length - 1].RefreshList(1);
        	}

        	var dynamicFilters = {};

        	var deleteElem = function (x, type) {
        		for (var i = 0; i < $scope.filteredPanels.length; i++) {
        			if ($scope.filteredPanels[i][type] === x.Id) {
        				$scope.panels.splice($scope.filteredPanels[i].index, 1);
        				for (var j = $scope.filteredPanels[i].index; j < $scope.panels.length; j++) {
        					$scope.panels[j].index -= 1;
        				}
        				delete dynamicFilters[$scope.filteredPanels[i].name];
        				$scope.filteredPanels.splice(i, 1);
        				i -= 1;
        			}
        		}

        		// quitando elementos seleccionados
        		for (var k = 0; k < $scope.instalacion2.length; k++) {
        			var flag = false;
        			for (i = 0; i < $scope.filteredPanels.length; i++) {
        				if ($scope.instalacion2[k].Id === $scope.filteredPanels[i].sucursal) {
        					flag = true;
        					break;
        				}
        			}
        			if (!flag)
        				$scope.instalacion2[k].selected = false;
        		}

        		for (k = 0; k < $scope.instalacion3.length; k++) {
        			flag = false;
        			for (i = 0; i < $scope.filteredPanels.length; i++) {
        				if ($scope.instalacion3[k].Id === $scope.filteredPanels[i].sucursal) {
        					flag = true;
        					break;
        				}
        			}
        			if (!flag)
        				$scope.instalacion3[k].selected = false;
        		}

        		for (k = 0; k < $scope.markets.length; k++) {
        			flag = false;
        			for (i = 0; i < $scope.filteredPanels.length; i++) {
        				if ($scope.markets[k].Id === $scope.filteredPanels[i].market) {
        					flag = true;
        					break;
        				}
        			}
        			if (!flag)
        				$scope.markets[k].selected = false;
        		}

        		for (k = 0; k < $scope.matrixes.length; k++) {
        			flag = false;
        			for (i = 0; i < $scope.filteredPanels.length; i++) {
        				if ($scope.matrixes[k].Id === $scope.filteredPanels[i].matrix) {
        					flag = true;
        					break;
        				}
        			}
        			if (!flag)
        				$scope.matrixes[k].selected = false;
        		}

        		for (k = 0; k < $scope.clasquims1.length; k++) {
        			flag = false;
        			for (i = 0; i < $scope.filteredPanels.length; i++) {
        				if ($scope.clasquims1[k].Id === $scope.filteredPanels[i].clasquim1) {
        					flag = true;
        					break;
        				}
        			}
        			if (!flag)
        				$scope.clasquims1[k].selected = false;
        		}

        		for (k = 0; k < $scope.clasquims2.length; k++) {
        			flag = false;
        			for (i = 0; i < $scope.filteredPanels.length; i++) {
        				if ($scope.clasquims2[k].Id === $scope.filteredPanels[i].clasquim2) {
        					flag = true;
        					break;
        				}
        			}
        			if (!flag)
        				$scope.clasquims2[k].selected = false;
        		}

        		for (k = 0; k < $scope.clasquims3.length; k++) {
        			flag = false;
        			for (i = 0; i < $scope.filteredPanels.length; i++) {
        				if ($scope.clasquims3[k].Id === $scope.filteredPanels[i].clasquim3) {
        					flag = true;
        					break;
        				}
        			}
        			if (!flag)
        				$scope.clasquims3[k].selected = false;
        		}
        		// fin quitando elementos seleccionados
        	}


        	function getInstalaciones(x, y) {
        		$http.post('/sucursal/GetSucursalesToFilter',
					{ x: x === null ? false : x, y: y === null ? false : y, recVr: $scope.RecVR })
				.success(function (response) {
					if (response.success === true) {
						for (var i = 0; i < $scope.filteredPanels.length; i++) {
							var flag = false;
							for (var j = 0; j < response.elements.length; j++) {
								if ($scope.filteredPanels[i].sucursal === response.elements[j].Id) {
									response.elements[j].selected = true;
									flag = true;
									break;
								}
							}
							if (!flag && $scope.filteredPanels[i].sucursal !== null && $scope.filteredPanels[i].sucursal !== 0) {
								deleteElem({ Id: $scope.filteredPanels[i].sucursal }, "sucursal");
								i -= 1;
							}
						}

						for (i = 0; i < response.elementsX.length; i++) {
							for (j = 0; j < response.elements.length; j++) {
								if (response.elementsX[i].name === response.elements[j].name) {
									response.elementsX[i].selected = response.elements[j].selected === undefined ? false : true;
									break;
								}
							}
						}

						for (i = 0; i < response.elementsY.length; i++) {
							for (j = 0; j < response.elements.length; j++) {
								if (response.elementsY[i].name === response.elements[j].name) {
									response.elementsY[i].selected = response.elements[j].selected === undefined ? false : true;
									break;
								}
							}
						}

						$scope.instalacion2 = response.elementsX;
						$scope.instalacion3 = response.elementsY;

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

        	$scope.selectInstalacion1 = function (x) {
        		x.selected = !x.selected;
        		getInstalaciones($scope.instalacion1[0].selected, $scope.instalacion1[1].selected);
        	}

        	$scope.RecVRChange = function (selection) {
        		if (selection === true) {
        			$scope.instalacion1[0].name = "Venden";
        			$scope.instalacion1[1].name = "Realizan";
        		} else {
        			$scope.instalacion1[0].name = "Con reconocimientos";
        			$scope.instalacion1[1].name = "Sin reconocimientos";
        		}
        		$scope.instalacion1[0].selected = false;
        		$scope.instalacion1[1].selected = false;
        		getInstalaciones($scope.instalacion1[0].selected, $scope.instalacion1[1].selected);
        	}

        	function getMarkets() {
        		$http.post('/market/GetMarketsToFilter')
				.success(function (response) {
					if (response.success === true) {
						$scope.markets = response.elements;
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

        	getMarkets();

        	// Actually, base matrixes
        	function getMatrixes() {
        		$http.post('/matrix/GetMatrixesToFilter')
				.success(function (response) {
					if (response.success === true) {
						$scope.matrixes = response.elements;
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

        	getMatrixes();

        	function getClasQuim() {
        		$http.post('/clasificacionquimica/GetClasQuimToFilter')
				.success(function (response) {
					if (response.success === true) {
						$scope.clasquims1 = response.clasquims1;
						$scope.clasquims2 = response.clasquims2;
						$scope.clasquims3 = response.clasquims3;
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

        	getClasQuim();

        	$scope.filteredPanels = [];

        	$scope.selectElement = function (x, type) {
        		x.selected = !x.selected;

        		if (x.selected) {

        			if (type === "sucursal") {
        				for (var i = 0; i < $scope.instalacion2.length; i++) {
        					if ($scope.instalacion2[i].Id === x.Id) {
        						$scope.instalacion2[i].selected = true;
        						break;
        					}
        				}
        				for (i = 0; i < $scope.instalacion3.length; i++) {
        					if ($scope.instalacion3[i].Id === x.Id) {
        						$scope.instalacion3[i].selected = true;
        						break;
        					}
        				}
        			}

        			var counter = 0;

        			for (var df in dynamicFilters) {
        				if (dynamicFilters.hasOwnProperty(df)) {
        					counter += 1;
        					break;
        				}
        			}

        			if (counter === 0) {
        				dynamicFilters[x.name] = {
        					"name": x.name,
        					"market": { "Id": 0, "name": "", "type": "market" },
        					"matrix": { "Id": 0, "name": "", "type": "matrix" },
        					"sucursal": { "Id": 0, "name": "", "type": "sucursal" },
        					"clasquim1": { "Id": 0, "name": "", "type": "clasquim1" },
        					"clasquim2": { "Id": 0, "name": "", "type": "clasquim2" },
        					"clasquim3": { "Id": 0, "name": "", "type": "clasquim3" },
        					"type": type
        				};
        				dynamicFilters[x.name][type].Id = x.Id;
        				dynamicFilters[x.name][type].name = x.name;

        				addElem(dynamicFilters[x.name]);
        			} else {
        				var flag = false;

        				for (df in dynamicFilters) {
        					if (dynamicFilters.hasOwnProperty(df)) {
        						//if (dynamicFilters[df].type !== type || flag === false) {
        						if (dynamicFilters[df][type].Id === 0 || flag === false) {
        							var name = (dynamicFilters[df].type !== type ? df + " > " : "") + x.name;
        							dynamicFilters[name] = {
        								"name": name,
        								"market": { "Id": dynamicFilters[df]["market"].Id, "name": dynamicFilters[df]["market"].name, "type": "market" },
        								"matrix": { "Id": dynamicFilters[df]["matrix"].Id, "name": dynamicFilters[df]["matrix"].name, "type": "matrix" },
        								"sucursal": { "Id": dynamicFilters[df]["sucursal"].Id, "name": dynamicFilters[df]["sucursal"].name, "type": "sucursal" },
        								"clasquim1": { "Id": dynamicFilters[df]["clasquim1"].Id, "name": dynamicFilters[df]["clasquim1"].name, "type": "clasquim1" },
        								"clasquim2": { "Id": dynamicFilters[df]["clasquim2"].Id, "name": dynamicFilters[df]["clasquim2"].name, "type": "clasquim2" },
        								"clasquim3": { "Id": dynamicFilters[df]["clasquim3"].Id, "name": dynamicFilters[df]["clasquim3"].name, "type": "clasquim3" },
        								"type": type
        							};
        							dynamicFilters[name][type].Id = x.Id;
        							dynamicFilters[name][type].name = x.name;
        							flag = true;
        							addElem(dynamicFilters[name]);
        						}
        					}
        				}
        			}

        			//if (type === "sucursal") {
        			//	dynamicFilters[x.name] = {
        			//		"name": x.name,
        			//		"market": {"Id": 0, "name": ""},
        			//		"matrix": { "Id": 0, "name": "" },
        			//		"method": { "Id": 0, "name": "" },
        			//		"sucursal": { "Id": 0, "name": "" },
        			//		"family": { "Id": 0, "name": "" }
        			//    };
        			//} else {
        			//    for (df in dynamicFilters) {
        			//        if (dynamicFilters.hasOwnProperty(df)) {
        			//        	dynamicFilters[df][type] = x.Id;
        			//	        dynamicFilters[df]["name"] += " > " + x.name;
        			//	        //addElem(df);
        			//        }
        			//    }
        			//}
        			//addElem(x, type);
        		} else {
        			deleteElem(x, type);
        		}
        	}

        	// Fin Filtros dinamicos
        }
	])
     .controller("ReportDialogController",
    [
        "$scope", "$routeParams", "$location", "$http", "$controller", "SecurityFact", "Notification", "ngDialog", "generalFactory",
        function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog, generalFactory) {

        	var errorDelay = 5000;


        	// TODO: modificar para que ademas de mostrar la notificacion ocurra alguna sennalizacion en el input
        	$scope.acceptDialog = function () {

        		$scope.confirm({
        			success: true
        		});
        	}
        }
    ])

            .directive("minimizeSheet", ['$document', function ($document) {
            	return {
            		link: function ($scope, elem, attrs) {

            			$scope.minimize = function (element) {
            				element.state = !element.state;
            				for (var i = 0; i < $scope.panels.length; i++) {
            					if ($scope.panels[i].state === false) {
            						$scope.taskbar = true;
            						break;
            					}
            					$scope.taskbar = false;
            				}
            			};
            		}
            	};
            }])

;
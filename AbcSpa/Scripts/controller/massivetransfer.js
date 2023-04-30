"use strict";

angular.module("abcadmin")
    .controller("MassiveTransferController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', "SecurityFact",
		"Notification", "counterFactory", "ngDialog",
        function ($scope, $route, $location, $http, $cookies, $controller, securityFact,
			notification, counterFactory, ngDialog) {

	        var errorDelay = 5000;

        	$scope.accessLevel = securityFact.checkAccessLevel("massivetransfer");

        	if ($scope.accessLevel === undefined || $scope.accessLevel === null)
        		securityFact.subscribe($scope, function getAlmt() {
        			$scope.accessLevel = securityFact.checkAccessLevel("massivetransfer");
        		});

        	function getSucursales() {
        		$http.post('/sucursal/GetSucursales')
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

        	getSucursales();

	        $scope.sucursal1 = $scope.sucursal2 = null;

        	$scope.isLoading1 = $scope.isLoading2 = false;
	        $scope.currentPage1 = $scope.currentPage2 = 1;
	        $scope.pageSize1 = $scope.pageSize2 = 10;

	        $scope.viewParameters1 = $scope.viewParameters2 = true;
	        $scope.viewGroups1 = $scope.viewGroups2 = true;
	        $scope.viewPackages1 = $scope.viewPackages2 = true;
	        $scope.tableWidht = 0;

	        $scope.RefreshList = function (page, panel, suc) {
		        if (panel === 1) {
		        	$scope.isLoading1 = true;
		        } else {
		        	$scope.isLoading2 = true;
		        }
        		
        		$http.post("/massivetransfer/refreshlist", {
        			page: (page === null || page === undefined) ?
						panel === 1 ? $scope.currentPage1 : $scope.currentPage2 : page,
        			pageSize: panel === 1 ? $scope.pageSize1 : $scope.pageSize2,
        			viewParameters: panel === 1 ? $scope.viewParameters1 : $scope.viewParameters2,
        			viewGroups: panel === 1 ? $scope.viewGroups1 : $scope.viewGroups2,
        			viewPackages: panel === 1 ? $scope.viewPackages1 : $scope.viewPackages2,
        			sucursalId: suc === undefined ? 0 : suc
        		})
				.success(function (response) {
					if (response.success) {
						if (panel === 1) {
							$scope.elementsList1 = response.elements;
							$scope.pageTotal1 = response.total;
						} else {
							$scope.elementsList2 = response.elements;
							$scope.pageTotal2 = response.total;
						}
					} else {
						notification.error({
							message: "Error al recuperar los datos. " + response.err, delay: errorDelay
						});
					}
					if (panel === 1) {
						$scope.isLoading1 = false;
					} else {
						$scope.isLoading2 = false;
					}
				})
				.error(function () {
					notification.error({
						message: "Error en la conexión con el servidor.", delay: errorDelay
					});
					if (panel === 1) {
						$scope.isLoading1 = false;
					} else {
						$scope.isLoading2 = false;
					}
				});
        	};

        	//to configure tree object
	        $scope.my_tree = {};
	        $scope.my_tree2 = {};

        	$scope.expanding_property = {
        		field: "Name",
        		displayName: "Clave Específica",
        		sortable: true,
        		filterable: true,
        		width: "150px"
        	};
        	$scope.tableWidht += 150;

        	$scope.col_defs = new Array();

        	$scope.col_defs.push({
        		field: "elemType",
        		displayName: "Tipo de artículo",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{row.branch[col.field] === 'package' ? 'Paquete' : row.branch[col.field]==='group' ? 'Grupo' : 'Parámetro'}}</div>",
        		width: "150px"
        	});
        	$scope.tableWidht += 150;

        	$scope.col_defs.push({
        		field: "Description",
        		levels: [1, 2, 3],
        		displayName: "Descripción Específica",
        		width: "200px",
        		cellTemplate: "<div>{{row.branch[col.field]}}</div>"
        	});
        	$scope.tableWidht += 200;

        	$scope.col_defs.push({
        		field: "Metodo",
        		displayName: "Método Analítico/Muestreo",
        		width: "250px",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{row.branch[col.field].Name}}</div>"
        	});
        	$scope.tableWidht += 250;

        	$scope.col_defs.push({
        		field: "Matrixes",
        		displayName: "Matriz",
        		levels: [1, 2, 3],
        		cellTemplate: "<div><span style='cursor:pointer; font-size:1.2rem' ng-click='cellTemplateScope.click(row.branch)' class='icon mif-windy4'></span></div>",
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
        		width: "50px"
        	});
        	$scope.tableWidht += 50;

        	$scope.col_defs.push({
        		field: "Matrixes",
        		displayName: "Línea de Negocio", //mercado
        		levels: [1, 2, 3],
        		cellTemplate: "<div  class='text-uppercase'>{{row.branch[col.field][0].BaseMatrix.Mercado}}</div>",
        		width: "150px"
        	});
        	$scope.tableWidht += 150;

        	$scope.col_defs.push({
        		field: "SucursalVende",
        		displayName: "Sucursal/Instalación donde se realiza",
        		levels: [1, 2, 3],
        		width: "150px",
        		cellTemplate: "<div>{{row.branch[col.field].Name}}</div>"
        	});
        	$scope.tableWidht += 150;

        	$scope.col_defs.push({
        		field: "SucursalRealiza",
        		levels: [1, 2, 3],
        		displayName: "Sucursal/Instalación donde se vende",
        		cellTemplate: "<div>{{row.branch[col.field].Name}}</div>",
        		width: "150px"
        	});
        	$scope.tableWidht += 150;

        	$scope.col_defs.push({
        		field: "CentroCosto",
        		displayName: "Centro de costo",
        		tooltip: "Centro de costo",
        		levels: [1, 2, 3],
        		width: "100px",
        		cellTemplate: "<div>{{row.branch[col.field].AnalyticsAreaKey}}</div>"
        	});
        	$scope.tableWidht += 100;

        	$scope.col_defs.push({
        		field: "Precio",
        		displayName: "Precio de Lista",
        		width: "150px",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{row.branch[col.field].Value}}({{row.branch[col.field].Currency.Name}})</div>",
        		//"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
        	});
        	$scope.tableWidht += 150;

        	$scope.col_defs.push({
        		field: "CentroCosto",
        		displayName: "Área Analítica",
        		width: "70px",
        		tooltip: "Área Analítica",
        		levels: [1, 2, 3],
        		cellTemplate: "<div>{{row.branch[col.field].Name}}</div>"
        	});
        	$scope.tableWidht += 70;
			
        	$scope.col_defs.push({
        		field: "MostrarLista",
        		displayName: "Mostrar Componentes",
        		levels: [1, 2],
        		width: "70px",
        		cellTemplate: "<div>{{row.branch[col.field] === true ? 'Si' : row.branch[col.field] === undefined ? '' : 'No'}}</div>",
        		tooltip: "Mostrar Lista de Componentes del Grupo"
        	});
        	$scope.tableWidht += 70;

        	// Este no esta en CAT PARAMETROS, pero creo que es un elemento importante para grupos.
			//TODO: revisar que se muestren los numeros que no se estan mostrando
        	$scope.col_defs.push({
        		field: "CantidadMuestreos",
        		displayName: "Cantidad de Muestreos",
        		levels: [2, 3],
        		cellTemplate: "<div>{{row.branch[col.field] !== null ? row.branch[col.field] : '-'}}</div>",
        		tooltip: "Cantidad de Muestreos (Muestreos Compuestos)",
        		width: "150px"
        	});
        	$scope.tableWidht += 150;

        	$scope.s1Pkgs = new Array();
        	$scope.s1Grps = new Array();
        	$scope.s1Prms = new Array();
        	$scope.s2Pkgs = new Array();
        	$scope.s2Grps = new Array();
        	$scope.s2Prms = new Array();
			
        	$scope.rowActions1 = [{
        		levels: [1, 2, 3],
        		width: "50px",
        		cellTemplate: "<input style='margin:0' ng-click='cellTemplateScope.click(row.branch)' ng-model='row.branch.check' type='checkbox'>",
        		cellTemplateScope: {
        			click: function (data) { // this works too: $scope.someMethod;
        				var flag = false;
        				if (data.elemType === "package") {
        					for (var j = 0; j < $scope.s1Pkgs.length; j++) {
        						if ($scope.s1Pkgs[j] === data.Id) {
        							$scope.s1Pkgs.splice(j, 1);
        							flag = true;
        							break;
        						}
        					}
					        if (!flag) {
					        	$scope.s1Pkgs.push(data.Id);
					        }
        				} else if (data.elemType === "group") {
        					for (var k = 0; k < $scope.s1Grps.length; k++) {
        						if ($scope.s1Grps[k] === data.Id) {
        							$scope.s1Grps.splice(k, 1);
        							flag = true;
        							break;
        						}
					        }
        					if (!flag) {
        						$scope.s1Grps.push(data.Id);
        					}
        				} else {
        					for (var i = 0; i < $scope.s1Prms.length; i++) {
        						if ($scope.s1Prms[i] === data.Id) {
        							$scope.s1Prms.splice(i, 1);
        							flag = true;
        							break;
						        }
        					}
					        if (!flag) {
					        	$scope.s1Prms.push(data.Id);
					        }
				        }
			        }
        		}
        	}];
        	$scope.rowActions2 = [{
        		levels: [1, 2, 3],
        		width: "50px",
        		cellTemplate: "<input style='margin:0' ng-click='cellTemplateScope.click(row.branch)' ng-model='row.branch.check' type='checkbox'>",
        		cellTemplateScope: {
        			click: function (data) { // this works too: $scope.someMethod;
        				var flag = false;
        				if (data.elemType === "package") {
        					for (var j = 0; j < $scope.s2Pkgs.length; j++) {
        						if ($scope.s2Pkgs[j] === data.Id) {
        							$scope.s2Pkgs.splice(j, 1);
        							flag = true;
        							break;
        						}
        					}
        					if (!flag) {
        						$scope.s2Pkgs.push(data.Id);
        					}
        				} else if (data.elemType === "group") {
        					for (var k = 0; k < $scope.s2Grps.length; k++) {
        						if ($scope.s2Grps[k] === data.Id) {
        							$scope.s2Grps.splice(k, 1);
        							flag = true;
        							break;
        						}
        					}
        					if (!flag) {
        						$scope.s2Grps.push(data.Id);
        					}
        				} else {
        					for (var i = 0; i < $scope.s2Prms.length; i++) {
        						if ($scope.s2Prms[i] === data.Id) {
        							$scope.s2Prms.splice(i, 1);
        							flag = true;
        							break;
        						}
        					}
        					if (!flag) {
        						$scope.s2Prms.push(data.Id);
        					}
        				}
        			}
        		}
        	}];
        	$scope.tableWidht += 50;

        	$scope.makeTransfer = function() {
        		if ($scope.s1Pkgs.length > 0 || $scope.s1Grps.length > 0 || $scope.s1Prms.length > 0
        			|| $scope.s2Pkgs.length > 0 || $scope.s2Grps.length > 0 || $scope.s2Prms.length > 0)
        		{
        			$http.post('/massivetransfer/TransferElement', {
        				suc1: $scope.sucursal1.Id,
        				s1Pkgs: $scope.s1Pkgs,
        				s1Grps: $scope.s1Grps,
        				s1Prms: $scope.s1Prms,
        				suc2: $scope.sucursal2.Id,
        				s2Pkgs: $scope.s2Pkgs,
        				s2Grps: $scope.s2Grps,
        				s2Prms: $scope.s2Prms
				        })
					.success(function (response) {
						if (response.success === true) {
							notification.success({
								message: "Transferencia realizada con éxito.",
								delay: 3000
							});
							$scope.RefreshList(1, 1, $scope.sucursal1.Id);
							$scope.RefreshList(1, 2, $scope.sucursal2.Id);
							$scope.s1Pkgs.length = 0;
							$scope.s1Grps.length = 0;
							$scope.s1Prms.length = 0;
							$scope.s2Pkgs.length = 0;
							$scope.s2Grps.length = 0;
							$scope.s2Prms.length = 0;
						} else {
							notification.error({
								message: "Error al transferir elementos.",
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
        }
    ])
    
;
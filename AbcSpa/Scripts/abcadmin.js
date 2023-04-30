"use strict";
var appRoot = angular.module("abcadmin",
    ["ngCookies", "ngRoute", "ngResource", "ngDialog", "ui-notification", "chart.js", "ui.router",
        "angular-dialgauge", "mgcrea.ngStrap", "ng-fusioncharts", "bw.paging", "treeGrid", 
        "FullScreenAngular", 'mgo-angular-wizard']);

appRoot
.config([
    "$routeProvider", "$httpProvider", "$locationProvider",
    "ngDialogProvider", "$datepickerProvider",
    function ($routeProvider, $httpProvider, $locationProvider, ngDialogProvider, $datepickerProvider) {

        $httpProvider.interceptors.push("authResponseInterceptor");

        ngDialogProvider.setDefaults({
            className: "ngdialog-theme-default",
            plain: false,
            showClose: true,
            closeByDocument: true,
            closeByEscape: true,
            appendTo: false
        });
		
    }])

.config(["$stateProvider", "$urlRouterProvider", function ($stateProvider, $urlRouterProvider) {

    $urlRouterProvider.otherwise('/login/');

    $stateProvider
        .state("login", {
            url: "/login/",
            views: {
                'header': {
                    templateUrl: "/Shared/Header"
                },
                'content': {
                    templateUrl: "/partialview/login",
                    controller: "LoginController"
                }
            }

        })
        .state("contact", {
            url: "/contact/",
            views: {
                'header': {
                    templateUrl: "/Shared/Header"
                },
                'content': "partialview/contact"
            }
        })
        .state('system', {
            url: '/system/',
            views: {
                "header": {
                    templateUrl: '/Shared/Header'
                },
                'sidebar': {
                    templateUrl: '/Shared/Sidebar'
                }
            }
        })
        .state('system.home', {
			url: 'home/',
			views: {
				'content@': {
					templateUrl: '/partialview/sheettable',
					controller: 'SheetController'
				}
			}
        })
        .state('system.report', {
            url: 'report/generatereport/?options&isExcel&orderId',
            views: {
                'content@': {
                    templateUrl: '/partialview/report'
                }
            }
        })
		.state('system.charts', {
			url: 'charts/',
			views: {
				'content@': {
					templateUrl: '/PartialView/Charts',
					controller: 'ChartController'
				}
			}
		})
        .state('system.user', {
            url: 'user/',
            views: {
                'content@': {
                    templateUrl: '/PartialView/Users',
                    controller: 'UserController'
                }
            }
        })
        .state('system.role', {
            url: 'role/',
            views: {
                'content@': {
                    templateUrl: '/partialview/role',
                    controller: 'RoleController'
                }
            }
        })
        // Relacionados con empresas
        .state('system.enterprises', {
            url: 'enterprises/',
            views: {
                'content@': {
                    templateUrl: '/partialview/enterprises',
                    controller: 'EnterpriseController'
                }
            }
        })
        .state('system.actions', {
            url: 'actions/',
            views: {
                'content@': {
                    templateUrl: '/partialview/actions',
                    controller: 'ActionsController'
                }
            }
        })
        .state('system.ack', {
            url: 'ack/',
            views: {
                'content@': {
                    templateUrl: '/partialview/ack',
                    controller: 'AckController'
                }
            }
        })
        .state('system.alcances', {
            url: 'alcances/',
            views: {
                'content@': {
                    templateUrl: '/partialview/alcance',
                    controller: 'AlcanceController'
                }
            }
        })
        // relacionados con matrices
        .state("system.matrixes", {
            url: "matrixes/",
            views: {
                'content@': {
                    templateUrl: '/partialview/matrixes',
                    controller: 'MatrixController'
                }
            }
        })
        .state("system.basematrix", {
            url: "basematrixes/",
            views: {
                'content@': {
                    templateUrl: "/partialview/basematrix",
                    controller: "BaseMatrixController"
                }
            }
        })
        // relacionados con metodo
        .state("system.container", {
            url: "container/",
            views: {
                'content@': {
                    templateUrl: '/partialview/container',
                    controller: 'ContainerController'
                }
            }
        })
        .state("system.preserver", {
            url: 'preserver/',
            views: {
                'content@': {
                    templateUrl: '/partialview/preserver',
                    controller: 'PreserverController'
                }
            }
        })
        .state('system.residue', {
            url: 'residue/',
            views: {
                'content@': {
                    templateUrl: '/partialview/residue',
                    controller: 'ResidueController'
                }
            }
        })
        .state('system.analyticsmethod', {
            url: 'analyticsmethod/',
            views: {
                'content@': {
                    templateUrl: '/partialview/analyticsmethod',
                    controller: 'AnalyticsMethodController'
                }
            }
        })
        .state('system.method', {
            url: 'method/',
            views: {
                'content@': {
                    templateUrl: '/partialview/method',
                    controller: 'MethodController'
                }
            }
        })
		.state("system.material", {
			url: 'material/',
			views: {
				'content@': {
					templateUrl: '/partialview/material',
					controller: 'MaterialController'
				}
			}
		})
        // Relacionados con analista
        .state('system.annalist', {
            url: 'annalist/',
            views: {
                'content@': {
                    templateUrl: '/partialview/annalist',
                    controller: 'AnnalistController'
                }
            }
        })
		// Relacionados con area analitica
        .state('system.analyticsarea', {
            url: 'analyticsarea/',
            views: {
                'content@': {
                    templateUrl: "/partialview/analyticsarea",
                    controller: "AnalyticsAreaController"
                }
            }
        })
		.state('system.centrocosto', {
			url: 'centrocosto/',
			views: {
				'content@': {
					templateUrl: "/partialview/centrocosto",
					controller: "CentroCostoController"
				}
			}
		})
		.state('system.unidadanalitica', {
			url: 'analyticsunit/',
			views: {
				'content@': {
					templateUrl: "/partialview/unidadanalitica",
					controller: "UnidadAnaliticaController"
				}
			}
		})
        .state('system.claveanalista', {
            url: 'annalistkey/',
            views: {
                'content@': {
                    templateUrl: "/partialview/annalistkey",
                    controller: "AnnalistKeyController"
                }
            }
        })
       //Relacionados con parámetros
        .state('system.param', {
            url: 'parameter/',
            views: {
                'content@': {
                    templateUrl: "/partialview/param",
                    controller: "ParamController"
                }
            }
        })
        .state('system.baseparam', {
            url: 'parameter/physicalparam/',
            views: {
                'content@': {
                    templateUrl: "/partialview/baseparam",
                    controller: "BaseParamController"
                }
            }
        })
        //.state("system.baseparamfamily", {
        //    url: "parameter/physicalparam/family",
        //    views: {
        //        'content@': {
        //            templateUrl: "/partialview/baseparamfamily",
        //            controller: "BaseParamFamilyController"
        //        }
        //    }
        //})
		.state("system.clasificacionquimica1", {
			url: "chemicalclasification1",
			views: {
				'content@': {
					templateUrl: "/partialview/clasificacionquimica",
					controller: "ClasificacionQuimicaController"
				}
			}
		})
		.state("system.clasificacionquimica2", {
			url: "chemicalclasification2",
			views: {
				'content@': {
					templateUrl: "/partialview/clasificacionquimica",
					controller: "ClasificacionQuimicaController"
				}
			}
		})
		.state("system.clasificacionquimica3", {
			url: "chemicalclasification3",
			views: {
				'content@': {
					templateUrl: "/partialview/clasificacionquimica",
					controller: "ClasificacionQuimicaController"
				}
			}
		})
        .state("system.currency", {
            url: "currency/",
            views: {
                'content@': {
                    templateUrl: "/partialview/currency",
                    controller: "CurrencyController"
                }
            }
        })
        .state("system.units", {
            url: "measureunits/",
            views: {
                'content@': {
                    templateUrl: "/partialview/measureunits",
                    controller: "MeasureUnitController"
                }
            }
        })
        .state("system.norm", {
            url: "norms/",
            views: {
                'content@': {
                    templateUrl: "/partialview/norm",
                    controller: "NormController"
                }
            }
        })

        .state("system.status", {
            url: "status/",
            views: {
                'content@': {
                    templateUrl: "/partialview/status",
                    controller: "StatusController"
                }
            }
        })
        .state("system.rama", {
            url: "branch/",
            views: {
                'content@': {
                    templateUrl: "/partialview/rama",
                    controller: "RamaController"
                }
            }
        })
         .state("system.lmp", {
             url: "maxpermitedlimits/",
             views: {
                 'content@': {
                     templateUrl: "/partialview/maxpermitedlimit",
                     controller: "MaxPermitedLimitController"
                 }
             }
         })

		.state("system.tiposervicio", {
			url: "servicetypes/",
			views: {
				'content@': {
					templateUrl: "/partialview/tiposervicio",
					controller: "TipoServicioController"
				}
			}
		})
        // Region, Market, Office and Sucursal
        .state('system.region', {
            url: 'region/',
            views: {
                'content@': {
                    templateUrl: "/partialview/region",
                    controller: "RegionController"
                }
            }
        })
        
        .state('system.sucursal', {
            url: 'sucursal/',
            views: {
                'content@': {
                    templateUrl: "/partialview/sucursal",
                    controller: "SucursalController"
                }
            }
        })
        
        .state('system.office', {
            url: 'office/',
            views: {
                'content@': {
                    templateUrl: "/partialview/office",
                    controller: "OfficeController"
                }
            }
        })
        
        .state('system.market', {
            url: 'market/',
            views: {
                'content@': {
                    templateUrl: "/partialview/market",
                    controller: "MarketController"
                }
            }
        })

        .state('system.massivetransfer', {
            url: 'massivetransfer/',
            views: {
                'content@': {
                    templateUrl: "/partialview/massivetransfer",
                    controller: "MassiveTransferController"
                }
            }
        })
        //Package,group
        .state('system.groups', {
            url: 'groups/',
            views: {
                'content@': {
                    templateUrl: "/partialview/group",
                    controller: "GroupController"
                }
            }
        })

        .state("system.package", {
            url: "packages/",
            views: {
                'content@': {
                    templateUrl: "/partialview/package",
                    controller: "PackageController"
                }
            }
        })
		// Bitacora de accesos a la web
        .state('system.useraccess', {
        	url: 'access/',
        	views: {
        		'content@': {
        			templateUrl: "/partialview/useraccess",
        			controller: "UserAccessController"
        		}
        	}
        })
		// Bitacora de cambios
        .state('system.audit', {
        	url: 'audit/',
        	views: {
        		'content@': {
        			templateUrl: "/partialview/audit",
        			controller: "AuditController"
        		}
        	}
        })
	;
}])

    .config(['$sceDelegateProvider',
        function ($sceDelegateProvider) {
            // Set a new whitelist
            $sceDelegateProvider.resourceUrlWhitelist(['self']);
        }])

    //.controller('RootController', ['$scope', '$rootScope', '$location',
    //   '$interval', 'ngDialog', 'SecurityFact',
    //    function ($scope, $rootScope, $location, $interval, ngDialog, securityFact) {
    //        console.log("RootController");
    //    }])

    .directive("screenDirective", ['$rootScope', 'counterFactory', 'SecurityFact',
        function ($rootScope, counterFactory, securityFact) {
        return {
            link: function ($scope, element, attrs) {
                $scope.compact = "compact";
                //$scope.side_size = "1";
                //$scope.main_size = "11";
	            $scope.sidebarOn = true;

                $scope.swap_sidebar = function () {
                    if ($scope.compact === 'compact') {
                        $scope.compact = '';
                        //$scope.side_size = "2";
                        //$scope.main_size = "10";
                    } else {
                        $scope.compact = 'compact';
                        //$scope.side_size = "1";
                        //$scope.main_size = "11";
                    }
                };
                $scope.elementselected =
                    [
                        { index: 0, iconname: 'mif-users', id: '#user_roles', count: 0 },			// usuarios + roles
                        { index: 1, iconname: 'mif-lab', id: '#parameters', count: 0 },			// parametros
                        { index: 2, iconname: 'mif-list', id: '#methods', count: 0 },				// metodos
                        { index: 3, iconname: 'mif-clipboard', id: '#reconocimientos', count: 0 },		// empresas
                        { index: 4, iconname: 'mif-broadcast', id: '#analyticsarea', count: 0 },				// areas analiticas
						{ index: 5, iconname: 'mif-air', id: '#baseparameters', count: 0 },        // parametros base
						{ index: 6, iconname: 'mif-windy4', id: '#matrixes', count: 0 },				// matrices
						{ index: 7, iconname: 'mif-library', id: '#sucursales', count: 0 },				// regiones
						{ index: 8, iconname: 'mif-weather', id: '#clasificacionquimica', count: 0 }	// clasificaciones quimicas
                    ];

                $scope.iconselectedChange = function (index, iconname, count) {
                    if (index !== -1) {
                        $scope.elementselected[index].iconname = iconname;
                        //$scope.elementselected[index].count = count;  //ahora mismo esta comentareado en el sidebar
                    }

                    for (var i = 0; i < $scope.elementselected.length; i++) {
                        if ($scope.elementselected[i].index === index) {
                            element.find($scope.elementselected[i].id).addClass('active');
                        } else {
                            element.find($scope.elementselected[i].id).removeClass('active');
                        }
                    }
                }

                //$rootScope.$on('counter:change', function () {
                counterFactory.subscribe($scope, function updateCounter() {
                    $scope.counter = counterFactory.getCounter();
                });

                //$rootScope.$on('page:refresh', function () {
                securityFact.subscribe($scope, function updateRole() {
                	var rights = securityFact.checkAllAccessLevel();
                	
		            // Checking Level Access
                	$scope.userAccess = rights["user"];
                	$scope.roleAccess = rights["role"];
                	$scope.methodAccess = rights["method"];
                	$scope.containerAccess = rights["container"];
                	$scope.preserverAccess = rights["preserver"];
                	$scope.residueAccess = rights["residue"];
                	$scope.analyticsmethodAccess = rights["analyticsmethod"];
                	$scope.ackAccess = rights["ack"];
                	$scope.annalistAccess = rights["annalist"];
                	$scope.analyticsareaAccess = rights["analyticsarea"];
                    $scope.annalistkeAccess = rights["annalistkey"];
                	$scope.baseparamAccess = rights["baseparam"];
                	$scope.baseparamfamilyAccess = rights["baseparamfamily"];
                	$scope.clasificacionquimicaAccess = rights["clasificacionquimica"];
                	$scope.measureunitsAccess = rights["measureunits"];
                	$scope.paramAccess = rights["param"];
                	$scope.groupAccess = rights["group"];
                	$scope.packageAccess = rights["package"];
                	$scope.currencyAccess = rights["currency"];
                	$scope.normAccess = rights["norm"];
                	$scope.statusAccess = rights["status"];//revisar bien esto
                	$scope.matrixAccess = rights["matrix"];
                	$scope.basematrixAccess = rights["basematrix"];
                	$scope.regionAccess = rights["region"];
                	$scope.materialAccess = rights["material"];
                	$scope.tiposervicioAccess = rights["tiposervicio"];
                    $scope.ramaAccess = rights["rama"];

                	if (($scope.userAccess === -1) && ($scope.roleAccess === -1) &&
						($scope.methodAccess === -1) && ($scope.containerAccess === -1) &&
						($scope.preserverAccess === -1) && ($scope.residueAccess === -1) &&
						($scope.analyticsmethodAccess === -1) && ($scope.ackAccess === -1) &&
						($scope.annalistAccess === -1) && ($scope.analyticsareaAccess === -1) &&
						($scope.baseparamAccess === -1) && ($scope.clasificacionquimicaAccess === -1) &&
						($scope.measureunitsAccess === -1) && ($scope.paramAccess === -1) &&
						($scope.groupAccess === -1) && ($scope.packageAccess === -1) &&
						($scope.currencyAccess === -1) && ($scope.normAccess === -1) &&
						($scope.matrixAccess === -1) && ($scope.basematrixAccess === -1) &&
						($scope.regionAccess === -1) && ($scope.materialAccess === -1) &&
						($scope.tiposervicioAccess === -1) && ($scope.ramaAccess === -1))
                	{
						$scope.sidebarOn = false;
	                	//$scope.main_size = "12";
                	} else {
                		$scope.sidebarOn = true;
                		//$scope.main_size = $scope.side_size === "1" ? "11" : "10";
	                }
                });
                
                //  $scope.$on('columnList:refresh', function (event, columnList) {
                //      console.log("Entro");
                //  });
                    
            }
        };
    }])

    .directive("bootstrapSwitch", [
            function () {
                return {
                    restrict: "A",
                    require: "?ngModel",
                    link: function (scope, element, attrs, ngModel) {
                        element.bootstrapSwitch();

                        element.on("switchChange.bootstrapSwitch", function (event, state) {
                            if (ngModel) {
                                scope.$apply(function () {
                                    ngModel.$setViewValue(state);
                                });
                            }
                        });

                        scope.$watch(attrs.ngModel, function (newValue, oldValue) {
                            if (newValue) {
                                element.bootstrapSwitch("state", true, true);
                            } else {
                                element.bootstrapSwitch("state", false, true);
                            }
                        });
                    }
                };
            }
    ])

    .directive("uniqueName", ["$http", "Notification", "ngDialog",  function ($http, notification, ngDialog) {
        return {
            restrict: "A",
            require: "ngModel",
            //scope: {
            //    Data: '@unObjectid',
            //    isIndb:'=dataIndb'
            //},
            link: function (scope, elem, attr, ngModel) {

                elem.bind('blur', function (e) {
                    if (!ngModel || !elem.val()) return;
                    var currentValue = elem.val();
                    scope.formAlert = "form-group";
                    scope.isIndb = false;

                	//var data = scope.ngDialogData.dData.Id == undefined || scope.ngDialogData.dData.d ? -1 : ngDialogData.dData.Id;
                    var data = scope.ngDialogData.dData.Id == undefined || scope.ngDialogData.dData.d ? 0 : scope.ngDialogData.dData.Id;

                    $http.get(attr.uniqueName, { params: { uniqueInput: currentValue, id: data } })
                        .success(function (response) {

                            if (response.success) {
                                if (response.data === true) {
                                    scope.formAlert = "has-error";
                                    scope.isIndb = true;
                                }
                            } else {
                                notification.error({
                                    message: "Error en la conexion con el servidor",
                                    delay: 5000
                                });
                            }

                        }).error(function (data, status, headers, config) {
                            console.log("something wrong");
                        });
                }).bind('focus', function (e) {
                    if (!ngModel || !elem.val()) return;
                    scope.formAlert = "form-group";
                    scope.isIndb = false;
                });
            }
        };
    }])

    .factory("dataProvider", function () {

        var currData;
        return {
            getCurrData: function () {
                return currData;
            },
            setCurrData: function (data) {
                currData = data;
            }
        };
    })

    .factory("counterFactory", ['$rootScope', '$http', 'Notification', function ($rootScope, $http, notification) {

        var counter;

        return {
            subscribe: function(scope, callback) {
                var handler = $rootScope.$on('counter:change', callback);
                scope.$on('$destroy', handler);
            },
            getCounter: function () {
            	//return (counter) ? counter[0] : false;
            	return (counter) ? counter : false;
            },
            setCounter: function (counts) {
                counter = counts;
                $rootScope.$emit('counter:change');
            },
            getAllCounts: function (activated) {
                $http.post('/home/getallcount', { active: activated })
                .success(function (response) {
                    if (response.success === true) {
                        counter = response.elements;
                        $rootScope.$emit('counter:change');
                    } else {
                        notification.error({
                            message: 'Error al recuperar datos.', delay: 5000
                        });
                    }
                })
                .error(function () {
                    notification.error({
                        message: 'Error en la conexión con el servidor.', delay: 5000
                    });
                });
            }
        };
    }])

	.directive("minimizeDirective", ['$document', function ($document) {
		return {
			link: function ($scope, elem, attrs) {

				$scope.minimize = function (element) {
					if (element.index === 0 && $scope.panels[0].state === false &&
						$scope.panels[1].state === false) {
						return;
					}

					element.state = !element.state;

					if (element.index === 1 && element.state === false) {
						$scope.panels[0].state = false;
					}

					if ($scope.panels[1].state === true) {
						if ($scope.panels[0].state === false) {
							elem.context.className = "col-lg-12";
						} else {
							elem.context.className = "col-lg-10";
						}
					}
					
					for (var i = 0; i < $scope.panels.length; i++) {
						if ($scope.panels[i].state === false) {
							$scope.taskbar = true;
							break;
						}
						$scope.taskbar = false;
					}
				}
			}
		}
	}])

	.directive("twoPanelsDirective", ['$document', function ($document) {
		return {
			link: function ($scope, elem, attrs) {

				$scope.minimize = function (element) {
					element.state = !element.state;

					var panel0 = $document.context.getElementById('panel0');
					var panel1 = $document.context.getElementById('panel1');

					if ($scope.panels[0].state)
						panel0.className = !$scope.panels[1].state ? "col-lg-12" : "col-lg-6";

					if ($scope.panels[1].state)
						panel1.className = !$scope.panels[0].state ? "col-lg-12" : "col-lg-6";
				}
			}
		}
	}])
    
    //.directive("pageSizeDirective", ['$document', function ($document) {
	//	return {
	//		link: function ($scope, elem, attrs) {

	//			$scope.$watch('pageSize', function () {
    //                if($scope.RefreshList !== undefined)
    //                    $scope.RefreshList(1);
    //            });
                
    //            $scope.$watch('pageSizeMore', function () {
    //                if($scope.RefreshListMore !== undefined)
    //                    $scope.RefreshListMore(1);
    //            });
	//		}
	//	}
	//}])
    
    .directive("updateSessionDirective", ['generalFactory', function (generalFactory) {
        return {
            link: function ($scope) {
                var update = setInterval(function() {                   
                    generalFactory.restartSession();                                      
                }, 50000);    // 50 segundos
            }
        }
    }])
;

appRoot.run(['$rootScope', 'SecurityFact', 'counterFactory',
    function ($rootScope, securityFact, counterFactory) {
        $rootScope.$on('$locationChangeStart', function () {
            $("#workregion").hide();
            $("#loadingSpinners").show();
            counterFactory.getAllCounts(true);
            securityFact.checkAuth();
        });
        $rootScope.$on('$locationChangeSuccess', function () {
            $("#loadingSpinners").hide();
            $("#workregion").show();
        });
    }]);

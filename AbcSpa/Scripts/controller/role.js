'use strict';
angular.module('abcadmin')
    .controller('RoleController', ['$scope', '$location', '$http', '$controller',
        'SecurityFact', 'PermisoFact', 'Notification', 'ngDialog',
        function ($scope, $location, $http, $controller, securityFact, permisoFact, notification, ngDialog) {
        	//Codigo a usar en todos los controladores.
        	//$controller('RootController',{$scope : $scope});
        	//$scope.employee = securityFact.employeeDataExpress();
        	//$scope.loadAvatar();
        	//Fin del codigo a utilizar en todos los controladores

        	// This controller groups similar functionalities of different controllers.
        	$controller('GeneralController', { $scope: $scope, $http: $http });
        	//-------------------------------------------------------------------------

        	var filters = function () {
        		return {
        			alta: $scope.alta,
        			baja: $scope.baja
        		}
        	};

        	var setActivationObjects = function (eObj, act) {
        		return { id: eObj.Id, active: act }
        	};

        	var saveObject = function (promise) {
        		var listaCp = new Array();
        		for (var i in promise.RightColl) {
        			if (promise.RightColl.hasOwnProperty(i)) {
        				if (promise.RightColl[i].activated)
        					listaCp.push(promise.RightColl[i]);
        			}
        		}

        		var roleObj = {
        			Id: promise.Id,
        			Name: promise.Name,
        			Description: promise.Description,
        			Active: true,
        			RightColl: listaCp,
        			ParamCols: promise.ParamCols
        		};

        		return { role: roleObj }
        	}

        	var editElement = function (element) {
        		for (var i in $scope.permisosList) {
        			if ($scope.permisosList.hasOwnProperty(i)) {
        				$scope.permisosList[i].activated = false;
        			}
        		}
        		return {
        			template: 'editDialog',
        			controller: 'EditRoleController',
        			className: 'ngdialog-theme-default ngdialog-width1000',
        			preCloseCallback: 'preCloseCallbackOnScope',
        			data: { dData: element, permisos: $scope.permisosList },
        			closeByEscape: true
        		}
        	}

        	$scope.mainFunction(
                "icon mif-organization", "Name",
                "/role/getroles", filters,
                "/role/updateroleactive", setActivationObjects,
                "/role/saveroledata", saveObject,
                editElement,
				"role"
                );

        	permisoFact.loadPermisos()
            .then(function (response) {
            	if (response.success) {
            		$scope.permisosList = response.permList;
            	} else {
            		notification.error({
            			message: 'Error al cargar los permisos para los Roles.', delay: 5000
            		});
            	}
            });

        	// Enterprise specifics
        	$scope.FilterGeneral = true;
        	$scope.FilterActive = true;
        	$scope.boxHeaderTitleActive = "Roles activos";
        	$scope.boxHeaderTitleDeactive = "Roles de baja";

        	$scope.panels = [
                {
                	index: 0,
                	name: 'Filtros',
                	state: false,
                	size: 3
                },
                {
                	index: 1,
                	name: 'Tabla Roles',
                	state: true,
                	size: 9
                }
        	];
        }
    ])
    .controller('EditRoleController',
    [
        '$scope', '$http', 'Notification',
        function ($scope, $http, notification) {
        	$scope.countPermiso = 0;
        	$scope.permisosList = $scope.ngDialogData.permisos;
        	var errorDelay = 5000;

        	var show = function (permiso) {
        		for (var i = 0; i < $scope.permisosList.length; i++) {
        			if (($scope.permisosList[i].Value === permiso.Value) && ($scope.permisosList[i].Level !== permiso.Level)) {
        				$scope.permisosList[i].show = !permiso.activated;
        				break;
        			}
        		}
        	}

        	function loadPermisologia() {
        		if ($scope.permisosList !== undefined)
        			for (var k = 0; k < $scope.permisosList.length; k++)
        				$scope.permisosList[k].show = true; // inicialmente se habilita para mostrar el check

        		if (($scope.ngDialogData.dData.RightColl !== undefined) && ($scope.ngDialogData.dData.RightColl.length > 0)) {
        			for (var i = 0; i < $scope.ngDialogData.dData.RightColl.length; i++) {
        				for (var j = 0; j < $scope.permisosList.length; j++) {
        					if ($scope.permisosList[j].activated) {
        						continue;
        					}
        					if ($scope.permisosList[j].Id === $scope.ngDialogData.dData.RightColl[i].Id) {
        						$scope.permisosList[j].activated = true;
        						show($scope.permisosList[j]);
        						break;
        					} else
        						$scope.permisosList[j].activated = false;
        				}
        			}
        		}
        	}
        	loadPermisologia();

        	function getColumnList() {
        		$http.post('/role/GetColumnList')
                .success(function (response) {
                	if (response.success === true) {
                		$scope.paramCols = response.columnList;

                		if ($scope.ngDialogData.dData.Id !== undefined)
                			for (var i = 0; i < $scope.ngDialogData.dData.ParamCols.length; i++) {
                				for (var j = i; j < $scope.paramCols.length; j++) {
                					if ($scope.paramCols[j].checked) {
                						continue;
                					}
                					if ($scope.paramCols[j].Id === $scope.ngDialogData.dData.ParamCols[i].Id) {
                						$scope.paramCols[j].checked = true;
                						break;
                					}
                					else
                						$scope.paramCols[j].checked = false;
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

        	getColumnList();

        	$scope.permisoActivo = function (permiso) {
        		permiso.activated = !permiso.activated;
        		show(permiso);
        	};

        	$scope.paramColActive = function (col) {
        		col.checked = !col.checked;
        	};

        	$scope.dialogTitle = $scope.ngDialogData.dData.Id === undefined ?
                "Nuevo Rol" : "Editar Rol " + $scope.ngDialogData.dData.Name;

        	$scope.acceptDialog = function () {
        		if ($scope.ngDialogData.dData.Name === undefined) {
        			notification.error({
        				message: 'Nombre del Rol indefinido', delay: errorDelay
        			});
        			return;
        		}

        		var paramColsList = new Array();
        		for (var i = 0; i < $scope.paramCols.length; i++) {
        			if ($scope.paramCols[i].checked) {
        				paramColsList.push($scope.paramCols[i]);
        			}
        		}

        		$scope.confirm({
        			Id: $scope.ngDialogData.dData.Id != null ? $scope.ngDialogData.dData.Id : 0,
        			Name: $scope.ngDialogData.dData.Name,
        			Description: $scope.ngDialogData.dData.Description,
        			RightColl: $scope.permisosList,
        			ParamCols: paramColsList
        		});
        	};
        }])

.factory('PermisoFact', ['$http', '$q',
    function ($http, $q) {
    	return {
    		loadPermisos: function () {
    			var deferredObject = $q.defer();
    			$http.post('/role/getaccesslist')
                .success(function (response) {
                	if (response.success === true) {
                		deferredObject.resolve({ success: true, permList: response.permList });
                	} else {
                		deferredObject.resolve({ success: false });
                	}
                })
                .error(function () {
                	deferredObject.resolve({ success: false });
                });
    			return deferredObject.promise;
    		}
    	}
    }])

;
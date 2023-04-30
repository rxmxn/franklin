"use strict";

angular.module("abcadmin")

.controller("UserController",
["$scope", "$route", "$location", "$http", "$cookies", '$controller', "SecurityFact", "Notification", "Fullscreen",
    function ($scope, $route, $location, $http, $cookies, $controller, securityFact, notification, Fullscreen) {

    	$scope.isFullscreen = false;
    	//-----------función q se llama para mostrar un div en fullscreen----------------
    	$scope.toggleFullScreen = function () {

    		$scope.isFullscreen = !$scope.isFullscreen;

    	}
    	//---------------------------------------------------------------------------------

    	// This controller groups similar functionalities of different controllers.
    	$controller('GeneralController', { $scope: $scope, $http: $http });
    	//-------------------------------------------------------------------------

    	var filters = function (page) {
    		return {
    			page: (page === null || page === undefined) ? $scope.currentPage : page,
    			pageSize: $scope.pageSize,
    			searchGeneral: $scope.searchGeneral,
    			fromDate: $scope.fromDate === undefined ? null : $scope.fromDate,
    			untilDate: $scope.untilDate === undefined ? null : $scope.untilDate,
    			alta: $scope.alta,
    			baja: $scope.baja
    		}
    	};

    	var setActivationObjects = function (eObj, act) {
    		return { id: eObj.Id, active: act }
    	};

    	var saveObject = function (promise) {
    		var employeeObj = {
    			Id: promise.Id,
    			Name: promise.Name,
    			LastNameFather: promise.LastNameFather,
    			LastNameMother: promise.LastNameMother,
    			Phone: promise.Phone,
    			Email: promise.Email,
    			Role: promise.Role,
    			UserName: promise.UserName,
    			UserPassword: promise.UserPassword,
    			Photo: promise.Photo,
    			Gender: promise.Gender,
    			Sucursales: promise.Sucursales
    		};
    		return {
    			employee: employeeObj
    		}
    	}

    	var editElement = function (element) {

    		return {
    			template: "editDialog",
    			controller: "NewUserController",
    			className: "ngdialog-theme-default ngdialog-width700",
    			preCloseCallback: "preCloseCallbackOnScope",
    			data: { dData: element },
    			closeByEscape: true
    		}
    	}

    	$scope.mainFunction(
            "icon mif-users", "Name",
            "/user/refreshlist", filters,
            "/user/saveactivestatus", setActivationObjects,
            "/user/saveemployee", saveObject,
            editElement,
            "user"
            );

    	// User specifics
    	$scope.FilterGeneral = true;
    	$scope.SearchGeneral = true;
    	$scope.FilterActive = true;
    	$scope.FilterDate = true;
    	$scope.boxHeaderTitleActive = "Usuarios activos";
    	$scope.boxHeaderTitleDeactive = "Usuarios de baja";

    	$scope.panels = [
			{
				index: 0,
				name: 'Filtros',
				state: false
			},
			{
				index: 1,
				name: 'Tabla Empleados',
				state: true
			}
    	];
    }])

//====================================================================================================================================
//====================================================================================================================================

.controller("NewUserController",
["$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog", "ImageUploadService",
    function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog, imageUploadService) {

    	var errorDelay = 5000;

    	$scope.avatarIMG = $scope.ngDialogData.dData.Gender === undefined || !$scope.ngDialogData.dData.Gender
    						? "/Content/img/usermale.jpg" : "/Content/img/female.jpg";

    	$scope.SexoChange = function () {
    		if (($scope.ngDialogData.dData.Photo === null) || ($scope.ngDialogData.dData.Photo === undefined)) {
    			if ($scope.ngDialogData.dData.Gender === true) {
    				$scope.avatarIMG = "/Content/img/female.jpg";
    			} else {
    				$scope.avatarIMG = "/Content/img/usermale.jpg";
    			}
    		}
    	};

    	function getAllRols(activated) {
    		$http.post('/user/getuserroles', { active: activated })
            .success(function (response) {
            	if (response.success === true) {
            		$scope.roles = response.elements;
            		$scope.fRoles = angular.copy($scope.roles);
            		//$scope.changeUserUI();
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
    	getAllRols(true);

    	$scope.selectAvatarforUpload = function (file) {
    		$scope.cleanAvatar();
    		imageUploadService.FileToUpload(file).then(function (response) {
    			if (response.success) {
    				$scope.ngDialogData.dData.Photo = response.fileName;
    			} else {
    				notification.error({
    					message: "Error al recuperar fichero.",
    					delay: 5000
    				});
    			}
    		});
    	};

    	//-------------------------------------------------------
    	$scope.cleanAvatar = function () {
    		//var nombre = $scope.newEmployee.Photo.split('/');
    		//console.log($scope.ngDialogData.dData.Photo);
    		//$http.post('/user/removeimage', {name: nombre[nombre.length-1]});
    		$scope.ngDialogData.dData.Photo = null;
    	};
    	//-------------------------------------------------------

    	function getUserSucursal(active) {
    		var id = $scope.ngDialogData.dData.Id === undefined ? 0 : $scope.ngDialogData.dData.Id;
    		$http.post('/user/GetUserSucursal', { userId: id, active: active })
				.success(function (response) {
					if (response.success === true) {
						$scope.sucursales = response.elements;
					} else {
						notification.error({
							message: 'Error al recuperar datos.',
							delay: errorDelay
						});
					}
				})
				.error(function () {
					notification.error({
						message: 'Error en la conexión con el servidor.',
						delay: errorDelay
					});
				});
    	}

    	getUserSucursal(true);

    	$scope.regionChecked = function (r) {
    		r.check = !r.check;
    	}

    	$scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
            "Nuevo empleado" : "Editar empleado " + $scope.ngDialogData.dData.UserName;

    	$scope.acceptDialog = function () {
    		var error = false;

    		if ($scope.ngDialogData.dData.Name === undefined) {
    			notification.error({
    				message: 'Nombre del empleado indefinido',
    				delay: errorDelay
    			});
    			error = true;
    		}
    		if ($scope.ngDialogData.dData.LastNameFather === undefined || $scope.ngDialogData.dData.LastNameMother===undefined) {
    			notification.error({
    				message: 'Apellido del empleado indefinido',
    				delay: errorDelay
    			});
    			error = true;
    		}
    		var phoneregex = /\(?([0-9]{3})\)?([ .-]?)([0-9]{3})\2([0-9]{4})/;
    		if ($scope.ngDialogData.dData.Phone === undefined ||
                !phoneregex.test($scope.ngDialogData.dData.Phone)) {
    			notification.error({
    				message: 'Teléfono del empleado indefinido o con error en el formato',
    				delay: errorDelay
    			});
    			error = true;
    		}
    		var emailregex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    		if ($scope.ngDialogData.dData.Email === undefined ||
                !emailregex.test($scope.ngDialogData.dData.Email)) {
    			notification.error({
    				message: 'Email del empleado indefinido o con error en el formato',
    				delay: errorDelay
    			});
    			error = true;
    		}
    		if ($scope.ngDialogData.dData.Role === undefined) {
    			notification.error({
    				message: 'Rol del empleado indefinido',
    				delay: errorDelay
    			});
    			error = true;
    		}
    		if ($scope.ngDialogData.dData.UserName === undefined) {
    			notification.error({
    				message: 'Nombre de usuario del empleado indefinido',
    				delay: errorDelay
    			});
    			error = true;
    		}
    		if ($scope.ngDialogData.dData.UserPassword === undefined) {
    			notification.error({
    				message: 'Contraseña del empleado indefinida',
    				delay: errorDelay
    			});
    			error = true;
    		}

    		var route = new Array();

    		for (var i = 0; i < $scope.sucursales.length; i++) {
    			if ($scope.sucursales[i].check === true) {
    				route.push({ 'Id': $scope.sucursales[i].Id });
    			}
    		}

    		if (route.length === 0) {
    			notification.error({
    				message: 'Debe seleccionar una Sucursal para el empleado',
    				delay: errorDelay
    			});
    			error = true;
    		}

    		if (error === true)
    			return;

    		$scope.confirm({
    			Id: $scope.ngDialogData.dData.Id != null ? $scope.ngDialogData.dData.Id : 0,
    			Name: $scope.ngDialogData.dData.Name,
    			LastNameFather: $scope.ngDialogData.dData.LastNameFather,
    			LastNameMother: $scope.ngDialogData.dData.LastNameMother,
    			Phone: $scope.ngDialogData.dData.Phone,
    			Email: $scope.ngDialogData.dData.Email,
    			Role: $scope.ngDialogData.dData.Role,
    			UserName: $scope.ngDialogData.dData.UserName,
    			UserPassword: $scope.ngDialogData.dData.UserPassword,
    			Photo: $scope.ngDialogData.dData.Photo,
    			Gender: $scope.ngDialogData.dData.Gender,
    			Sucursales: route
    		});
    	}
    }])

// .factory("ImageUploadService", function ($http, $q) {

//     var fac = {};
//     fac.UploadFile = function (file) {
//         var formData = new FormData();
//         formData.append("file", file);

//         var defer = $q.defer();
//         $http.post("/user/saveimage", formData, {
//             withCredentials: true,
//             headers: { 'Content-Type': undefined },
//             transformRequest: angular.identity
//         })
//         .success(function (d) {
//             defer.resolve(d);
//         })
//         .error(function () {
//             defer.reject("File Upload Failed!");
//         });

//         return defer.promise;
//     };
//     return fac;
// })

;


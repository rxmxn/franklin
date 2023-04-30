"use strict";

angular.module("abcadmin")

.controller("navCtrl", ["$scope", 'SecurityFact',
	function ($scope, securityFact) {

		$scope.isLogged = securityFact.isLogged();

		$scope.closeSession = function () {
			$scope.isLogged = false;
			securityFact.logout();
		}

	}])

.directive("navDirective", ['$rootScope', 'SecurityFact',
    function ($rootScope, securityFact) {
    	return {
    		link: function ($scope, element, attrs) {

    			var loadAvatar = function (employee) {
    				var avatar = '/Content/img/usermale.jpg';
    				if (employee != null) {
    					if (employee.Photo == null) {
    						if (employee.Gender === 1)
    							avatar = '/Content/img/female.jpg';
    					} else {
    						avatar = employee.Photo;
    					}
    				}
    				return avatar;
    			}

    			// inicializando sessionAvatar
    			$scope.sessionAvatar = '/Content/img/usermale.jpg';

    			//$rootScope.$on('page:refresh', function () {
    			securityFact.subscribe($scope, function updateEmployee() {
    				$scope.employee = securityFact.employeeDataExpress();
    				$scope.sessionAvatar = loadAvatar($scope.employee);
    				$scope.roleName = $scope.employee.UserName === 'root' ? "Administrador" : $scope.employee.Role.Name;

    				var rights = securityFact.checkAllAccessLevel();
    				// Checking Level Access (to show in the menu)
    				$scope.viewCharts = rights["charts"];
    				$scope.viewUserAccess = rights["useraccess"];
    				$scope.viewAudit = rights["audit"];

    				// Para llenar el comboBox de sucursales
    				//if ($scope.employee.UserName !== "root") {
    				//	$scope.sucursales = new Array();
    				//	for (var i = 0; i < $scope.employee.Sucursales.length; i++) {
    				//		var suc = $scope.employee.Sucursales[i].Region.Name + ": " +
    				//			$scope.employee.Sucursales[i].Office.Name + "/" +
    				//			$scope.employee.Sucursales[i].Office.Market.Name;
    				//		$scope.sucursales.push(suc);
    				//	}
    				//}
    			});
    		}
    	};
    }])

// .factory("currentSucursalFactory", function () {
// 	var currentSucursal;
// 	return {
// 		// call this function if you need filter by the selected Sucursal
// 		getCurrentSucursal: function () {
// 			return currentSucursal;
// 		},
// 		setCurrentSucursal: function (data) {
// 			currentSucursal = data;
// 		}
// 	};
// })

// .directive('currentSucursalComboDirective', ["currentSucursalFactory",
// 	function (currentSucursalFactory) {
// 		return {
// 			link: function (scope) {
// 				scope.$watch('menu.sucursalSelected', function () {
// 					currentSucursalFactory.setCurrentSucursal(this.last);
// 				});
// 			}
// 		};
// 	}])

;
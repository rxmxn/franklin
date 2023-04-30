"use strict";

angular.module("abcadmin")
    .controller("ClasificacionQuimicaController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller',
        function ($scope, $route, $location, $http, $cookies, $controller) {

            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------
            
            function urlContains(url, word) {
            	if (url.indexOf(word) > -1) {
            		return true;
            	}
            	return false;
            }

	        $scope.Level = urlContains($location.path(), "/chemicalclasification1") === true ? 1
		        : urlContains($location.path(), "/chemicalclasification2") ? 2 : 3;

            var filters = function (page) {
                return {
                    page: (page === null || page === undefined) ? $scope.currentPage : page,
                    pageSize: $scope.pageSize,
                    searchGeneral: $scope.searchGeneral,
                	alta: $scope.alta,
                	baja: $scope.baja,
					level: $scope.Level
                }
            };
			
            var setActivationObjects = function (eObj, act) {
                return { id: eObj.Id, active: act }
            };

            var saveObject = function (promise) {
	            var clasificacionquimicaObj = {
		            Id: promise.Id,
		            Name: promise.Name,
		            Active: promise.Active,
		            Description: promise.Description,
		            Level: promise.Level
            };
                return { clasificacionquimica: clasificacionquimicaObj }
            }

            var editElement = function (element) {
                return {
                    template: "editDialog",
                    controller: "NewClasificacionQuimicaController",
                    className: "ngdialog-theme-default",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element, level: $scope.Level },
                    closeByEscape: true
                }
            }

            var icon = $scope.Level === 1 ? "icon mif-weather" :
					   $scope.Level === 2 ? "icon mif-weather2" : "icon mif-weather3";

            $scope.mainFunction(
                icon, "Name",
                "/clasificacionquimica/refreshlist", filters,
                "/clasificacionquimica/saveactivestatus", setActivationObjects,
                "/clasificacionquimica/saveclasificacionquimica", saveObject,
                editElement,
				"clasificacionquimica"
                );

            // ClasificacionQuimica specifics
            $scope.SearchGeneral = true;
            $scope.FilterActive = true;
            $scope.boxHeaderTitleActive = "Clasificaciones Químicas nivel " + $scope.Level + " activas";
            $scope.boxHeaderTitleDeactive = "Clasificaciones Químicas nivel " + $scope.Level + " de baja";

            $scope.panels = [
                {
                    index: 0,
                    name: 'Filtros',
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: "Tabla de Clasificaciones Químicas nivel " + $scope.Level,
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewClasificacionQuimicaController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog",
        function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog) {

            var errorDelay = 5000;

            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nueva clasificación química nivel " + $scope.ngDialogData.level 
                : "Editar clasificación química nivel " + $scope.ngDialogData.level + " " + $scope.ngDialogData.dData.Name;

            $scope.acceptDialog = function () {
                var error = false;

                if ($scope.ngDialogData.dData.Name === undefined) {
                    notification.error({
                    	message: "Nombre de la clasificación química indefinido",
                        delay: errorDelay
                    });
                    error = true;
                }
                if (error === true)
                    return;

                $scope.confirm({
                    Id: $scope.ngDialogData.dData.Id != null ?
                        $scope.ngDialogData.dData.Id : 0,
                    Name: $scope.ngDialogData.dData.Name,
                    Active: $scope.ngDialogData.dData.Active,
                    Description: $scope.ngDialogData.dData.Description,
                    Level: $scope.ngDialogData.level
                });

            }
        }

    ])
;
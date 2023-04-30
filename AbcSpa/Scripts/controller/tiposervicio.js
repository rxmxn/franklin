"use strict";

angular.module("abcadmin")
    .controller("TipoServicioController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller',
        function ($scope, $route, $location, $http, $cookies, $controller) {
            
            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------
            
            var filters = function ()
            {
                return {
                	alta: $scope.alta,
                	baja: $scope.baja
                }
            };
			
            var setActivationObjects = function (eObj, act) {
                return { id: eObj.Id, active: act }
            };

            var saveObject = function (promise) {
                var tiposervicioObj = {
                    Id: promise.Id,
                    Name: promise.Name,
                    Active: promise.Active,
                    Description: promise.Description
                };
                return { tiposervicio: tiposervicioObj }
            }

            var editElement = function (element) {
                return {
                    template: "editDialog",
                    controller: "NewTipoServicioController",
                    className: "ngdialog-theme-default",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }

            $scope.mainFunction(
                "icon mif-folder", "Name",
                "/tiposervicio/refreshlist", filters,
                "/tiposervicio/saveactivestatus", setActivationObjects,
                "/tiposervicio/savetiposervicio", saveObject,
                editElement,
				"tiposervicio"	// to get the access level
                );

            // TipoServicio specifics
            $scope.FilterGeneral = true;
            $scope.FilterActive = true;
            $scope.boxHeaderTitleActive = "Tipos de servicio activos";
            $scope.boxHeaderTitleDeactive = "Tipos de servicio de baja";
			
            $scope.panels = [
                {
                    index: 0,
                    name: 'Filtros',
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: 'Tabla Tipos de servicio',
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewTipoServicioController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog",
        function($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog) {
            
            var errorDelay = 5000;

            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nuevo tipo de servicio" : "Editar tipo de servicio " + $scope.ngDialogData.dData.Name;

            $scope.acceptDialog = function () {
                var error = false;

                if ($scope.ngDialogData.dData.Name === undefined) {
                    notification.error({
                        message: "Nombre del tipo de servicio indefinido",
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
                    Description: $scope.ngDialogData.dData.Description
                });

            }
        }
        
    ])
;
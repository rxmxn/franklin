"use strict";

angular.module("abcadmin")
    .controller("AlcanceController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller',
        function($scope, $route, $location, $http, $cookies, $controller) {
            
            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------
            
            var filters = function (page)
            {
                return {
                    page: (page === null || page === undefined) ? $scope.currentPage : page,
                    pageSize: $scope.pageSize,
                    alta: $scope.alta,
                    baja: $scope.baja
                }
            };


            var setActivationObjects = function (eObj, act) {
                return { id: eObj.Id, active: act }
            };

            var saveObject = function (promise) {
                var alcanceObj = {
                    Id: promise.Id,
                    Name: promise.Name,
                    Active: promise.Active,
                    Description: promise.Description,
                    ZonaGeografica: promise.ZonaGeografica
                };
                return { alcance: alcanceObj }
            }

            var editElement = function (element) {
                return {
                    template: "editDialog",
                    controller: "NewAlcanceController",
                    className: "ngdialog-theme-default",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }

            $scope.mainFunction(
                "icon mif-unlink", "Name",
                "/alcance/refreshlist", filters,
                "/alcance/saveactivestatus", setActivationObjects,
                "/alcance/savealcance", saveObject,
                editElement,
				"ack"
                );

            // Alcance specifics
            $scope.FilterGeneral = true;
            $scope.FilterActive = true;
            // $scope.boxHeaderTitleActive = "Categorías activas";
            // $scope.boxHeaderTitleDeactive = "Categorías de baja";

            $scope.panels = [
                {
                    index: 0,
                    name: 'Filtros',
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: 'Tabla Alcances de Reconociminetos',
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewAlcanceController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog",
        function($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog) {
            
            var errorDelay = 5000;
			
            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nuevo Alcance de Reconociminetos" : "Editar Alcance de Reconociminetos " + $scope.ngDialogData.dData.Name;

            $scope.acceptDialog = function () {
                var error = false;

                if ($scope.ngDialogData.dData.Name === undefined) {
                    notification.error({
                    	message: "Alcance indefinido",
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
                    ZonaGeografica: $scope.ngDialogData.dData.ZonaGeografica
                });

            }
        }
        
    ])
;
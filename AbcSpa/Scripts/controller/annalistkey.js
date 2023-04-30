"use strict";

angular.module("abcadmin")
    .controller("AnnalistKeyController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller',
        function ($scope, $route, $location, $http, $cookies, $controller) {
            
            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------
            
            var filters = function (page)
            {
                return {
                    page: (page === null || page === undefined) ? $scope.currentPage : page,
                    pageSize: $scope.pageSize,
                    searchGeneral: $scope.searchGeneral,
                	alta: $scope.alta,
                	baja: $scope.baja
                }
            };
			
            var setActivationObjects = function (eObj, act) {
                return { id: eObj.Id, active: act }
            };

            var saveObject = function (promise) {
                var annalistKeyObj = {
                    Id: promise.Id,
                    Clave: promise.Name,
                    Active: promise.Active,
                    Description: promise.Description
                };
                return { annalistKey: annalistKeyObj }
            }

            var editElement = function (element) {
                return {
                    template: "editDialog",
                    controller: "NewAnnalistKeyController",
                    className: "ngdialog-theme-default",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }

            $scope.mainFunction(
                "icon mif-key", "Name",
                "/annalistkey/refreshlist", filters,
                "/annalistkey/saveactivestatus", setActivationObjects,
                "/annalistkey/saveannalistkey", saveObject,
                editElement,
				"annalistkey"	// to get the access level
                );

            // Residue specifics
            $scope.SearchGeneral = true;
            $scope.FilterActive = true;
            // $scope.boxHeaderTitleActive = "Residuos activos";
            // $scope.boxHeaderTitleDeactive = "Residuos de baja";
			
            $scope.panels = [
                {
                    index: 0,
                    name: 'Filtros',
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: 'Tabla de Claves de analistas',
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewAnnalistKeyController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog",
        function($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog) {
            
            var errorDelay = 5000;

            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nueva clave de analista" : "Editar clave de analista" + $scope.ngDialogData.dData.Clave;

            $scope.acceptDialog = function () {
                var error = false;

                if ($scope.ngDialogData.dData.Clave === undefined) {
                    notification.error({
                        message: "Campo Clave indefinido",
                        delay: errorDelay
                    });
                    error = true;
                }
                if (error === true)
                    return;

                $scope.confirm({
                    Id: $scope.ngDialogData.dData.Id != null ?
                        $scope.ngDialogData.dData.Id : 0,
                    Name: $scope.ngDialogData.dData.Clave,
                    Active: $scope.ngDialogData.dData.Active,
                    Description: $scope.ngDialogData.dData.Description
                });

            }
        }
        
    ])
;
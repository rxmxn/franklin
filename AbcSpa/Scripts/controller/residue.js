"use strict";

angular.module("abcadmin")
    .controller("ResidueController",
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
                var residueObj = {
                    Id: promise.Id,
                    Name: promise.Name,
                    Active: promise.Active,
                    Description: promise.Description
                };
                return { residue: residueObj }
            }

            var editElement = function (element) {
                return {
                    template: "editDialog",
                    controller: "NewResidueController",
                    className: "ngdialog-theme-default",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }

            $scope.mainFunction(
                "icon mif-bin", "Name",
                "/residue/refreshlist", filters,
                "/residue/saveactivestatus", setActivationObjects,
                "/residue/saveresidue", saveObject,
                editElement,
				"residue"	// to get the access level
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
                    name: 'Tabla Tipos de Desechos',
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewResidueController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog",
        function($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog) {
            
            var errorDelay = 5000;

            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nuevo Tipo de Desecho" : "Editar Tipo de Desecho " + $scope.ngDialogData.dData.Name;

            $scope.acceptDialog = function () {
                var error = false;

                if ($scope.ngDialogData.dData.Name === undefined) {
                    notification.error({
                        message: "Nombre del Tipo de Desecho indefinido",
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
"use strict";

angular.module("abcadmin")
    .controller("PreserverController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', "SecurityFact",
        function ($scope, $route, $location, $http, $cookies, $controller, securityFact) {
            
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
                var preserverObj = {
                    Id: promise.Id,
                    Name: promise.Name,
                    Active: promise.Active,
                    Description: promise.Description
                };
                return { preserver: preserverObj }
            }

            
            var editElement = function (element) {
                return {
                    template: "editDialog",
                    controller: "NewPreserverController",
                    className: "ngdialog-theme-default",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }

            $scope.mainFunction(
                "icon mif-magic-wand", "Name",
                "/preserver/refreshlist", filters,
                "/preserver/saveactivestatus", setActivationObjects,
                "/preserver/savepreserver", saveObject,
                editElement,
				"preserver"	// to get the access level
                );

            // Preserver specifics
            $scope.SearchGeneral = true;
            $scope.FilterActive = true;
            $scope.boxHeaderTitleActive = "Preservadores activos";
            $scope.boxHeaderTitleDeactive = "Preservadores de baja";

            // minimize
            //$scope.minimize = function (element) {
            //    element.state = !element.state;

            //    if (element.index === 1 && $scope.panels[0].state === true) {
            //        $scope.panels[0].state = element.state;
            //    }
            //    else if ((element.index === 0 && element.state === true)
            //        && $scope.panels[1].state === false) {
            //        $scope.panels[1].state = element.state;
            //        $scope.panels[1].size = 9;
            //    }
            //    else if ((element.index === 0 && element.state === false)
            //        && $scope.panels[1].state === true) {
            //        $scope.panels[1].size = 12;
            //    }
            //    else if (element.index === 0 && element.state === true) {
            //        $scope.panels[1].size = 9;
            //    }
            //    else if (element.index === 1 && $scope.panels[0].state === false) {
            //        $scope.panels[1].size = 12;
            //    }
            //}

            $scope.panels = [
                {
                    index: 0,
                    name: 'Filtros',
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: 'Tabla Preservadores',
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewPreserverController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog",
        function($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog) {
            
            var errorDelay = 5000;

            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nuevo preservador" : "Editar preservador " + $scope.ngDialogData.dData.Name;

            // TODO: modificar para que ademas de mostrar la notificacion ocurra alguna sennalizacion en el input
            $scope.acceptDialog = function () {
                var error = false;

                if ($scope.ngDialogData.dData.Name === undefined) {
                    notification.error({
                        message: "Nombre del preservador indefinido",
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
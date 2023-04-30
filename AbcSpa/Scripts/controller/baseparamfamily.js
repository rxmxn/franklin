"use strict";

angular.module("abcadmin")
    .controller("BaseParamFamilyController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller',
        function ($scope, $route, $location, $http, $cookies, $controller) {

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
                var baseparamfamilyObj = {
                    Id: promise.Id,
                    Name: promise.Name,
                    Active: promise.Active,
                    Description: promise.Description
                };
                return { baseparamfamily: baseparamfamilyObj }
            }

            var editElement = function (element) {
                return {
                    template: "editDialog",
                    controller: "NewBaseParamFamilyController",
                    className: "ngdialog-theme-default",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }

            $scope.mainFunction(
                "icon mif-windy", "Name",
                "/baseparamfamily/refreshlist", filters,
                "/baseparamfamily/saveactivestatus", setActivationObjects,
                "/baseparamfamily/savebaseparamfamily", saveObject,
                editElement,
				"baseparamfamily"
                );

            // BaseParamFamily specifics
            $scope.FilterGeneral = true;
            $scope.FilterActive = true;
            $scope.boxHeaderTitleActive = "Familias de parámetros activos";
            $scope.boxHeaderTitleDeactive = "Familias de parámetros de baja";

            $scope.panels = [
                {
                    index: 0,
                    name: 'Filtros',
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: 'Tabla de Familias de parámetros',
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewBaseParamFamilyController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog",
        function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog) {

            var errorDelay = 5000;

            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nueva familia de parámetros" : "Editar familia de parámetros";

            // TODO: modificar para que ademas de mostrar la notificacion ocurra alguna sennalizacion en el input
            $scope.acceptDialog = function () {
                var error = false;

                if ($scope.ngDialogData.dData.Name === undefined) {
                    notification.error({
                        message: "Nombre de la familia de parámetros indefinido",
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
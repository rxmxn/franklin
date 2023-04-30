﻿"use strict";

angular.module("abcadmin")
    .controller("CurrencyController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', 'ngDialog',
        function ($scope, $route, $location, $http, $cookies, $controller, ngDialog) {

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
                var currencyObj = {
                    Id: promise.Id,
                    Name: promise.Name,
                    Active: promise.Active,
                    Description: promise.Description
                };
                return { currency: currencyObj }
            }

            var editElement = function (element) {
                return {
                    template: "editDialog",
                    controller: "NewCurrencyController",
                    className: "ngdialog-theme-default",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }

            $scope.mainFunction(
                "icon mif-coins", "Name",
                "/currency/refreshlist", filters,
                "/currency/saveactivestatus", setActivationObjects,
                "/currency/savecurrency", saveObject,
                editElement,
				"currency"
                );

            // Currency specifics
            $scope.FilterGeneral = true;
            $scope.FilterActive = true;
            $scope.boxHeaderTitleActive = "Monedas activos";
            $scope.boxHeaderTitleDeactive = "Monedas de baja";

            $scope.panels = [
                {
                    index: 0,
                    name: 'Filtros',
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: 'Tabla monedas',
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewCurrencyController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog",
        function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog) {

            var errorDelay = 5000;

            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nuevo moneda" : "Editar moneda " + $scope.ngDialogData.dData.Name;

            // TODO: modificar para que ademas de mostrar la notificacion ocurra alguna sennalizacion en el input
            $scope.acceptDialog = function () {
                var error = false;

                if ($scope.ngDialogData.dData.Name === undefined) {
                    notification.error({
                        message: "Nombre del moneda indefinido",
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
"use strict";

angular.module("abcadmin")
    .controller("NormController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller',
        function ($scope, $route, $location, $http, $cookies, $controller) {

            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------

            var filters = function (page) {
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
                var normObj = {
                    Id: promise.Id,
                    Name: promise.Name,
                    Active: promise.Active,
                    Description: promise.Description,
                    Alcance: promise.Alcance,
                    FechaEntVigor: promise.FechaEntVigor,
                    Estado: promise.Estado
                };
                return { norm: normObj }
            }

            var editElement = function (element) {
                return {
                    template: "editDialog",
                    controller: "NewNormController",
                    className: "ngdialog-theme-default ngdialog-width700",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }

            $scope.mainFunction(
                "icon mif-traffic-cone", "Name",
                "/norm/refreshlist", filters,
                "/norm/saveactivestatus", setActivationObjects,
                "/norm/savenorm", saveObject,
                editElement,
				"norm"
                );

            // Norm specifics
            $scope.FilterGeneral = true;
            $scope.FilterActive = true;
            $scope.boxHeaderTitleActive = "Normas activos";
            $scope.boxHeaderTitleDeactive = "Normas de baja";

            $scope.panels = [
                {
                    index: 0,
                    name: 'Filtros',
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: 'Tabla Normas',
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewNormController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog",
        function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog) {

            var errorDelay = 5000;

            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nuevo norma" : "Editar norma " + $scope.ngDialogData.dData.Name;

            //function getMatrixes() {
            //    $http.post('/matrix/refreshList',{page:1,pageSize:10})
            //	.success(function (response) {
            //	    if (response.success === true) {
            //	        $scope.matrixes = response.elements;
            //	    } else {
            //	        notification.error({
            //	            message: "Error al recuperar elementos.",
            //	            delay: errorDelay
            //	        });
            //	    }
            //	})
            //	.error(function () {
            //	    notification.error({
            //	        message: "Error al conectar con el servidor.",
            //	        delay: errorDelay
            //	    });
            //	});
            //}

            //getMatrixes();

            function getStatus() {
                $http.post('/status/refreshList', { page: 1, pageSize: 100 })
				.success(function (response) {
				    if (response.success === true) {
				        $scope.statusList = response.elements;
				    } else {
				        notification.error({
				            message: "Error al recuperar elementos.",
				            delay: errorDelay
				        });
				    }
				})
				.error(function () {
				    notification.error({
				        message: "Error al conectar con el servidor.",
				        delay: errorDelay
				    });
				});
            }

            getStatus();

            // TODO: modificar para que ademas de mostrar la notificacion ocurra alguna sennalizacion en el input
            $scope.acceptDialog = function () {
                var error = false;

                if ($scope.ngDialogData.dData.Name === undefined) {
                    notification.error({
                        message: "Nombre del norma indefinido",
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
                    Alcance: $scope.ngDialogData.dData.Alcance,
                    FechaEntVigor: $scope.ngDialogData.dData.FechaEntVigor,
                    Estado: ($scope.ngDialogData.dData.Estado) ? { Id: $scope.ngDialogData.dData.Estado.Id } : null
                });

            }
        }

    ])
;
"use strict";

function copy(o) {
    var output, v, key;
    output = Array.isArray(o) ? [] : {};
    for (key in o) {
        v = o[key];
        output[key] = (typeof v === "object") ? copy(v) : v;
    }
    return output;
}

angular.module("abcadmin")
    .controller("MatrixController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', "SecurityFact", "Notification", "ngDialog", "dataProvider", "generalFactory", "counterFactory",
        function ($scope, $route, $location, $http, $cookies, $controller, securityFact, notification, ngDialog, dataProvider, generalFactory, counterFactory) {
            
            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------
            $scope.toggleFullScreen = function () {

                $scope.isFullscreen = !$scope.isFullscreen;
            }

            var filters = function (page) {
                return {
                    page: (page === null || page === undefined) ? $scope.currentPage : page,
                    pageSize: $scope.pageSize,
                    searchGeneral: $scope.searchGeneral,
                    searchMatrix: $scope.searchMatrix,
                    searchSubmatrix: $scope.searchSubmatrix,
                    bMatrixId: $scope.bMatrixId,
                    marketId: $scope.marketId,
                    fromDate: $scope.fromDate === undefined ? null : $scope.fromDate,
                    untilDate: $scope.untilDate === undefined ? null : $scope.untilDate,
                    alta: $scope.alta,
                    baja: $scope.baja
                }
            };

            var setActivationObjects = function (eObj, act) {
                return { id: eObj.Id, active: act }
            };

            var saveObject = function (promise) {

                var matrixObj = {
                    Id: promise.Id,
                    BaseMatrix: promise.BaseMatrix,
                    Description: promise.Description,
                    Name: promise.Name,
                    SubMatrix: promise.SubMatrix,
                    SubMtrxDescription: promise.SubMtrxDescription,
                    Active: promise.Active,
                    Sucursal: promise.Sucursal
                };
                return { matrix: matrixObj }
            }

            $scope.editElement = function (data) {

                var id = (data === null) ? 0 : data.Id;
                $http.post("/matrix/GetElems", { id: id })
                            .success(function (response) {
                                if (response.success) {

                                    var tdata = {};
                                    angular.extend(tdata, data);

                                    ngDialog.openConfirm({
                                        template: "Shared/EditMatrix",
                                        controller: "NewMatrixController",
                                        className: "ngdialog-theme-default ngdialog-width1000",
                                        preCloseCallback: "preCloseCallbackOnScope",
                                        data: {
                                            dData: tdata,
                                            baseMatrixList: response.baseMatrixList,
                                            matrixList: response.matrixList,
                                            submatrixList: response.submatrixList
                                        },
                                        closeByEscape: true
                                    })

                                    .then(function (promise) {
                                        //Accepting
                                        $scope.save(promise);

                                    }, function (reason) {
                                        //Rejecting
                                    });
                                } else {
                                    notification.error({
                                        message: "Error al recuperar los datos. " + response.err, delay: 5000
                                    });
                                }
                                $scope.isLoading = false;
                            })
                    .error(function () {
                        notification.error({
                            message: "Error en la conexión con el servidor.", delay: 5000
                        });
                        $scope.isLoading = false;
                    });
            }

            $scope.mainFunction(
                "icon mif-windy4", "Name",
                "/matrix/refreshlist", filters,
                "/matrix/saveactivestatus", setActivationObjects,
                "/matrix/savematrix", saveObject,
                null,
                "matrix"
            );

            $scope.setMatrix = function (matrix) {
                dataProvider.setCurrData(matrix);
            }

            function getFilters() {
                $http.post("/matrix/getFilters")
                    .success(function (response) {
                        if (response.success) {
                            $scope.matrixList = response.matrixList;
                            $scope.subMatrixList = response.submatrixList;
                            $scope.baseMatrixList = response.baseMatrixList;
                            $scope.marketList = response.marketList;
                           
                        } else {
                            notification.error({
                                message: "Error en la conexión con el servidor.",
                                delay: 5000
                            });
                            $scope.initSaving = false;
                        }
                    })
                    .error(function () {
                        notification.error({
                            message: "Se obtuvo un error del servidor. Ver detalles.",
                            delay: 5000
                        });
                        $scope.initSaving = false;
                    });
            }

            getFilters();

            // Matrix specifics
           // $scope.FilterGeneral = true;
           // $scope.SearchGeneral = true; // if true then the FilterName will appear in the SidebarFilter
            $scope.MatrixFilterActive = true;
            $scope.bMatrixFilterActive = true;
            $scope.subMatrixFilterActive = true;
            $scope.MarketFilterActive = true;
            $scope.FilterActive = true;
            $scope.FilterDate = true;
            $scope.boxHeaderTitleActive = "Matrices activas";
            $scope.boxHeaderTitleDeactive = "Matrices de baja";

            $scope.panels = [
                {
                    index: 0,
                    name: "FILTROS",
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: "TABLA DE MATRICES",
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewMatrixController",
    [
        "$scope", "$routeParams", "$location", "$http", "$controller", "SecurityFact", "Notification", "ngDialog",
        function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog) {

            var errorDelay = 5000;
			
            $scope.bmChange = function () {
                for (var i = 0; i < $scope.ngDialogData.baseMatrixList.length; i++) {
                    if ($scope.ngDialogData.baseMatrixList[i].Id === $scope.ngDialogData.dData.BaseMatrix.Id) {
                        $scope.ngDialogData.dData.BaseMatrix = copy($scope.ngDialogData.baseMatrixList[i]);
                        break;
                    }
                }
            }

            $scope.getMatrixName=function() {
                console.log('ok');
            }
			
            $scope.dialogTitle = $scope.ngDialogData.dData.Id === undefined ?
                 "Nueva matriz" : "Editar matriz " + $scope.ngDialogData.dData.Name;

            $scope.acceptDialog = function () {
                var error = false;
                if ($scope.ngDialogData.dData.BaseMatrix === undefined) {
                    notification.error({
                        message: "Grupo de matriz indefinido",
                        delay: errorDelay
                    });
                    error = true;
                }

                if (error === true)
                    return;

                $scope.confirm({
                    Id: ($scope.ngDialogData.dData.Id !== undefined) ? $scope.ngDialogData.dData.Id : 0,
                    BaseMatrix: $scope.ngDialogData.dData.BaseMatrix,
                    Name: $scope.ngDialogData.dData.Name,
                    Description: $scope.ngDialogData.dData.Description,
                    SubMatrix: $scope.ngDialogData.dData.SubMatrix,
                    SubMtrxDescription: $scope.ngDialogData.dData.SubMtrxDescription,
                    Active: $scope.ngDialogData.dData.Active,
                });
            };
        }
    ])

    .directive("chCombo", ["$http",
        function ($http) {
            return {
                link: function (scope) {
                    scope.$watch('ngDialogData.dData.Sucursal.Id', function () {
                        if (scope.ngDialogData.dData.Sucursal !== undefined) {
                            //console.log(scope.ngDialogData.dData.BaseParam.Id);
                            if (scope.ngDialogData.dData.Sucursal.Id === null) {
                                scope.matrixList = [];
                            } else {

                                $http.post("/matrix/getmatrixforsucursal", { id: scope.ngDialogData.dData.Sucursal.Id })
                                    .success(function (response) {
                                        if (response.success === true) {
                                            scope.matrixList = response.elements;
                                        }
                                    })
                                    .error(function () {
                                        //                                        console.log("no entra");
                                    });
                            }
                        }
                    });
                }
            }
        }]);


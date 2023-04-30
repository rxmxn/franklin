"use strict";
//************************ojo agregar al prototipo de los arreglos****************
function copy(o) {
    var output, v, key;
    output = Array.isArray(o) ? [] : {};
    for (key in o) {
        v = o[key];
        output[key] = (typeof v === "object") ? copy(v) : v;
    }
    return output;
}
//*******************************************************************************
angular.module("abcadmin")
    .controller("MaxPermitedLimitController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', "SecurityFact", "Notification", "ngDialog", "dataProvider", "generalFactory", "counterFactory", "InfoService",
        function ($scope, $route, $location, $http, $cookies, $controller, securityFact, notification, ngDialog, dataProvider, generalFactory, counterFactory, InfoService) {

            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------
            $scope.toggleFullScreen = function () {

                $scope.isFullscreen = !$scope.isFullscreen;
                if ($scope.accessLevel === 1) {
                    if ($scope.isFullscreen)
                        $scope.col_defs.splice($scope.col_defs.length - 3, 3);
                    else {
                        $scope.col_defs.push({
                            field: "edit",
                            displayName: "Editar",
                            levels: [1],
                            cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Editar'><span class='icon mif-pencil' style='color: #1b6eae; font-size: 1rem;'></span></a></div>",
                            //"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
                            cellTemplateScope: {
                                click: function (data) { // this works too: $scope.someMethod;
                                    data.d = false;
                                    $scope.editElement(data);
                                }
                            },
                            width: "50px"
                        });

                        $scope.col_defs.push({
                            field: "duplicate",
                            displayName: "Duplicar",
                            levels: [1],
                            width: "50px",
                            cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Duplicar'><span class='icon mif-floppy-disk' style='color: #1b6eae; font-size: 1rem;'></span></a></div>",
                            //"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
                            cellTemplateScope: {
                                click: function (data) { // this works too: $scope.someMethod;
                                    data.d = true;
                                    $scope.edit(data);
                                }
                            }
                        });

                        $scope.col_defs.push({
                            field: "",
                            displayName: "Dar baja/alta",
                            levels: [1],
                            width: "50px",
                            cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title=\"{{row.branch.Active ? 'Dar baja' : 'Dar alta'}}\"><span ng-class=\"[row.branch.Active ? 'fa fa-thumbs-down' : 'fa fa-thumbs-up']\" ng-style=\"{'color': row.branch.Active ?'red':'green'}\" style='font-size:24px;'></span></a></div>",
                            //"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
                            cellTemplateScope: {
                                click: function (data) {
                                    $scope.setActivation(data, !data.Active);
                                }
                            }
                        });
                    }
                }
            }

            var filters = function (page) {
                $("#loadingSpinners").show();
                return {
                    page: (page === null || page === undefined) ? $scope.currentPage : page,
                    pageSize: $scope.pageSize,
                    searchGeneral: $scope.searchGeneral,
                    // fromDate: $scope.fromDate === undefined ? null : $scope.fromDate,
                    // untilDate: $scope.untilDate === undefined ? null : $scope.untilDate,
                    alta: $scope.alta,
                    baja: $scope.baja
                }
            };

            var setActivationObjects = function (eObj, act) {
                return { id: eObj.Id, active: act }
            };

            var address = function (elementType) {
                switch (elementType) {
                    case "parameter":
                        return "/param/saveparam";
                    case "group":
                        return "/group/savegroup";
                    case "package":
                        return "/package/savepackage";
                    case "matrix":
                        return "/matrix/savematrix";
                    default:
                        return "";
                }

            }

            var saveObject = function (promise) {

                var maxPermitedLimitObj = {
                    Id: promise.Id,
                    Name: promise.Name,
                    Description: promise.Description,
                    Active: promise.Active,
                    ParamRoutes: promise.ParamRoutes
                }

                return { maxPermitedLimit: maxPermitedLimitObj }
            }

            var save = function (promise) {
                $scope.initSaving = true;

                $http.post("/maxpermitedlimit/SaveMaxPermitedLimit", saveObject(promise))
                    .success(function (response) {
                        if (response.success) {
                            notification.success({
                                message: "Datos guardados correctamente.",
                                delay: 3000
                            });
                            $scope.initSaving = false;
                            $scope.RefreshList();
                            counterFactory.getAllCounts(true);
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
                            message: "Se obtuvo un error en la comunicación con el servidor.",
                            delay: 5000
                        });
                        $scope.initSaving = false;
                    });
            }

            $scope.editElement = function (data) {
                $("#loadingSpinners").show();
                //var accAddress = (data.elemType === "package") ? "/package/getaccessories" : (data.elemType === "group") ? "/group/getparamsforgroup" : "/matrix/getsucursal";
                var accAddress = "/maxpermitedlimit/getallelements";
                var id = (data === null) ? 0 : data.Id;
                $http.post(accAddress, { page: 1, pageSize: 10, Id: id, searchGeneral: "", viewParameters: true, viewGroups: true, viewPackages: true })
                            .success(function (response) {
                                if (response.success) {
                                    var elements = response.elements;
                                    if (data != null) {
                                        for (var i = 0; i < elements.length; i++) {
                                            for (var j = 0; j < data.children.length; j++) {
                                                if (elements[i].Id === data.children[j].Id && elements[i].elemType === data.children[j].elemType) {
                                                    elements[i].check = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    var template = "Shared/EditLMP";
                                    var dialogController = "NewMaxPermitedLimitController";

                                    var tElement = {};
                                    var colDef = copy($scope.col_defs.slice(0, $scope.col_defs.length - 2));

                                    angular.extend(tElement, data);

                                    ngDialog.openConfirm({
                                        template: template,
                                        controller: dialogController,
                                        className: "ngdialog-theme-default ngdialog-width2000",
                                        preCloseCallback: "preCloseCallbackOnScope",
                                        data: { dData: tElement, expanding_property: $scope.expanding_property, col_defs: colDef, elementList: elements, elemUsedByLMP: response.elemUsedByLMP, total: response.total, tableWidht: $scope.tableWidht },
                                        closeByEscape: true
                                    })

                                    .then(function (promise) {
                                        //Accepting
                                        save(promise);

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
                "/maxpermitedlimit/refreshlist", filters,
                "/maxpermitedlimit/saveactivestatus", setActivationObjects,
                "/maxpermitedlimit/SaveMaxPermitedLimit", saveObject,
                null,
                "group"
            );

            $scope.setMatrix = function (matrix) {
                dataProvider.setCurrData(matrix);
            }

            // Matrix specifics
            $scope.FilterGeneral = true;
            $scope.SearchGeneral = true; // if true then the FilterName will appear in the SidebarFilter
            $scope.FilterActive = true;
            $scope.boxHeaderTitleActive = "Límites activos";
            $scope.boxHeaderTitleDeactive = "Límites de baja";

            $scope.panels = [
                {
                    index: 0,
                    name: "FILTROS",
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: "TABLA DE LÍMITES MÁXIMOS PERMISIBLES",
                    state: true,
                    size: 9
                }
            ];

            //to configure treeGrid

            //$scope.f = function () {
            //    var index = $scope.col_defs.length - 1;
            //    $scope.col_defs[index].displayName = row.branch.Active ? "Dar Alta" : "Dar Baja";
            //    $scope.col_defs[index].cellTemplate = row.branch.Active ? "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Dar alta'><span class='fa fa-thumbs-up' style='color:#1b6eae; font-size: 1rem;'></span></a></div>" :
            //    "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Dar baja'><span class='fa fa-thumbs-down' style='color: #1b6eae; font-size: 1rem;'></span></a></div>";
            //    return;
            //}

            $scope.tableWidth = 300;

            $scope.rowActions = [];

            $scope.my_tree = {};
            $scope.expanding_property = {
                field: "Name",
                displayName: "Nombre",
                sortable: true,
                filterable: true,
                width: "200px"
            };
            $scope.col_defs = new Array();
            $scope.col_defs.push({
                field: "Description",
                displayName: "Descripción Específica",
                width: "300px"
            });

            $scope.col_defs.push({
                field: "elemType",
                displayName: "Tipo de artículo",
                levels: [2, 3, 4],
                cellTemplate: "<div class='text-uppercase'>{{(row.branch[col.field]==='group')?'Grupo':(row.branch[col.field]==='parameter')?'Parámetro':'paquete'}}</div>",
                width: "150px"
            });

            $scope.col_defs.push({
                field: "MaxPermitedLimit",
                displayName: "LMP",
                width: "100px",
                tooltip: "Límite Máximo Permitido",
                levels: [2, 3, 4],
                cellTemplate: "<div>{{row.branch[col.field].Value.toFixed(row.branch[col.field].DecimalsPoints)}}</div>"
            });


            $scope.col_defs.push({
                field: "GenericKey",
                displayName: "Clave Genérica",
                width: "100px"
            });

            $scope.col_defs.push({
                field: "GenericDescription",
                displayName: "Descripción Genérica",
                width: "150px"
            });

            //$scope.col_defs.push({
            //    field: "CentroCosto",
            //    displayName: "CeC",
            //    tooltip: "Centro de Costo",
            //    levels: [2, 3, 4],
            //    width: "50px"
            //    // cellTemplate: "<div>{{row.branch[col.field]}}</div>"
            //});

            //$scope.col_defs.push({
            //    field: "Precio",
            //    displayName: "Precio",
            //    levels: [1, 2, 3],
            //    width: "100px",
            //    cellTemplate: "<div>{{row.branch[col.field]}}</div>"
            //    //"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
            //});

            //$scope.col_defs.push({
            //    field: "AutolabAssignedAreaName",
            //    displayName: "NAAA",
            //    width: "100px",
            //    tooltip: "Nombre de Área Autolab Asignada"
            //});


            //$scope.col_defs.push({
            //    field: "Annalists",
            //    displayName: "Analistas",
            //    //cellTemplate: "<ul style='margin-left: 0; padding-left: 1rem;'><li ng-repeat='an in row.branch[col.field]'>{{an}}</li></ul>",
            //    levels: [2, 3, 4],
            //    width: "150px",
            //    tooltip: "Analistas",
            //    cellTemplate: "<a href='' ng-repeat='an in row.branch[col.field]' ng-click='cellTemplateScope.click(an)'>{{an}}{{row.branch[col.field][row.branch[col.field].length -1] !== an ? ',' : ''}} </a>",
            //    cellTemplateScope: {
            //        click: function (data) {
            //            $scope.viewAnnalistInfo(data);
            //        }
            //    }
            //});

            $scope.viewAnnalistInfo = function (a) {
                InfoService.viewAnnalistInfo(a);
            }

            $scope.col_defs.push({
                field: "Unit",
                displayName: "Unidad de Reporte",
                levels: [2, 3, 4],
                width: "150px",
                cellTemplate: "<div>{{row.branch[col.field]}}</div>"
            });

            $scope.col_defs.push({
                field: "DecimalesReporte",
                displayName: "Decimales para Reporte",
                levels: [2, 3, 4],
                width: "150px",
                // cellTemplate: "<div>{{row.branch[col.field]}}</div>"
            });

            var printAccess = function () {
                if ($scope.accessLevel === 1) {
                    $scope.col_defs.push({
                        field: "edit",
                        displayName: "Editar",
                        levels: [1],
                        cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Editar'><span class='icon mif-pencil' style='color: #1b6eae; font-size: 1rem;'></span></a></div>",
                        //"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
                        cellTemplateScope: {
                            click: function (data) { // this works too: $scope.someMethod;
                                data.d = false;
                                $scope.editElement(data);
                            }
                        },
                        width: "50px"
                    });

                    $scope.col_defs.push({
                        field: "duplicate",
                        displayName: "Duplicar",
                        levels: [1],
                        width: "50px",
                        cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Duplicar'><span class='icon mif-floppy-disk' style='color: #1b6eae; font-size: 1rem;'></span></a></div>",
                        //"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
                        cellTemplateScope: {
                            click: function (data) { // this works too: $scope.someMethod;
                                data.d = true;
                                $scope.editElement(data);
                            }
                        }
                    });

                    $scope.col_defs.push({
                        field: "",
                        displayName: "Dar baja/alta",
                        levels: [1],
                        width: "100px",
                        cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title=\"{{row.branch.Active ? 'Dar baja' : 'Dar alta'}}\"><span ng-class=\"[row.branch.Active ? 'fa fa-thumbs-down' : 'fa fa-thumbs-up']\" ng-style=\"{'color': row.branch.Active ?'red':'green'}\" style='font-size:24px;'></span></a></div>",
                        cellTemplateScope: {
                            click: function (data) {
                                $scope.setActivation(data, !data.Active);
                            }
                        }
                    });
                }

                for (var j = 0; j < $scope.col_defs.length; j++) {
                    if ($scope.col_defs[j]["width"] === undefined) {
                        $scope.tableWidth += 100;
                        continue;
                    }
                    $scope.tableWidth += 1 * $scope.col_defs[j]["width"].slice(0, $scope.col_defs[j]["width"].length - 2);
                }
            }

            if ($scope.accessLevel === null) {
                securityFact.subscribe($scope, function updateALparameter() {
                    printAccess();
                });
            }
            else
                printAccess();


        }
    ])
    .controller("NewMaxPermitedLimitController",
    [
        "$scope", "$routeParams", "$location", "$http", "$controller", "SecurityFact", "Notification", "ngDialog", "$filter",
        function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog, $filter) {
            $scope.prevIndx = 0;
            var errorDelay = 5000;
            $scope.selectedItems = new Array();
            $scope.my_tree = {};
            var searchKey = "";
            $scope.ngDialogData.viewParameters = true;
            $scope.ngDialogData.viewGroups = true;
            $scope.ngDialogData.viewPackages = true;
            $scope.isLoading = false;
            $scope.pageSize = 10;
            $scope.pageTotal = $scope.ngDialogData.total;
            $scope.currentPage = 1;
            //$scope.elementList = new Array();
            var elementList = $scope.ngDialogData.elementList;
            $scope.elementList = elementList;
            // $scope.rowActions = [];
            $scope.tableWidth = 150;
            //  $scope.ngDialogData.col_defs.splice($scope.ngDialogData.col_defs.length - 1, 1);
            $scope.ngDialogData.col_defs[1].levels.splice(1, 1);
            //angular.extend($scope.col_defs, $scope.ngDialogData.col_defs);
            // $scope.ngDialogData.col_defs[1].cellTemplate = "<div data-ng-if=\"row.branch.elemType=='parameter'\"><input  type='number' placeholder='0' style='margin-top: 10px; border: none' data-ng-model='row.branch[col.field].Value'></div>";
            $scope.rowActions = [{
                levels: [1],
                //icon: "icon-plus  glyphicon glyphicon-plus  fa fa-plus",
                cellTemplate: "<input style='margin:0' ng-click='cellTemplateScope.click(row.branch)' ng-model='row.branch.check' type='checkbox'>",
                cellTemplateScope: {
                    click: function (data) { // this works too: $scope.someMethod;
                        if (data.check === true) {
                            for (var i = 0; i < $scope.ngDialogData.elemUsedByLMP.length; i++)
                                if ($scope.ngDialogData.elemUsedByLMP[i].Id === data.Id && $scope.ngDialogData.elemUsedByLMP[i].elemType === data.elemType) {
                                    $scope.ngDialogData.elemUsedByLMP.splice(i, 1);
                                    break;
                                }
                        }
                    }
                }
            }];
            $scope.expanding_property = {
                field: "Name",
                displayName: "Clave Específica",
                sortable: true,
                filterable: true,
                width: "300px"
            };

            $scope.col_defs = new Array();
            $scope.col_defs.push({
                field: "elemType",
                displayName: "Tipo de artículo",
                levels: [1, 2, 3],
                cellTemplate: "<div class='text-uppercase'>{{(row.branch[col.field]==='group')?'Grupo':(row.branch[col.field]==='parameter')?'Parámetro':'paquete'}}</div>",
                width: "200px"
            });

            $scope.col_defs.push({
                field: "Description",
                displayName: "Descripción Específica",
                width: "400px"
            });

            $scope.col_defs.push({
                field: "MaxPermitedLimit",
                displayName: "LMP",
                width: "100px",
                tooltip: "Límite Máximo Permitido",
                levels: [1, 2, 3],
                cellTemplate: "<div data-ng-if=\"row.branch.elemType=='parameter'\"><input  type='number' style='margin-top: 10px; width:70px' data-ng-model='row.branch[col.field].Value'></div>"
            });

            $scope.col_defs.push({
                field: "MaxPermitedLimit",
                displayName: "Decimales",
                width: "100px",
                tooltip: "Decimales para el Límite Máximo Permitido",
                levels: [1, 2, 3],
                cellTemplate: "<div data-ng-if=\"row.branch.elemType=='parameter'\"><input  type='number' maxlength='8' style='margin-top: 10px; width:40px' data-ng-model='row.branch[col.field].DecimalsPoints'></div>"
            });

            $scope.col_defs.push({
                field: "Unit",
                displayName: "Unidad de Reporte",
                levels: [1, 2, 3],
                width: "200px",
                cellTemplate: "<div>{{row.branch[col.field]}}</div>"
            });

            //$scope.col_defs.push({
            //    field: "Precio",
            //    displayName: "Precio",
            //    levels: [1, 2, 3],
            //    width: "100px",
            //    cellTemplate: "<div>{{row.branch[col.field]}}</div>"
            //    //"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
            //});

            //$scope.col_defs.push({
            //    field: "AutolabAssignedAreaName",
            //    displayName: "NAAA",
            //    width: "100px",
            //    tooltip: "Nombre de Área Autolab Asignada"
            //});



            //$scope.col_defs.push({
            //    field: "CentroCosto",
            //    displayName: "CeC",
            //    tooltip: "Centro de Costo",
            //    levels: [1, 2, 3],
            //    width: "300px"
            //    // cellTemplate: "<div>{{row.branch[col.field]}}</div>"
            //});

            //$scope.col_defs.push({
            //    field: "Annalists",
            //    displayName: "Nombre del analista",
            //    cellTemplate: "<div>{{row.branch[col.field]}}</div>",
            //    levels: [1, 2, 3],
            //    width: "300px",
            //    tooltip: "Nombre del analista"
            //});

            for (var j = 0; j < $scope.col_defs.length; j++) {
                if ($scope.col_defs[j]["width"] === undefined) {
                    $scope.tableWidth += 100;
                    continue;
                }
                $scope.tableWidth += 1 * $scope.col_defs[j]["width"].slice(0, $scope.col_defs[j]["width"].length - 2);
            }

            $scope.$on('wizard:stepChanged', function (data, obj) {
                if (obj.index === 1 && $scope.prevIndx === 0) {
                    $scope.rowActions.splice(0, 1);
                    //$scope.col_defs[1].cellTemplate = "<div data-ng-if=\"row.branch.elemType=='parameter'\"><input  type='number' placeholder='0' style='margin-top: 10px; border: none' data-ng-model='row.branch[col.field].Value'></div>";
                    for (var i = 0; i < elementList.length/*$scope.ngDialogData.elementList.length*/; i++) {
                        if (elementList[i] && elementList[i].check === true /*$scope.ngDialogData.elementList[i].check === true*/) {
                            $scope.selectedItems.push(elementList[i]/*$scope.ngDialogData.elementList[i]*/);
                        }

                    }
                    for (var j = 0; j < $scope.ngDialogData.elemUsedByLMP.length; j++) {
                        var flag = true;
                        for (var k = 0; k < $scope.selectedItems.length; k++)
                            if ($scope.ngDialogData.elemUsedByLMP[j].Id === $scope.selectedItems[k].Id && $scope.ngDialogData.elemUsedByLMP[j].elemType === $scope.selectedItems[k].elemType) {
                                flag = false;
                                break;
                            }
                        if (flag) {
                            $scope.ngDialogData.elemUsedByLMP[j].check = true;
                            $scope.selectedItems.push($scope.ngDialogData.elemUsedByLMP[j]);
                        }
                    }
                } else if (obj.index === 0 && $scope.prevIndx === 1) {


                    $scope.selectedItems = [];
                    $scope.rowActions.push({
                        levels: [1],
                        //icon: "icon-plus  glyphicon glyphicon-plus  fa fa-plus",
                        cellTemplate: "<input style='margin:0' ng-click='cellTemplateScope.click(row.branch)' ng-model='row.branch.check' type='checkbox'>",
                        cellTemplateScope: {
                            click: function (data) { // this works too: $scope.someMethod;
                            }
                        }
                    });
                    //  $scope.col_defs[1].cellTemplate = "<div>{{row.branch[col.field].Value}}</div>";
                }

                $scope.prevIndx = obj.index;
            });

            $scope.dialogTitle = $scope.ngDialogData.dData.Id === undefined ?
                "Nuevo límite máximo permisible" : "Editar límite máximo permisible " + $scope.ngDialogData.dData.Name;

            var takeParamforcollection = function (selectedItems) {
                var paramArray = [];
                var jerq = [];
                var takeParam = function (elems) {
                    jerq[elems.elemType] = elems.Id;
                    if (elems.children.length === 0) {
                        if (elems.MaxPermitedLimit != null) {
                            paramArray.push({
                                //Matrix: { Id: jerq["matrix"] },
                                Package: { Id: jerq["package"] },
                                Group: { Id: jerq["group"] },
                                Parameter: { Id: jerq["parameter"] },
                                Value: elems.MaxPermitedLimit.Value,
                                DecimalsPoints: elems.MaxPermitedLimit.DecimalsPoints,
                                Id: elems.MaxPermitedLimit.Id
                            });
                        }
                        return;
                    } else {

                        for (var k = 0; k < elems.children.length; k++) {

                            if (k !== 0 && elems.children[k - 1].elemType !== elems.children[k].elemType) {
                                jerq[elems.children[k - 1].elemType] = 0;
                            }
                            takeParam(elems.children[k]);

                        }


                    }
                }
                for (var i = 0; i < selectedItems.length; i++) {
                    jerq = [];
                    takeParam(selectedItems[i]);
                }
                return paramArray;
            }

            $scope.acceptDialog = function () {
                var error = false;
                var paramRoutes = takeParamforcollection($scope.selectedItems);

                if ($scope.ngDialogData.dData.Name === undefined) {
                    notification.error({
                        message: "Nombre de Límite máximo permitido indefinido",
                        delay: errorDelay
                    });
                    error = true;
                }
                if (error === true)
                    return;

                $scope.confirm({
                    Id: $scope.ngDialogData.dData.Id != null && !$scope.ngDialogData.dData.d ?
                        $scope.ngDialogData.dData.Id : 0,
                    Name: $scope.ngDialogData.dData.Name,
                    Description: $scope.ngDialogData.dData.Description,
                    Active: $scope.ngDialogData.dData.Active,
                    ParamRoutes: paramRoutes
                });
            };


            $scope.getElem = function (pageSize, searchGeneral) {
                searchKey = searchGeneral;
                elementList = [];
                $scope.refreshElementList(1, pageSize);
            }

            $scope.refreshElementList = function (page, pageSize) {

                var id = $scope.ngDialogData.dData.Id == undefined ? 0 : $scope.ngDialogData.dData.Id;

                if (!elementList[(page - 1) * pageSize] || searchKey) {
                    $scope.isLoading = true;
                    $http.post("/maxpermitedlimit/getallelements", {
                        page: page, pageSize: pageSize, Id: id,
                        searchGeneral: searchKey,
                        viewPackages: $scope.ngDialogData.viewPackages,
                        viewGroups: $scope.ngDialogData.viewGroups,
                        viewParameters: $scope.ngDialogData.viewParameters})
                                 .success(function (response) {
                                     if (response.success) {

                                         var elements = response.elements;
                                         if ($scope.ngDialogData.dData.children) {
                                             for (var i = 0; i < elements.length; i++) {
                                                 for (var j = 0; j < $scope.ngDialogData.elemUsedByLMP.length; j++) {
                                                     if (elements[i].Id === $scope.ngDialogData.elemUsedByLMP[j].Id && elements[i].elemType === $scope.ngDialogData.elemUsedByLMP[j].elemType) {
                                                         elements[i].check = true;
                                                         break;
                                                     }
                                                 }
                                             }
                                         }

                                         for (var i = 0; i < elements.length; i++)
                                             elementList[(page - 1) * pageSize + i] = copy(elements[i]);

                                         $scope.pageTotal = response.total;

                                         var end = (page - 1) * pageSize + $scope.pageSize;
                                         $scope.elementList = elementList.slice((page - 1) * pageSize, end);
                                     }
                                     $scope.isLoading = false;
                                 })
                         .error(function () {
                             notification.error({
                                 message: "Error en la conexión con el servidor.", delay: 5000
                             });
                             $scope.isLoading = false;
                         });
                } else {
                    var end = (page - 1) * pageSize + $scope.pageSize;
                    $scope.elementList = elementList.slice((page - 1) * pageSize, end);
                }
            }


        }
    ])
.filter("getElem", function ($filter) {
    return function (data, searchGeneral, page, pageSize) {

        if (angular.isArray(data) && angular.isString(searchGeneral)) {
            var filteredData = $filter("filter")(data, { Name: searchGeneral });
            // filteredData = filteredData.splice(10, 10);

            //if (filteredData.length < data.length) {
            //    var elemCount = data.length - filteredData.length;
            //    $scope.refreshElementList($scope.actualIndex, elemCount);
            //    $scope.actualIndex += $scope.requestedElem.length;
            //    filteredData.concat($scope.requestedElem); 
            return filteredData.slice((page - 1) * pageSize, page * pageSize);
        }
        return data.slice((page - 1) * pageSize, page * pageSize);
        //return $filter("limitTo")(data, 5);

        //} else {
        //    return data;
        //}
    };
});
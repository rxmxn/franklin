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
    .controller("PackageController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', "dataProvider", "ngDialog", "SecurityFact", "generalFactory", "Notification", "counterFactory",
        function ($scope, $route, $location, $http, $cookies, $controller, dataProvider, ngDialog, securityFact, generalFactory, notification, counterFactory) {

            $scope.isFullscreen = false;
            $scope.myheight = true;

            $scope.spcSI = true; //debe estar por defecto en false. Se puso en true pq si no no sale ningun elemento.
            $scope.spcNO = true;

            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------

            //-----------función q se llama para mostrar un div en fullscreen----------------
            $scope.toggleFullScreen = function () {

                $scope.isFullscreen = !$scope.isFullscreen;
                if ($scope.accessLevel === 1) {
                    if ($scope.isFullscreen)
                        $scope.col_defs.splice($scope.col_defs.length - 3, 3);
                    else {
                        $scope.col_defs.push({
                            field: "edit",
                            displayName: "Editar",
                            levels: [1, 2],
                            width: "50px",
                            cellTemplate: "<a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Editar'><span class='icon mif-pencil' style='color: #1b6eae; font-size: 1rem;'></span></a>",
                            //"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
                            cellTemplateScope: {
                                click: function (data) { // this works too: $scope.someMethod;
                                    data.d = false;
                                    $scope.editElement(data);
                                }
                            }
                        });

                        $scope.col_defs.push({
                            field: "duplicate",
                            displayName: "Duplicar",
                            levels: [1, 2],
                            width: "50px",
                            cellTemplate: "<a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Duplicar'><span class='icon mif-floppy-disk' style='color: #1b6eae; font-size: 1rem;'></span></a>",
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
                            cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title=\"{{row.branch.Active ? 'Dar baja' : 'Dar alta'}}\"><span ng-class=\"[row.branch.Active ? 'fa fa-thumbs-down' : 'fa fa-thumbs-up']\" ng-style=\"{'color': row.branch.Active ?'red':'green'}\" style='font-size:24px;'></span></a></div>",
                            cellTemplateScope: {
                                click: function (data) {
                                    $scope.setActivation(data, !data.Active);
                                }
                            },
                            width: "50px"
                        });
                    }
                }
            }
            //---------------------------------------------------------------------------------

            var filters = function (page) {
                return {
                    page: (page === null || page === undefined) ? $scope.currentPage : page,
                    pageSize: $scope.pageSize,
                    // searchGeneral: $scope.searchGeneral,
                    alta: $scope.alta,
                    baja: $scope.baja,
                    spcSi: $scope.spcSI,
                    spcNo: $scope.spcNO,
                    searchGeneral: $scope.searchGenericKey,
                    searchDescription: $scope.searchGenericDescription,
                    sucId: $scope.sucursalId,
                    bLine: $scope.bLine
                }
            };

            var getFilters = function () {
                $http.post("/package/getFilters")
                    .success(function (response) {
                        if (response.success) {
                            $scope.sucursalList = response.sucList;
                            $scope.BusinessLineList = response.marketList;

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
                    default:
                        return "";
                }

            }

            var saveObject = function (promise) {
                if (promise.elemType === "package") {
                    return generalFactory.getPackageObject(promise);
                } else if (promise.elemType === "group") {
                    return generalFactory.getGroupObject(promise);
                } else {
                    return generalFactory.getParamObject(promise);
                }
            }

            var save = function (promise) {
                $scope.initSaving = true;

                $http.post(address(promise.elemType), saveObject(promise))
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
                            message: "Se obtuvo un error del servidor. Ver detalles.",
                            delay: 5000
                        });
                        $scope.initSaving = false;
                    });
            }

            $scope.editElement = function (data) {

                var tElement = {};
                angular.extend(tElement, data);
                $("#loadingSpinners").show();

                var id = (data === null) ? 0 : data.Id;
                //$http.post("/package/getaccessories", { id: id })
                var actionpost = (data != null && data.elemType === "parameter") ? "/param/GetRootPrintResults" : (data != null && data.elemType === "group") ? "/group/getparamsforgroup" : "/package/getaccessories";
                $http.post(actionpost, { id: id })
                            .success(function (response) {
                                if (response.success) {
                                    if (data != null && data.elemType === "package") {
                                        var paramList = response.paramList;
                                        var groupList = response.groupList;
                                        for (var i = 0; i < paramList.length; i++) {
                                            for (var j = 0; j < data.children.length; j++) {
                                                if (paramList[i].Id === data.children[j].Id && paramList[i].elemType === data.children[j].elemType) {
                                                    paramList[i].check = true;
                                                    break;
                                                }
                                            }
                                        }
                                        for (i = 0; i < groupList.length; i++) {
                                            for (j = 0; j < data.children.length; j++) {
                                                if (groupList[i].Id === data.children[j].Id && groupList[i].elemType === data.children[j].elemType) {
                                                    groupList[i].check = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else if (data != null && data.elemType === "group") {
                                        var paramList = response.paramList;
                                        for (var i = 0; i < paramList.length; i++) {
                                            if (paramList[i].ParamPrintResults == null)
                                                paramList[i].ParamPrintResults = { Id: 0, Yes: false, }
                                            for (var j = 0; j < data.children.length; j++) {
                                                if (paramList[i].Id === data.children[j].Id && paramList[i].elemType === data.children[j].elemType) {
                                                    paramList[i].check = true;
                                                    break;
                                                }
                                            }
                                        }
                                    } else if (data != null && data.elemType === "parameter") {
                                        response.paramPrintResult === null ? tElement.ParamPrintResults = { Id: 0, Yes: false } : tElement.ParamPrintResults = { Id: response.paramPrintResult.Id, Yes: response.paramPrintResult.Yes }
                                        tElement.DecimalesReporte = response.decimalReport;
                                    } else {
                                        var paramList = response.paramList;
                                        var groupList = response.groupList;
                                    }


                                    if (data != null) {

                                    }
                                    var template = (id === 0 || data.elemType === "package") ? "Shared/EditPackage" : (data.elemType === "group") ? "Shared/EditGroup" : "Shared/EditParam";
                                    var dialogController = (id === 0 || data.elemType === "package") ? "NewPackageController" : (data.elemType === "group") ? "NewGroupController" : "NewParamController";

                                    var colDef = copy($scope.col_defs);


                                    ngDialog.openConfirm({
                                        template: template,
                                        controller: dialogController,
                                        className: "ngdialog-theme-default ngdialog-width2000",
                                        preCloseCallback: "preCloseCallbackOnScope",
                                        data: {
                                            dData: tElement,
                                            expanding_property: $scope.expanding_property,
                                            col_defs: colDef,
                                            paramList: paramList,
                                            pTotal: response.pTotal,
                                            groupList: groupList,
                                            gTotal: response.gTotal,
                                            maxpermitedlimitList: response.PermitedLimits
                                        },
                                        closeByEscape: true
                                    })

                                    .then(function (promise) {
                                        //Accepting
                                        save(promise);
                                        getFilters();
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
                "icon mif-suitcase", "Name",
                "/package/refreshlist", filters,
                "/package/saveactivestatus", setActivationObjects,
                "/package/savepackage", saveObject,
                null,
				"package"
                );

            // Package specifics
            $scope.FilterGeneral = true;
            $scope.FilterActive = true;
            $scope.FilterSoloParaCotizar = true;
            $scope.SucursalFilterActive = true;
            $scope.ParamFilterActive = true;
            $scope.BusinessLineFilterActive = true;
            //$scope.boxHeaderTitleActive = "Paquetes activos";
            //$scope.boxHeaderTitleDeactive = "Paquetes de baja";

            $scope.panels = [
                {
                    index: 0,
                    name: "FILTROS",
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: "TABLA DE PAQUETES",
                    state: true,
                    size: 9
                }
            ];

            //to configure tree data

            //$scope.f = function () {
            //    var index = $scope.col_defs.length - 1;
            //    $scope.col_defs[index].displayName = row.branch.Active ? "Dar Alta" : "Dar Baja";
            //    $scope.col_defs[index].cellTemplate = row.branch.Active ? "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Dar alta'><span class='fa fa-thumbs-up' style='color:#1b6eae; font-size: 1rem;'></span></a></div>" :
            //        "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Dar baja'><span class='fa fa-thumbs-down' style='color: #1b6eae; font-size: 1rem;'></span></a></div>";
            //    return;
            //}

            $scope.my_tree = {};
            $scope.tableWidht = 150;

            $scope.expanding_property = {
                field: "Name",
                displayName: "Clave Específica",
                sortable: true,
                filterable: true,
                width: "200px"
            };

            $scope.col_defs = new Array();

            $scope.col_defs.push({
                field: "elemType",
                displayName: "Tipo de artículo",
                levels: [1, 2, 3],
                cellTemplate: "<div class='text-uppercase'>{{(row.branch[col.field]==='package')?'Paquete':(row.branch[col.field]==='group')?'Grupo':'Parámetro'}}</div>",
                width: "150px"
            });

            $scope.col_defs.push({
                field: "Description",
                //levels: [1, 2],
                displayName: "Descripción Específica",
                width: "170px"
            });

            // Este no esta en CAT PARAMETROS, pero creo que es un elemento importante para grupos.
            $scope.col_defs.push({
                field: "CantidadMuestreos",
                displayName: "Cantidad de Muestreos",
                levels: [3],
                cellTemplate: "<div>{{row.branch[col.field]}}</div>",
                tooltip: "Cantidad de Muestreos (Muestreos Compuestos)",
                width: "150px"
            });

            $scope.col_defs.push({
                field: "Matrixes",
                displayName: "Matriz",
                levels: [1, 2, 3],
                cellTemplate: "<div><span style='cursor:pointer; font-size:1.2rem' ng-click='cellTemplateScope.click(row.branch)' class='icon mif-windy4'></span></div>",
                cellTemplateScope: {
                    click: function (data) {

                        var addr = (data.elemType === "package") ? "/package/getMatrixes" : (data.elemType === "group") ? "/group/getMatrixes" : "/param/getMatrixes";

                        $http.post(addr, { Id: data.Id })
                         .success(function (response) {
                             if (response.success) {

                                 ngDialog.openConfirm({
                                     template: "Shared/ShowMatrixesDlg",
                                     controller: "dlgController",
                                     className: "ngdialog-theme-default ngdialog-width1000",
                                     preCloseCallback: "preCloseCallbackOnScope",
                                     data: { dData: response.Matrixes },
                                     closeByEscape: true
                                 })
                            .then(function (promise) {
                                //Accepting

                            }, function (reason) {
                                //Rejecting

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
                },
                width: "50px"

            });

            $scope.col_defs.push({
                field: "Matrixes",
                displayName: "Línea de Negocio", //mercado
                levels: [1, 2, 3],
                cellTemplate: "<div  class='text-uppercase'>{{row.branch[col.field][0].BaseMatrix.Mercado}}</div>",
                width: "150px"
            });

            $scope.col_defs.push({
                field: "Precio",
                displayName: "Precio de Lista",
                levels: [1, 2, 3],
                cellTemplate: "<div style='text-align: center;'>{{row.branch[col.field].Value}}({{row.branch[col.field].Currency.Name}})</div>",
                //"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
                width: "70px"
            });

            $scope.col_defs.push({
                field: "Active",
                displayName: "Estatus",
                levels: [1, 2, 3],
                cellTemplate: "<div style='text-align: center;'>{{row.branch[col.field] === false ? 'BAJA' : 'ALTA'}}</div>",
                width: "50px"
            });

            $scope.col_defs.push({
                field: "Norms",
                displayName: "Normas",
                tooltip: "Norma",
                levels: [1],
                cellTemplate: "<div><span ng-repeat='n in row.branch[col.field]'><a href='' ng-click='cellTemplateScope.click(n)'> {{n.Name}}{{row.branch[col.field][row.branch[col.field].length -1].Name !== n.Name ? ',' : ''}}</a></span></div>",
                cellTemplateScope: {
                    click: function (norm) {
                        ngDialog.openConfirm({
                            template: "Shared/ShowNormInfo",
                            controller: "dlgController",
                            className: "ngdialog-theme-default ngdialog-width1000",
                            preCloseCallback: "preCloseCallbackOnScope",
                            data: { dData: norm },
                            closeByEscape: true
                        })
                       .then(function (promise) {
                           //Accepting
                       }, function (reason) {
                           //Rejecting
                       });
                    }
                },
                width: "150px"
            });

            //$scope.col_defs.push({
            //    field: "MaxPermitedLimit",
            //    displayName: "LMP",
            //    tooltip: "Límite Máximo Permitido",
            //    levels: [1, 2, 3],
            //    cellTemplate: "<div><span style='cursor:pointer; font-size:1.2rem' ng-if=\"row.branch.elemType=='parameter'\" ng-click='cellTemplateScope.click(row.branch)' class='mif-thermometer2 icon'></span></div>",
            //    cellTemplateScope: {
            //        click: function (data) {

            //            $http.post("/param/getallmaxpermitedlimit", { Id: data.Id })
            //             .success(function (response) {
            //                 if (response.success) {

            //                     ngDialog.openConfirm({
            //                         template: "Shared/ShowLMP",
            //                         controller: "SetLMPDialogController",
            //                         className: "ngdialog-theme-default",
            //                         preCloseCallback: "preCloseCallbackOnScope",
            //                         data: { dData: response.LMP, paramKey: data.ParamUniquekey },
            //                         closeByEscape: true
            //                     })
            //                .then(function (promise) {
            //                    //Accepting

            //                }, function (reason) {
            //                    //Rejecting

            //                });


            //                 }
            //                 $scope.isLoading = false;
            //             })
            //     .error(function () {
            //         notification.error({
            //             message: "Error en la conexión con el servidor.", delay: 5000
            //         });
            //         $scope.isLoading = false;
            //     });

            //        }
            //    },
            //    width: "50px"
            //});

            //$scope.col_defs.push({
            //    field: "DecimalesReporte",
            //    displayName: "Decimales Reporte",
            //    tooltip: "Decimales para Reporte",
            //    levels: [1, 2, 3],
            //    cellTemplate: "<div style='text-align: center;'>{{row.branch[col.field]===null?'N/A':row.branch[col.field]}}</div>",
            //    width: "90px"

            //});

            $scope.col_defs.push({
                field: "PublishInAutolab",
                displayName: "Solo para cotizar",
                levels: [1, 2, 3],
                cellTemplate: "<div>{{row.branch[col.field] === false ? 'Si' : 'No'}}</div>",
                tooltip: "Solo para cotizar",
                width: "90px"
            });

            //$scope.col_defs.push({
            //    field: "CuentaEstadistica",
            //    displayName: "CPE",
            //    levels: [1, 2, 3],
            //    cellTemplate: "<div>{{row.branch[col.field] === true ? 'Si' : 'No'}}</div>",
            //    tooltip: "Cuenta para Estadística",
            //    width: "50px"
            //});

            $scope.col_defs.push({
                field: "SellSeparated",
                displayName: "SVS",
                levels: [1, 2, 3],
                cellTemplate: "<div ng-style=\"{'background-color': (row.branch[col.field] === true) ? 'transparent' : 'red'}\">{{row.branch[col.field] === true ? 'Si' : 'No se vende por separado'}}</div>",
                tooltip: "Se vende por separado",
                width: "150px"
            });

            $scope.col_defs.push({
                field: "Sucursal",
                displayName: "Sucursal/Instalación",
                levels: [1, 2],
                width: "150px",
                tooltip: "Sucursal/Instalación del Grupo",
                cellTemplate: "<div>{{((row.branch.elemType==='group' || row.branch.elemType==='package') && row.branch[col.field].Name)? row.branch[col.field].Name + '/' + row.branch[col.field].Region.Name: row.branch.elemType==='parameter'?'':'N/A'}}</div>"
            });

            for (var j = 0; j < $scope.col_defs.length; j++) {
                if ($scope.col_defs[j]["width"] === undefined) {
                    $scope.tableWidht += 100;
                    continue;
                }
                $scope.tableWidht += 1 * $scope.col_defs[j]["width"].slice(0, $scope.col_defs[j]["width"].length - 2);
            }

            var printAccess = function () {
                if ($scope.accessLevel === 1) {
                    $scope.col_defs.push({
                        field: "edit",
                        displayName: "Editar",
                        levels: [1, 2, 3],
                        width: "50px",
                        cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title='Editar'><span class='icon mif-pencil' style='color: #1b6eae; font-size: 1rem;'></span></a></div>",
                        //"<input ng-click='cellTemplateScope.click(row.branch[col.field])' type='checkbox'data-placement='right'>"
                        cellTemplateScope: {
                            click: function (data) { // this works too: $scope.someMethod;
                                data.d = false;
                                $scope.editElement(data);
                            }
                        }
                    });
                    $scope.tableWidht += 50;

                    $scope.col_defs.push({
                        field: "duplicate",
                        displayName: "Duplicar",
                        levels: [1, 2, 3],
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
                    $scope.tableWidht += 50;

                    $scope.col_defs.push({
                        field: "",
                        displayName: "Dar baja/alta",
                        levels: [1],
                        width: "50px",
                        cellTemplate: "<div style='text-align:center'><a href='' data-ng-click='cellTemplateScope.click(row.branch)' data-toggle='tooltip' title=\"{{row.branch.Active ? 'Dar baja' : 'Dar alta'}}\"><span ng-class=\"[row.branch.Active ? 'fa fa-thumbs-down' : 'fa fa-thumbs-up']\" ng-style=\"{'color': row.branch.Active ?'red':'green'}\" style='font-size:24px;'></span></a></div>",
                        cellTemplateScope: {
                            click: function (data) {
                                $scope.setActivation(data, !data.Active);
                            }
                        }
                    });
                    $scope.tableWidht += 50;
                }

            }

            if ($scope.accessLevel === null) {
                securityFact.subscribe($scope, function updateALpckg() {
                    printAccess();
                });
            }
            else
                printAccess();

            //$scope.my_tree_handler = function (branch) {
            //	//console.log("you clicked on", branch);
            //}
        }
    ])
    .controller("NewPackageController",
    [
        "$scope", "$routeParams", "$location", "$http", "$controller", "SecurityFact", "Notification", "ngDialog", "generalFactory",
        function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog, generalFactory) {

            var errorDelay = 5000;
            $scope.pTotal = $scope.ngDialogData.pTotal;
            $scope.gTotal = $scope.ngDialogData.gTotal;
            var paramlist = $scope.ngDialogData.paramList;
            var groupList = $scope.ngDialogData.groupList;
            $scope.paramList = paramlist;
            $scope.groupList = groupList;
            $scope.pageSize = 10;
            $scope.currentPage = 1;
            $scope.gcurrentPage = 1;
            // Checking Column List
            $scope.columnList = securityFact.getColumnList();
            var searchParamKey = "";
            var searchGroupKey = "";
            $scope.ngDialogData.dData.Parameters = [];
            $scope.ngDialogData.dData.Groups = [];

            if ($scope.ngDialogData.dData.children) {
                for (var l = 0; l < $scope.ngDialogData.dData.children.length; l++) {
                    if ($scope.ngDialogData.dData.children[l].elemType === 'parameter') {
                        $scope.ngDialogData.dData.Parameters.push($scope.ngDialogData.dData.children[l]);
                    } else {
                        $scope.ngDialogData.dData.Groups.push($scope.ngDialogData.dData.children[l]);
                    }
                }
            }

            if ($scope.columnList === undefined) {
                securityFact.subscribe($scope, function updateCLparam() {
                    $scope.columnList = securityFact.getColumnList();
                });
            }

            function getTiposServicio() {
                generalFactory.getAll('/tiposervicio/RefreshList', { activeOption: true })
                    .then(function (response) {
                        if (response.success) {
                            $scope.tiposServicio = response.elements;
                        } else {
                            notification.error({
                                message: "Error al recuperar tipos de servicio.",
                                delay: errorDelay
                            });
                        }
                    });
            }

            getTiposServicio();

            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ? "Nuevo Paquete" : "Editar Paquete " + $scope.ngDialogData.dData.Name;
            $scope.tableWidht = 170;
            $scope.ngDialogData.col_defs.splice($scope.ngDialogData.col_defs.length - 3, 3);

            $scope.rowActions = [{
                levels: [1],
                //icon: "icon-plus  glyphicon glyphicon-plus  fa fa-plus",
                cellTemplate: "<input style='margin:0' ng-click='cellTemplateScope.click(row.branch)' ng-model='row.branch.check' type='checkbox'>",
                cellTemplateScope: {
                    click: function (data) { // this works too: $scope.someMethod;

                    }
                }
            }];

            for (var j = 0; j < $scope.ngDialogData.col_defs.length; j++) {
                if ($scope.ngDialogData.col_defs[j]["width"] === undefined) {
                    $scope.tableWidht += 100;
                    continue;
                }
                $scope.tableWidht += 1 * $scope.ngDialogData.col_defs[j]["width"].slice(0, $scope.ngDialogData.col_defs[j]["width"].length - 2);
            }
            //-----------------------rellenar combo de matrices y matrices bases-----------------------------------------------
            $scope.mtrxChecked = function (mtrx) {
                if (mtrx.check) {
                    for (var i = 0; i < $scope.ngDialogData.dData.Matrixes.length; i++) {
                        if (mtrx.Id === $scope.ngDialogData.dData.Matrixes[i].Id) {
                            $scope.ngDialogData.dData.Matrixes.splice(i, 1);
                            break;
                        }
                    }
                    return;
                }
                if ($scope.ngDialogData.dData.Matrixes === undefined)
                    $scope.ngDialogData.dData.Matrixes = [];
                $scope.ngDialogData.dData.Matrixes.push({ Id: mtrx.Id });
                return;
            }

            function getMatrixes() {
                $http.post("/param/getbasematrix", { active: true })
                    .success(function (response) {
                        if (response.success === true) {
                            $scope.baseMatrixList = response.baseMatrix;

                            if ($scope.ngDialogData.dData.Matrixes) {
                                var j = 0;
                                for (var i = 0; i < $scope.ngDialogData.dData.Matrixes.length; i++) {
                                    for (; j < $scope.baseMatrixList.length; j++) {
                                        if ($scope.ngDialogData.dData.Matrixes[i].BaseMatrix.Id === $scope.baseMatrixList[j].Id) {
                                            $scope.ngDialogData.BaseMatrix = $scope.baseMatrixList[j];
                                            j = $scope.baseMatrixList.length;
                                            break;
                                        }
                                    }
                                    for (var k = 0; k < $scope.ngDialogData.BaseMatrix.Matrixes.length; k++) {
                                        if ($scope.ngDialogData.BaseMatrix.Matrixes[k].Id === $scope.ngDialogData.dData.Matrixes[i].Id) {
                                            $scope.ngDialogData.BaseMatrix.Matrixes[k].check = true;
                                            break;
                                        }
                                    }
                                }

                            }
                        } else {
                            notification.error({
                                message: "Error al recuperar datos.",
                                delay: errorDelay
                            });
                        }
                    })
                    .error(function () {
                        notification.error({
                            message: "Error en la conexión con el servidor.",
                            delay: errorDelay
                        });
                    });
            }
            getMatrixes();
            //------------------------------------------------------------------------------------------------------

            $scope.getParamPage = function (page, pagesize) {

                if (!paramlist[(page - 1) * pagesize] || searchParamKey) {

                    $scope.isloading = true;
                    $http.post("/package/getparampage", { id: 0, page: page, pagesize: pagesize, searchgeneral: searchParamKey })
                    .success(function (response) {
                        if (response.success) {
                            var params = response.paramList;
                            for (var i = 0; i < params.length; i++) {
                                if (params[i].paramprintresults == null)
                                    params[i].paramprintresults = { id: 0, yes: false }
                                if ($scope.ngDialogData.dData.children)
                                    for (var j = 0; j < $scope.ngDialogData.dData.children.length; j++) {
                                        if (params[i].id === $scope.ngDialogData.dData.children[j].id) {
                                            params[i].check = true;
                                            break;
                                        }
                                    }
                            }

                            for (i = 0; i < params.length ; i++)
                                paramlist[(page - 1) * pagesize + i] = copy(params[i]);

                            $scope.pTotal = response.pTotal;

                            var end = (page - 1) * pagesize + $scope.pageSize;
                            $scope.paramList = paramlist.slice((page - 1) * pagesize, end);

                        } else {
                            notification.error({
                                message: "error al recuperar los datos. " + response.err,
                                delay: 5000
                            });
                        }
                        $scope.isloading = false;
                    })
                        .error(function () {
                            notification.error({
                                message: "error en la conexión con el servidor.",
                                delay: errordelay
                            });
                            $scope.isloading = false;
                        });
                } else {
                    var end = (page - 1) * pagesize + $scope.pageSize;
                    $scope.paramList = paramlist.slice((page - 1) * pagesize, end);
                }
            }

            $scope.searchpfunction = function (searchGeneral) {
                searchParamKey = searchGeneral;
                paramlist = [];
                $scope.getParamPage(1, $scope.pageSize);
            }

            //------------------------------------------------------------------------------------------------------

            $scope.getGroupPage = function (page, pageSize) {

                if (!groupList[(page - 1) * pageSize] || searchGroupKey) {

                    $scope.isloading = true;
                    $http.post("/package/getgrouppage", { page: page, pageSize: pageSize, searchgeneral: searchGroupKey })
                    .success(function (response) {
                        if (response.success) {
                            var groups = response.groupList;
                            for (var i = 0; i < groups.length; i++) {
                                if ($scope.ngDialogData.dData.children)
                                    for (var j = 0; j < $scope.ngDialogData.dData.children.length; j++) {
                                        if (groups[i].id === $scope.ngDialogData.dData.children[j].id && $scope.ngDialogData.dData.children[j].elemType === "group") {
                                            groups[i].check = true;
                                            break;
                                        }
                                    }
                            }

                            for (i = 0; i < groups.length ; i++)
                                groupList[(page - 1) * pageSize + i] = copy(groups[i]);

                            $scope.gTotal = response.gTotal;

                            var end = (page - 1) * pageSize + pageSize;
                            $scope.groupList = groupList.slice((page - 1) * pageSize, end);

                        } else {
                            notification.error({
                                message: "error al recuperar los datos. " + response.err,
                                delay: 5000
                            });
                        }
                        $scope.isloading = false;
                    })
                        .error(function () {
                            notification.error({
                                message: "error en la conexión con el servidor.",
                                delay: errordelay
                            });
                            $scope.isloading = false;
                        });
                } else {
                    var end = (page - 1) * pageSize + pageSize;
                    $scope.groupList = groupList.slice((page - 1) * pageSize, end);
                }
            }

            $scope.searchGfunction = function (searchGeneral) {
                searchGroupKey = searchGeneral;
                groupList = [];
                $scope.getGroupPage(1, $scope.pageSize);
            }

            //------------------------------------------------------------------------------------------------------
            $scope.currentStep = null;

            $scope.nextPage = function () {
                if ($scope.currentStep === "page4" && $scope.ngDialogData.BaseMatrix) {

                    $http.post('/group/GetSucursales', { bMatrixId: $scope.ngDialogData.BaseMatrix.Id })
					.success(function (response) {
					    if (response.success === true) {
					        $scope.sucursales = response.elements;
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
            }
            //-----------------------------------rellenar tabla de normas-------------------
            function getNorms() {
                generalFactory.getAll('/norm/refreshList', { page: 1, pageSize: 100 }).then(function (response) {
                    if (response.success) {
                        $scope.norms = response.elements;

                        if ($scope.ngDialogData.dData.Id !== undefined)
                            for (var i = 0; i < $scope.ngDialogData.dData.Norms.length; i++) {
                                for (var j = 0; j < $scope.norms.length; j++) {
                                    if ($scope.norms[j].checked) {
                                        continue;
                                    }
                                    if ($scope.norms[j].Id === $scope.ngDialogData.dData.Norms[i].Id) {
                                        $scope.norms[j].checked = true;
                                        break;
                                    }
                                    else
                                        $scope.norms[j].checked = false;
                                }
                            }

                    } else {
                        notification.error({
                            message: "Error al recuperar métodos.",
                            delay: 5000
                        });
                    }
                });
            }

            getNorms();

            //----------------------------------------------------------------------------
            // Rellenar combo de normas
            //function getNorms() {
            //    generalFactory.getAll('/package/getnorms', { active: true }).then(function (response) {
            //        if (response.success) {
            //            $scope.norms = response.elements;
            //        } else {
            //            notification.error({
            //                message: "Error al recuperar métodos.",
            //                delay: 5000
            //            });
            //        }
            //    });
            //}

            //getNorms();

            // TODO: modificar para que ademas de mostrar la notificacion ocurra alguna sennalizacion en el input
            $scope.acceptDialog = function () {
                var error = false;
                var normList = new Array();
                for (var i = 0; i < $scope.norms.length; i++) {
                    if (!$scope.norms[i].checked) {
                        continue;
                    }
                    normList.push({ Id: $scope.norms[i].Id });

                }

                var parameters = [];
                var groups = [];

                for (i = 0; i < $scope.ngDialogData.paramList.length; i++)
                    if ($scope.ngDialogData.paramList[i].check)
                        parameters.push($scope.ngDialogData.paramList[i]);

                for (i = 0; i < $scope.ngDialogData.groupList.length; i++)
                    if ($scope.ngDialogData.groupList[i].check)
                        groups.push($scope.ngDialogData.groupList[i]);

                if ($scope.ngDialogData.dData.Name === undefined) {
                    notification.error({
                        message: "Nombre del paquete indefinido",
                        delay: errorDelay
                    });
                    error = true;
                }

                if ($scope.ngDialogData.dData.Sucursal === null) {
                    notification.error({
                        message: "Instalación/Sucursal indefinida",
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
                    Active: $scope.ngDialogData.dData.Active,
                    MaxPermitedLimit: $scope.ngDialogData.dData.MaxPermitedLimit,
                    Description: $scope.ngDialogData.dData.Description,
                    // Norm: $scope.ngDialogData.dData.Norm,
                    Norms: normList,
                    Parameters: parameters,
                    Groups: groups,
                    ParamPrintResults: $scope.ngDialogData.dData.ParamPrintResults,
                    TipoServicio: $scope.ngDialogData.dData.TipoServicio,
                    SellSeparated: $scope.ngDialogData.dData.SellSeparated,
                    CuentaEstadistica: $scope.ngDialogData.dData.CuentaEstadistica,
                    DecimalesReporte: $scope.ngDialogData.dData.DecimalesReporte,
                    Matrixes: $scope.ngDialogData.dData.Matrixes,
                    elemType: "package",
                    Sucursal: $scope.ngDialogData.dData.Sucursal
                });

            }
        }

    ])

.directive('editPackageSelectDirective', [function () {
    return {
        link: function (scope) {

            scope.$watch('ngDialogData.dData.Norm.Id', function () {
                if (scope.ngDialogData.dData.Norm !== undefined && scope.norms !== undefined) {

                    for (var i = 0; i < scope.norms.length; i++) {
                        if (scope.norms[i].Id === scope.ngDialogData.dData.Norm.Id) {
                            scope.ngDialogData.dData.Norm.Name = scope.norms[i].Name;
                            scope.ngDialogData.dData.Norm.Description = scope.norms[i].Description;
                            break;
                        }
                    }

                }
            });
        }
    };
}])

;
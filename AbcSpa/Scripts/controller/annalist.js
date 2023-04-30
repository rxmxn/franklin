"use strict";

angular.module("abcadmin")
    .controller("AnnalistController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', "RecService", "Notification",
        function ($scope, $route, $location, $http, $cookies, $controller, recService, notification) {

            var errorDelay = 5000;

            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------

            var filters = function (page) {
                return {
                    page: (page === null || page === undefined) ? $scope.currentPage : page,
                    pageSize: $scope.pageSize,
                    alta: $scope.alta,
                    baja: $scope.baja,
                    signatario: $scope.viewSignatario
                }
            };

            var setActivationObjects = function (eObj, act) {
                return { id: eObj.Id, active: act }
            };

            var saveObject = function (promise) {
                var annalistObj = {
                    Id: promise.Id,
                    Name: promise.Name,
                    LastNameFather: promise.LastNameFather,
                    LastNameMother: promise.LastNameMother,
                    Key: promise.Key,
                    Phone: promise.Phone,
                    Email: promise.Email,
                    Active: promise.Active,
                    Description: promise.Description,
                    Photo: promise.Photo,
                    Gender: promise.Gender,
                    Sucursales: promise.Sucursales,
                    Firma: promise.Firma,
                    Curriculum: promise.Curriculum,
                    FechaAlta: promise.FechaAlta,
                    NoEmpleado: promise.NoEmpleado,
                    Puesto: promise.Puesto
                };
                return { annalist: annalistObj }
            }

            var editElement = function (element) {
                return {
                    template: "editDialog",
                    controller: "NewAnnalistController",
                    className: "ngdialog-theme-default ngdialog-width700",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }

            $scope.mainFunction(
                "icon mif-user", "Name",
                "/annalist/refreshlist", filters,
                "/annalist/saveactivestatus", setActivationObjects,
                "/annalist/saveannalist", saveObject,
                editElement,
				"annalist"
                );

            // Annalist specifics
            $scope.FilterGeneral = true;
            $scope.FilterActive = true;
            $scope.SignatariosFilterActive = true;
            //$scope.boxHeaderTitleActive = "Analistas activos";
            //$scope.boxHeaderTitleDeactive = "Analistas de baja";
            //$scope.updateFromIntelesis = false;

            $scope.panels = [
                {
                    index: 0,
                    name: 'Filtros',
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: "Tabla de Analistas",
                    state: true,
                    size: 9
                }
            ];

            $scope.viewRecAdqs = function (an) {
                recService.viewRecAdqs(an);
            }

            $scope.updateFromIntelesis = function () {
                $http.post('/annalist/UpdateFromIntelesis')
					.success(function (response) {
					    if (response.success === true) {
					        $scope.elementsList = response.elements;
					    } else {
					        notification.error({
					            message: 'Error al recuperar datos.',
					            delay: errorDelay
					        });
					    }
					})
					.error(function () {
					    notification.error({
					        message: 'Error en la conexión con el servidor.',
					        delay: errorDelay
					    });
					});
            }

            $scope.getFile = function (filePath) {
                var a = 0;
                window.open(filePath, "newtab");
            }

        }
    ])
    .controller("NewAnnalistController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog", "ImageUploadService",
        function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog, imageUploadService) {

            var errorDelay = 5000;

            $scope.avatarIMG = "/Content/img/usermale.jpg";

            $scope.SexoChange = function () {
                if (($scope.ngDialogData.dData.Photo === null) || ($scope.ngDialogData.dData.Photo === undefined)) {
                    if ($scope.ngDialogData.dData.Gender === true) {
                        $scope.avatarIMG = "/Content/img/female.jpg";
                    } else {
                        $scope.avatarIMG = "/Content/img/usermale.jpg";
                    }
                }
            };
            $scope.SexoChange();

            $scope.selectAvatarforUpload = function (file) {
                $scope.cleanAvatar();
                imageUploadService.FileToUpload(file).then(function (response) {
                    if (response.success) {
                        $scope.ngDialogData.dData.Photo = response.fileName;
                    } else {
                        notification.error({
                            message: "Error al recuperar fichero.",
                            delay: 5000
                        });
                    }
                });
            };

            //-------------------------------------------------------
            $scope.cleanAvatar = function () {
                //var nombre = $scope.newEmployee.Photo.split('/');
                //console.log($scope.ngDialogData.dData.Photo);
                //$http.post('/user/removeimage', {name: nombre[nombre.length-1]});
                $scope.ngDialogData.dData.Photo = null;
            };

            //-------------------------------------------------------
            $scope.selectFileforUpload = function (file, field) {
                $scope.cleanFile(file[0].name);
                imageUploadService.FileToUpload(file).then(function (response) {
                    if (response.success) {
                        // $scope.ngDialogData.dData.Firma = response.fileName;
                        $scope.ngDialogData.dData[field] = response.fileName;
                    } else {
                        notification.error({
                            message: "Error al recuperar fichero.",
                            delay: 5000
                        });
                    }
                });
            };

            //-------------------------------------------------------
            $scope.cleanFile = function (name, field) {
                if ($scope.ngDialogData.dData[field]) {
                    //var name = $scope.ngDialogData.dData.Firma.slice(17, $scope.ngDialogData.dData.Firma.length);
                    $http.post('/files/removeimage', { name: name });
                    $scope.ngDialogData.dData[field] = null;
                }
            };

           function getAnnalistSucursal(active) {
                var id = $scope.ngDialogData.dData.Id === undefined ? 0 : $scope.ngDialogData.dData.Id;
                $http.post('/annalist/GetAnnalistSucursal', { annalistId: id, active: active })
					.success(function (response) {
					    if (response.success === true) {
					        $scope.sucursales = response.elements;
					    } else {
					        notification.error({
					            message: 'Error al recuperar datos.',
					            delay: errorDelay
					        });
					    }
					})
					.error(function () {
					    notification.error({
					        message: 'Error en la conexión con el servidor.',
					        delay: errorDelay
					    });
					});
            }

            getAnnalistSucursal(true);

            $scope.regionChecked = function (r) {
                r.check = !r.check;
            }

            $scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nuevo analista" : "Editar analista " + $scope.ngDialogData.dData.Key;

            // TODO: modificar para que ademas de mostrar la notificacion ocurra alguna sennalizacion en el input
            $scope.acceptDialog = function () {
                var error = false;

                if ($scope.ngDialogData.dData.Name === undefined) {
                    notification.error({
                        message: "Nombre del analista indefinido",
                        delay: errorDelay
                    });
                    error = true;
                }
                if ($scope.ngDialogData.dData.Key === undefined) {
                    notification.error({
                        message: "Clave del analista indefinida",
                        delay: errorDelay
                    });
                    error = true;
                }
                var phoneregex = /\(?([0-9]{3})\)?([ .-]?)([0-9]{3})\2([0-9]{4})/;
                if ($scope.ngDialogData.dData.Phone !== undefined &&
					!phoneregex.test($scope.ngDialogData.dData.Phone)) {
                    notification.error({
                        message: 'El teléfono del analista presenta error en el formato',
                        delay: errorDelay
                    });
                    error = true;
                }
                var emailregex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
                if ($scope.ngDialogData.dData.Email !== undefined &&
					!emailregex.test($scope.ngDialogData.dData.Email)) {
                    notification.error({
                        message: 'El email del analista presenta error en el formato',
                        delay: errorDelay
                    });
                    error = true;
                }

                var routeSucursal = new Array();

                for (var i = 0; i < $scope.sucursales.length; i++) {
                    if ($scope.sucursales[i].check === true) {
                        routeSucursal.push({ 'Id': $scope.sucursales[i].Id });
                    }
                }

                if (routeSucursal.length === 0) {
                    notification.error({
                        message: 'Debe seleccionar una Sucursal para el empleado',
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
                    LastNameFather: $scope.ngDialogData.dData.LastNameFather,
                    LastNameMother: $scope.ngDialogData.dData.LastNameMother,
                    Key: $scope.ngDialogData.dData.Key,
                    Phone: $scope.ngDialogData.dData.Phone,
                    Email: $scope.ngDialogData.dData.Email,
                    Active: $scope.ngDialogData.dData.Active,
                    Description: $scope.ngDialogData.dData.Description,
                    Photo: $scope.ngDialogData.dData.Photo,
                    Gender: $scope.ngDialogData.dData.Gender,
                    Sucursales: routeSucursal,
                    FechaAlta: $scope.ngDialogData.dData.FechaAlta,
                    Curriculum: $scope.ngDialogData.dData.Curriculum,
                    Firma: $scope.ngDialogData.dData.Firma,
                    NoEmpleado: $scope.ngDialogData.dData.NoEmpleado,
                    Puesto: $scope.ngDialogData.dData.Puesto

                });
            }
        }
    ])

.controller("ShowAnnalistAcksController",
    [
        "$scope", "RecService",
        function ($scope, recService) {
            $scope.viewParams = function (aName, roId, ackName, entName) {
                recService.viewParams(aName, roId, ackName, entName);
            }
        }
    ])
;
"use strict";

angular.module("abcadmin")
    .controller("EnterpriseController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', "SecurityFact", "Notification", "ngDialog",
        function($scope, $route, $location, $http, $cookies, $controller, securityFact, notification, ngDialog) {

            // This controller groups similar functionalities of different controllers.
            $controller('GeneralController', { $scope: $scope, $http: $http });
            //-------------------------------------------------------------------------

           // $scope.tipoList = [{ Name: "Interna", Value: 0 }, { Name: "Externa", Value: 1 }];
            var filters = function (page)
            {
                return {
                    page: (page === null || page === undefined) ? $scope.currentPage : page,
                    pageSize: $scope.pageSize,
                    searchGeneral: $scope.searchGeneral,
                    //actionId: $scope.actionId,
                    tipo: $scope.tipo,
                    ackId: $scope.ackId,
                    alta: $scope.alta,
                    baja: $scope.baja
                }
            };
            
            var setActivationObjects = function (eObj, act) {
                return { id: eObj.Id, active: act }
            };

            var saveObject = function (promise) {
                var enterpriseObj = {
                    Id: promise.Id,
                    Name: promise.Name,
                    Active: promise.Active,
                    Description: promise.Description,
                    //Accion: promise.Accion
					//Ack: promise.Ack
                    Sede: promise.Sede,
                    Tipo: promise.Tipo
                };
                return { enterprise: enterpriseObj }
            }

            var editElement = function (element) {
                return {
                    template: "editDialog",
                    controller: "NewEnterpriseController",
                    className: "ngdialog-theme-default",
                    preCloseCallback: "preCloseCallbackOnScope",
                    data: { dData: element },
                    closeByEscape: true
                }
            }

            $scope.mainFunction(
                "icon mif-location-city", "Name",
                "/enterprise/refreshlist", filters,
                "/enterprise/saveactivestatus", setActivationObjects,
                "/enterprise/saveenterprise", saveObject,
                editElement,
				"ack"
                );

            // Enterprise specifics
            $scope.SearchGeneral = true;
            $scope.tipoEmpFilter = true;
            $scope.FilterActive = true;
            // $scope.boxHeaderTitleActive = "Instituciones activas";
            // $scope.boxHeaderTitleDeactive = "Instituciones de baja";
			
            $scope.panels = [
                {
                    index: 0,
                    name: 'Filtros',
                    state: false,
                    size: 3
                },
                {
                    index: 1,
                    name: 'Tabla Instituciones',
                    state: true,
                    size: 9
                }
            ];
        }
    ])
    .controller("NewEnterpriseController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog",
        function($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog) {

            var errorDelay = 5000;

            // function getAllActions(active) {
            //     $http.post('/actions/getallactions', { active: active })
            //     .success(function (response) {
            //         if (response.success === true) {
            //             $scope.actions = response.elements;
            //         } else {
            //             notification.error({
            //                 message: 'Error al recuperar datos.', delay: errorDelay
            //             });
            //         }
            //     })
            //     .error(function () {
            //         notification.error({
            //             message: 'Error en la conexión con el servidor.', delay: errorDelay
            //         });
            //     });
            // }

            // getAllActions(true);

            // function getAllAck(active) {
            // 	$http.post('/ack/getack', { active: active })
            //     .success(function (response) {
            //     	if (response.success === true) {
            //     		$scope.acks = response.elements;
            //     	} else {
            //     		notification.error({
            //     			message: 'Error al recuperar datos.', delay: errorDelay
            //     		});
            //     	}
            //     })
            //     .error(function () {
            //     	notification.error({
            //     		message: 'Error en la conexión con el servidor.', delay: errorDelay
            //     	});
            //     });
            // }

            // getAllAck(true);

            $scope.tipoChanged = function (tipo){
                $scope.ngDialogData.dData.Tipo = tipo === "0";
            }
			
            $scope.tipo = $scope.ngDialogData.dData.Tipo;

            $scope.dialogTitle = $scope.ngDialogData.dData.Id === undefined ?
                "Nueva institución" : "Editar institución " + $scope.ngDialogData.dData.Name;

            $scope.acceptDialog = function () {
                var error = false;

                if ($scope.ngDialogData.dData.Name == undefined) {
                    notification.error({
                    	message: "Nombre de la institución indefinido",
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
                    //Accion: $scope.ngDialogData.dData.Accion,
                    //Ack: $scope.ngDialogData.dData.Ack.Id !== null
					//	? $scope.ngDialogData.dData.Ack : null
                    Sede: $scope.ngDialogData.dData.Sede,
                    Tipo: $scope.ngDialogData.dData.Tipo
                });

            }
        }
        
    ])
;
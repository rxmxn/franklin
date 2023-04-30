"use strict";

angular.module("abcadmin")
    .controller("GeneralController",
    [
        "$scope", "$route", "$location", "$http", "$cookies", '$controller', "SecurityFact", "Notification", "ngDialog", "counterFactory", '$rootScope',
        function ($scope, $route, $location, $http, $cookies, $controller, securityFact, notification, ngDialog, counterFactory, $rootScope) {
            
            var errorDelay = 5000;
            var normalDelay = 3000;

            $scope.taskbar = true;

            $scope.mainFunction = function (
                iconClass, orderByField,
                postString, filtersList,    // parameters to the RefreshList function (string, json)
                setActivationPost, setActivationObjects, // parameters to the setActivation function (string, json)
                savePost, saveObject,  
                editElement, // characteristics of the edit dialog in the view
                accessLevel
                ) {
                
                $scope.iconClass = iconClass;
                
                $scope.orderByField = orderByField;
                $scope.reverseSort = false;

                $scope.isLoading = false;
                
                // Checking Column List
                $scope.columnList = securityFact.getColumnList();
                // Checking Level Access
                $scope.accessLevel = securityFact.checkAccessLevel(accessLevel);
                
                if($scope.columnList === undefined || $scope.accessLevel === null)
                    securityFact.subscribe($scope, function getCLandAL () {                
                        $scope.columnList = securityFact.getColumnList();
                        $scope.accessLevel = securityFact.checkAccessLevel(accessLevel);
                    });                

                //write all code for the pagination
                $scope.currentPage = 1;
                $scope.pageSize = 10; 

                //$scope.viewDeactivated = false;
                $scope.elementsList = null;

                $scope.alta = true;
                $scope.baja = false;

                if (postString !== null) {
                	$scope.RefreshList = function (page) {
                		$scope.isLoading = true;

                		$http.post(postString, filtersList(page))
						.success(function (response) {
							if (response.success) {
								$scope.elementsList = response.elements;
								$scope.pageTotal = response.total;
							} else {
								notification.error({
									message: "Error al recuperar los datos. " + response.err, delay: errorDelay
								});
							}
							$scope.isLoading = false;
						})
						.error(function () {
							notification.error({
								message: "Error en la conexión con el servidor.", delay: errorDelay
							});
							$scope.isLoading = false;
						});
                	};
                }               	
                $scope.RefreshList();

            	// Activation filter, to see the elements active or deactive.
                if (setActivationPost !== null) {
                	$scope.setActivation = function (eObj, act) {
                		$http.post(setActivationPost, setActivationObjects(eObj, act))
						.success(function (response) {
							if (response.success) {
								counterFactory.getAllCounts(true);
								notification.success({
									message: "Datos procesados correctamente.", delay: normalDelay
								});
								$scope.RefreshList();
							} else {
								notification.error({
									message: response.details, delay: errorDelay
								});
							}
						})
						.error(function () {
							notification.error({
								message: "Error en la conexión con el servidor.", delay: errorDelay
							});
						});
                	};
	            }
                //-------------------------------------------------------------
                // Date filters
                $scope.dateFilter = function () {
                    $scope.initDateFilter = true;
                    $scope.RefreshList();
                    $scope.initDateFilter = false;
                };
                $scope.dateFilterRemove = function () {
                    $scope.initDateFilter = true;
                    $scope.untilDate = "";
                    $scope.fromDate = "";
                    $scope.RefreshList();
                    $scope.initDateFilter = false;
                };
                //-------------------------------------------------------------
                // Name filter
                $scope.nameFilter = function () {
                    // $scope.searchGeneral updated
                    $scope.RefreshList();
                };
                //-------------------------------------------------------------
            	// Functions to save new elements in the DB
                if (savePost !== null) {
                	$scope.save = function (promise) {
                		$scope.initSaving = true;

                		$http.post(savePost, saveObject(promise))
							.success(function (response) {
								if (response.success) {
									notification.success({
										message: "Datos guardados correctamente.",
										delay: normalDelay
									});
									$scope.initSaving = false;
									$scope.RefreshList();
									counterFactory.getAllCounts(true);
								} else {
									notification.error({
										message: "Error en la conexión con el servidor.",
										delay: errorDelay
									});
									$scope.initSaving = false;
								}
							})
							.error(function () {
								notification.error({
									message: "Se obtuvo un error del servidor. Ver detalles.",
									delay: errorDelay
								});
								$scope.initSaving = false;
							});
                	}
	            }
                
                //--------------------function that is invoked when switch activated/deactivated change-----
                $scope.getElement = function (f) {
                    if (f!=undefined) 
                        f();
                        $scope.RefreshList(1);
                    }
				
                //------------------------------------------------------------------------------------------
            	// To edit the element in the dialog
                if (editElement !== null) {
                	$scope.edit = function (element) {
                		var tElement = {};
                		angular.extend(tElement, element);

                		ngDialog.openConfirm(editElement(tElement))
							.then(function (promise) {
								//Accepting
								$scope.save(promise);
							}, function (reason) {
								//Rejecting
							});
                	};
	            }

                $scope.preCloseCallbackOnScope = function () {
                    return false;
                };

            };
        }
    ]);
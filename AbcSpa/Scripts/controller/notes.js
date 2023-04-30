"use strict";

angular.module("abcadmin")

	.controller("NotesCtrl", ["$scope", '$controller', "$http", '$rootScope',
	function ($scope, $controller, $http, $rootScope) {

		// This controller groups similar functionalities of different controllers.
		$controller('GeneralController', { $scope: $scope, $http: $http });
		//-------------------------------------------------------------------------
		$scope.viewDeactivated = false;
		var filters = function () {
			return {
				done: $scope.viewDeactivated
			}
		};

		var setDoneObjects = function (eObj, done) {
			return { id: eObj.Id, done: done }
		};

		var saveObject = function (promise) {
			var noteObj = {
				Id: promise.Id,
				Done: promise.Done,
				Description: promise.Description
			};
			return { note: noteObj }
		}

		var editElement = function (element) {
			return {
				template: "Shared/EditNote",
				controller: "NewNotesController",
				className: "ngdialog-theme-default",
				preCloseCallback: "preCloseCallbackOnScope",
				data: { dData: element },
				closeByEscape: true
			}
		}

		$scope.mainFunction(
                "Nota", "CreatedTime",
                "/notes/refreshlist", filters,
                "/notes/savedonestatus", setDoneObjects,
                "/notes/savenote", saveObject,
                editElement
                );

		$rootScope.$on('element:deleted', function () {
			$scope.RefreshList();
		});

		$scope.boxHeaderTitleActive = "Tareas por hacer";
		$scope.boxHeaderTitleDeactive = "Tareas realizadas";
	}])

.controller("NewNotesController",
    [
        "$scope", "$routeParams", "$location", "$http", '$controller', "SecurityFact", "Notification", "ngDialog", '$rootScope',
        function ($scope, $routeParams, $location, $http, $controller, securityFact, notification, ngDialog, $rootScope) {

        	var normalDelay = 3000;
        	var errorDelay = 5000;

        	$scope.dialogTitle = $scope.ngDialogData.dData.Id == undefined ?
                "Nueva tarea" : "Editar tarea";

        	// TODO: modificar para que ademas de mostrar la notificacion ocurra alguna sennalizacion en el input
        	$scope.acceptDialog = function () {
        		var error = false;

        		if ($scope.ngDialogData.dData.Description === undefined) {
        			notification.error({
        				message: "Nota en blanco",
        				delay: errorDelay
        			});
        			error = true;
        		}
        		if (error === true)
        			return;

        		$scope.confirm({
        			Id: $scope.ngDialogData.dData.Id != null ?
                        $scope.ngDialogData.dData.Id : 0,
        			Done: $scope.ngDialogData.dData.Done,
        			Description: $scope.ngDialogData.dData.Description
        		});
        	}

        	$scope.deleteNote = function () {
        		$http.post('/notes/deletenote', { id: $scope.ngDialogData.dData.Id })
                .success(function (response) {
                	if (response.success) {
                		$scope.closeThisDialog();
                		notification.success({
                			message: "Nota eliminada correctamente.", delay: normalDelay
                		});
                		$rootScope.$emit('element:deleted');
                	} else {
                		notification.error({
                			message: "Error al eliminar nota." + response.details, delay: errorDelay
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

    ])
	;
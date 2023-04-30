"use strict";
angular.module("abcadmin")
	.factory("RecService", ["$http", "$q", "Notification", "ngDialog",
		function ($http, $q, notification, ngDialog) {

			var rec = {};
			var errorDelay = 5000;

			rec.viewRecAdqs = function (an) {
				$http.post("/sucursal/GetAnnalistRecAdqs", { annalistId: an.Id })
					.success(function (response) {
						if (response.success) {
							ngDialog.openConfirm({
								template: "Shared/ShowAnnalistAcks",
								controller: "ShowAnnalistAcksController",
								className: "ngdialog-theme-default ngdialog-width700",
								preCloseCallback: "preCloseCallbackOnScope",
								data: { dData: response.elements, annalist: an.Key },
								closeByEscape: true
							})
							.then(function (promise) {
								//Accepting
							}, function (reason) {
								//Rejecting
							});
						}
					})
				 .error(function () {
				 	notification.error({
				 		message: "Error en la conexión con el servidor.", delay: errorDelay
				 	});
				 });
			}

			rec.viewParams = function (aName, roId, ackName, entName) {
				$http.post("/sucursal/getannalistparams", { recOtorgId: roId })
					.success(function (response) {
						if (response.success) {
							ngDialog.openConfirm({
								template: "Shared/ShowParamsMethods",
								//controller: "SetLMPDialogController",
								className: "ngdialog-theme-default",
								preCloseCallback: "preCloseCallbackOnScope",
								data: {
									dData: response.elements,
									annalist: aName,
									rec: entName === undefined || entName === null ? ackName : ackName + "/" + entName
								},
								closeByEscape: true
							})
							.then(function (promise) {
								//Accepting
							}, function (reason) {
								//Rejecting
							});
						}
					})
				 .error(function () {
				 	notification.error({
				 		message: "Error en la conexión con el servidor.", delay: errorDelay
				 	});
				 });
			}

			rec.viewAckParams = function (ackId, ackName, ackKey) {
				$http.post("/ack/GetAckParams", { ackId: ackId })
					.success(function (response) {
						if (response.success) {
							ngDialog.openConfirm({
								template: "Shared/ShowAckParams",
								//controller: "SetLMPDialogController",
								className: "ngdialog-theme-default ngdialog-width700",
								preCloseCallback: "preCloseCallbackOnScope",
								data: {
									dData: response.elements,
									ack: ackName,
								    ackKey:ackKey
								},
								closeByEscape: true
							})
							.then(function (promise) {
								//Accepting
							}, function (reason) {
								//Rejecting
							});
						}
					})
				 .error(function () {
				 	notification.error({
				 		message: "Error en la conexión con el servidor.", delay: errorDelay
				 	});
				 });
			}

			return rec;
		}])

	.factory("InfoService", ["$http", "$q", "Notification", "ngDialog", "RecService",
		function ($http, $q, notification, ngDialog, recService) {

			var x = {};
			var errorDelay = 5000;

			x.viewAnnalistInfo = function (a) {
				$("#loadingSpinners").show();
				$http.post("/Annalist/GetAnnalistInfo", { annalistKey: a })
					.success(function (response) {
						if (response.success) {
							ngDialog.openConfirm({
								template: "Shared/ShowAnnalistInfo",
								className: "ngdialog-theme-default",
								preCloseCallback: "preCloseCallbackOnScope",
								data: { 
									dData: response.elements, 
									annalist: a,
									avatarIMG: "/Content/img/usermale.jpg",
									rec: recService.viewRecAdqs
								},
								closeByEscape: true
							})
							.then(function (promise) {
								//Accepting
							}, function (reason) {
								//Rejecting
							});
						}
					})
				 .error(function () {
				 	notification.error({
				 		message: "Error en la conexión con el servidor.", delay: errorDelay
				 	});
				 });
			}

			x.viewAckInfo = function (a) {
				$("#loadingSpinners").show();
				$http.post("/Ack/GetAckInfo", { ackKey: a })
					.success(function (response) {
						if (response.success) {
							ngDialog.openConfirm({
								template: "Shared/ShowAckInfo",
								className: "ngdialog-theme-default",
								preCloseCallback: "preCloseCallbackOnScope",
								data: { 
									dData: response.elements, 
									ack: a,
									rec: recService.viewAckParams
								},
								closeByEscape: true
							})
							.then(function (promise) {
								//Accepting
							}, function (reason) {
								//Rejecting
							});
						}
					})
				 .error(function () {
				 	notification.error({
				 		message: "Error en la conexión con el servidor.", delay: errorDelay
				 	});
				 });
			}

			return x;
		}]);

	;
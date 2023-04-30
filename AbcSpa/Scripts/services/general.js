"use strict";
angular.module("abcadmin")

.factory("generalFactory", ["$q", "$route", "$location", "$http", "SecurityFact", "Notification", "ngDialog", "counterFactory",
    function($q, $route, $location, $http, securityFact, notification, ngDialog, counterFactory) {
        
        var errorDelay = 5000;
        var normalDelay = 3000;

        return {
        	RefreshList: function (postString, filtersList, page) {
        		var deferredObject = $q.defer();
                $http.post(postString, filtersList(page))
                .success(function (response) {
                	if (response.success) {
                		deferredObject.resolve({
                			success: true,
                			elements: response.elements,
                			total: response.total
                		});
                    } else {
                        notification.error({
                            message: "Error al recuperar los datos. " + response.err, delay: errorDelay
                        });
                        deferredObject.resolve({ success: false });
                    }
                })
                .error(function () {
                    notification.error({
                        message: "Error en la conexión con el servidor.", delay: errorDelay
                    });
                    deferredObject.resolve({ success: false });
                });
                return deferredObject.promise;
            },
        	setActivation: function (setActivationPost, eObj, act) {
        		var deferredObject = $q.defer();
            	$http.post(setActivationPost, { id: eObj.Id, active: act })
                .success(function (response) {
                    if (response.success) {
                        counterFactory.getAllCounts(true);
                        notification.success({
                            message: "Datos procesados correctamente.", delay: normalDelay
                        });
                        deferredObject.resolve({ success: true });   // if true then call RefreshList function
                    } else {
                        notification.error({
                            message: "Error al recuperar los datos." + response.details, delay: errorDelay
                        });
                        deferredObject.resolve({ success: false });
                    }
                })
                .error(function () {
                    notification.error({
                        message: "Error en la conexión con el servidor.", delay: errorDelay
                    });
                    deferredObject.resolve({ success: false });
                });
            	return deferredObject.promise;
            },
            save: function (savePost, saveObject) {
            	var deferredObject = $q.defer();
                $http.post(savePost, saveObject)
                    .success(function (response) {
                        if (response.success) {
                            notification.success({
                                message: "Datos guardados correctamente.",
                                delay: normalDelay
                            });
                            counterFactory.getAllCounts(true);
                            deferredObject.resolve({ success: true }); // if true then call RefreshList function
                        } else {
                            notification.error({
                                message: "Error en la conexión con el servidor.",
                                delay: errorDelay
                            });
                            deferredObject.resolve({ success: false });
                        }
                    })
                    .error(function () {
                        notification.error({
                            message: "Se obtuvo un error del servidor. Ver detalles.",
                            delay: errorDelay
                        });
                        deferredObject.resolve({ success: false });
                    });
                return deferredObject.promise;
            },
            getAll: function (address, json) {
                var deferredObject = $q.defer();
                $http.post(address, json)
                .success(function (response) {
                    if (response.success === true) {
                        deferredObject.resolve({ success: true, elements: response.elements });
                    } else {
                        deferredObject.resolve({ success: false });
                    }
                })
                .error(function () {
                    deferredObject.resolve({ success: false });
                });
                return deferredObject.promise;
            },
            edit: function (editElement, element, acceptFunction, rejectFunction) {
            	var tElement = {};
            	angular.extend(tElement, element);
            	ngDialog.openConfirm(editElement(tElement))
				.then(function (promise) {
					//Accepting
					acceptFunction(promise);
				}, function (reason) {
					//Rejecting
		            if (rejectFunction !== undefined && rejectFunction !== null) {
		            	rejectFunction(reason);
		            }
				});
            },
            restartSession: function () {                
                $http.post("/security/RestartSession")
                .success(function (response) {
                    if (response.success === true) {
                        return true;
                    } else {
                        notification.error({
                            message: response.err,
                            delay: errorDelay
                        });
                        return false;
                    }
                })
                .error(function () {
                    notification.error({
                            message: "Se obtuvo un error del servidor. Ver detalles.",
                            delay: errorDelay
                        });
                    return false;
                });
            },
            getParamObject: function (promise) {
            	var paramObj = {
            		Id: promise.Id,
            		Description: promise.Description,
            		Active: promise.Active,
            		Annalists: promise.Annalists,
            		PerTurnCapacity: promise.PerTurnCapacity,
            		PerWeekCapacity: promise.PerWeekCapacity,
            		AutolabAssignedAreaName: promise.AutolabAssignedAreaName,
            		ParamUniquekey: promise.ParamUniquekey,
            		GenericKeyForStatistic: promise.GenericKeyForStatistic,
            		Precio: promise.Precio,
            		BaseParam: promise.BaseParam,
            		Unit: promise.Unit,
            		CentroCosto: promise.CentroCosto,
            		ParamPrintResults: promise.ParamPrintResults,
            		Metodo: promise.Metodo,
            		ParamRoutes: promise.ParamRoutes,
            		//GenericDescription: promise.GenericDescription,
            		GenericKey: promise.GenericKey,
            		Rama: promise.Rama,
            		ResiduoPeligroso: promise.ResiduoPeligroso,
            		ReportaCliente: promise.ReportaCliente,
            		TipoServicio: promise.TipoServicio,
            		SellSeparated: promise.SellSeparated,
            		CuentaEstadistica: promise.CuentaEstadistica,
            		SucursalVende: promise.SucursalVende,
            		SucursalRealiza: promise.SucursalRealiza,
            		DecimalesReporte: promise.DecimalesReporte,
            		Groups: promise.Groups,
            		Packages: promise.Packages,
            		Matrixes: promise.Matrixes,
            		Week: promise.Week,
            		UnidadAnalitica: promise.UnidadAnalitica,
            		AnalyticsMethod: promise.AnalyticsMethod,
            		Formula: promise.Formula,
            		QcObj: promise.QcObj,
            		Uncertainty: promise.Uncertainty,
                    InternetPublish: promise.InternetPublish,
                    AnalysisTime: promise.AnalysisTime,
                    Residue: promise.Residue,
                    //TipoFormula: promise.TipoFormula,
                    Container: promise.Container,
                    Preserver: promise.Preserver,
                    RequiredVolume: promise.RequiredVolume,
                    MinimumVolume: promise.MinimumVolume,
                    ReportLimit:promise.ReportLimit,
                    DeliverTime: promise.DeliverTime,
                    DetectionLimit: promise.DetectionLimit,
                    CuantificationLimit: promise.CuantificationLimit,
                    MaxTimeBeforeAnalysis: promise.MaxTimeBeforeAnalysis,
                    LabDeliverTime: promise.LabDeliverTime,
                    ReportTime: promise.ReportTime,
                    TipoFormula: promise.TipoFormula,
                    AnnalistKey: promise.AnnalistKey
            	};
            	return { param: paramObj }
            },
            getGroupObject: function(promise) {
			    var groupObj = {
				    Id: promise.Id,
				    MaxPermitedLimit: promise.MaxPermitedLimit,
				    Name: promise.Name,
				    Active: promise.Active,
				    Description: promise.Description,
				    Parameters: promise.Parameters,
				    ParamPrintResults: promise.ParamPrintResults,
				    Matrixes: promise.Matrixes,
				    TipoServicio: promise.TipoServicio,
				    SellSeparated: promise.SellSeparated,
				    CuentaEstadistica: promise.CuentaEstadistica,
				    DecimalesReporte: promise.DecimalesReporte,
				    MostrarLista: promise.MostrarLista,
				    Packages: promise.Packages,
				    DispParamId: promise.DispParamId,
				    Week: promise.Week,
				    ComplexSamplings: promise.ComplexSamplings,
				    ClasificacionQuimica1: promise.ClasificacionQuimica1,
				    ClasificacionQuimica2: promise.ClasificacionQuimica2,
				    ClasificacionQuimica3: promise.ClasificacionQuimica3,
				    Sucursal: promise.Sucursal
			    };
			    return { group: groupObj /*, prms: gprms */}
            },
            getPackageObject: function(promise) {
            	var packageObj = {
            		Id: promise.Id,
            		Name: promise.Name,
            		MaxPermitedLimit: promise.MaxPermitedLimit,
            		Active: promise.Active,
            		Description: promise.Description,
            	    //Norm: promise.Norm,
                    Norms: promise.Norms,
            		TipoServicio: promise.TipoServicio,
            		SellSeparated: promise.SellSeparated,
            		CuentaEstadistica: promise.CuentaEstadistica,
            		Matrixes: promise.Matrixes,
            		DecimalesReporte: promise.DecimalesReporte,
            		Sucursal: promise.Sucursal
            		//Parameters: promise.Parameters,
            		//Groups: promise.Groups
            	};

            	var grps = [];
            	for (var i = 0; i < promise.Groups.length; i++) {
            		grps.push(promise.Groups[i].Id);
            	}

            	var prms = [];
            	for (i = 0; i < promise.Parameters.length; i++) {
            		prms.push(promise.Parameters[i].Id);
            	}

            	return { pack: packageObj, grps: grps, prms: prms }
            }
        }
    }])
;
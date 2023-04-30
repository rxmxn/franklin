using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace AbcPersistent.Models
{
    public class AbcWebContextSeeder : DropCreateDatabaseIfModelChanges<AbcContext>
    {
        // Creating default values for the first time the DB is initialized
        protected override void Seed(AbcContext context)
        {
            var abcContext = new AbcContext();

			// Adding Rights
			// View Charts
			var viewChartsRight = new Right
			{
				Value = "charts",
				Name = "Ver Gráficas.",
				Description = "Privililegio para poder ver gráficas.",
				Level = 2
			};
			abcContext.RightSet.Add(viewChartsRight);
			// View UserChanges
			var viewUserAccessRight = new Right
			{
				Value = "useraccess",
				Name = "Ver Bitácora de Accesos a la Web.",
				Description = "Privililegio para poder ver la Bitácora de Accesos a la Web.",
				Level = 2
			};
			abcContext.RightSet.Add(viewUserAccessRight);
			// View Audit
			var viewAuditRight = new Right
			{
				Value = "audit",
				Name = "Ver Bitácora de Cambios.",
				Description = "Privililegio para poder ver la Bitácora de Cambios.",
				Level = 2
			};
			abcContext.RightSet.Add(viewAuditRight);
			// View Massive Transfer
			var viewMassiveTransferRight = new Right
			{
				Value = "massivetransfer",
				Name = "Ver Transferencias Masivas.",
				Description = "Privililegio para poder ver Transferencias Masivas.",
				Level = 2
			};
			abcContext.RightSet.Add(viewMassiveTransferRight);
			// User
			var createUserRight = new Right
            {
                Value = "user",
                Name = "Crear/Editar usuarios.",
                Description = "Privililegio para poder crear y editar usuarios.",
				Level = 1
            };
			var viewUserRight = new Right
			{
				Value = "user",
				Name = "Ver usuarios.",
				Description = "Privililegio para poder ver usuarios.",
				Level = 2
			};
			abcContext.RightSet.Add(createUserRight);
			abcContext.RightSet.Add(viewUserRight);
			// Role
			var createRolesRight = new Right
            {
                Value = "role",
                Name = "Crear/Editar roles.",
                Description = "Privililegio para poder crear y editar roles de usuarios.",
				Level = 1
			};
			var viewRolesRight = new Right
			{
				Value = "role",
				Name = "Ver roles.",
				Description = "Privililegio para poder ver roles de usuarios.",
				Level = 2
			};
			abcContext.RightSet.Add(createRolesRight);
			abcContext.RightSet.Add(viewRolesRight);
			// Method
			var createMethodRight = new Right
            {
                Value = "method",
                Name = "Crear/Editar métodos.",
                Description = "Privililegio para poder crear y editar métodos. ",
				Level = 1
			};
			var viewMethodRight = new Right
			{
				Value = "method",
				Name = "Ver métodos.",
				Description = "Privililegio para poder ver métodos. ",
				Level = 2
			};
            abcContext.RightSet.Add(createMethodRight);
			abcContext.RightSet.Add(viewMethodRight);
			// Container
			var createContainerRight = new Right
			{
				Value = "container",
				Name = "Crear/Editar envases.",
				Description = "Privililegio para poder crear y editar envases. ",
				Level = 1
			};
			var viewContainerRight = new Right
			{
				Value = "container",
				Name = "Ver envases.",
				Description = "Privililegio para poder ver envases. ",
				Level = 2
			};
			abcContext.RightSet.Add(createContainerRight);
			abcContext.RightSet.Add(viewContainerRight);
			// Material
			var createMaterialRight = new Right
			{
				Value = "material",
				Name = "Crear/Editar materiales.",
				Description = "Privililegio para poder crear y editar materiales. ",
				Level = 1
			};
			var viewMaterialRight = new Right
			{
				Value = "material",
				Name = "Ver materiales.",
				Description = "Privililegio para poder ver materiales. ",
				Level = 2
			};
			abcContext.RightSet.Add(createMaterialRight);
			abcContext.RightSet.Add(viewMaterialRight);
			// Preserver
			var createPreserverRight = new Right
			{
				Value = "preserver",
				Name = "Crear/Editar preservadores.",
				Description = "Privililegio para poder crear y editar preservadores. ",
				Level = 1
			};
			var viewPreserverRight = new Right
			{
				Value = "preserver",
				Name = "Ver preservadores.",
				Description = "Privililegio para poder ver preservadores. ",
				Level = 2
			};
			abcContext.RightSet.Add(createPreserverRight);
			abcContext.RightSet.Add(viewPreserverRight);
			// Residue
			var createResidueRight = new Right
			{
				Value = "residue",
				Name = "Crear/Editar desechos.",
				Description = "Privililegio para poder crear y editar desechos. ",
				Level = 1
			};
			var viewResidueRight = new Right
			{
				Value = "residue",
				Name = "Ver desechos.",
				Description = "Privililegio para poder ver desechos. ",
				Level = 2
			};
			abcContext.RightSet.Add(createResidueRight);
			abcContext.RightSet.Add(viewResidueRight);
			// AnalyticsMethod
			var createAnalyticsMethodRight = new Right
			{
				Value = "analyticsmethod",
				Name = "Crear/Editar métodos analíticos.",
				Description = "Privililegio para poder crear y editar métodos analíticos. ",
				Level = 1
			};
			var viewAnalyticsMethodRight = new Right
			{
				Value = "analyticsmethod",
				Name = "Ver métodos analíticos.",
				Description = "Privililegio para poder ver métodos analíticos. ",
				Level = 2
			};
			abcContext.RightSet.Add(createAnalyticsMethodRight);
			abcContext.RightSet.Add(viewAnalyticsMethodRight);
			// Ack
			var createAckRight = new Right
			{
				Value = "ack",
				Name = "Crear/Editar reconocimientos.",
				Description = "Privililegio para poder crear y editar reconocimientos.",
				Level = 1
			};
			var viewAckRight = new Right
			{
				Value = "ack",
				Name = "Ver reconocimientos.",
				Description = "Privililegio para poder ver reconocimientos.",
				Level = 2
			};
			abcContext.RightSet.Add(createAckRight);
			abcContext.RightSet.Add(viewAckRight);
			// Annalist
			var createAnnalistRight = new Right
			{
				Value = "annalist",
				Name = "Crear/Editar analistas.",
				Description = "Privililegio para poder crear y editar analistas.",
				Level = 1
			};
			var viewAnnalistRight = new Right
			{
				Value = "annalist",
				Name = "Ver analistas.",
				Description = "Privililegio para poder ver analistas.",
				Level = 2
			};
			abcContext.RightSet.Add(createAnnalistRight);
			abcContext.RightSet.Add(viewAnnalistRight);
            //AnnalistKey
            var createAnnalistKeyRight = new Right
            {
                Value = "annalistkey",
                Name = "Crear/Editar Claves de analistas.",
                Description = "Privililegio para poder crear y editar claves de analistas.",
                Level = 1
            };
            var viewAnnalistKeyRight = new Right
            {
                Value = "annalistkey",
                Name = "Ver claves de analistas.",
                Description = "Privililegio para poder ver claves de analistas.",
                Level = 2
            };
            abcContext.RightSet.Add(createAnnalistKeyRight);
            abcContext.RightSet.Add(viewAnnalistKeyRight);

			// AnalyticsArea
			var createAnalyticsAreaRight = new Right
			{
				Value = "analyticsarea",
				Name = "Crear/Editar áreas analíticas.",
				Description = "Privililegio para poder crear y editar áreas analíticas.",
				Level = 1
			};
			var viewAnalyticsAreaRight = new Right
			{
				Value = "analyticsarea",
				Name = "Ver áreas analíticas.",
				Description = "Privililegio para poder ver áreas analíticas.",
				Level = 2
			};
			abcContext.RightSet.Add(createAnalyticsAreaRight);
			abcContext.RightSet.Add(viewAnalyticsAreaRight);
			// BaseParam
			var createBaseParamRight = new Right
			{
				Value = "baseparam",
				Name = "Crear/Editar parámetros base.",
				Description = "Privililegio para poder crear y editar parámetros base.",
				Level = 1
			};
			var viewBaseParamRight = new Right
			{
				Value = "baseparam",
				Name = "Ver parámetros base.",
				Description = "Privililegio para poder ver parámetros base.",
				Level = 2
			};
			abcContext.RightSet.Add(createBaseParamRight);
			abcContext.RightSet.Add(viewBaseParamRight);
			
			// Clasificacion Right
			var createClasificacionQuimicaRight = new Right
			{
				Value = "clasificacionquimica",
				Name = "Crear/Editar clasificación química.",
				Description = "Privililegio para poder crear y editar clasificaciones químicas.",
				Level = 1
			};
			var viewClasificacionQuimicaRight = new Right
			{
				Value = "clasificacionquimica",
				Name = "Ver clasificación química.",
				Description = "Privililegio para poder ver clasificaciones químicas.",
				Level = 2
			};
			abcContext.RightSet.Add(createClasificacionQuimicaRight);
			abcContext.RightSet.Add(viewClasificacionQuimicaRight);
			// MeasureUnits
			var createMeasureUnitsRight = new Right
			{
				Value = "measureunits",
				Name = "Crear/Editar unidades de medida.",
				Description = "Privililegio para poder crear y editar unidades de medida.",
				Level = 1
			};
			var viewMeasureUnitsRight = new Right
			{
				Value = "measureunits",
				Name = "Ver unidades de medida.",
				Description = "Privililegio para poder ver unidades de medida.",
				Level = 2
			};
			abcContext.RightSet.Add(createMeasureUnitsRight);
			abcContext.RightSet.Add(viewMeasureUnitsRight);
			// Param
			var createParamRight = new Right
			{
				Value = "param",
				Name = "Crear/Editar parámetros.",
				Description = "Privililegio para poder crear y editar parámetros.",
				Level = 1
			};
			var viewParamRight = new Right
			{
				Value = "param",
				Name = "Ver parámetros.",
				Description = "Privililegio para poder ver parámetros.",
				Level = 2
			};
			abcContext.RightSet.Add(createParamRight);
			abcContext.RightSet.Add(viewParamRight);
			// Currency
			var createCurrencyRight = new Right
			{
				Value = "currency",
				Name = "Crear/Editar monedas.",
				Description = "Privililegio para poder crear y editar monedas.",
				Level = 1
			};
			var viewCurrencyRight = new Right
			{
				Value = "currency",
				Name = "Ver monedas.",
				Description = "Privililegio para poder ver monedas.",
				Level = 2
			};
			abcContext.RightSet.Add(createCurrencyRight);
			abcContext.RightSet.Add(viewCurrencyRight);
			// Norm
			var createNormRight = new Right
			{
				Value = "norm",
				Name = "Crear/Editar normas.",
				Description = "Privililegio para poder crear y editar normas.",
				Level = 1
			};
			var viewNormRight = new Right
			{
				Value = "norm",
				Name = "Ver normas.",
				Description = "Privililegio para poder ver normas.",
				Level = 2
			};
			abcContext.RightSet.Add(createNormRight);
			abcContext.RightSet.Add(viewNormRight);

            //status
            var createStatusRight = new Right
            {
                Value = "status",
                Name = "Crear/Editar status.",
                Description = "Privililegio para poder crear y editar status.",
                Level = 1
            };
            var viewStatusRight = new Right
            {
                Value = "status",
                Name = "Ver Status.",
                Description = "Privililegio para poder ver status.",
                Level = 2
            };
            abcContext.RightSet.Add(createStatusRight);
            abcContext.RightSet.Add(viewStatusRight);

			// Group
			var createGroupRight = new Right
			{
				Value = "group",
				Name = "Crear/Editar grupos.",
				Description = "Privililegio para poder crear y editar grupos.",
				Level = 1
			};
			var viewGroupRight = new Right
			{
				Value = "group",
				Name = "Ver grupos.",
				Description = "Privililegio para poder ver grupos.",
				Level = 2
			};
			abcContext.RightSet.Add(createGroupRight);
			abcContext.RightSet.Add(viewGroupRight);
			// Package
			var createPackageRight = new Right
			{
				Value = "package",
				Name = "Crear/Editar paquetes.",
				Description = "Privililegio para poder crear y editar paquetes.",
				Level = 1
			};
			var viewPackageRight = new Right
			{
				Value = "package",
				Name = "Ver paquetes.",
				Description = "Privililegio para poder ver paquetes.",
				Level = 2
			};
			abcContext.RightSet.Add(createPackageRight);
			abcContext.RightSet.Add(viewPackageRight);
			// Matrix
			var createMatrixRight = new Right
			{
				Value = "matrix",
				Name = "Crear/Editar matrices.",
				Description = "Privililegio para poder crear y editar matrices.",
				Level = 1
			};
			var viewMatrixRight = new Right
			{
				Value = "matrix",
				Name = "Ver matrices.",
				Description = "Privililegio para poder ver matrices.",
				Level = 2
			};
			abcContext.RightSet.Add(createMatrixRight);
			abcContext.RightSet.Add(viewMatrixRight);
			// Base Matrix
			var createBaseMatrixRight = new Right
			{
				Value = "basematrix",
				Name = "Crear/Editar matrices base.",
				Description = "Privililegio para poder crear y editar matrices base.",
				Level = 1
			};
			var viewBaseMatrixRight = new Right
			{
				Value = "basematrix",
				Name = "Ver matrices base.",
				Description = "Privililegio para poder ver matrices base.",
				Level = 2
			};
			abcContext.RightSet.Add(createBaseMatrixRight);
			abcContext.RightSet.Add(viewBaseMatrixRight);
			// Region
			var createRegionRight = new Right
			{
				Value = "region",
				Name = "Crear/Editar regiones.",
				Description = "Privililegio para poder crear y editar regiones.",
				Level = 1
			};
			var viewRegionRight = new Right
			{
				Value = "region",
				Name = "Ver regiones.",
				Description = "Privililegio para poder ver regiones.",
				Level = 2
			};
			abcContext.RightSet.Add(createRegionRight);
			abcContext.RightSet.Add(viewRegionRight);
			// TipoServicio
			var createTipoServicioRight = new Right
			{
				Value = "tiposervicio",
				Name = "Crear/Editar tipos de servicio.",
				Description = "Privililegio para poder crear y editar tipos de servicio.",
				Level = 1
			};
			var viewTipoServicioRight = new Right
			{
				Value = "tiposervicio",
				Name = "Ver tipos de servicio.",
				Description = "Privililegio para poder ver tipos de servicio.",
				Level = 2
			};
			abcContext.RightSet.Add(createTipoServicioRight);
			abcContext.RightSet.Add(viewTipoServicioRight);
			// Rama
			var createRamaRight = new Right
			{
				Value = "rama",
				Name = "Crear/Editar ramas.",
				Description = "Privililegio para poder crear y editar ramas.",
				Level = 1
			};
			var viewRamaRight = new Right
			{
				Value = "rama",
				Name = "Ver ramas.",
				Description = "Privililegio para poder ver ramas.",
				Level = 2
			};
			abcContext.RightSet.Add(createRamaRight);
			abcContext.RightSet.Add(viewRamaRight);
			// End Adding Rights

			// Adding ParamCols
			var currencyCol = new ParamCol()
	        {
				Name = "Moneda",
				Description = "Columna para ver Moneda",
				Key = "Currency"
			};
	        abcContext.ParamColSet.Add(currencyCol);

			var measureunitCol = new ParamCol()
			{
				Name = "Unidad de Medida",
				Description = "Columna para ver Unidad de Medida",
				Key = "MeasureUnit"
			};
			abcContext.ParamColSet.Add(measureunitCol);

			var delivertimeCol = new ParamCol()
			{
				Name = "Tiempo de entrega",
				Description = "Columna para ver Tiempo de entrega",
				Key = "DeliverTime"
			};
			abcContext.ParamColSet.Add(delivertimeCol);

			var ackCol = new ParamCol()
			{
				Name = "Reconocimiento",
				Description = "Columna para ver Reconocimientos",
				Key = "Ack"
			};
			abcContext.ParamColSet.Add(ackCol);

			var analyticsareaCol = new ParamCol()
			{
				Name = "Centro de Costo",
				Description = "Columna para ver Centros de Costo",
				Key = "AnalyticsArea"
			};
			abcContext.ParamColSet.Add(analyticsareaCol);

			var analyticsmethodCol = new ParamCol()
			{
				Name = "Método Analítico",
				Description = "Columna para ver Métodos Analíticos",
				Key = "AnalyticsMethod"
			};
			abcContext.ParamColSet.Add(analyticsmethodCol);

			var normCol = new ParamCol()
			{
				Name = "Norma",
				Description = "Columna para ver Normas",
				Key = "Norm"
			};
			abcContext.ParamColSet.Add(normCol);

			var maxpermitedlimitCol = new ParamCol()
			{
				Name = "Límite Máximo Permitido",
				Description = "Columna para ver Límites Máximos Permitidos",
				Key = "MaxPermitedLimit"
			};
			abcContext.ParamColSet.Add(maxpermitedlimitCol);

			var detectionlimitCol = new ParamCol()
			{
				Name = "Límite de Detección",
				Description = "Columna para ver Límites de Detección",
				Key = "DetectionLimit"
			};
			abcContext.ParamColSet.Add(detectionlimitCol);

			var cuantificationlimitCol = new ParamCol()
			{
				Name = "Límite de Cuantificación",
				Description = "Columna para ver Límites de Cuantificación",
				Key = "CuantificationLimit"
			};
			abcContext.ParamColSet.Add(cuantificationlimitCol);

			var uncertaintyCol = new ParamCol()
			{
				Name = "Incertidumbre",
				Description = "Columna para ver Incertidumbre",
				Key = "Uncertainty"
			};
			abcContext.ParamColSet.Add(uncertaintyCol);

			var maxtimebeforeanalysisCol = new ParamCol()
			{
				Name = "Tiempo Máximo antes del Análisis",
				Description = "Columna para ver Tiempos Máximos antes del Análisis",
				Key = "MaxTimeBeforeAnalysis"
			};
			abcContext.ParamColSet.Add(maxtimebeforeanalysisCol);

			var labdelivertimeCol = new ParamCol()
			{
				Name = "Tiempo de entrega al Laboratorio",
				Description = "Columna para ver Tiempos de entrega al Laboratorio",
				Key = "LabDeliverTime"
			};
			abcContext.ParamColSet.Add(labdelivertimeCol);

			var residueCol = new ParamCol()
			{
				Name = "Desecho",
				Description = "Columna para ver Desechos",
				Key = "Residue"
			};
			abcContext.ParamColSet.Add(residueCol);

			var preserverCol = new ParamCol()
			{
				Name = "Preservador",
				Description = "Columna para ver Preservadores",
				Key = "Preserver"
			};
			abcContext.ParamColSet.Add(preserverCol);

			var containerCol = new ParamCol()
			{
				Name = "Envase",
				Description = "Columna para ver Envases",
				Key = "Container"
			};
			abcContext.ParamColSet.Add(containerCol);

			var autolabnameCol = new ParamCol()
			{
				Name = "Nombre de Área asignado por Autolab",
				Description = "Columna para ver Nombre de Área asignado por Autolab",
				Key = "AutolabAssignedAreaName"
			};
			abcContext.ParamColSet.Add(autolabnameCol);

			var qcCol = new ParamCol()
			{
				Name = "Qc",
				Description = "Columna para ver Qc",
				Key = "Qc"
			};
			abcContext.ParamColSet.Add(qcCol);

			var annalistCol = new ParamCol()
			{
				Name = "Analista",
				Description = "Columna para ver Analistas",
				Key = "AnnalistName"
			};
			abcContext.ParamColSet.Add(annalistCol);

			var perturncapacityCol = new ParamCol()
			{
				Name = "Capacidad por turno",
				Description = "Columna para ver Capacidades por turno",
				Key = "PerTurnCapacity"
			};
			abcContext.ParamColSet.Add(perturncapacityCol);

			var perweekcapacityCol = new ParamCol()
			{
				Name = "Capacidad por semana",
				Description = "Columna para ver Capacidades por semana",
				Key = "PerWeekCapacity"
			};
			abcContext.ParamColSet.Add(perweekcapacityCol);

			var reporttimeCol = new ParamCol()
			{
				Name = "Tiempo de Reporte",
				Description = "Columna para ver Tiempos de Reporte",
				Key = "ReportTime"
			};
			abcContext.ParamColSet.Add(reporttimeCol);

			var analysistimeCol = new ParamCol()
			{
				Name = "Tiempo de Análisis",
				Description = "Columna para ver Tiempos de Análisis",
				Key = "AnalysisTime"
			};
			abcContext.ParamColSet.Add(analysistimeCol);

			var requiredvolumeCol = new ParamCol()
			{
				Name = "Volúmen Requerido",
				Description = "Columna para ver Volúmen Requerido",
				Key = "RequiredVolume"
			};
			abcContext.ParamColSet.Add(requiredvolumeCol);

			var minimumvolumeCol = new ParamCol()
			{
				Name = "Volúmen Mínimo",
				Description = "Columna para ver Volúmen Mínimo",
				Key = "MinimumVolume"
			};
			abcContext.ParamColSet.Add(minimumvolumeCol);

			var formulaCol = new ParamCol()
			{
				Name = "Fórmula",
				Description = "Columna para ver Fórmula",
				Key = "Formula"
			};
			abcContext.ParamColSet.Add(formulaCol);

			var internetpublishCol = new ParamCol()
			{
				Name = "Publicar en Internet",
				Description = "Columna para ver Publicar en Internet",
				Key = "InternetPublish"
			};
			abcContext.ParamColSet.Add(internetpublishCol);

			var baseparamfamilyCol = new ParamCol()
			{
				Name = "Familia de Parámetros Base",
				Description = "Columna para ver Familia de Parámetros Base",
				Key = "BaseParamFamily"
			};
			abcContext.ParamColSet.Add(baseparamfamilyCol);

			var clasificacionquimicaCol = new ParamCol()
			{
				Name = "Clasificación Química",
				Description = "Columna para ver Clasificaciones Químicas",
				Key = "ClasificacionQuimica"
			};
			abcContext.ParamColSet.Add(clasificacionquimicaCol);
			// End Adding ParamCols

			// Adding Roles
			// Admin
			var adminRole = new Role()
            {
                Name = "Administrador",
                Description = "Rol para privilegios de Administrador",
                Active = true
            };
			adminRole.RightColl.Add(viewUserAccessRight);
			adminRole.RightColl.Add(viewChartsRight);
			adminRole.RightColl.Add(viewAuditRight);
			adminRole.RightColl.Add(viewMassiveTransferRight);

			adminRole.RightColl.Add(createUserRight);
            adminRole.RightColl.Add(createRolesRight);
            adminRole.RightColl.Add(createMethodRight);
			adminRole.RightColl.Add(createContainerRight);
			adminRole.RightColl.Add(createResidueRight);
			adminRole.RightColl.Add(createPreserverRight);
			adminRole.RightColl.Add(createAnalyticsMethodRight);
			adminRole.RightColl.Add(createAckRight);
			adminRole.RightColl.Add(createAnnalistRight);
            adminRole.RightColl.Add(createAnnalistKeyRight);
	        adminRole.RightColl.Add(createAnalyticsAreaRight);
            adminRole.RightColl.Add(createBaseParamRight);
			//adminRole.RightColl.Add(createBaseParamFamilyRight);
			adminRole.RightColl.Add(createParamRight);
			adminRole.RightColl.Add(createGroupRight);
			adminRole.RightColl.Add(createPackageRight);
			adminRole.RightColl.Add(createCurrencyRight);
			adminRole.RightColl.Add(createMatrixRight);
			adminRole.RightColl.Add(createBaseMatrixRight);
			adminRole.RightColl.Add(createRegionRight);
			adminRole.RightColl.Add(createNormRight);
            adminRole.RightColl.Add(createStatusRight);
			adminRole.RightColl.Add(createMeasureUnitsRight);
			adminRole.RightColl.Add(createMaterialRight);
			adminRole.RightColl.Add(createClasificacionQuimicaRight);
			adminRole.RightColl.Add(createTipoServicioRight);
			adminRole.RightColl.Add(createRamaRight);

			adminRole.ParamCols.Add(currencyCol);
			adminRole.ParamCols.Add(measureunitCol);
			adminRole.ParamCols.Add(delivertimeCol);
			adminRole.ParamCols.Add(ackCol);
			adminRole.ParamCols.Add(analyticsareaCol);
			adminRole.ParamCols.Add(analyticsmethodCol);
			adminRole.ParamCols.Add(normCol);
			adminRole.ParamCols.Add(maxpermitedlimitCol);
			adminRole.ParamCols.Add(detectionlimitCol);
			adminRole.ParamCols.Add(cuantificationlimitCol);
			adminRole.ParamCols.Add(uncertaintyCol);
			adminRole.ParamCols.Add(maxtimebeforeanalysisCol);
			adminRole.ParamCols.Add(labdelivertimeCol);
			adminRole.ParamCols.Add(residueCol);
			adminRole.ParamCols.Add(containerCol);
			adminRole.ParamCols.Add(preserverCol);
			adminRole.ParamCols.Add(autolabnameCol);
			adminRole.ParamCols.Add(qcCol);
			adminRole.ParamCols.Add(annalistCol);
			adminRole.ParamCols.Add(perturncapacityCol);
			adminRole.ParamCols.Add(perweekcapacityCol);
			adminRole.ParamCols.Add(reporttimeCol);
			adminRole.ParamCols.Add(analysistimeCol);
			adminRole.ParamCols.Add(requiredvolumeCol);
			adminRole.ParamCols.Add(minimumvolumeCol);
			adminRole.ParamCols.Add(internetpublishCol);
			adminRole.ParamCols.Add(baseparamfamilyCol);
			adminRole.ParamCols.Add(formulaCol);
			adminRole.ParamCols.Add(clasificacionquimicaCol);

			abcContext.RoleSet.Add(adminRole);
            // End Admin
            // businessman
            var businessRole = new Role()
            {
                Name = "Comercial",
                Description = "Rol para privilegios de comerciales",
                Active = true
            };
			
			businessRole.ParamCols.Add(currencyCol);
			businessRole.ParamCols.Add(internetpublishCol);

			abcContext.RoleSet.Add(businessRole);
            // End businessman
            // labtech
            var labtechRole = new Role()
            {
                Name = "Técnico Operativo Laboratorio",
                Description = "Rol para privilegios de Técnico Operativo Laboratorio",
                Active = true
            };
			
			labtechRole.ParamCols.Add(measureunitCol);
			labtechRole.ParamCols.Add(delivertimeCol);
			labtechRole.ParamCols.Add(analyticsareaCol);
			labtechRole.ParamCols.Add(analyticsmethodCol);
			labtechRole.ParamCols.Add(detectionlimitCol);
			labtechRole.ParamCols.Add(cuantificationlimitCol);
			labtechRole.ParamCols.Add(maxtimebeforeanalysisCol);
			labtechRole.ParamCols.Add(labdelivertimeCol);
			labtechRole.ParamCols.Add(residueCol);
			labtechRole.ParamCols.Add(containerCol);
			labtechRole.ParamCols.Add(preserverCol);
			labtechRole.ParamCols.Add(autolabnameCol);
			labtechRole.ParamCols.Add(perturncapacityCol);
			labtechRole.ParamCols.Add(perweekcapacityCol);
			labtechRole.ParamCols.Add(reporttimeCol);
			labtechRole.ParamCols.Add(analysistimeCol);
			labtechRole.ParamCols.Add(requiredvolumeCol);
			labtechRole.ParamCols.Add(minimumvolumeCol);
			labtechRole.ParamCols.Add(baseparamfamilyCol);
			labtechRole.ParamCols.Add(formulaCol);
			labtechRole.ParamCols.Add(clasificacionquimicaCol);

			abcContext.RoleSet.Add(labtechRole);
            // End labtech
            // sampletech
            var sampletechRole = new Role()
            {
                Name = "Técnico Operativo Muestreo",
                Description = "Rol para privilegios de Técnico Operativo Muestreo",
                Active = true
            };
			sampletechRole.RightColl.Add(viewChartsRight);

			sampletechRole.ParamCols.Add(measureunitCol);
			sampletechRole.ParamCols.Add(delivertimeCol);
			sampletechRole.ParamCols.Add(analyticsareaCol);
			sampletechRole.ParamCols.Add(analyticsmethodCol);
			sampletechRole.ParamCols.Add(detectionlimitCol);
			sampletechRole.ParamCols.Add(cuantificationlimitCol);
			sampletechRole.ParamCols.Add(maxtimebeforeanalysisCol);
			sampletechRole.ParamCols.Add(labdelivertimeCol);
			sampletechRole.ParamCols.Add(residueCol);
			sampletechRole.ParamCols.Add(containerCol);
			sampletechRole.ParamCols.Add(preserverCol);
			sampletechRole.ParamCols.Add(autolabnameCol);
			sampletechRole.ParamCols.Add(perturncapacityCol);
			sampletechRole.ParamCols.Add(perweekcapacityCol);
			sampletechRole.ParamCols.Add(reporttimeCol);
			sampletechRole.ParamCols.Add(analysistimeCol);
			sampletechRole.ParamCols.Add(requiredvolumeCol);
			sampletechRole.ParamCols.Add(minimumvolumeCol);
			sampletechRole.ParamCols.Add(baseparamfamilyCol);
			sampletechRole.ParamCols.Add(formulaCol);
			sampletechRole.ParamCols.Add(clasificacionquimicaCol);

			abcContext.RoleSet.Add(sampletechRole);
            // End sampletech
            // qualityRole
            var qualityRole = new Role()
            {
                Name = "Calidad",
                Description = "Rol para privilegios de Calidad",
                Active = true
            };
			
			qualityRole.ParamCols.Add(ackCol);
			qualityRole.ParamCols.Add(normCol);
			qualityRole.ParamCols.Add(maxpermitedlimitCol);
			qualityRole.ParamCols.Add(uncertaintyCol);
			qualityRole.ParamCols.Add(qcCol);
			qualityRole.ParamCols.Add(annalistCol);

			abcContext.RoleSet.Add(qualityRole);
            // End qualityRole
            // regionManager
            var regionManagerRole = new Role()
            {
                Name = "Gerente Regional",
                Description = "Rol para privilegios de Gerente Regional",
                Active = true
            };
			regionManagerRole.RightColl.Add(viewParamRight);
			regionManagerRole.RightColl.Add(viewGroupRight);
			regionManagerRole.RightColl.Add(viewPackageRight);
			regionManagerRole.RightColl.Add(viewUserRight);
			abcContext.RoleSet.Add(regionManagerRole);
            // End regionManager
            // manager
            var managerRole = new Role()
            {
                Name = "Dirección",
                Description = "Rol para privilegios de Dirección",
                Active = true
            };
			managerRole.RightColl.Add(viewParamRight);
			managerRole.RightColl.Add(viewGroupRight);
			managerRole.RightColl.Add(viewPackageRight);
			managerRole.RightColl.Add(viewUserRight);
			abcContext.RoleSet.Add(managerRole);
			// End manager
			// End Adding Roles

            // Currency
            var myCurrency = new Currency()
            {
                Name = "USD",
                Description = "Dolar americano",
                Active = true
            };
            abcContext.CurrencySet.Add(myCurrency);

            var myCurrency1 = new Currency()
            {
                Name = "MX",
                Description ="Peso mexicano",
                Active = true
            };

            abcContext.CurrencySet.Add(myCurrency1);

			var myCurrency2 = new Currency()
			{
				Name = "EU",
				Description = "Euros",
				Active = true
			};

			abcContext.CurrencySet.Add(myCurrency2);
			//End Currency

			//Units 
			var unitna = new MeasureUnit()
			{
				Name = "NA",
				Description = "Descripción de NA"
			};
			abcContext.MeasureUnitSet.Add(unitna);

			var unitmgkg = new MeasureUnit()
            {
                Name = "mg/kg",
                Description = "Descripción de mg/kg"
			};
            abcContext.MeasureUnitSet.Add(unitmgkg);

            var unitnmp100ml = new MeasureUnit()
            {
                Name = "NMP/100 mL",
                Description = "Descripción de NMP/100 mL"
			};
            abcContext.MeasureUnitSet.Add(unitnmp100ml);

			var unit3 = new MeasureUnit()
			{
				Name = "Cualitativo",
				Description = "Descripción de Cualitativo"
			};
			abcContext.MeasureUnitSet.Add(unit3);

			var unitmgl = new MeasureUnit()
			{
				Name = "mg/L",
				Description = "Descripción de mg/L"
			};
			abcContext.MeasureUnitSet.Add(unitmgl);

			var unit5 = new MeasureUnit()
			{
				Name = "ug/L",
				Description = "Descripción de ug/L"
			};
			abcContext.MeasureUnitSet.Add(unit5);

			var unit6 = new MeasureUnit()
			{
				Name = "U Pt/Co",
				Description = "Descripción de U Pt/Co"
			};
			abcContext.MeasureUnitSet.Add(unit6);

			var unit7 = new MeasureUnit()
			{
				Name = "uS/cm",
				Description = "Descripción de uS/cm"
			};
			abcContext.MeasureUnitSet.Add(unit7);

			var unit8 = new MeasureUnit()
			{
				Name = "No. UMBRAL",
				Description = "Descripción de No. UMBRAL"
			};
			abcContext.MeasureUnitSet.Add(unit8);

			var unituph = new MeasureUnit()
			{
				Name = "UpH",
				Description = "Descripción de UpH"
			};
			abcContext.MeasureUnitSet.Add(unituph);

			var unit10 = new MeasureUnit()
			{
				Name = "UTN",
				Description = "Descripción de UTN"
			};
			abcContext.MeasureUnitSet.Add(unit10);

			var unit11 = new MeasureUnit()
			{
				Name = "UFC/mL",
				Description = "Descripción de UFC/mL"
			};
			abcContext.MeasureUnitSet.Add(unit11);

			var unitls = new MeasureUnit()
			{
				Name = "L/s",
				Description = "Descripción de L/s"
			};
			abcContext.MeasureUnitSet.Add(unitls);

			var unit13 = new MeasureUnit()
			{
				Name = "UFC/100mL",
				Description = "Descripción de UFC/100mL"
			};
			abcContext.MeasureUnitSet.Add(unit13);

			var unit14 = new MeasureUnit()
			{
				Name = "mg/L CaCO3",
				Description = "Descripción de mg/L CaCO3"
			};
			abcContext.MeasureUnitSet.Add(unit14);

			var unit15 = new MeasureUnit()
			{
				Name = "Bq/L",
				Description = "Descripción de Bq/L"
			};
			abcContext.MeasureUnitSet.Add(unit15);

			var unit16 = new MeasureUnit()
			{
				Name = "%",
				Description = "Descripción de %"
			};
			abcContext.MeasureUnitSet.Add(unit16);

			var unit17 = new MeasureUnit()
			{
				Name = "mg/kg",
				Description = "Descripción de mg/kg"
			};
			abcContext.MeasureUnitSet.Add(unit17);

			var unit18 = new MeasureUnit()
			{
				Name = "% AREA",
				Description = "Descripción de % AREA"
			};
			abcContext.MeasureUnitSet.Add(unit18);

			var unit19 = new MeasureUnit()
			{
				Name = "% PESO",
				Description = "Descripción de % PESO"
			};
			abcContext.MeasureUnitSet.Add(unit19);

			var unitgradoscelcius = new MeasureUnit()
			{
				Name = "°C",
				Description = "Descripción de °C"
			};
			abcContext.MeasureUnitSet.Add(unitgradoscelcius);

			var unit21 = new MeasureUnit()
			{
				Name = "UFC/cm2",
				Description = "Descripción de UFC/cm2"
			};
			abcContext.MeasureUnitSet.Add(unit21);

			var unitadimens = new MeasureUnit()
			{
				Name = "Adimens.",
				Description = "Descripción de Adimens."
			};
			abcContext.MeasureUnitSet.Add(unitadimens);

			var unit23 = new MeasureUnit()
			{
				Name = "mg/m3",
				Description = "Descripción de mg/m3"
			};
			abcContext.MeasureUnitSet.Add(unit23);

			var unit24 = new MeasureUnit()
			{
				Name = "g/mL",
				Description = "Descripción de g/mL"
			};
			abcContext.MeasureUnitSet.Add(unit24);

			var unitmll = new MeasureUnit()
			{
				Name = "mL/L",
				Description = "Descripción de mL/L"
			};
			abcContext.MeasureUnitSet.Add(unitmll);

			var unithhl = new MeasureUnit()
			{
				Name = "HH/L",
				Description = "Descripción de HH/L"
			};
			abcContext.MeasureUnitSet.Add(unithhl);
			//End Units

			//Clasificacion Quimica
			var cq1 = new ClasificacionQuimica()
	        {
		        Name = "FISICOQUIMICOS",
				Level = 1
	        };
	        abcContext.ClasificacionQuimicaSet.Add(cq1);

			var cq2 = new ClasificacionQuimica()
			{
				Name = "PARASITARIOS",
				Level = 1
			};
			abcContext.ClasificacionQuimicaSet.Add(cq2);

			var cq3 = new ClasificacionQuimica()
			{
				Name = "COMPUESTOS INORGÁNICOS",
				Level = 1
			};
			abcContext.ClasificacionQuimicaSet.Add(cq3);

			var cq4 = new ClasificacionQuimica()
			{
				Name = "BIOLÓGICOS",
				Level = 1
			};
			abcContext.ClasificacionQuimicaSet.Add(cq4);

			var cq5 = new ClasificacionQuimica()
			{
				Name = "MEDICIONES DIRECTAS",
				Level = 1
			};
			abcContext.ClasificacionQuimicaSet.Add(cq5);

			var cq6 = new ClasificacionQuimica()
			{
				Name = "MATERIA ORGÁNICA",
				Level = 2
			};
			abcContext.ClasificacionQuimicaSet.Add(cq6);

			var cq7 = new ClasificacionQuimica()
			{
				Name = "SÓLIDOS",
				Level = 2
			};
			abcContext.ClasificacionQuimicaSet.Add(cq7);

			var cq8 = new ClasificacionQuimica()
			{
				Name = "NUTRIENTES",
				Level = 2
			};
			abcContext.ClasificacionQuimicaSet.Add(cq8);

			var cq9 = new ClasificacionQuimica()
			{
				Name = "METALES",
				Level = 2
			};
			abcContext.ClasificacionQuimicaSet.Add(cq9);

			var cq10 = new ClasificacionQuimica()
			{
				Name = "MINERALES",
				Level = 2
			};
			abcContext.ClasificacionQuimicaSet.Add(cq10);

			//end Clasificacion Quimica

			// Market (Mercado)
			var market1 = new Market()
            {
                Name = "Medio Ambiente",
                Description = "Esta es la descripción del mercado Medio Ambiente"
            };
            abcContext.MarketSet.Add(market1);

            var market2 = new Market()
            {
                Name = "Emisiones a la Atmósfera y Ambiente Laboral",
                Description = "Esta es la descripción del mercado Emisiones a la Atmósfera y Ambiente Laboral"
            };
            abcContext.MarketSet.Add(market2);

            var market3 = new Market()
            {
                Name = "Alimentos",
                Description = "Esta es la descripción del mercado Alimentos"
            };
            abcContext.MarketSet.Add(market3);
            // end Market

            // Offices (Sucursales)
            var office1 = new Office()
            {
                Name = "ABC",
                Description = "Sucursal ABC, correspondiente al mercado Medio Ambiente",
                Market =  market1
            };
            abcContext.OfficeSet.Add(office1);

            var office2 = new Office()
            {
                Name = "Fermi",
                Description = "Sucursal Fermi, correspondiente al mercado Emisiones a la Atmósfera y Ambiente Laboral",
                Market = market2
            };
            abcContext.OfficeSet.Add(office2);

            var office3 = new Office()
            {
                Name = "GamaTec",
                Description = "Sucursal GamaTec, correspondiente al mercado Alimentos",
                Market = market3
            };
            abcContext.OfficeSet.Add(office3);
			// end Offices

			// Regions
	        var region1 = new Region()
	        {
		        Name = "Cd. de México",
		        Description = "Ciudad de México, DF",
		        Active = true
	        };
			region1.Offices.Add(office1);
			region1.Offices.Add(office2);
			abcContext.RegionSet.Add(region1);

			var region2 = new Region()
			{
				Name = "Guadalajara",
				Description = "Guadalajara",
				Active = true
			};
			region2.Offices.Add(office1);
			region2.Offices.Add(office2);
			abcContext.RegionSet.Add(region2);
			
			// end Regions

			// Sucursales
			var suc1 = new Sucursal()
	        {
				Name = "Mérida",
		        //Office = office1,
		        Region = region1,
		        Active = true,
				Vende = true,
				Realiza = true
			};
	        suc1.Offices.Add(office1);
			suc1.Offices.Add(office2);
	        abcContext.SucursalSet.Add(suc1);

			var suc2 = new Sucursal()
			{
				Name = "Cancún",
				//Office = office2,
				Region = region1,
				Active = true,
				Vende = true,
				Realiza = true
			};
			suc2.Offices.Add(office2);
			abcContext.SucursalSet.Add(suc2);

			var suc3 = new Sucursal()
			{
				Name = "Villahermosa",
				//Office = office2,
				Region = region2,
				Active = true,
				Realiza = true
			};
			suc3.Offices.Add(office2);
			abcContext.SucursalSet.Add(suc3);

			var suc4 = new Sucursal()
			{
				Name = "Jalisco",
				//Office = office1,
				Region = region2,
				Active = true,
				Vende = true,
				Realiza = true
			};
			suc4.Offices.Add(office1);
			abcContext.SucursalSet.Add(suc4);

			var suc5 = new Sucursal()
			{
				Name = "Alvaro Obregón",
				//Office = office1,
				Region = region1,
				Active = true,
				Vende = true,
				Realiza = true
			};
			suc5.Offices.Add(office1);
			abcContext.SucursalSet.Add(suc5);
			// end Sucursales

			// Adding users
			var adminUser1 = new User()
			{
				Name = "Ramón",
				LastNameFather = "Carrasco",
				UserName = "ramon",
				UserPassword = Cryptografy.CalcHash("R@m0n"),
				Active = true,
				Email = "ramoncd89@gmail.com",
				UserCreateDate = DateTime.Now,
				Phone = "+5352705111",
				Role = adminRole
			};
			adminUser1.Sucursales.Add(suc1);
			adminUser1.Sucursales.Add(suc2);
			adminUser1.Sucursales.Add(suc3);
			adminUser1.Sucursales.Add(suc4);
			adminUser1.Sucursales.Add(suc5);
			abcContext.UserSet.Add(adminUser1);

			var adminUser2 = new User()
			{
				Name = "Adiel",
				LastNameFather = "Miranda",
				UserName = "adiel",
				UserPassword = Cryptografy.CalcHash("Adi3l"),
				Active = true,
				Email = "adielm86@gmail.com",
				UserCreateDate = DateTime.Now,
				Phone = "+5353278303",
				Role = adminRole
			};
			adminUser2.Sucursales.Add(suc1);
			adminUser2.Sucursales.Add(suc2);
			adminUser2.Sucursales.Add(suc3);
			adminUser2.Sucursales.Add(suc4);
			adminUser2.Sucursales.Add(suc5);
			abcContext.UserSet.Add(adminUser2);

			var adminUser3 = new User()
			{
				Name = "Administrador",
				UserName = "admin",
				UserPassword = Cryptografy.CalcHash("admin"),
				Active = true,
				Email = "admin@mail.com",
				UserCreateDate = DateTime.Now,
				Phone = "+5555555555",
				Role = adminRole
			};
			adminUser3.Sucursales.Add(suc1);
			adminUser3.Sucursales.Add(suc2);
			adminUser3.Sucursales.Add(suc3);
			adminUser3.Sucursales.Add(suc4);
			adminUser3.Sucursales.Add(suc5);
			abcContext.UserSet.Add(adminUser3);

			var comertialUser = new User()
			{
				Name = "Comercial",
				UserName = "comercial",
				UserPassword = Cryptografy.CalcHash("comercial"),
				Active = true,
				Email = "comercial@mail.com",
				UserCreateDate = DateTime.Now,
				Phone = "+5556668862",
				Role = businessRole
			};
			comertialUser.Sucursales.Add(suc1);
			comertialUser.Sucursales.Add(suc2);
			comertialUser.Sucursales.Add(suc3);
			comertialUser.Sucursales.Add(suc4);
			comertialUser.Sucursales.Add(suc5);
			abcContext.UserSet.Add(comertialUser);
			
			var labtechUser = new User()
			{
				Name = "Técnico",
				LastNameFather = "de Laboratorio",
				UserName = "labtech",
				UserPassword = Cryptografy.CalcHash("labtech"),
				Active = true,
				Email = "labtech@mail.com",
				UserCreateDate = DateTime.Now,
				Phone = "+5556668863",
				Role = labtechRole
			};
			labtechUser.Sucursales.Add(suc1);
			labtechUser.Sucursales.Add(suc2);
			labtechUser.Sucursales.Add(suc3);
			labtechUser.Sucursales.Add(suc4);
			labtechUser.Sucursales.Add(suc5);
			abcContext.UserSet.Add(labtechUser);

			var sampletechUser = new User()
			{
				Name = "Técnico",
				LastNameFather = "de Muestreo",
				UserName = "sampletech",
				UserPassword = Cryptografy.CalcHash("sampletech"),
				Active = true,
				Email = "sampletech@mail.com",
				UserCreateDate = DateTime.Now,
				Phone = "+5556668864",
				Role = sampletechRole
			};
			sampletechUser.Sucursales.Add(suc1);
			sampletechUser.Sucursales.Add(suc2);
			sampletechUser.Sucursales.Add(suc3);
			sampletechUser.Sucursales.Add(suc4);
			sampletechUser.Sucursales.Add(suc5);
			abcContext.UserSet.Add(sampletechUser);
			
			var qualityUser = new User()
			{
				Name = "Calidad",
				UserName = "quality",
				UserPassword = Cryptografy.CalcHash("quality"),
				Active = true,
				Email = "quality@mail.com",
				UserCreateDate = DateTime.Now,
				Phone = "+5556668865",
				Role = qualityRole
			};
			qualityUser.Sucursales.Add(suc1);
			qualityUser.Sucursales.Add(suc2);
			qualityUser.Sucursales.Add(suc3);
			qualityUser.Sucursales.Add(suc4);
			qualityUser.Sucursales.Add(suc5);
			abcContext.UserSet.Add(qualityUser);

			var regionManagerUser = new User()
			{
				Name = "Gerente",
				LastNameFather = "de Región",
				UserName = "gerente",
				UserPassword = Cryptografy.CalcHash("gerente"),
				Active = true,
				Email = "gerente@mail.com",
				UserCreateDate = DateTime.Now,
				Phone = "+5556668866",
				Role = regionManagerRole
			};
			regionManagerUser.Sucursales.Add(suc1);
			regionManagerUser.Sucursales.Add(suc2);
			regionManagerUser.Sucursales.Add(suc3);
			regionManagerUser.Sucursales.Add(suc4);
			regionManagerUser.Sucursales.Add(suc5);
			abcContext.UserSet.Add(regionManagerUser);

			var managerUser = new User()
			{
				Name = "Manager",
				UserName = "manager",
				UserPassword = Cryptografy.CalcHash("manager"),
				Active = true,
				Email = "manager@mail.com",
				UserCreateDate = DateTime.Now,
				Phone = "+5556668867",
				Role = managerRole
			};
			managerUser.Sucursales.Add(suc1);
			managerUser.Sucursales.Add(suc2);
			managerUser.Sucursales.Add(suc3);
			managerUser.Sucursales.Add(suc4);
			managerUser.Sucursales.Add(suc5);
			abcContext.UserSet.Add(managerUser);
			// end Adding users

			//Reconocimientos
			// Accion
			var Accion1 = new Accion()
            {
                Name = "Categoría 1",
				Description = "Ejemplo: Acreditado"
			};
            abcContext.AccionSet.Add(Accion1);

            var Accion2 = new Accion()
            {
				Name = "Categoría 2",
				Description = "Ejemplo: Aprobado"
			};
            abcContext.AccionSet.Add(Accion2);

            var Accion3 = new Accion()
            {
				Name = "Categoría 3",
				Description = "Ejemplo: Autorizado"
			};
            abcContext.AccionSet.Add(Accion3);

            var Accion4 = new Accion()
            {
				Name = "Categoría 4",
				Description = "Ejemplo: Distrito Federal"
			};
            abcContext.AccionSet.Add(Accion4);

            var Accion5 = new Accion()
            {
				Name = "Categoría 5",
				Description = "Ejemplo: Baja California"
			};
            abcContext.AccionSet.Add(Accion5);

            var Accion6 = new Accion()
            {
				Name = "Categoría 6",
				Description = "Ejemplo: Nuevo León"
            };
            abcContext.AccionSet.Add(Accion6);
			// end Accion

			// Enterprises
	        var ent1 = new Enterprise()
	        {
		        Name = "Institución 1 e",
		        Description = "Ejemplo: EMA1",
				Active = true,
				Tipo = true
			};
	        abcContext.EnterpriseSet.Add(ent1);

			var ent1I = new Enterprise()
			{
				Name = "Institución 1 i",
				Description = "Ejemplo: EMA1",
				Active = true
			};
			abcContext.EnterpriseSet.Add(ent1I);

			var ent2 = new Enterprise()
			{
				Name = "Institución 2 e",
				Description = "Ejemplo: CONAGUA1",
				Active = true,
				Tipo = true
			};
			abcContext.EnterpriseSet.Add(ent2);

			var ent2I = new Enterprise()
			{
				Name = "Institución 2 i",
				Description = "Ejemplo: CONAGUA1",
				Active = true,
				Tipo = true
			};
			abcContext.EnterpriseSet.Add(ent2I);

			var ent12 = new Enterprise()
			{
				Name = "Institución 3 e",
				Description = "Ejemplo: PADLA1",
				Active = true,
				Tipo = true
			};
			abcContext.EnterpriseSet.Add(ent12);

			var ent12I = new Enterprise()
			{
				Name = "Institución 3 i",
				Description = "Ejemplo: PADLA1",
				Active = true
			};
			abcContext.EnterpriseSet.Add(ent12I);

			var ent11 = new Enterprise()
			{
				Name = "Institución 4 e",
				Description = "Ejemplo: COFREPIS",
				Active = true,
				Tipo = true
			};
			abcContext.EnterpriseSet.Add(ent11);

			var ent11I = new Enterprise()
			{
				Name = "Institución 4 i",
				Description = "Ejemplo: COFREPIS",
				Active = true
			};
			abcContext.EnterpriseSet.Add(ent11I);

			var ent17 = new Enterprise()
			{
				Name = "Institución 5 e",
				Description = "Ejemplo: EMA2",
				Active = true,
				Tipo = true
			};
			abcContext.EnterpriseSet.Add(ent17);

			var ent17I = new Enterprise()
			{
				Name = "Institución 5 i",
				Description = "Ejemplo: EMA2",
				Active = true
			};
			abcContext.EnterpriseSet.Add(ent17I);

			var ent7 = new Enterprise()
			{
				Name = "Institución 6 e",
				Description = "Ejemplo: EMA3",
				Active = true,
				Tipo = true
			};
			abcContext.EnterpriseSet.Add(ent7);

			var ent7I = new Enterprise()
			{
				Name = "Institución 6 i",
				Description = "Ejemplo: EMA3",
				Active = true
			};
			abcContext.EnterpriseSet.Add(ent7I);

			var entA = new Enterprise()
			{
				Name = "Institución 7 e",
				Description = "Ejemplo: CONAGUA2",
				Active = true,
				Tipo = true
			};
			abcContext.EnterpriseSet.Add(entA);

			var entAI = new Enterprise()
			{
				Name = "Institución 7 i",
				Description = "Ejemplo: CONAGUA2",
				Active = true
			};
			abcContext.EnterpriseSet.Add(entAI);

			var entB = new Enterprise()
			{
				Name = "Institución 8 e",
				Description = "Ejemplo: CONAGUA3",
				Active = true,
				Tipo = true
			};
			abcContext.EnterpriseSet.Add(entB);

			var entBI = new Enterprise()
			{
				Name = "Institución 8 i",
				Description = "Ejemplo: CONAGUA3",
				Active = true
			};
			abcContext.EnterpriseSet.Add(entBI);
			// end Enterprises

			// Acknowledgemnts (Reconocimientos)
			//var ack0 = new Ack()
			//{
			//	Name = "EIDAS",
			//	Description = "Reconocimientos Internos",
			//	Key = "0"
			//};
			//abcContext.AckSet.Add(ack0);

			var ack1 = new Ack()
			{
				Name = "Reconocimientos 1 E",
				Description = "Ejemplo: Reconocimientos Federales",
				Key = "1",
				Accion = Accion1,
				Enterprise = ent1
			};
			abcContext.AckSet.Add(ack1);

			var ack1I = new Ack()
			{
				Name = "Reconocimientos 1 I",
				Description = "Ejemplo: Reconocimientos Federales",
				Key = "1i",
				Accion = Accion1,
				Enterprise = ent1I
			};
			abcContext.AckSet.Add(ack1I);

			var ack2 = new Ack()
			{
				Name = "Reconocimientos 2 E",
				Description = "Ejemplo: Reconocimientos Locales-Regionales",
				Key = "2",
				Accion = Accion2,
				Enterprise = ent2
			};
			abcContext.AckSet.Add(ack2);

			var ack2I = new Ack()
			{
				Name = "Reconocimientos 2 I",
				Description = "Ejemplo: Reconocimientos Locales-Regionales",
				Key = "2i",
				Accion = Accion2,
				Enterprise = ent2I
			};
			abcContext.AckSet.Add(ack2I);

			var ack12 = new Ack()
			{
				Name = "Reconocimientos 12 E",
				Description = "Ejemplo: Reconocimientos Locales-Regionales 2",
				Key = "12",
				Accion = Accion3,
				Enterprise = ent12
			};
			abcContext.AckSet.Add(ack12);

			var ack12I = new Ack()
			{
				Name = "Reconocimientos 12 I",
				Description = "Ejemplo: Reconocimientos Locales-Regionales 2",
				Key = "12i",
				Accion = Accion3,
				Enterprise = ent12I
			};
			abcContext.AckSet.Add(ack12I);

			var ack11 = new Ack()
			{
				Name = "Reconocimientos 11 E",
				Description = "Ejemplo: Reconocimientos Federales 2",
				Key = "11",
				Accion = Accion4,
				Enterprise = ent11
			};
			abcContext.AckSet.Add(ack11);

			var ack11I = new Ack()
			{
				Name = "Reconocimientos 11 I",
				Description = "Ejemplo: Reconocimientos Federales 2",
				Key = "11i",
				Accion = Accion4,
				Enterprise = ent11I
			};
			abcContext.AckSet.Add(ack11I);

			var ack17 = new Ack()
			{
				Name = "Reconocimientos 17 E",
				Description = "Ejemplo: Reconocimientos Federales 3",
				Key = "17",
				Accion = Accion5,
				Enterprise = ent17
			};
			abcContext.AckSet.Add(ack17);

			var ack17I = new Ack()
			{
				Name = "Reconocimientos 17 I",
				Description = "Ejemplo: Reconocimientos Federales 3",
				Key = "17i",
				Accion = Accion5,
				Enterprise = ent17I
			};
			abcContext.AckSet.Add(ack17I);

			var ack7 = new Ack()
			{
				Name = "Reconocimientos 7 E",
				Description = "Ejemplo: Reconocimientos Federales 4",
				Key = "7",
				Accion = Accion6,
				Enterprise = ent7
			};
			abcContext.AckSet.Add(ack7);

			var ack7I = new Ack()
			{
				Name = "Reconocimientos 7 I",
				Description = "Ejemplo: Reconocimientos Federales 4",
				Key = "7i",
				Accion = Accion6,
				Enterprise = ent7I
			};
			abcContext.AckSet.Add(ack7I);

			var ackA = new Ack()
			{
				Name = "Reconocimientos A E",
				Description = "Ejemplo: Otros Reconocimientos 1",
				Key = "A",
				Accion = Accion1,
				Enterprise = entA
			};
			abcContext.AckSet.Add(ackA);

			var ackAI = new Ack()
			{
				Name = "Reconocimientos A I",
				Description = "Ejemplo: Otros Reconocimientos 1",
				Key = "Ai",
				Accion = Accion1,
				Enterprise = entAI
			};
			abcContext.AckSet.Add(ackAI);

			var ackB = new Ack()
			{
				Name = "Reconocimientos B E",
				Description = "Ejemplo: Otros Reconocimientos 2",
				Key = "B",
				Accion = Accion2,
				Enterprise = entB
			};
			abcContext.AckSet.Add(ackB);

			var ackBI = new Ack()
			{
				Name = "Reconocimientos B I",
				Description = "Ejemplo: Otros Reconocimientos 2",
				Key = "Bi",
				Accion = Accion2,
				Enterprise = entBI
			};
			abcContext.AckSet.Add(ackBI);
			// end Ack

			// Centro Costo
			var cc00075 = new CentroCosto()
	        {
				Number = "00075",
				Tipo = CentroCosto.TipoCentroCosto.Mixto
			};
	        abcContext.CentroCostoSet.Add(cc00075);

	        var aaAO = new AnalyticsArea()
	        {
		        Key = "AO",
		        Sucursal = suc5,
				CentroCosto = cc00075
	        };
            abcContext.AnalyticsAreaSet.Add(aaAO);

			var cc044 = new CentroCosto()
			{
				Number = "044",
				Tipo = CentroCosto.TipoCentroCosto.Mixto
			};
			abcContext.CentroCostoSet.Add(cc044);

			var aaAP = new AnalyticsArea()
			{
				Key = "AP",
				Sucursal = suc5,
				CentroCosto = cc044
			};
			abcContext.AnalyticsAreaSet.Add(aaAP);

			var cc023 = new CentroCosto()
			{
				Number = "023",
				Tipo = CentroCosto.TipoCentroCosto.Mixto
			};
			abcContext.CentroCostoSet.Add(cc023);

			var aaAR = new AnalyticsArea()
			{
				Key = "AR",
				Sucursal = suc5,
				CentroCosto = cc023
			};
			abcContext.AnalyticsAreaSet.Add(aaAR);
			
			var cc123 = new CentroCosto()
			{
				Number = "123",
				Tipo = CentroCosto.TipoCentroCosto.Mixto
			};
			abcContext.CentroCostoSet.Add(cc123);

			var aaJA = new AnalyticsArea()
			{
				Key = "JA",
				Sucursal = suc4,
				CentroCosto = cc123
			};
			abcContext.AnalyticsAreaSet.Add(aaJA);

			var cc027 = new CentroCosto()
			{
				Number = "027",
				Tipo = CentroCosto.TipoCentroCosto.Mixto
			};
			abcContext.CentroCostoSet.Add(cc027);

			var aaAI = new AnalyticsArea()
			{
				Key = "AI",
				Sucursal = suc5,
				CentroCosto = cc027
			};
			abcContext.AnalyticsAreaSet.Add(aaAI);

			var cc022 = new CentroCosto()
			{
				Number = "022",
				Tipo = CentroCosto.TipoCentroCosto.Mixto
			};
			abcContext.CentroCostoSet.Add(cc022);

			var aaAG = new AnalyticsArea()
			{
				Key = "AG",
				Sucursal = suc5,
				CentroCosto = cc022
			};
			abcContext.AnalyticsAreaSet.Add(aaAG);

			var cc021 = new CentroCosto()
			{
				Number = "021",
				Tipo = CentroCosto.TipoCentroCosto.Mixto
			};
			abcContext.CentroCostoSet.Add(cc021);

			var aaAH = new AnalyticsArea()
			{
				Key = "AH",
				Sucursal = suc5,
				CentroCosto = cc021
			};
			abcContext.AnalyticsAreaSet.Add(aaAH);

			var cc026 = new CentroCosto()
			{
				Number = "026",
				Tipo = CentroCosto.TipoCentroCosto.Mixto
			};
			abcContext.CentroCostoSet.Add(cc026);

			var aaAK = new AnalyticsArea()
			{
				Key = "AK",
				Sucursal = suc5,
				CentroCosto = cc026
			};
			abcContext.AnalyticsAreaSet.Add(aaAK);

			var cc031 = new CentroCosto()
			{
				Number = "031",
				Tipo = CentroCosto.TipoCentroCosto.Mixto
			};
			abcContext.CentroCostoSet.Add(cc031);

			var aaAX = new AnalyticsArea()
			{
				Key = "AX",
				Sucursal = suc5,
				CentroCosto = cc031
			};
			abcContext.AnalyticsAreaSet.Add(aaAX);
			// end Analytics Area

			// Annalists
			var jiu = new Annalist()
	        {
				Name = "J.",
				LastNameFather = "I.",
				LastNameMother = "U.",
				Gender = false,
				Active = true,
				Phone = "+5352489755",
				Email = "jiu@mail.com",
				Key = "JIU"
	        };
			jiu.Sucursales.Add(suc4);
			jiu.Sucursales.Add(suc5);

	        abcContext.AnnalistSet.Add(jiu);

			var ats = new Annalist()
			{
				Name = "A.",
				LastNameFather = "T.",
				LastNameMother = "S.",
				Gender = false,
				Active = true,
				Phone = "+5352489756",
				Email = "ats@mail.com",
				Key = "ATS"
			};
			ats.Sucursales.Add(suc5);
			ats.Sucursales.Add(suc4);

			abcContext.AnnalistSet.Add(ats);

			var rrp = new Annalist()
			{
				Name = "R.",
				LastNameFather = "R.",
				LastNameMother = "P.",
				Gender = false,
				Active = true,
				Phone = "+5352489757",
				Email = "rrp@mail.com",
				Key = "RRP"
			};
			rrp.Sucursales.Add(suc5);

			abcContext.AnnalistSet.Add(rrp);

			var vsm = new Annalist()
			{
				Name = "V.",
				LastNameFather = "S.",
				LastNameMother = "M.",
				Gender = false,
				Active = true,
				Phone = "+5352489757",
				Email = "vsm@mail.com",
				Key = "VSM"
			};
			vsm.Sucursales.Add(suc5);

			abcContext.AnnalistSet.Add(vsm);

			var erd = new Annalist()
			{
				Name = "E.",
				LastNameFather = "R.",
				LastNameMother = "D.",
				Gender = false,
				Active = true,
				Phone = "+5352489757",
				Email = "erd@mail.com",
				Key = "ERD"
			};
			erd.Sucursales.Add(suc5);

			abcContext.AnnalistSet.Add(erd);

			var vpr = new Annalist()
			{
				Name = "V.",
				LastNameFather = "P.",
				LastNameMother = "R.",
				Gender = false,
				Active = true,
				Phone = "+5352489757",
				Email = "vpr@mail.com",
				Key = "VPR"
			};
			vpr.Sucursales.Add(suc5);

			abcContext.AnnalistSet.Add(vpr);

			var clr = new Annalist()
			{
				Name = "C.",
				LastNameFather = "L.",
				LastNameMother = "R.",
				Gender = false,
				Active = true,
				Phone = "+5352489757",
				Email = "clr@mail.com",
				Key = "CLR"
			};
			clr.Sucursales.Add(suc4);

			abcContext.AnnalistSet.Add(clr);

			var lbj = new Annalist()
			{
				Name = "L.",
				LastNameFather = "B.",
				LastNameMother = "J.",
				Gender = false,
				Active = true,
				Phone = "+5352489757",
				Email = "lbj@mail.com",
				Key = "LBJ"
			};
			lbj.Sucursales.Add(suc4);

			abcContext.AnnalistSet.Add(lbj);

			var gar = new Annalist()
			{
				Name = "G.",
				LastNameFather = "A.",
				LastNameMother = "R.",
				Gender = false,
				Active = true,
				Phone = "+5352489757",
				Email = "gar@mail.com",
				Key = "GAR"
			};
			gar.Sucursales.Add(suc4);

			abcContext.AnnalistSet.Add(gar);

			var ces = new Annalist()
			{
				Name = "C.",
				LastNameFather = "E.",
				LastNameMother = "S.",
				Gender = false,
				Active = true,
				Phone = "+5352489757",
				Email = "ces@mail.com",
				Key = "CES"
			};
			ces.Sucursales.Add(suc4);

			abcContext.AnnalistSet.Add(ces);

			var epg = new Annalist()
			{
				Name = "E.",
				LastNameFather = "P.",
				LastNameMother = "G.",
				Gender = false,
				Active = true,
				Phone = "+5352489757",
				Email = "epg@mail.com",
				Key = "EPG"
			};
			epg.Sucursales.Add(suc5);

			abcContext.AnnalistSet.Add(epg);

			var cir = new Annalist()
			{
				Name = "C.",
				LastNameFather = "I.",
				LastNameMother = "R.",
				Gender = false,
				Active = true,
				Phone = "+5352489757",
				Email = "cir@mail.com",
				Key = "CIR"
			};
			cir.Sucursales.Add(suc5);

			abcContext.AnnalistSet.Add(cir);

			var apg = new Annalist()
			{
				Name = "A.",
				LastNameFather = "P.",
				LastNameMother = "G.",
				Gender = false,
				Active = true,
				Phone = "+5352489757",
				Email = "apg@mail.com",
				Key = "APG"
			};
			apg.Sucursales.Add(suc5);

			abcContext.AnnalistSet.Add(apg);

			var jcr = new Annalist()
			{
				Name = "J.",
				LastNameFather = "C.",
				LastNameMother = "R.",
				Gender = false,
				Active = true,
				Phone = "+5352489757",
				Email = "jcr@mail.com",
				Key = "JCR"
			};
			jcr.Sucursales.Add(suc5);

			abcContext.AnnalistSet.Add(jcr);

			var ivr = new Annalist()
			{
				Name = "I.",
				LastNameFather = "V.",
				LastNameMother = "R.",
				Gender = false,
				Active = true,
				Phone = "+5352489757",
				Email = "ivr@mail.com",
				Key = "IVR"
			};
			ivr.Sucursales.Add(suc5);

			abcContext.AnnalistSet.Add(ivr);

			var ctr = new Annalist()
			{
				Name = "C.",
				LastNameFather = "T.",
				LastNameMother = "R.",
				Gender = false,
				Active = true,
				Phone = "+5352489757",
				Email = "ctr@mail.com",
				Key = "CTR"
			};
			ctr.Sucursales.Add(suc5);

			abcContext.AnnalistSet.Add(ctr);

			var amp = new Annalist()
			{
				Name = "A.",
				LastNameFather = "M.",
				LastNameMother = "P.",
				Gender = false,
				Active = true,
				Phone = "+5352489757",
				Email = "amp@mail.com",
				Key = "AMP"
			};
			amp.Sucursales.Add(suc5);

			abcContext.AnnalistSet.Add(amp);

			var cel = new Annalist()
			{
				Name = "C.",
				LastNameFather = "E.",
				LastNameMother = "L.",
				Gender = false,
				Active = true,
				Phone = "+5352489757",
				Email = "cel@mail.com",
				Key = "CEL"
			};
			cel.Sucursales.Add(suc5);

			abcContext.AnnalistSet.Add(cel);

			var dvg = new Annalist()
			{
				Name = "D.",
				LastNameFather = "V.",
				LastNameMother = "G.",
				Gender = false,
				Active = true,
				Phone = "+5352489757",
				Email = "dvg@mail.com",
				Key = "DVG"
			};
			dvg.Sucursales.Add(suc5);

			abcContext.AnnalistSet.Add(dvg);

			var mph = new Annalist()
			{
				Name = "M.",
				LastNameFather = "P.",
				LastNameMother = "H.",
				Gender = false,
				Active = true,
				Phone = "+5352489757",
				Email = "mph@mail.com",
				Key = "MPH"
			};
			mph.Sucursales.Add(suc5);

			abcContext.AnnalistSet.Add(mph);

			var jol = new Annalist()
			{
				Name = "J.",
				LastNameFather = "O.",
				LastNameMother = "L.",
				Gender = false,
				Active = true,
				Phone = "+5352489757",
				Email = "cel@mail.com",
				Key = "JOL"
			};
			jol.Sucursales.Add(suc5);

			abcContext.AnnalistSet.Add(jol);
			// end Annalists

			// Analytics Method
			var am1 = new AnalyticsMethod()
			{
				Active = true,
				Name = "POLAROGRAFÍA",
				Description = "Descripción de la técnica  Polarografía"
			};
			abcContext.AnalyticsMethodSet.Add(am1);

			var am2 = new AnalyticsMethod()
			{
				Active = true,
				Name = "OBS. MICROSCÓPICA",
				Description = "Descripción de la técnica OBS. MICROSCÓPICA"
			};
			abcContext.AnalyticsMethodSet.Add(am2);

			var am3 = new AnalyticsMethod()
			{
				Active = true,
				Name = "VOLUMETRÍA",
				Description = "Descripción de la técnica VOLUMETRÍA"
			};
			abcContext.AnalyticsMethodSet.Add(am3);

			var am4 = new AnalyticsMethod()
			{
				Active = true,
				Name = "GRAVIMETRÍA",
				Description = "Descripción de la técnica GRAVIMETRÍA"
			};
			abcContext.AnalyticsMethodSet.Add(am4);

			var am5 = new AnalyticsMethod()
			{
				Active = true,
				Name = "FIAS/ ESPECTR. VIS",
				Description = "Descripción de la técnica FIAS/ ESPECTR. VIS"
			};
			abcContext.AnalyticsMethodSet.Add(am5);

			var am6 = new AnalyticsMethod()
			{
				Active = true,
				Name = "ICP/AES",
				Description = "Descripción de la técnica ICP/AES"
			};
			abcContext.AnalyticsMethodSet.Add(am6);

			var am7 = new AnalyticsMethod()
			{
				Active = true,
				Name = "AAE/VF/FIAS",
				Description = "Descripción de la técnica AAE/VF/FIAS"
			};
			abcContext.AnalyticsMethodSet.Add(am7);

			var am8 = new AnalyticsMethod()
			{
				Active = true,
				Name = "HORNO MICROONDAS",
				Description = "Descripción de la técnica HORNO MICROONDAS"
			};
			abcContext.AnalyticsMethodSet.Add(am8);

			var am9 = new AnalyticsMethod()
			{
				Active = true,
				Name = "NMP",
				Description = "Descripción de la técnica NMP"
			};
			abcContext.AnalyticsMethodSet.Add(am9);

			var am10 = new AnalyticsMethod()
			{
				Active = true,
				Name = "CÁLCULO",
				Description = "Descripción de la técnica CÁLCULO"
			};
			abcContext.AnalyticsMethodSet.Add(am10);

			var am11 = new AnalyticsMethod()
			{
				Active = true,
				Name = "MEDICIÓN DIRECTA",
				Description = "Descripción de la técnica MEDICIÓN DIRECTA"
			};
			abcContext.AnalyticsMethodSet.Add(am11);

			var am12 = new AnalyticsMethod()
			{
				Active = true,
				Name = "OBSERVACIÓN DIRECTA",
				Description = "Descripción de la técnica OBSERVACIÓN DIRECTA"
			};
			abcContext.AnalyticsMethodSet.Add(am12);

			var am13 = new AnalyticsMethod()
			{
				Active = true,
				Name = "POTENCIOMETRÍA",
				Description = "Descripción de la técnica POTENCIOMETRÍA"
			};
			abcContext.AnalyticsMethodSet.Add(am13);

			var am14 = new AnalyticsMethod()
			{
				Active = true,
				Name = "TERMOMÉTRICA",
				Description = "Descripción de la técnica TERMOMÉTRICA"
			};
			abcContext.AnalyticsMethodSet.Add(am14);
			// end Analytics Method

			// Material
			var mat1 = new Material()
			{
				Name = "Plático",
				Description = "Apropiado para contenedores de Plático",
				Active = true
			};
			abcContext.MaterialSet.Add(mat1);

			var mat2 = new Material()
			{
				Name = "Plástico Estéril",
				Description = "Apropiado para contenedores de Plástico Estéril",
				Active = true
			};
			abcContext.MaterialSet.Add(mat2);

			var mat3 = new Material()
			{
				Name = "Plástico",
				Description = "Apropiado para contenedores de Plástico",
				Active = true
			};
			abcContext.MaterialSet.Add(mat3);

			var mat4 = new Material()
			{
				Name = "Vidrio Transparente",
				Description = "Apropiado para contenedores de Vidrio Transparente",
				Active = true
			};
			abcContext.MaterialSet.Add(mat4);
			// end Material

			// Container
			var cont1 = new Container()
			{
				Active = true,
				Name = "Plático",
				Description = "Descripción del Contenedor de Plático",
				Material = mat1
			};
			abcContext.ContainerSet.Add(cont1);

			var cont2 = new Container()
			{
				Active = true,
				Name = "Plástico Estéril",
				Description = "Descripción del Contenedor de Plástico Estéril",
				Material = mat2
			};
			abcContext.ContainerSet.Add(cont2);

			var cont3 = new Container()
			{
				Active = true,
				Name = "Plástico",
				Description = "Descripción del Contenedor de Plástico",
				Material = mat3
			};
			abcContext.ContainerSet.Add(cont3);

			var cont4 = new Container()
			{
				Active = true,
				Name = "Vidrio Transparente",
				Description = "Descripción del Contenedor Vidrio Transparente",
				Material = mat4
			};
			abcContext.ContainerSet.Add(cont4);

			var cont5 = new Container()
			{
				Active = true,
				Name = "BOLSA LIMPIA O EMPAQUE ORIGINAL",
				Description = "Descripción del Contenedor BOLSA LIMPIA O EMPAQUE ORIGINAL",
				Material = mat2
			};
			abcContext.ContainerSet.Add(cont5);

			var cont6 = new Container()
			{
				Active = true,
				Name = "FRASCO DE VIDRIO",
				Description = "Descripción del Contenedor FRASCO DE VIDRIO",
				Material = mat4
			};
			abcContext.ContainerSet.Add(cont6);

			var cont7 = new Container()
			{
				Active = true,
				Name = "VIALES DE 40mL(VOLUMEN CERO) TAPA DE TEFLON",
				Description = "Descripción del Contenedor VIALES DE 40mL(VOLUMEN CERO) TAPA DE TEFLON",
				Material = mat3
			};
			abcContext.ContainerSet.Add(cont7);

			var cont8 = new Container()
			{
				Active = true,
				Name = "PORTAFILTRO",
				Description = "Descripción del Contenedor PORTAFILTRO",
				Material = mat3
			};
			abcContext.ContainerSet.Add(cont8);

			var cont9 = new Container()
			{
				Active = true,
				Name = "CARTUCHO DE CARTON ACTIVADO CON 400 Y 200 mg",
				Description = "Descripción del Contenedor CARTUCHO DE CARTON ACTIVADO CON 400 Y 200 mg",
				Material = mat3
			};
			abcContext.ContainerSet.Add(cont9);

			var cont10 = new Container()
			{
				Active = true,
				Name = "CARTUCHO XAD - 70VS DE 200 / 100 mg",
				Description = "Descripción del Contenedor CARTUCHO XAD - 70VS DE 200 / 100 mg",
				Material = mat2
			};
			abcContext.ContainerSet.Add(cont10);

			var cont11 = new Container()
			{
				Active = true,
				Name = "BOLSA TEDLAR",
				Description = "Descripción del Contenedor BOLSA TEDLAR",
				Material = mat3
			};
			abcContext.ContainerSet.Add(cont11);

			var cont12 = new Container()
			{
				Active = true,
				Name = "POLIETILENO",
				Description = "Descripción del Contenedor POLIETILENO",
				Material = mat3
			};
			abcContext.ContainerSet.Add(cont12);
			// end Container

			// Residue
			var res1 = new Residue()
			{
				Active = true,
				Name = "Residuo 1",
				Description = "Descripción del Residuo 1"
			};
			abcContext.ResidueSet.Add(res1);
			// end Residue

			// Preserver
			var pres1 = new Preserver()
			{
				Active = true,
				Name = "FRIO",
				Description = "Descripción del Preservador FRIO"
			};
			abcContext.PreserverSet.Add(pres1);

			var pres2 = new Preserver()
			{
				Active = true,
				Name = "ÁCIDO NÍTRICO",
				Description = "Descripción del Preservador HNO3"
			};
			abcContext.PreserverSet.Add(pres2);

			var pres3 = new Preserver()
			{
				Active = true,
				Name = "FRIO 4°C",
				Description = "Descripción del Preservador FRIO 4°C"
			};
			abcContext.PreserverSet.Add(pres3);

			var pres4 = new Preserver()
			{
				Active = true,
				Name = "H2SO4, pH<2",
				Description = "Descripción del Preservador H2SO4, pH<2"
			};
			abcContext.PreserverSet.Add(pres4);

			var pres5 = new Preserver()
			{
				Active = true,
				Name = "ÁCIDO SULFÚRICO",
				Description = "Descripción del Preservador HSO4"
			};
			abcContext.PreserverSet.Add(pres5);

			var pres6 = new Preserver()
			{
				Active = true,
				Name = "pH < 2",
				Description = "Descripción del Preservador pH < 2"
			};
			abcContext.PreserverSet.Add(pres6);

			var pres7 = new Preserver()
			{
				Active = true,
				Name = "ÁCIDO CLORHÍDRICO",
				Description = "Descripción del Preservador HNO3"
			};
			abcContext.PreserverSet.Add(pres7);

			var pres8 = new Preserver()
			{
				Active = true,
				Name = "4 °C",
				Description = "Descripción del Preservador 4 °C"
			};
			abcContext.PreserverSet.Add(pres8);

			var pres9 = new Preserver()
			{
				Active = true,
				Name = "<=5 °C",
				Description = "Descripción del Preservador <=5 °C"
			};
			abcContext.PreserverSet.Add(pres9);

			var pres10 = new Preserver()
			{
				Active = true,
				Name = "BOLSA TEDLAR",
				Description = "Descripción del Preservador BOLSA TEDLAR"
			};
			abcContext.PreserverSet.Add(pres10);

			var pres11 = new Preserver()
			{
				Active = true,
				Name = "LUGAR FRESCO Y SECO",
				Description = "Descripción del Preservador LUGAR FRESCO Y SECO"
			};
			abcContext.PreserverSet.Add(pres11);

			var pres13 = new Preserver()
			{
				Active = true,
				Name = "NO REQUIERE",
				Description = "NO REQUIERE"
			};
			abcContext.PreserverSet.Add(pres13);

			var pres14 = new Preserver()
			{
				Active = true,
				Name = "HNO3 pH<2",
				Description = "Descripción del Preservador HNO3 pH<2"
			};
			abcContext.PreserverSet.Add(pres14);

			var pres15 = new Preserver()
			{
				Active = true,
				Name = "NINGUNO",
				Description = "NINGUNO"
			};
			abcContext.PreserverSet.Add(pres15);

			var pres16 = new Preserver()
			{
				Active = true,
				Name = "ACIDO NITRICO",
				Description = "Descripción del Preservador ACIDO NITRICO"
			};
			abcContext.PreserverSet.Add(pres16);
			// end Preserver

			// Base Matrixes
			var bm1 = new BaseMatrix()
			{
				Active = true,
				Name = "AGUAS",
				Mercado = market1
			};
			abcContext.BaseMatrixSet.Add(bm1);

			var bm2 = new BaseMatrix()
			{
				Active = true,
				Name = "AGUA PARA USO Y CONSUMO HUMANO",
				Mercado = market1
			};
			abcContext.BaseMatrixSet.Add(bm2);

			var bm3 = new BaseMatrix()
			{
				Active = true,
				Name = "ALIMENTOS",
				Mercado = market3
			};
			abcContext.BaseMatrixSet.Add(bm3);

			var bm4 = new BaseMatrix()
			{
				Active = true,
				Name = "BEBIDAS NO ALCOHOLICAS",
				Mercado = market1
			};
			abcContext.BaseMatrixSet.Add(bm4);

			var bm5 = new BaseMatrix()
			{
				Active = true,
				Name = "MATERIAS PRIMAS",
				Mercado = market1
			};
			abcContext.BaseMatrixSet.Add(bm5);

			var bm7 = new BaseMatrix()
			{
				Active = true,
				Name = "SUPERFICIES VIVAS / INERTES",
				Mercado = market1
			};
			abcContext.BaseMatrixSet.Add(bm7);

			var bm8 = new BaseMatrix()
			{
				Active = true,
				Name = "AGUA POTABLE",
				Mercado = market2
			};
			abcContext.BaseMatrixSet.Add(bm8);

			var bm9 = new BaseMatrix()
			{
				Active = true,
				Name = "AGUAS NATURALES, RESIDUALES, SALOBRES Y DE MAR",
				Mercado = market3
			};
			abcContext.BaseMatrixSet.Add(bm9);

			var bm10 = new BaseMatrix()
			{
				Active = true,
				Name = "AIRE AMBIENTE",
				Mercado = market1
			};
			abcContext.BaseMatrixSet.Add(bm10);

			var bm11 = new BaseMatrix()
			{
				Active = true,
				Name = "AMBIENTE LABORAL",
				Mercado = market2
			};
			abcContext.BaseMatrixSet.Add(bm11);

			var bm12 = new BaseMatrix()
			{
				Active = true,
				Name = "ARTICULOS DE CERAMICA VIDRIADA",
				Mercado = market3
			};
			abcContext.BaseMatrixSet.Add(bm12);

			var bm13 = new BaseMatrix()
			{
				Active = true,
				Name = "BIOGAS",
				Mercado = market1
			};
			abcContext.BaseMatrixSet.Add(bm13);

			var bm14 = new BaseMatrix()
			{
				Active = true,
				Name = "BIOTA VEGETAL",
				Mercado = market2
			};
			abcContext.BaseMatrixSet.Add(bm14);

			var bm15 = new BaseMatrix()
			{
				Active = true,
				Name = "COMBUSTIBLES",
				Mercado = market1
			};
			abcContext.BaseMatrixSet.Add(bm15);

			var bm16 = new BaseMatrix()
			{
				Active = true,
				Name = "EMISIONES A LA ATMOSFERA",
				Mercado = market3
			};
			abcContext.BaseMatrixSet.Add(bm16);
			// end Base Matrixes

			// Matrixes
			var matrixAguasResiduales = new Matrix()
			{
				Active = true,
				MatrixCreateDate = DateTime.Now,
				Name = "AGUAS RESIDUALES",
				BaseMatrix = bm1,
				Description = "AR",
				SubMatrix = "INDUSTRIALES",
				SubMtrxDescription = "IN",
				Sucursal = suc5
			};
			abcContext.MatrixSet.Add(matrixAguasResiduales);

			var matrix2 = new Matrix()
			{
				Active = true,
				MatrixCreateDate = DateTime.Now,
				Name = "AGUA PARA USO Y CONSUMO HUMANO",
				SubMatrix = "EMBOTELLADA",
				SubMtrxDescription = "EM",
				BaseMatrix = bm2,
				Description = "AP"
				//Sucursal = suc3
			};
			abcContext.MatrixSet.Add(matrix2);

			var matrix3 = new Matrix()
			{
				Active = true,
				Name = "MATERIA PRIMA MINERALES",
				SubMatrix = "METÁLICOS",
				SubMtrxDescription = "ME",
				MatrixCreateDate = DateTime.Now,
				BaseMatrix = bm5,
				Description = "MP"
				//Sucursal = suc3
			};
			abcContext.MatrixSet.Add(matrix3);

			var matrix4 = new Matrix()
			{
				Active = true,
				Name = "ALIMENTOS",
				MatrixCreateDate = DateTime.Now,
				BaseMatrix = bm3,
				SubMatrix = "",
				SubMtrxDescription = "",
				Description = "AL"
				//Sucursal = suc4
			};
			abcContext.MatrixSet.Add(matrix4);
			// end Matrixes

			// Methods
			var method1 = new Method()
			{
				Active = true,
				Name = "CALCULO (SUMA DE N-NO3+N-NO2+NTK)",
				Description = "Descripción del Método CALCULO (SUMA DE N-NO3+N-NO2+NTK)",
				//Formula = "AGNO2+AGNO3+AGNTK",
				//InternetPublish = true,
				//Preserver = null,
				////Residue = null,
				//Container = null,
				//AnalyticsMethod = am10,
				//RequiredVolume = null,
				//MinimumVolume = null,
				//DeliverTime = 10,
				//MaxTimeBeforeAnalysis = null,
				//LabDeliverTime = 10,
				//ReportTime = 1,
				//AnalysisTime = 2,
				//QcObj = null,
				//	DetectionLimit = null,
				//CuantificationLimit =  null,
				//Uncertainty = null
			};
			//method1.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method1);

			var method2 = new Method()
			{
				Active = true,
				Name = "UNE-EN-14084-2003",
				Description = "Descripción del Método UNE-EN-14084-2003",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am2,
				//RequiredVolume = 200,
				//MinimumVolume = 100,
				//DeliverTime = 5,
				//MaxTimeBeforeAnalysis = 1,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				////AnalysisTime = 5,
				////QcObj = new Qc() { HasQc = true, LowerLimit = 0, UpperLimit = 0 },
				//DetectionLimit = new Limit() { Value = 3, Decimals = 0 },
				//CuantificationLimit = new Limit() { Value = 6, Decimals = 0 },
				//Uncertainty = new Limit() { Value = 1, Decimals = 1 }
			};
			//method2.Annalists.Add(annalist3);
			//method2.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method2);

			var method3 = new Method()
			{
				Active = true,
				Name = "NMX AA-042-1987",
				Description = "Descripción del Método NMX AA-042-1987",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am9,
				//RequiredVolume = 200,
				//MinimumVolume = 100,
				//DeliverTime = 10,
				//Container = cont2,
				//Preserver = pres1,
				//MaxTimeBeforeAnalysis = 1,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				//////AnalysisTime = 4,
				//////  QcObj = null,
				//////  DetectionLimit = null,
				//CuantificationLimit = new Limit() { Value = 3, Decimals = 1 },
				//Uncertainty = new Limit() { Value = 35.2, Decimals = 1 }
			};
			//method3.Annalists.Add(annalist3);
			//method3.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method3);

			var method4 = new Method()
			{
				Active = true,
				Name = "EPA 3015-1996",
				Description = "Descripción del Método EPA 3015-1996",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am8,
				//RequiredVolume = 250,
				//MinimumVolume = 50,
				//DeliverTime = 10,
				//Container = cont1,
				//Preserver = pres2,
				//MaxTimeBeforeAnalysis = 1,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				//AnalysisTime = 2,
				//QcObj = new Qc { HasQc = true, LowerLimit = 100, UpperLimit = 100 },
				// DetectionLimit = null,
				// CuantificationLimit = null,
				// Uncertainty = null
			};
			//method4.Annalists.Add(annalist2);
			//method4.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method4);

			var method5 = new Method()
			{
				Active = true,
				Name = "NMX AA-004-SCFI-2013",
				Description = "Descripción del Método NMX AA-004-SCFI-2013",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am3,
				//RequiredVolume = 2000,
				//MinimumVolume = 1000,
				//DeliverTime = 10,
				//Container = cont1,
				//Preserver = pres1,
				//MaxTimeBeforeAnalysis = 7,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				////AnalysisTime = 1,
				//// QcObj = null,
				//// DetectionLimit = null,
				//CuantificationLimit = new Limit() { Decimals = 2, Value = 0.1 },
				//Uncertainty = new Limit() { Decimals = 1, Value = 20.1 }
			};
			//method5.Annalists.Add(annalist1);
			//method5.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method5);

			var method6 = new Method()
			{
				Active = true,
				Name = "CALCULO",
				Description = "Descripción del Método CALCULO",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am11,
				//RequiredVolume = null,
				//MinimumVolume = null,
				//DeliverTime = 10,
				////Container = null,
				////Preserver = null,
				//MaxTimeBeforeAnalysis = null,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				//AnalysisTime = 4,
				// QcObj = null,
				//DetectionLimit = null,
				// CuantificationLimit = null,
				//Uncertainty = new Limit() { Value = 16.3, Decimals = 1 }
			};
			//method6.Annalists.Add(annalist3);
			//method6.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method6);

			var method7 = new Method()
			{
				Active = true,
				Name = "NMX AA-005-SCFI-2013",
				Description = "Descripción del Método NMX AA-005-SCFI-2013",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am4,
				//RequiredVolume = 2000,
				//MinimumVolume = 1000,
				//DeliverTime = 10,
				//Container = cont4,
				//Preserver = pres7,
				//MaxTimeBeforeAnalysis = 28,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				////AnalysisTime = 2,
				////   QcObj = null,
				////  DetectionLimit = null,
				//CuantificationLimit = new Limit() { Decimals = 1, Value = 5 },
				//Uncertainty = new Limit() { Value = 25.6, Decimals = 1 }
			};
			//method7.Annalists.Add(annalist2);
			//method7.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method7);

			var method8 = new Method()
			{
				Active = true,
				Name = "NMX AA-007-SCFI-2013",
				Description = "Descripción del Método NMX AA-007-SCFI-2013",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am14,
				//RequiredVolume = null,
				//MinimumVolume = null,
				//DeliverTime = 10,
				//Container = cont3,
				////  Preserver = null,
				//MaxTimeBeforeAnalysis = 1,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				////AnalysisTime = 3,
				////  QcObj = null,
				//// DetectionLimit = null,
				//CuantificationLimit = new Limit() { Decimals = 1, Value = 0.1 },
				//Uncertainty = new Limit() { Value = 14.8, Decimals = 1 }
			};
			//method8.Annalists.Add(annalist1);
			//method8.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method8);

			var method9 = new Method()
			{
				Active = true,
				Name = "NMX AA-008-SCFI-2011",
				Description = "Descripción del Método NMX AA-008-SCFI-2011",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am13,
				//RequiredVolume = 250,
				//MinimumVolume = 100,
				//DeliverTime = 10,
				//Container = cont3,
				//Preserver = pres1,
				//MaxTimeBeforeAnalysis = 1,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				////AnalysisTime = 3,
				////QcObj = new Qc() { HasQc = true, LowerLimit = 95, UpperLimit = 105 },
				////  DetectionLimit = null,
				//CuantificationLimit = new Limit() { Decimals = 1, Value = 4 },
				//Uncertainty = new Limit() { Value = 5.9, Decimals = 1 }
			};
			//method9.Annalists.Add(annalist2);
			//method9.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method9);

			var method10 = new Method()
			{
				Active = true,
				Name = "NMX AA-026-SCFI-2010/US EPA 351.2-1993 (I)",
				Description = "Descripción del Método NMX AA-026-SCFI-2010/US EPA 351.2-1993 (I)",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am5,
				//RequiredVolume = 100,
				//MinimumVolume = 20,
				//DeliverTime = 10,
				//Container = cont1,
				//Preserver = pres5,
				//MaxTimeBeforeAnalysis = 28,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				////AnalysisTime = 2,
				////QcObj = new Qc() { HasQc = true, LowerLimit = 90, UpperLimit = 110 },
				//DetectionLimit = new Limit() { Decimals = 4, Value = 0.03 },
				//CuantificationLimit = new Limit() { Decimals = 4, Value = 0.10 },
				//Uncertainty = new Limit() { Decimals = 1, Value = 18.7 },
			};
			//method10.Annalists.Add(annalist1);
			//method10.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method10);

			var method11 = new Method()
			{
				Active = true,
				Name = "NMX AA-026-SCFI-2001/US EPA 351.2-1993 (I)",
				Description = "Descripción del Método NMX AA-026-SCFI-2001/US EPA 351.2-1993 (I)",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am5,
				//RequiredVolume = 100,
				//MinimumVolume = 20,
				//DeliverTime = 10,
				//Container = cont3,
				//Preserver = pres5,
				//MaxTimeBeforeAnalysis = 28,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				//AnalysisTime = 3,
				//QcObj = new Qc() { HasQc = true, LowerLimit = 100, UpperLimit = 100 },
				// DetectionLimit = null,
				//    CuantificationLimit = null,
				//  Uncertainty = null
			};
			//method11.Annalists.Add(annalist3);
			//method11.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method11);

			var method12 = new Method()
			{
				Active = true,
				Name = "NMX AA-051-SCFI-2001/ US EPA 7074A 1994 (I)",
				Description = "Descripción del Método NMX AA-051-SCFI-2001/ US EPA 7074A 1994 (I)",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am7,
				//RequiredVolume = 250,
				//MinimumVolume = 50,
				//DeliverTime = 10,
				//Container = cont1,
				//Preserver = pres2,
				//MaxTimeBeforeAnalysis = 28,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				////AnalysisTime = 1,
				////QcObj = new Qc() { HasQc = true, LowerLimit = 80, UpperLimit = 120 },
				//DetectionLimit = new Limit() { Decimals = 6, Value = 0.00005 },
				//CuantificationLimit = new Limit() { Decimals = 4, Value = 0.0005 },
				//Uncertainty = new Limit() { Decimals = 1, Value = 15.8 },
			};
			//method12.Annalists.Add(annalist2);
			//method12.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method12);

			var method13 = new Method()
			{
				Active = true,
				Name = "NMX AA-051-SCFI-2001/US EPA 6010C-2007 (I)",
				Description = "Descripción del Método NMX AA-051-SCFI-2001/US EPA 6010C-2007 (I)",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am6,
				//RequiredVolume = 250,
				//MinimumVolume = 50,
				//DeliverTime = 10,
				//Container = cont1,
				//Preserver = pres2,
				//MaxTimeBeforeAnalysis = 180,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				////AnalysisTime = 2,
				////QcObj = new Qc() { HasQc = true, LowerLimit = 80, UpperLimit = 120 },
				//DetectionLimit = new Limit() { Decimals = 5, Value = 0.00139 },
				//CuantificationLimit = new Limit() { Decimals = 4, Value = 0.010 },
				//Uncertainty = new Limit() { Decimals = 1, Value = 25.4 },
			};
			//method13.Annalists.Add(annalist1);
			//method13.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method13);

			var method14 = new Method()
			{
				Active = true,
				Name = "NMX AA-029-SCFI-2001/US EPA 365.1-1993",
				Description = "Descripción del Método NMX AA-029-SCFI-2001/US EPA 365.1-1993",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am5,
				//RequiredVolume = 100,
				//MinimumVolume = 20,
				//DeliverTime = 10,
				//Container = cont3,
				//Preserver = pres5,
				//MaxTimeBeforeAnalysis = 28,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				//AnalysisTime = 1,
				//QcObj = new Qc() { HasQc = true, LowerLimit = 100, UpperLimit = 100 },
				//  DetectionLimit = null,
				// CuantificationLimit = null,
				// Uncertainty = null,
			};
			//method14.Annalists.Add(annalist3);
			//method14.Annalists.Add(annalist1);
			//method14.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method14);

			var method15 = new Method()
			{
				Active = true,
				Name = "NMX AA-034-SCFI-2001",
				Description = "Descripción del Método NMX AA-034-SCFI-2001",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am4,
				//RequiredVolume = 500,
				//MinimumVolume = 250,
				//DeliverTime = 10,
				//Container = cont1,
				//Preserver = pres1,
				//MaxTimeBeforeAnalysis = 7,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				////AnalysisTime = 2,
				////QcObj = new Qc() { HasQc = true, LowerLimit = 80, UpperLimit = 120 },
				////  DetectionLimit = null,
				//CuantificationLimit = new Limit() { Decimals = 1, Value = 5 },
				//Uncertainty = new Limit() { Decimals = 1, Value = 15.8 },
			};
			//method15.Annalists.Add(annalist2);
			//method15.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method15);
			
			var method17 = new Method()
			{
				Active = true,
				Name = "NMX AA-058-SCFI-2001/US EPA 335.3-1978 (I)",
				Description = "Descripción del Método NMX AA-058-SCFI-2001/US EPA 335.3-1978 (I)",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am5,
				//RequiredVolume = 100,
				//MinimumVolume = 20,
				//DeliverTime = 10,
				//Container = cont3,
				//// Preserver = null,
				//MaxTimeBeforeAnalysis = 14,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				////AnalysisTime = 2,
				////QcObj = new Qc() { HasQc = true, LowerLimit = 80, UpperLimit = 120 },
				//DetectionLimit = new Limit() { Decimals = 4, Value = 0.0005 },
				//CuantificationLimit = new Limit() { Decimals = 4, Value = 0.005 },
				//Uncertainty = new Limit() { Decimals = 1, Value = 25.1 },
			};
			//method17.Annalists.Add(annalist1);
			//method17.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method17);

			var method18 = new Method()
			{
				Active = true,
				Name = "NMX AA-079-SCFI-2001/US EPA 353.2-1993 (I)",
				Description = "Descripción del Método NMX AA-079-SCFI-2001/US EPA 353.2-1993 (I)",
				//Formula = "AGNO2+AGNO3",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am5,
				//RequiredVolume = 100,
				//MinimumVolume = 20,
				//DeliverTime = 10,
				//Container = cont3,
				//Preserver = pres1,
				//MaxTimeBeforeAnalysis = 2,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				////AnalysisTime = 2,
				////QcObj = new Qc() { HasQc = true, LowerLimit = 90, UpperLimit = 110 },
				//DetectionLimit = new Limit() { Decimals = 4, Value = 0.0015 },
				//CuantificationLimit = new Limit() { Decimals = 4, Value = 0.0100 },
				//Uncertainty = new Limit() { Decimals = 1, Value = 23.8 },
			};
			//method18.Annalists.Add(annalist3);
			//method18.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method18);

			var method19 = new Method()
			{
				Active = true,
				Name = "NMX AA-099-SCFI-2006/US EPA 353.2-1993 (I)",
				Description = "NMX AA-099-SCFI-2006/US EPA 353.2-1993 (I)",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am5,
				//RequiredVolume = 100,
				//MinimumVolume = 20,
				//DeliverTime = 10,
				//Container = cont3,
				//Preserver = pres1,
				//MaxTimeBeforeAnalysis = 2,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				////AnalysisTime = 2,
				////QcObj = new Qc() { HasQc = true, LowerLimit = 90, UpperLimit = 110 },
				//DetectionLimit = new Limit() { Decimals = 4, Value = 0.0006 },
				//CuantificationLimit = new Limit() { Decimals = 4, Value = 0.0050 },
				//Uncertainty = new Limit() { Decimals = 1, Value = 23.8 },
			};
			//method19.Annalists.Add(annalist2);
			//method19.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method19);

			var method20 = new Method()
			{
				Active = true,
				Name = "NMX AA-113-SCFI-2012",
				Description = "Descripción del Método NMX AA-113-SCFI-2012",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am2,
				//RequiredVolume = 5000,
				//MinimumVolume = 5000,
				//DeliverTime = 10,
				//Container = cont1,
				//Preserver = pres1,
				//MaxTimeBeforeAnalysis = 60,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				////AnalysisTime = 5,
				//// QcObj = null,
				//// DetectionLimit = null,
				//CuantificationLimit = new Limit() { Decimals = 1, Value = 0.2 },
				//Uncertainty = new Limit() { Decimals = 1, Value = 40.4 },
			};
			//method20.Annalists.Add(annalist1);
			//method20.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method20);

			var method21 = new Method()
			{
				Active = true,
				Name = "NMX-AA-006-SCFI-2010",
				Description = "Descripción del Método NMX-AA-006-SCFI-2010",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am12,
				//RequiredVolume = 1000,
				//MinimumVolume = 500,
				//DeliverTime = 10,
				//Container = cont3,
				//Preserver = pres1,
				//MaxTimeBeforeAnalysis = 1,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				//AnalysisTime = 3,
				// QcObj = null,
				//  DetectionLimit = null,
				//CuantificationLimit = null,
				// Uncertainty = null,
			};
			//method20.Annalists.Add(annalist1);
			//method21.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method21);

			var method22 = new Method()
			{
				Active = true,
				Name = "NMX-AA-029-SCFI-2001/US EPA 365.1-1993 (I)",
				Description = "Descripción del Método NMX-AA-029-SCFI-2001/US EPA 365.1-1993 (I)",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am5,
				//RequiredVolume = 100,
				//MinimumVolume = 20,
				//DeliverTime = 10,
				//Container = cont3,
				//Preserver = pres5,
				//MaxTimeBeforeAnalysis = 28,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				////AnalysisTime = 2,
				////QcObj = new Qc() { HasQc = true, LowerLimit = 90, UpperLimit = 110 },
				//DetectionLimit = new Limit() { Decimals = 4, Value = 0.0014 },
				//CuantificationLimit = new Limit() { Decimals = 4, Value = 0.005 },
				//Uncertainty = new Limit() { Decimals = 1, Value = 17.5 },
			};
			//method20.Annalists.Add(annalist1);
			//method22.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method22);

			var method23 = new Method()
			{
				Active = true,
				Name = "NMX AA-028-SCFI-2001",
				Description = "Descripción del Método NMX AA-034-SCFI-2001",
				//Formula = "",
				//InternetPublish = true,
				//Residue = res1,
				//AnalyticsMethod = am1,
				//RequiredVolume = 500,
				//MinimumVolume = 250,
				//DeliverTime = 10,
				//Container = cont1,
				//Preserver = pres1,
				//MaxTimeBeforeAnalysis = 2,
				//LabDeliverTime = 3,
				//ReportTime = 2,
				////AnalysisTime = 6,
				////QcObj = new Qc() { HasQc = true, LowerLimit = 80, UpperLimit = 120 },
				////  DetectionLimit = null,
				//CuantificationLimit = new Limit() { Decimals = 1, Value = 2 },
				//Uncertainty = new Limit() { Decimals = 1, Value = 25.0 },
			};
			//method15.Annalists.Add(annalist2);
			//method23.Matrixes.Add(matrixAguasResiduales);
			abcContext.MethodSet.Add(method23);

			// end Methods

			// Base Param
			var bpnitrogeno = new BaseParam()
	        {
				Active = true,
				Name = "NITROGENO",
				Description = "Contiene NITRATOS, NITRITOS, NITRATOS+NITRITOS y NITROGENO TOTAL",
				ClasificacionQuimica1 = cq1,
				ClasificacionQuimica2 = cq8
			};
			bpnitrogeno.Units.Add(unitmgl);
			bpnitrogeno.Matrixes.Add(matrixAguasResiduales);
	        abcContext.BaseParamSet.Add(bpnitrogeno);

			var bpdbo = new BaseParam()
			{
				Active = true,
				Name = "DBO",
				Description = "Contiene DBO TOTAL",
				ClasificacionQuimica1 = cq1,
				ClasificacionQuimica2 = cq6
			};
			bpdbo.Units.Add(unitmgl);
			bpdbo.Matrixes.Add(matrixAguasResiduales);
			abcContext.BaseParamSet.Add(bpdbo);

			var bphuevos = new BaseParam()
			{
				Active = true,
				Name = "HUEVOS",
				Description = "Contiene HUEVOS DE HELMINTO",
				ClasificacionQuimica1 = cq2
			};
			bphuevos.Units.Add(unithhl);
			bphuevos.Matrixes.Add(matrixAguasResiduales);
			abcContext.BaseParamSet.Add(bphuevos);

			var bpsolidos = new BaseParam()
			{
				Active = true,
				Name = "SOLIDOS",
				Description = "Contiene SOLIDOS SEDIMENTALES y SUSPENDIDOS TOTALES",
				ClasificacionQuimica1 = cq1,
				ClasificacionQuimica2 = cq7
			};
			bpsolidos.Units.Add(unitmgl);
			bpsolidos.Units.Add(unitmll);
			bpsolidos.Matrixes.Add(matrixAguasResiduales);
			abcContext.BaseParamSet.Add(bpsolidos);

			var bpcompinorg = new BaseParam()
			{
				Active = true,
				Name = "COMPUESTOS INORGANICOS",
				Description = "Contiene ARSENICO, CADMIO, CROMO, COBRE, NIQUEL, MERCURIO, PLOMO, ZINC Y DIGESTION ACIDA POR MICROONDAS",
				ClasificacionQuimica1 = cq3,
				ClasificacionQuimica2 = cq9
			};
			bpcompinorg.Units.Add(unitmgl);
			bpcompinorg.Units.Add(unitna);
			bpcompinorg.Matrixes.Add(matrixAguasResiduales);
			abcContext.BaseParamSet.Add(bpcompinorg);

			var bpbiologicos = new BaseParam()
			{
				Active = true,
				Name = "BIOLOGICOS",
				Description = "Contiene COLIFORMES FECALES",
				ClasificacionQuimica1 = cq4
			};
			bpbiologicos.Units.Add(unitnmp100ml);
			bpbiologicos.Matrixes.Add(matrixAguasResiduales);
			abcContext.BaseParamSet.Add(bpbiologicos);

			var bpfosforo = new BaseParam()
			{
				Active = true,
				Name = "FOSFORO",
				Description = "Contiene FOSFORO TOTAL y DIGESTION FOSFORO TOTAL",
                ClasificacionQuimica1 = cq1,
				ClasificacionQuimica2 = cq8
			};
			bpfosforo.Units.Add(unitmgl);
			bpfosforo.Units.Add(unitna);
			bpfosforo.Matrixes.Add(matrixAguasResiduales);
			abcContext.BaseParamSet.Add(bpfosforo);

			var bpgrasasaceites = new BaseParam()
			{
				Active = true,
				Name = "GRASAS Y ACEITES",
				Description = "Contiene GRASAS Y ACEITES, así como su PROMEDIO PONDERADO",
				ClasificacionQuimica1 = cq1,
				ClasificacionQuimica2 = cq6
			};
			bpgrasasaceites.Units.Add(unitmgl);
			bpgrasasaceites.Matrixes.Add(matrixAguasResiduales);
			abcContext.BaseParamSet.Add(bpgrasasaceites);

			var bpflujo = new BaseParam()
			{
				Active = true,
				Name = "FLUJO",
				Description = "Contiene FLUJO",
				ClasificacionQuimica1 = cq5
			};
			bpflujo.Units.Add(unitls);
			bpflujo.Matrixes.Add(matrixAguasResiduales);
			abcContext.BaseParamSet.Add(bpflujo);

			var bpmatflotante = new BaseParam()
			{
				Active = true,
				Name = "MATERIA FLOTANTE",
				Description = "Contiene MATERIA FLOTANTE",
				ClasificacionQuimica1 = cq5
			};
			bpmatflotante.Units.Add(unitna);
			bpmatflotante.Matrixes.Add(matrixAguasResiduales);
			abcContext.BaseParamSet.Add(bpmatflotante);

			var bpph = new BaseParam()
			{
				Active = true,
				Name = "PH",
				Description = "Contiene PH DE CAMPO",
				ClasificacionQuimica1 = cq5
			};
			bpph.Units.Add(unituph);
			bpph.Matrixes.Add(matrixAguasResiduales);
			abcContext.BaseParamSet.Add(bpph);

			var bptemp = new BaseParam()
			{
				Active = true,
				Name = "TEMPERATURA",
				Description = "Contiene TEMPERATURA DE CAMPO",
				ClasificacionQuimica1 = cq5
			};
			bptemp.Units.Add(unitgradoscelcius);
			bptemp.Matrixes.Add(matrixAguasResiduales);
			abcContext.BaseParamSet.Add(bptemp);

			var bpcianuros = new BaseParam()
			{
				Active = true,
				Name = "CIANUROS",
				Description = "Contiene CIANUROS TOTALES",
				ClasificacionQuimica1 = cq1,
				ClasificacionQuimica2 = cq10
			};
			bpcianuros.Units.Add(unitmgl);
			bpcianuros.Matrixes.Add(matrixAguasResiduales);
			abcContext.BaseParamSet.Add(bpcianuros);
			// end Base Param

			// Prices
			var price0 = new Price()
			{
				Value = 0,
				Currency = myCurrency1
			};
			abcContext.PriceSet.Add(price0);

			var price286 = new Price()
	        {
				Value = 286,
				Currency = myCurrency1
	        };
	        abcContext.PriceSet.Add(price286);

			var price365 = new Price()
			{
				Value = 365,
				Currency = myCurrency1
			};
			abcContext.PriceSet.Add(price365);

			var price136 = new Price()
			{
				Value = 136,
				Currency = myCurrency1
			};
			abcContext.PriceSet.Add(price136);

			var price177 = new Price()
			{
				Value = 177,
				Currency = myCurrency1
			};
			abcContext.PriceSet.Add(price177);

			var price299 = new Price()
			{
				Value = 299,
				Currency = myCurrency1
			};
			abcContext.PriceSet.Add(price299);

			var price203 = new Price()
			{
				Value = 203,
				Currency = myCurrency1
			};
			abcContext.PriceSet.Add(price203);

			var price326 = new Price()
			{
				Value = 326,
				Currency = myCurrency1
			};
			abcContext.PriceSet.Add(price326);

			var price244 = new Price()
			{
				Value = 244,
				Currency = myCurrency1
			};
			abcContext.PriceSet.Add(price244);

			var price122 = new Price()
			{
				Value = 122,
				Currency = myCurrency1
			};
			abcContext.PriceSet.Add(price122);
			// end Prices

			// Tipo Servicio
			var analisislab = new TipoServicio()
			{
				Name = "Análisis de Laboratorio"
			};
			abcContext.TipoServicioSet.Add(analisislab);

			var analisiscam = new TipoServicio()
			{
				Name = "Análisis de Campo"
			};
			abcContext.TipoServicioSet.Add(analisiscam);
			// end Tipo Servicio

			// Ramas
	        var ramaAguas = new Rama()
	        {
		        Name = "Aguas"
	        };
	        abcContext.RamaSet.Add(ramaAguas);

			// Param
			var paramAGDBOT = new Param()
	        {
				BaseParam = bpdbo,
				ParamUniquekey = "AGDBOT",
				Description = "DBO TOTAL",
				Metodo = method23,
				////GenericKey ="",
				RequiredVolume = 500,
				MinimumVolume = 250,
				DeliverTime = 10,
				Container = cont1,
				Preserver = pres1,
				MaxTimeBeforeAnalysis = 2,
				LabDeliverTime = 3,
				ReportTime = 2,
				CuantificationLimit = new Limit() { Decimals = 1, Value = 2 },
				//GenericDescription = "DBO",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc00075,
				Precio = price286,
				Active = true,
				Unit = unitmgl,
				//UnitReport = unitmgl,
				DecimalesReporte = 1,
				ResiduoPeligroso = false,
				//AnnalistKey = "A35",
				ReportaCliente = true,
				////publishinautolab = true,
				PerTurnCapacity = 390,
				PerWeekCapacity = 600,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = true,
					Saturday = true,
					Sunday = true
				},
				SellSeparated = true,
				CuentaEstadistica = true
				////Matrix = matrixAguasResiduales
			};
			paramAGDBOT.Annalists.Add(jiu);
			paramAGDBOT.Annalists.Add(ats);
			paramAGDBOT.Annalists.Add(rrp);
			paramAGDBOT.Annalists.Add(vsm);
			paramAGDBOT.Annalists.Add(epg);
			paramAGDBOT.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGDBOT);

			var paramAGHUEVPK = new Param()
			{
				BaseParam = bphuevos,
				ParamUniquekey = "AGHUEVPK",
				Description = "HUEVOS DE HELMINTO",
				Metodo = method20,
				////GenericKey ="",
				RequiredVolume = 5000,
				MinimumVolume = 5000,
				DeliverTime = 10,
				Container = cont1,
				Preserver = pres1,
				MaxTimeBeforeAnalysis = 60,
				LabDeliverTime = 3,
				ReportTime = 2,
				CuantificationLimit = new Limit() { Decimals = 1, Value = 0.2 },
				//GenericDescription = "HUEVOS DE HELMINTOS",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc044,
				Precio = price365,
				Active = true,
				Unit = unithhl,
				//UnitReport = unithhl,
				DecimalesReporte = 1,
				ResiduoPeligroso = false,
				//AnnalistKey = "A11",
				ReportaCliente = true,
				////publishinautolab = true,
				PerTurnCapacity = 65,
				PerWeekCapacity = 100,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = false,
					Wednesday = true,
					Thursday = false,
					Friday = true,
					Saturday = false,
					Sunday = false
				},
				SellSeparated = true,
				CuentaEstadistica = true
				////Matrix = matrixAguasResiduales
			};
			paramAGHUEVPK.Annalists.Add(jiu);
			paramAGHUEVPK.Annalists.Add(ats);
			paramAGHUEVPK.Annalists.Add(erd);
			paramAGHUEVPK.Annalists.Add(vpr);
			paramAGHUEVPK.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGHUEVPK);

			var paramAGSOLSED = new Param()
			{
				BaseParam = bpsolidos,
				ParamUniquekey = "AGSOLSED",
				Description = "SOLIDOS SEDIMENTABLES",
				Metodo = method5,
				////GenericKey ="",
				RequiredVolume = 2000,
				MinimumVolume = 1000,
				DeliverTime = 10,
				Container = cont1,
				Preserver = pres1,
				MaxTimeBeforeAnalysis = 7,
				LabDeliverTime = 3,
				ReportTime = 2,
				//AnalysisTime = 1,
				// QcObj = null,
				// DetectionLimit = null,
				CuantificationLimit = new Limit() { Decimals = 2, Value = 0.1 },
				//GenericDescription = "SOLIDOS SEDIMENTABLES",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc023,
				Precio = price136,
				Active = true,
				Unit = unitmll,
				//UnitReport = unitmll,
				DecimalesReporte = 2,
				ResiduoPeligroso = false,
				//AnnalistKey = "A31",
				ReportaCliente = true,
				////publishinautolab = true,
				PerTurnCapacity = 130,
				PerWeekCapacity = 200,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = false,
					Wednesday = true,
					Thursday = false,
					Friday = true,
					Saturday = false,
					Sunday = false
				},
				SellSeparated = true,
				CuentaEstadistica = true
				////Matrix = matrixAguasResiduales
			};
			paramAGSOLSED.Annalists.Add(jiu);
			paramAGSOLSED.Annalists.Add(ats);
			paramAGSOLSED.Annalists.Add(rrp);
			paramAGSOLSED.Annalists.Add(vsm);
			paramAGSOLSED.Annalists.Add(cir);
			paramAGSOLSED.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGSOLSED);

			var paramGAGSST = new Param()
			{
				BaseParam = bpsolidos,
				ParamUniquekey = "G-AGSST",
				Description = "SOLIDOS SUSPENDIDOS TOTALES",
				Metodo = method15,
				////GenericKey ="",
				RequiredVolume = 500,
				MinimumVolume = 250,
				DeliverTime = 10,
				Container = cont1,
				Preserver = pres1,
				MaxTimeBeforeAnalysis = 7,
				LabDeliverTime = 3,
				ReportTime = 2,
				CuantificationLimit = new Limit() { Decimals = 1, Value = 5 },
				//GenericDescription = "SOLIDOS SUSPENDIDOS TOTALES",
				TipoServicio = analisislab,
				SucursalRealiza = suc4,
				Rama = ramaAguas,
				CentroCosto = cc123,
				Precio = price177,
				Active = true,
				Unit = unitmgl,
				//UnitReport = unitmgl,
				DecimalesReporte = 1,
				ResiduoPeligroso = false,
				//AnnalistKey = "G1",
				ReportaCliente = true,
				////publishinautolab = true,
				PerTurnCapacity = 325,
				PerWeekCapacity = 500,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = true,
					Saturday = true,
					Sunday = false
				},
				SellSeparated = true,
				CuentaEstadistica = true
				////Matrix = matrixAguasResiduales
			};
			paramGAGSST.Annalists.Add(ats);
			paramGAGSST.Annalists.Add(jiu);
			paramGAGSST.Annalists.Add(clr);
			paramGAGSST.Annalists.Add(lbj);
			paramGAGSST.Annalists.Add(gar);
			paramGAGSST.Annalists.Add(ces);
			paramGAGSST.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramGAGSST);

			var paramAGNTK = new Param()
			{
				BaseParam = bpnitrogeno,
				ParamUniquekey = "AGNTK",
				Description = "NITROGENO TOTAL KJELDHAL (NTK)",
				Metodo = method10,
				////GenericKey ="",
				RequiredVolume = 100,
				MinimumVolume = 20,
				DeliverTime = 10,
				Container = cont1,
				Preserver = pres5,
				MaxTimeBeforeAnalysis = 28,
				LabDeliverTime = 3,
				ReportTime = 2,
				DetectionLimit = new Limit() { Decimals = 4, Value = 0.03 },
				CuantificationLimit = new Limit() { Decimals = 4, Value = 0.10 },
				//GenericDescription = "NITROGENO KJELDHAL",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc027,
				Precio = price299,
				Active = true,
				Unit = unitmgl,
				//UnitReport = unitmgl,
				DecimalesReporte = 4,
				ResiduoPeligroso = false,
				//AnnalistKey = "A18",
				ReportaCliente = true,
				////publishinautolab = true,
				PerTurnCapacity = 390,
				PerWeekCapacity = 600,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = true,
					Saturday = true,
					Sunday = false
				},
				SellSeparated = false,
				CuentaEstadistica = false
				////Matrix = matrixAguasResiduales
			};
			paramAGNTK.Annalists.Add(jiu);
			paramAGNTK.Annalists.Add(ats);
			paramAGNTK.Annalists.Add(apg);
			paramAGNTK.Annalists.Add(jcr);
			paramAGNTK.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGNTK);

			var paramAGDIGNTK = new Param()
			{
				BaseParam = bpdbo,
				ParamUniquekey = "AGDIGNTK",
				Description = "DIGESTION NITROGENO TOTAL KJELDHAL (NTK)",
				Metodo = method10,
				////GenericKey ="",
				RequiredVolume = 100,
				MinimumVolume = 20,
				DeliverTime = 10,
				Container = cont1,
				Preserver = pres5,
				MaxTimeBeforeAnalysis = 28,
				LabDeliverTime = 3,
				ReportTime = 2,
				DetectionLimit = new Limit() { Decimals = 4, Value = 0.03 },
				CuantificationLimit = new Limit() { Decimals = 4, Value = 0.10 },
				//GenericDescription = "DIGESTIÓN DE NTK",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc027,
				Precio = price0,
				Active = true,
				Unit = unitna,
				//UnitReport = unitna,
				DecimalesReporte = 0,
				ResiduoPeligroso = false,
				//AnnalistKey = "A28",
				ReportaCliente = true,
				////publishinautolab = true,
				PerTurnCapacity = 390,
				PerWeekCapacity = 600,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = true,
					Saturday = true,
					Sunday = false
				},
				SellSeparated = false,
				CuentaEstadistica = false
				////Matrix = matrixAguasResiduales
			};
			paramAGDIGNTK.Annalists.Add(jiu);
			paramAGDIGNTK.Annalists.Add(ats);
			paramAGDIGNTK.Annalists.Add(apg);
			paramAGDIGNTK.Annalists.Add(jcr);
			paramAGDIGNTK.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGDIGNTK);

			var paramAGAS = new Param()
			{
				BaseParam = bpcompinorg,
				ParamUniquekey = "AGAS",
				Description = "ARSENICO",
				Metodo = method13,
				////GenericKey ="",
				RequiredVolume = 250,
				MinimumVolume = 50,
				DeliverTime = 10,
				Container = cont1,
				Preserver = pres2,
				MaxTimeBeforeAnalysis = 180,
				LabDeliverTime = 3,
				ReportTime = 2,
				DetectionLimit = new Limit() { Decimals = 5, Value = 0.00139 },
				CuantificationLimit = new Limit() { Decimals = 4, Value = 0.010 },
				//GenericDescription = "ARSENICO",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc022,
				Precio = price286,
				Active = true,
				Unit = unitmgl,
				//UnitReport = unitmgl,
				DecimalesReporte = 4,
				ResiduoPeligroso = false,
				//AnnalistKey = "A01",
				ReportaCliente = true,
				////publishinautolab = true,
				PerTurnCapacity = 390,
				PerWeekCapacity = 600,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = false,
					Saturday = false,
					Sunday = false
				},
				SellSeparated = true,
				CuentaEstadistica = false
				////Matrix = matrixAguasResiduales
			};
			paramAGAS.Annalists.Add(jiu);
			paramAGAS.Annalists.Add(ats);
			paramAGAS.Annalists.Add(ivr);
			paramAGAS.Annalists.Add(ctr);
			paramAGAS.Annalists.Add(amp);
			paramAGAS.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGAS);

			var paramAGCD = new Param()
			{
				BaseParam = bpcompinorg,
				ParamUniquekey = "AGCD",
				Description = "CADMIO",
				Metodo = method13,
				////GenericKey ="",
				RequiredVolume = 250,
				MinimumVolume = 50,
				DeliverTime = 10,
				Container = cont1,
				Preserver = pres2,
				MaxTimeBeforeAnalysis = 180,
				LabDeliverTime = 3,
				ReportTime = 2,
				DetectionLimit = new Limit() { Decimals = 5, Value = 0.00139 },
				CuantificationLimit = new Limit() { Decimals = 4, Value = 0.010 },
				//GenericDescription = "CADMIO",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc022,
				Precio = price177,
				Active = true,
				Unit = unitmgl,
				//UnitReport = unitmgl,
				DecimalesReporte = 4,
				ResiduoPeligroso = false,
				//AnnalistKey = "A01",
				ReportaCliente = true,
				////publishinautolab = true,
				PerTurnCapacity = 390,
				PerWeekCapacity = 600,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = false,
					Saturday = false,
					Sunday = false
				},
				SellSeparated = true,
				CuentaEstadistica = false
				////Matrix = matrixAguasResiduales
			};
			paramAGCD.Annalists.Add(jiu);
			paramAGCD.Annalists.Add(ats);
			paramAGCD.Annalists.Add(ivr);
			paramAGCD.Annalists.Add(ctr);
			paramAGCD.Annalists.Add(amp);
			paramAGCD.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGCD);

			var paramAGCR = new Param()
			{
				BaseParam = bpcompinorg,
				ParamUniquekey = "AGCR",
				Description = "CROMO",
				Metodo = method13,
				////GenericKey ="",
				RequiredVolume = 250,
				MinimumVolume = 50,
				DeliverTime = 10,
				Container = cont1,
				Preserver = pres2,
				MaxTimeBeforeAnalysis = 180,
				LabDeliverTime = 3,
				ReportTime = 2,
				DetectionLimit = new Limit() { Decimals = 5, Value = 0.00139 },
				CuantificationLimit = new Limit() { Decimals = 4, Value = 0.010 },
				//GenericDescription = "CROMO",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc022,
				Precio = price177,
				Active = true,
				Unit = unitmgl,
				//UnitReport = unitmgl,
				DecimalesReporte = 4,
				ResiduoPeligroso = false,
				//AnnalistKey = "A01",
				ReportaCliente = true,
				////publishinautolab = true,
				PerTurnCapacity = 390,
				PerWeekCapacity = 600,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = false,
					Saturday = false,
					Sunday = false
				},
				SellSeparated = true,
				CuentaEstadistica = false
				////Matrix = matrixAguasResiduales
			};
			paramAGCR.Annalists.Add(jiu);
			paramAGCR.Annalists.Add(ats);
			paramAGCR.Annalists.Add(ivr);
			paramAGCR.Annalists.Add(ctr);
			paramAGCR.Annalists.Add(amp);
			paramAGCR.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGCR);

			var paramAGCU = new Param()
			{
				BaseParam = bpcompinorg,
				ParamUniquekey = "AGCU",
				Description = "COBRE",
				Metodo = method13,
				////GenericKey ="",
				RequiredVolume = 250,
				MinimumVolume = 50,
				DeliverTime = 10,
				Container = cont1,
				Preserver = pres2,
				MaxTimeBeforeAnalysis = 180,
				LabDeliverTime = 3,
				ReportTime = 2,
				DetectionLimit = new Limit() { Decimals = 5, Value = 0.00139 },
				CuantificationLimit = new Limit() { Decimals = 4, Value = 0.010 },
				//GenericDescription = "COBRE",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc022,
				Precio = price177,
				Active = true,
				Unit = unitmgl,
				//UnitReport = unitmgl,
				DecimalesReporte = 4,
				ResiduoPeligroso = false,
				//AnnalistKey = "A01",
				ReportaCliente = true,
				////publishinautolab = true,
				PerTurnCapacity = 390,
				PerWeekCapacity = 600,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = false,
					Saturday = false,
					Sunday = false
				},
				SellSeparated = true,
				CuentaEstadistica = false,
				//Matrix = matrixAguasResiduales
			};
			paramAGCU.Annalists.Add(jiu);
			paramAGCU.Annalists.Add(ats);
			paramAGCU.Annalists.Add(ivr);
			paramAGCU.Annalists.Add(ctr);
			paramAGCU.Annalists.Add(amp);
			paramAGCU.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGCU);

			var paramAGNI = new Param()
			{
				BaseParam = bpcompinorg,
				ParamUniquekey = "AGNI",
				Description = "NIQUEL",
				Metodo = method13,
				////GenericKey ="",
				RequiredVolume = 250,
				MinimumVolume = 50,
				DeliverTime = 10,
				Container = cont1,
				Preserver = pres2,
				MaxTimeBeforeAnalysis = 180,
				LabDeliverTime = 3,
				ReportTime = 2,
				DetectionLimit = new Limit() { Decimals = 5, Value = 0.00139 },
				CuantificationLimit = new Limit() { Decimals = 4, Value = 0.010 },
				//GenericDescription = "NIQUEL",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc022,
				Precio = price177,
				Active = true,
				Unit = unitmgl,
				//UnitReport = unitmgl,
				DecimalesReporte = 4,
				ResiduoPeligroso = false,
				//AnnalistKey = "A01",
				ReportaCliente = true,
				////publishinautolab = true,
				PerTurnCapacity = 390,
				PerWeekCapacity = 600,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = false,
					Saturday = false,
					Sunday = false
				},
				SellSeparated = true,
				CuentaEstadistica = false,
				//Matrix = matrixAguasResiduales
			};
			paramAGNI.Annalists.Add(jiu);
			paramAGNI.Annalists.Add(ats);
			paramAGNI.Annalists.Add(ivr);
			paramAGNI.Annalists.Add(ctr);
			paramAGNI.Annalists.Add(amp);
			paramAGNI.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGNI);

			var paramAGHG = new Param()
			{
				BaseParam = bpcompinorg,
				ParamUniquekey = "AGHG",
				Description = "MERCURIO",
				Metodo = method12,
				////GenericKey ="",
				RequiredVolume = 250,
				MinimumVolume = 50,
				DeliverTime = 10,
				Container = cont1,
				Preserver = pres2,
				MaxTimeBeforeAnalysis = 28,
				LabDeliverTime = 3,
				ReportTime = 2,
				DetectionLimit = new Limit() { Decimals = 6, Value = 0.00005 },
				CuantificationLimit = new Limit() { Decimals = 4, Value = 0.0005 },
				//GenericDescription = "MERCURIO",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc021,
				Precio = price286,
				Active = true,
				Unit = unitmgl,
				//UnitReport = unitmgl,
				DecimalesReporte = 4,
				ResiduoPeligroso = false,
				//AnnalistKey = "A15",
				ReportaCliente = true,
				////publishinautolab = true,
				PerTurnCapacity = 520,
				PerWeekCapacity = 800,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = true,
					Saturday = false,
					Sunday = false
				},
				SellSeparated = true,
				CuentaEstadistica = false,
				//Matrix = matrixAguasResiduales
			};
			paramAGHG.Annalists.Add(jiu);
			paramAGHG.Annalists.Add(ats);
			paramAGHG.Annalists.Add(ivr);
			paramAGHG.Annalists.Add(ctr);
			paramAGHG.Annalists.Add(amp);
			paramAGHG.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGHG);

			var paramAGPB = new Param()
			{
				BaseParam = bpcompinorg,
				ParamUniquekey = "AGPB",
				Description = "PLOMO",
				Metodo = method13,
				////GenericKey ="",
				RequiredVolume = 250,
				MinimumVolume = 50,
				DeliverTime = 10,
				Container = cont1,
				Preserver = pres2,
				MaxTimeBeforeAnalysis = 180,
				LabDeliverTime = 3,
				ReportTime = 2,
				DetectionLimit = new Limit() { Decimals = 5, Value = 0.00139 },
				CuantificationLimit = new Limit() { Decimals = 4, Value = 0.010 },
				//GenericDescription = "PLOMO",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc022,
				Precio = price177,
				Active = true,
				Unit = unitmgl,
				//UnitReport = unitmgl,
				DecimalesReporte = 4,
				ResiduoPeligroso = false,
				//AnnalistKey = "A01",
				ReportaCliente = true,
				////publishinautolab = true,
				PerTurnCapacity = 390,
				PerWeekCapacity = 600,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = false,
					Saturday = false,
					Sunday = false
				},
				SellSeparated = true,
				CuentaEstadistica = false,
				//Matrix = matrixAguasResiduales
			};
			paramAGPB.Annalists.Add(jiu);
			paramAGPB.Annalists.Add(ats);
			paramAGPB.Annalists.Add(ivr);
			paramAGPB.Annalists.Add(ctr);
			paramAGPB.Annalists.Add(amp);
			paramAGPB.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGPB);

			var paramAGZN = new Param()
			{
				BaseParam = bpcompinorg,
				ParamUniquekey = "AGZN",
				Description = "ZINC",
				Metodo = method13,
				////GenericKey ="",
				RequiredVolume = 250,
				MinimumVolume = 50,
				DeliverTime = 10,
				Container = cont1,
				Preserver = pres2,
				MaxTimeBeforeAnalysis = 180,
				LabDeliverTime = 3,
				ReportTime = 2,
				DetectionLimit = new Limit() { Decimals = 5, Value = 0.00139 },
				CuantificationLimit = new Limit() { Decimals = 4, Value = 0.010 },
				//GenericDescription = "ZINC",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc022,
				Precio = price177,
				Active = true,
				Unit = unitmgl,
				//UnitReport = unitmgl,
				DecimalesReporte = 4,
				ResiduoPeligroso = false,
				//AnnalistKey = "A01",
				ReportaCliente = true,
				////publishinautolab = true,
				PerTurnCapacity = 390,
				PerWeekCapacity = 600,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = false,
					Saturday = false,
					Sunday = false
				},
				SellSeparated = true,
				CuentaEstadistica = false,
				//Matrix = matrixAguasResiduales
			};
			paramAGZN.Annalists.Add(jiu);
			paramAGZN.Annalists.Add(ats);
			paramAGZN.Annalists.Add(ivr);
			paramAGZN.Annalists.Add(ctr);
			paramAGZN.Annalists.Add(amp);
			paramAGZN.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGZN);

			var paramAGDIG = new Param()
			{
				BaseParam = bpcompinorg,
				ParamUniquekey = "AGDIG",
				Description = "DIGESTION ACIDA POR MICROONDAS (AG)",
				Metodo = method4,
				RequiredVolume = 250,
				MinimumVolume = 50,
				DeliverTime = 10,
				Container = cont1,
				Preserver = pres2,
				MaxTimeBeforeAnalysis = 1,
				LabDeliverTime = 3,
				ReportTime = 2,
				////GenericKey ="",
				//GenericDescription = "DIGESTIÓN DE METALES",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc022,
				Precio = price203,
				Active = true,
				Unit = unitna,
				//UnitReport = unitna,
				DecimalesReporte = 0,
				ResiduoPeligroso = false,
				//AnnalistKey = "A02",
				ReportaCliente = true,
				////publishinautolab = true,
				PerTurnCapacity = 390,
				PerWeekCapacity = 600,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = true,
					Saturday = false,
					Sunday = false
				},
				SellSeparated = true,
				CuentaEstadistica = true,
				//Matrix = matrixAguasResiduales
			};
			paramAGDIG.Annalists.Add(jiu);
			paramAGDIG.Annalists.Add(ats);
			paramAGDIG.Annalists.Add(ivr);
			paramAGDIG.Annalists.Add(ctr);
			paramAGDIG.Annalists.Add(amp);
			paramAGDIG.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGDIG);

			var paramAGCFNMP = new Param()
			{
				BaseParam = bpbiologicos,
				ParamUniquekey = "AGCFNMP",
				Description = "COLIFORMES FECALES A 44±1°C/24H EN CALDO EC",
				Metodo = method3,
				//GenericKey ="AGCFNMP",
				RequiredVolume = 200,
				MinimumVolume = 100,
				DeliverTime = 10,
				Container = cont2,
				Preserver = pres1,
				MaxTimeBeforeAnalysis = 1,
				LabDeliverTime = 3,
				ReportTime = 2,
				CuantificationLimit = new Limit() { Value = 3, Decimals = 1 },
				//GenericDescription = "COLIFORMES FECALES",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc026,
				Precio = price0,
				Active = true,
				Unit = unitnmp100ml,
				//UnitReport = unitnmp100ml,
				DecimalesReporte = 0,
				ResiduoPeligroso = false,
				//AnnalistKey = "A14",
				ReportaCliente = true,
				////publishinautolab = true,
				PerTurnCapacity = 468,
				PerWeekCapacity = 720,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = true,
					Saturday = true,
					Sunday = true
				},
				SellSeparated = false,
				CuentaEstadistica = true,
				//Matrix = matrixAguasResiduales
			};
			paramAGCFNMP.Annalists.Add(jiu);
			paramAGCFNMP.Annalists.Add(ats);
			paramAGCFNMP.Annalists.Add(rrp);
			paramAGCFNMP.Annalists.Add(vsm);
			paramAGCFNMP.Annalists.Add(epg);
			paramAGCFNMP.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGCFNMP);

			var paramAGCFNMG = new Param()
			{
				BaseParam = bpbiologicos,
				ParamUniquekey = "AGCFNMG",
				Description = "MEDIA GEOMETRICA DE COLIFORMES FECALES",
				Metodo = method6,
				////GenericKey ="",
				RequiredVolume = null,
				MinimumVolume = null,
				DeliverTime = 10,
				MaxTimeBeforeAnalysis = null,
				LabDeliverTime = 3,
				ReportTime = 2,
				//GenericDescription = "COLIFORMES FECALES",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc026,
				Precio = price0,
				Active = true,
				Unit = unitnmp100ml,
				//UnitReport = unitnmp100ml,
				DecimalesReporte = 0,
				ResiduoPeligroso = false,
				//AnnalistKey = "A14",
				ReportaCliente = true,
				//publishinautolab = true,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = true,
					Saturday = true,
					Sunday = true
				},
				SellSeparated = false,
				CuentaEstadistica = false,
				//Matrix = matrixAguasResiduales
			};
			paramAGCFNMG.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGCFNMG);

			var paramAGCN = new Param()
			{
				BaseParam = bpcianuros,
				ParamUniquekey = "AGCN",
				Description = "CIANUROS TOTALES",
				Metodo = method17,
				////GenericKey ="",
				RequiredVolume = 100,
				MinimumVolume = 20,
				DeliverTime = 10,
				Container = cont3,
				MaxTimeBeforeAnalysis = 14,
				LabDeliverTime = 3,
				ReportTime = 2,
				DetectionLimit = new Limit() { Decimals = 4, Value = 0.0005 },
				CuantificationLimit = new Limit() { Decimals = 4, Value = 0.005 },
				//GenericDescription = "CIANUROS TOTALES",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc027,
				Precio = price326,
				Active = true,
				Unit = unitmgl,
				//UnitReport = unitmgl,
				DecimalesReporte = 4,
				ResiduoPeligroso = false,
				//AnnalistKey = "A18",
				ReportaCliente = true,
				//publishinautolab = true,
				PerTurnCapacity = 260,
				PerWeekCapacity = 400,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = true,
					Saturday = true,
					Sunday = false
				},
				SellSeparated = true,
				CuentaEstadistica = true,
				//Matrix = matrixAguasResiduales
			};
			paramAGCN.Annalists.Add(jiu);
			paramAGCN.Annalists.Add(ats);
			paramAGCN.Annalists.Add(apg);
			paramAGCN.Annalists.Add(jcr);
			paramAGCN.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGCN);

			var paramAGNO3 = new Param()
			{
				BaseParam = bpnitrogeno,
				ParamUniquekey = "AGNO3",
				Description = "NITRATOS (NITROGENO DE)",
				Metodo = method18,
				////GenericKey ="",
				RequiredVolume = 100,
				MinimumVolume = 20,
				DeliverTime = 10,
				Container = cont3,
				Preserver = pres1,
				MaxTimeBeforeAnalysis = 2,
				LabDeliverTime = 3,
				ReportTime = 2,
				DetectionLimit = new Limit() { Decimals = 4, Value = 0.0015 },
				CuantificationLimit = new Limit() { Decimals = 4, Value = 0.0100 },
				//GenericDescription = "NITRATOS",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc027,
				Precio = price244,
				Active = true,
				Unit = unitmgl,
				//UnitReport = unitmgl,
				DecimalesReporte = 4,
				ResiduoPeligroso = false,
				//AnnalistKey = "A08",
				ReportaCliente = true,
				//publishinautolab = true,
				PerTurnCapacity = 468,
				PerWeekCapacity = 720,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = true,
					Saturday = true,
					Sunday = false
				},
				SellSeparated = true,
				CuentaEstadistica = true,
				//Matrix = matrixAguasResiduales
			};
			paramAGNO3.Annalists.Add(jiu);
			paramAGNO3.Annalists.Add(ats);
			paramAGNO3.Annalists.Add(apg);
			paramAGNO3.Annalists.Add(jcr);
			paramAGNO3.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGNO3);

			var paramAGNO2 = new Param()
			{
				BaseParam = bpnitrogeno,
				ParamUniquekey = "AGNO2",
				Description = "NITRITOS (NITROGENO DE)",
				Metodo = method18,
				////GenericKey ="",
				RequiredVolume = 100,
				MinimumVolume = 20,
				DeliverTime = 10,
				Container = cont3,
				Preserver = pres1,
				MaxTimeBeforeAnalysis = 2,
				LabDeliverTime = 3,
				ReportTime = 2,
				DetectionLimit = new Limit() { Decimals = 4, Value = 0.0015 },
				CuantificationLimit = new Limit() { Decimals = 4, Value = 0.0100 },
				//GenericDescription = "NITRITOS",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc027,
				Precio = price244,
				Active = true,
				Unit = unitmgl,
				//UnitReport = unitmgl,
				DecimalesReporte = 4,
				ResiduoPeligroso = false,
				//AnnalistKey = "A08",
				ReportaCliente = true,
				//publishinautolab = true,
				PerTurnCapacity = 468,
				PerWeekCapacity = 720,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = true,
					Saturday = true,
					Sunday = false
				},
				SellSeparated = true,
				CuentaEstadistica = true,
				//Matrix = matrixAguasResiduales
			};
			paramAGNO2.Annalists.Add(jiu);
			paramAGNO2.Annalists.Add(ats);
			paramAGNO2.Annalists.Add(apg);
			paramAGNO2.Annalists.Add(jcr);
			paramAGNO2.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGNO2);

			var paramAGNO2NO3 = new Param()
			{
				BaseParam = bpnitrogeno,
				ParamUniquekey = "AGNO2NO3",
				Description = "NITRITOS + NITRATOS (P/CALCULO DE  NT)",
				Metodo = method18,
				////GenericKey ="",
				RequiredVolume = 100,
				MinimumVolume = 20,
				DeliverTime = 10,
				Container = cont3,
				Preserver = pres1,
				MaxTimeBeforeAnalysis = 2,
				LabDeliverTime = 3,
				ReportTime = 2,
				DetectionLimit = new Limit() { Decimals = 4, Value = 0.0015 },
				CuantificationLimit = new Limit() { Decimals = 4, Value = 0.0100 },
				//GenericDescription = "SUMA DE NO2+NO3",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc027,
				Precio = price0,
				Active = true,
				Unit = unitmgl,
				//UnitReport = unitmgl,
				DecimalesReporte = 4,
				ResiduoPeligroso = false,
				//AnnalistKey = "A08",
				ReportaCliente = false,
				//publishinautolab = true,
				PerTurnCapacity = 468,
				PerWeekCapacity = 720,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = true,
					Saturday = true,
					Sunday = false
				},
				SellSeparated = false,
				CuentaEstadistica = false,
				//Matrix = matrixAguasResiduales
			};
			paramAGNO2NO3.Annalists.Add(jiu);
			paramAGNO2NO3.Annalists.Add(ats);
			paramAGNO2NO3.Annalists.Add(apg);
			paramAGNO2NO3.Annalists.Add(jcr);
			paramAGNO2NO3.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGNO2NO3);

			var paramAGNTOT = new Param()
			{
				BaseParam = bpnitrogeno,
				ParamUniquekey = "AGNTOT",
				Description = "NITROGENO TOTAL",
				Metodo = method1,
				RequiredVolume = null,
				MinimumVolume = null,
				DeliverTime = 10,
				MaxTimeBeforeAnalysis = null,
				LabDeliverTime = 10,
				ReportTime = 1,
				////GenericKey ="",
				//GenericDescription = "NITROGENO TOTAL",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc027,
				Precio = price0,
				Active = true,
				Unit = unitmgl,
				//UnitReport = unitmgl,
				DecimalesReporte = 1,
				ResiduoPeligroso = false,
				//AnnalistKey = "A18",
				ReportaCliente = true,
				//publishinautolab = true,
				PerTurnCapacity = 468,
				PerWeekCapacity = 720,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = true,
					Saturday = true,
					Sunday = false
				},
				SellSeparated = false,
				CuentaEstadistica = false,
				//Matrix = matrixAguasResiduales
			};
			paramAGNTOT.Annalists.Add(jiu);
			paramAGNTOT.Annalists.Add(ats);
			paramAGNTOT.Annalists.Add(apg);
			paramAGNTOT.Annalists.Add(jcr);
			paramAGNTOT.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGNTOT);

			var paramAGPTOT = new Param()
			{
				BaseParam = bpfosforo,
				ParamUniquekey = "AGPTOT",
				Description = "FOSFORO TOTAL",
				Metodo = method22,
				////GenericKey ="",
				RequiredVolume = 100,
				MinimumVolume = 20,
				DeliverTime = 10,
				Container = cont3,
				Preserver = pres5,
				MaxTimeBeforeAnalysis = 28,
				LabDeliverTime = 3,
				ReportTime = 2,
				DetectionLimit = new Limit() { Decimals = 4, Value = 0.0014 },
				CuantificationLimit = new Limit() { Decimals = 4, Value = 0.005 },
				//GenericDescription = "FOSFORO TOTAL",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc027,
				Precio = price244,
				Active = true,
				Unit = unitmgl,
				//UnitReport = unitmgl,
				DecimalesReporte = 4,
				ResiduoPeligroso = false,
				//AnnalistKey = "A18",
				ReportaCliente = true,
				//publishinautolab = true,
				PerTurnCapacity = 390,
				PerWeekCapacity = 600,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = true,
					Saturday = true,
					Sunday = false
				},
				SellSeparated = false,
				CuentaEstadistica = false,
				//Matrix = matrixAguasResiduales
			};
			paramAGPTOT.Annalists.Add(jiu);
			paramAGPTOT.Annalists.Add(ats);
			paramAGPTOT.Annalists.Add(apg);
			paramAGPTOT.Annalists.Add(jcr);
			paramAGPTOT.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGPTOT);

			var paramAGDIGPT = new Param()
			{
				BaseParam = bpfosforo,
				ParamUniquekey = "AGDIGPT",
				Description = "DIGESTION FOSFORO TOTAL",
				Metodo = method14,
				////GenericKey ="",
				RequiredVolume = 100,
				MinimumVolume = 20,
				DeliverTime = 10,
				Container = cont3,
				Preserver = pres5,
				MaxTimeBeforeAnalysis = 28,
				LabDeliverTime = 3,
				ReportTime = 2,
				//GenericDescription = "DIGESTION DE FOSFORO",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc027,
				Precio = price0,
				Active = true,
				Unit = unitna,
				//UnitReport = unitna,
				DecimalesReporte = 0,
				ResiduoPeligroso = false,
				//AnnalistKey = "A28",
				ReportaCliente = true,
				//publishinautolab = true,
				PerTurnCapacity = 390,
				PerWeekCapacity = 600,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = true,
					Saturday = true,
					Sunday = false
				},
				SellSeparated = false,
				CuentaEstadistica = false,
				//Matrix = matrixAguasResiduales
			};
			paramAGDIGPT.Annalists.Add(jiu);
			paramAGDIGPT.Annalists.Add(ats);
			paramAGDIGPT.Annalists.Add(apg);
			paramAGDIGPT.Annalists.Add(jcr);
			paramAGDIGPT.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGDIGPT);

			var paramAGGYA = new Param()
			{
				BaseParam = bpgrasasaceites,
				ParamUniquekey = "AGGYA",
				Description = "GRASAS Y ACEITES",
				Metodo = method7,
				////GenericKey ="",
				RequiredVolume = 2000,
				MinimumVolume = 1000,
				DeliverTime = 10,
				Container = cont4,
				Preserver = pres7,
				MaxTimeBeforeAnalysis = 28,
				LabDeliverTime = 3,
				ReportTime = 2,
				CuantificationLimit = new Limit() { Decimals = 1, Value = 5 },
				//GenericDescription = "GRASAS Y ACEITES",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc023,
				Precio = price0,
				Active = true,
				Unit = unitmgl,
				//UnitReport = unitmgl,
				DecimalesReporte = 1,
				ResiduoPeligroso = false,
				//AnnalistKey = "A12",
				ReportaCliente = true,
				//publishinautolab = true,
				PerTurnCapacity = 423,
				PerWeekCapacity = 650,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = true,
					Saturday = true,
					Sunday = false
				},
				SellSeparated = false,
				CuentaEstadistica = true,
				//Matrix = matrixAguasResiduales
			};
			paramAGGYA.Annalists.Add(jiu);
			paramAGGYA.Annalists.Add(ats);
			paramAGGYA.Annalists.Add(cel);
			paramAGGYA.Annalists.Add(jcr);
			paramAGGYA.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGGYA);

			var paramAGGYAPP = new Param()
			{
				BaseParam = bpgrasasaceites,
				ParamUniquekey = "AGGYAPP",
				Description = "PROMEDIO PONDERADO DE GRASAS Y ACEITES",
				Metodo = method6,
				////GenericKey ="",
				RequiredVolume = null,
				MinimumVolume = null,
				DeliverTime = 10,
				MaxTimeBeforeAnalysis = null,
				LabDeliverTime = 3,
				ReportTime = 2,
				//GenericDescription = "GRASAS Y ACEITES",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc023,
				Precio = price0,
				Active = true,
				Unit = unitmgl,
				//UnitReport = unitmgl,
				DecimalesReporte = 1,
				ResiduoPeligroso = false,
				//AnnalistKey = "A12",
				ReportaCliente = true,
				//publishinautolab = true,
				PerTurnCapacity = 423,
				PerWeekCapacity = 650,
				Week = new WeekProgram()
				{
					Monday = true,
					Tuesday = true,
					Wednesday = true,
					Thursday = true,
					Friday = true,
					Saturday = true,
					Sunday = false
				},
				SellSeparated = false,
				CuentaEstadistica = false,
				//Matrix = matrixAguasResiduales
			};
			paramAGGYAPP.Annalists.Add(jiu);
			paramAGGYAPP.Annalists.Add(ats);
			paramAGGYAPP.Annalists.Add(cel);
			paramAGGYAPP.Annalists.Add(jcr);
			paramAGGYAPP.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGGYAPP);

			var paramAGFLUJO = new Param()
			{
				BaseParam = bpflujo,
				ParamUniquekey = "AGFLUJO",
				Description = "FLUJO",
				Metodo = method6,
				////GenericKey ="",
				RequiredVolume = null,
				MinimumVolume = null,
				DeliverTime = 10,
				MaxTimeBeforeAnalysis = null,
				LabDeliverTime = 3,
				ReportTime = 2,
				//GenericDescription = "FLUJO",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc031,
				Precio = price0,
				Active = true,
				Unit = unitls,
				//UnitReport = unitls,
				DecimalesReporte = 4,
				ResiduoPeligroso = false,
				//AnnalistKey = "A20",
				ReportaCliente = false,
				//publishinautolab = true,
				PerTurnCapacity = 33,
				PerWeekCapacity = 50,
				SellSeparated = false,
				CuentaEstadistica = true,
				//Matrix = matrixAguasResiduales
			};
			paramAGFLUJO.Annalists.Add(jiu);
			paramAGFLUJO.Annalists.Add(ats);
			paramAGFLUJO.Annalists.Add(dvg);
			paramAGFLUJO.Annalists.Add(mph);
			paramAGFLUJO.Annalists.Add(jol);
			paramAGFLUJO.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGFLUJO);

			var paramAGMFCA = new Param()
			{
				BaseParam = bpmatflotante,
				ParamUniquekey = "AGMFCA",
				Description = "MATERIA FLOTANTE EN CAMPO",
				Metodo = method21,
				////GenericKey ="",
				RequiredVolume = 1000,
				MinimumVolume = 500,
				DeliverTime = 10,
				Container = cont3,
				Preserver = pres1,
				MaxTimeBeforeAnalysis = 1,
				LabDeliverTime = 3,
				ReportTime = 2,
				//GenericDescription = "MATERIA FLOTANTE",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc031,
				Precio = price122,
				Active = true,
				Unit = unitna,
				//UnitReport = unitna,
				DecimalesReporte = 0,
				ResiduoPeligroso = false,
				//AnnalistKey = "A20",
				ReportaCliente = true,
				//publishinautolab = true,
				PerTurnCapacity = 33,
				PerWeekCapacity = 50,
				SellSeparated = false,
				CuentaEstadistica = true,
				//Matrix = matrixAguasResiduales
			};
			paramAGMFCA.Annalists.Add(jiu);
			paramAGMFCA.Annalists.Add(ats);
			paramAGMFCA.Annalists.Add(dvg);
			paramAGMFCA.Annalists.Add(mph);
			paramAGMFCA.Annalists.Add(jol);
			paramAGMFCA.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGMFCA);

			var paramAGPHCA = new Param()
			{
				BaseParam = bpph,
				ParamUniquekey = "AGPHCA",
				Description = "PH EN CAMPO",
				Metodo = method9,
				////GenericKey ="",
				RequiredVolume = 250,
				MinimumVolume = 100,
				DeliverTime = 10,
				Container = cont3,
				Preserver = pres1,
				MaxTimeBeforeAnalysis = 1,
				LabDeliverTime = 3,
				ReportTime = 2,
				CuantificationLimit = new Limit() { Decimals = 1, Value = 4 },
				//GenericDescription = "PH DE CAMPO",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc031,
				Precio = price0,
				Active = true,
				Unit = unituph,
				//UnitReport = unituph,
				DecimalesReporte = 2,
				ResiduoPeligroso = false,
				//AnnalistKey = "A20",
				ReportaCliente = true,
				//publishinautolab = true,
				PerTurnCapacity = 33,
				PerWeekCapacity = 50,
				SellSeparated = false,
				CuentaEstadistica = true,
				//Matrix = matrixAguasResiduales
			};
			paramAGPHCA.Annalists.Add(jiu);
			paramAGPHCA.Annalists.Add(ats);
			paramAGPHCA.Annalists.Add(dvg);
			paramAGPHCA.Annalists.Add(mph);
			paramAGPHCA.Annalists.Add(jol);
			paramAGPHCA.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGPHCA);

			var paramAGTEMP = new Param()
			{
				BaseParam = bpph,
				ParamUniquekey = "AGTEMP",
				Description = "TEMPERATURA EN CAMPO",
				Metodo = method8,
				////GenericKey ="",
				RequiredVolume = null,
				MinimumVolume = null,
				DeliverTime = 10,
				Container = cont3,
				MaxTimeBeforeAnalysis = 1,
				LabDeliverTime = 3,
				ReportTime = 2,
				CuantificationLimit = new Limit() { Decimals = 1, Value = 0.1 },
				//GenericDescription = "TEMPERATURA DE CAMPO",
				TipoServicio = analisislab,
				SucursalRealiza = suc5,
				Rama = ramaAguas,
				CentroCosto = cc031,
				Precio = price0,
				Active = true,
				Unit = unitgradoscelcius,
				//UnitReport = unitgradoscelcius,
				DecimalesReporte = 1,
				ResiduoPeligroso = false,
				//AnnalistKey = "A20",
				ReportaCliente = true,
				//publishinautolab = true,
				PerTurnCapacity = 33,
				PerWeekCapacity = 50,
				SellSeparated = false,
				CuentaEstadistica = true,
				//Matrix = matrixAguasResiduales
			};
			paramAGTEMP.Annalists.Add(jiu);
			paramAGTEMP.Annalists.Add(ats);
			paramAGTEMP.Annalists.Add(dvg);
			paramAGTEMP.Annalists.Add(mph);
			paramAGTEMP.Annalists.Add(jol);
			paramAGTEMP.Matrixes.Add(matrixAguasResiduales);
			abcContext.ParamSet.Add(paramAGTEMP);
			// end Params

			// Groups
			var groupAGNTOTK = new Group()
	        {
				Name = "AGNTOTK",
				Description = "NITROGENO TOTAL KJELDHAL",
				TipoServicio = analisislab,
				Active = true,
				//publishinautolab = true,
				SellSeparated = true,
				CuentaEstadistica = true,
				//MuestreosCompuestos = false,
				MostrarLista = true,
				Sucursal = suc5
            };
			groupAGNTOTK.Parameters.Add(paramAGNTK);
			groupAGNTOTK.Parameters.Add(paramAGDIGNTK);
			groupAGNTOTK.Matrixes.Add(matrixAguasResiduales);
			abcContext.GroupSet.Add(groupAGNTOTK);

			var groupAGMTN01 = new Group()
			{
				Name = "AGMTN01",
				Description = "METALES NOM 001",
				TipoServicio = analisislab,
				Active = true,
				//publishinautolab = true,
				SellSeparated = true,
				CuentaEstadistica = true,
				//MuestreosCompuestos = false,
				MostrarLista = false,
				Sucursal = suc5
            };
			groupAGMTN01.Parameters.Add(paramAGAS);
			groupAGMTN01.Parameters.Add(paramAGCD);
			groupAGMTN01.Parameters.Add(paramAGCR);
			groupAGMTN01.Parameters.Add(paramAGCU);
			groupAGMTN01.Parameters.Add(paramAGNI);
			groupAGMTN01.Parameters.Add(paramAGHG);
			groupAGMTN01.Parameters.Add(paramAGPB);
			groupAGMTN01.Parameters.Add(paramAGZN);
			groupAGMTN01.Parameters.Add(paramAGDIG);
			groupAGMTN01.Matrixes.Add(matrixAguasResiduales);
			abcContext.GroupSet.Add(groupAGMTN01);

			var groupAGBIO601 = new Group()
			{
				Name = "AGBIO601",
				Description = "BIOLOGICOS NOM 001 6M",
				TipoServicio = analisislab,
				Active = true,
				//publishinautolab = true,
				SellSeparated = true,
				CuentaEstadistica = true,
				//MuestreosCompuestos = true,
				MostrarLista = false,
				Sucursal = suc5
			};
			groupAGBIO601.Parameters.Add(paramAGCFNMP);
			groupAGBIO601.Parameters.Add(paramAGCFNMG);
			groupAGBIO601.Matrixes.Add(matrixAguasResiduales);
			abcContext.GroupSet.Add(groupAGBIO601);

			//var complexAGBIO601 = new ComplexSampling()
			//{
			//	Group = groupAGBIO601,
			//	Param = paramAGCFNMP,
			//	CantidadMuestreos = 6
			//};
			//abcContext.ComplexSamplingSet.Add(complexAGBIO601);

			var groupFQN1 = new Group()
			{
				Name = "FQN1",
				Description = "FISICOQUIMICOS NOM 001-FI",
				TipoServicio = analisislab,
				Active = true,
				//publishinautolab = true,
				SellSeparated = false,
				CuentaEstadistica = false,
				//MuestreosCompuestos = false,
				MostrarLista = true,
				Sucursal = suc5
            };
			groupFQN1.Parameters.Add(paramAGCN);
			groupFQN1.Parameters.Add(paramAGNO3);
			groupFQN1.Parameters.Add(paramAGNO2);
			groupFQN1.Parameters.Add(paramAGNO2NO3);
			groupFQN1.Parameters.Add(paramAGNTOT);
			groupFQN1.Matrixes.Add(matrixAguasResiduales);
			abcContext.GroupSet.Add(groupFQN1);

			var groupAGPTOTAL = new Group()
			{
				Name = "AGPTOTAL",
				Description = "FOSFORO TOTAL",
				TipoServicio = analisislab,
				Active = true,
				//publishinautolab = true,
				SellSeparated = true,
				CuentaEstadistica = true,
				//MuestreosCompuestos = false,
				MostrarLista = true,
				Sucursal = suc5
			};
			groupAGPTOTAL.Parameters.Add(paramAGPTOT);
			groupAGPTOTAL.Parameters.Add(paramAGDIGPT);
			groupAGPTOTAL.Matrixes.Add(matrixAguasResiduales);
			abcContext.GroupSet.Add(groupAGPTOTAL);

			var groupFQ6MGYA = new Group()
			{
				Name = "FQ6MGYA",
				Description = "FISICOQUIMICOS 6M - GYA",
				TipoServicio = analisislab,
				Active = true,
				//publishinautolab = true,
				SellSeparated = true,
				CuentaEstadistica = false,
				//MuestreosCompuestos = true,
				MostrarLista = false,
				Sucursal = suc5
			};
			groupFQ6MGYA.Parameters.Add(paramAGGYA);
			groupFQ6MGYA.Parameters.Add(paramAGGYAPP);
			groupFQ6MGYA.Matrixes.Add(matrixAguasResiduales);
			abcContext.GroupSet.Add(groupFQ6MGYA);

			//var complexFQ6MGYA = new ComplexSampling()
			//{
			//	Group = groupFQ6MGYA,
			//	Param = paramAGGYA,
			//	CantidadMuestreos = 6
			//};
			//abcContext.ComplexSamplingSet.Add(complexFQ6MGYA);

			var groupAGGVOL6M = new Group()
			{
				Name = "AGGVOL6M",
				Description = "GASTO VOLUMETRICO (6 MUESTREOS)",
				TipoServicio = analisiscam,
				Active = true,
				//publishinautolab = true,
				SellSeparated = false,
				CuentaEstadistica = false,
				//MuestreosCompuestos = true,
				MostrarLista = false,
				Sucursal = suc5
			};
			groupAGGVOL6M.Parameters.Add(paramAGFLUJO);
			groupAGGVOL6M.Matrixes.Add(matrixAguasResiduales);
			abcContext.GroupSet.Add(groupAGGVOL6M);

			//var complexAGGVOL6M = new ComplexSampling()
			//{
			//	Group = groupAGGVOL6M,
			//	Param = paramAGFLUJO,
			//	CantidadMuestreos = 6
			//};
			//abcContext.ComplexSamplingSet.Add(complexAGGVOL6M);

			var groupFQ6MAC = new Group()
			{
				Name = "FQ6MAC",
				Description = "FISICOQUIMICOS 6M - AC",
				TipoServicio = analisiscam,
				Active = true,
				//publishinautolab = true,
				SellSeparated = false,
				CuentaEstadistica = false,
				//MuestreosCompuestos = true,
				MostrarLista = false, 
				Sucursal = suc5
			};
			groupFQ6MAC.Parameters.Add(paramAGMFCA);
			groupFQ6MAC.Parameters.Add(paramAGPHCA);
			groupFQ6MAC.Parameters.Add(paramAGTEMP);
			groupFQ6MAC.Matrixes.Add(matrixAguasResiduales);
			abcContext.GroupSet.Add(groupFQ6MAC);

			//var complexFQ6MAC1 = new ComplexSampling()
			//{
			//	Group = groupFQ6MAC,
			//	Param = paramAGMFCA,
			//	CantidadMuestreos = 6
			//};
	  //      abcContext.ComplexSamplingSet.Add(complexFQ6MAC1);

			//var complexFQ6MAC2 = new ComplexSampling()
			//{
			//	Group = groupFQ6MAC,
			//	Param = paramAGPHCA,
			//	CantidadMuestreos = 6
			//};
			//abcContext.ComplexSamplingSet.Add(complexFQ6MAC2);

			//var complexFQ6MAC3 = new ComplexSampling()
			//{
			//	Group = groupFQ6MAC,
			//	Param = paramAGTEMP,
			//	CantidadMuestreos = 6
			//};
			//abcContext.ComplexSamplingSet.Add(complexFQ6MAC3);
			// end Groups

			// Norms
			
			// Packages
			var pkAGPKNOM6 = new Package()
			{
				Name = "AGPKNOM6",
				Description = "NOM-001-SEMARNAT-1996 (6 MUESTREOS)",
				Active = true,
				//publishinautolab = true,
				SellSeparated = true,
				CuentaEstadistica = true,
				Sucursal = suc5
			};
			pkAGPKNOM6.Parameters.Add(paramAGDBOT);
			pkAGPKNOM6.Parameters.Add(paramAGHUEVPK);
			pkAGPKNOM6.Parameters.Add(paramAGSOLSED);
			pkAGPKNOM6.Parameters.Add(paramGAGSST);
			pkAGPKNOM6.Groups.Add(groupAGNTOTK);
			pkAGPKNOM6.Groups.Add(groupAGMTN01);
			pkAGPKNOM6.Groups.Add(groupAGBIO601);
			pkAGPKNOM6.Groups.Add(groupFQN1);
			pkAGPKNOM6.Groups.Add(groupAGPTOTAL);
			pkAGPKNOM6.Groups.Add(groupFQ6MGYA);
			pkAGPKNOM6.Groups.Add(groupAGGVOL6M);
			pkAGPKNOM6.Groups.Add(groupFQ6MAC);
			pkAGPKNOM6.Matrixes.Add(matrixAguasResiduales);
			abcContext.PackageSet.Add(pkAGPKNOM6);
			// end Packages
			
			// Tipo Signatario
	        var ts1 = new TipoSignatario()
	        {
		        Active = true,
		        Name = "Operativo"
	        };
	        abcContext.TipoSignatarioSet.Add(ts1);

			var ts2 = new TipoSignatario()
			{
				Active = true,
				Name = "Ejecutivo"
			};
			abcContext.TipoSignatarioSet.Add(ts2);
			// end Tipo Signatario
			
			// Reconocimientos Adquiridos
			var raATS = new RecAdq()
	        {
		        NivelAdquirido = RecAdq.AcquiredLevel.Signatario,
		        Annalist = ats,
		        TipoSignatario = ts1
	        };
	        abcContext.RecAdqSet.Add(raATS);

			var raRRPsig = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Signatario,
				Annalist = rrp,
				TipoSignatario = ts2
			};
			abcContext.RecAdqSet.Add(raRRPsig);

			var raRRPeidas = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Eidas,
				Annalist = rrp
			};
			abcContext.RecAdqSet.Add(raRRPeidas);

			var raVSMsig = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Signatario,
				Annalist = vsm,
				TipoSignatario = ts1
			};
			abcContext.RecAdqSet.Add(raVSMsig);

			var raVSMeidas = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Eidas,
				Annalist = vsm
			};
			abcContext.RecAdqSet.Add(raVSMeidas);

			var raJIU = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Signatario,
				Annalist = jiu,
				TipoSignatario = ts2
			};
			abcContext.RecAdqSet.Add(raJIU);

			var raCLR = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Signatario,
				Annalist = clr,
				TipoSignatario = ts1
			};
			abcContext.RecAdqSet.Add(raCLR);

			var raERDsig = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Signatario,
				Annalist = erd,
				TipoSignatario = ts1
			};
			abcContext.RecAdqSet.Add(raERDsig);

			var raERDeidas = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Eidas,
				Annalist = erd
			};
			abcContext.RecAdqSet.Add(raERDeidas);

			var raEPGeidas = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Eidas,
				Annalist = epg
			};
			abcContext.RecAdqSet.Add(raEPGeidas);

			var raVPRsig = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Signatario,
				Annalist = vpr,
				TipoSignatario = ts2
			};
			abcContext.RecAdqSet.Add(raVPRsig);

			var raVPReidas = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Eidas,
				Annalist = vpr
			};
			abcContext.RecAdqSet.Add(raVPReidas);

			var raCIReidas = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Eidas,
				Annalist = cir
			};
			abcContext.RecAdqSet.Add(raCIReidas);

			var raLBJsig = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Signatario,
				Annalist = lbj,
				TipoSignatario = ts1
			};
			abcContext.RecAdqSet.Add(raLBJsig);

			var raLBJeidas = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Eidas,
				Annalist = lbj
			};
			abcContext.RecAdqSet.Add(raLBJeidas);

			var raGAReidas = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Eidas,
				Annalist = gar
			};
			abcContext.RecAdqSet.Add(raGAReidas);

			var raCESeidas = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Eidas,
				Annalist = ces
			};
			abcContext.RecAdqSet.Add(raCESeidas);

			var raAPGsig = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Signatario,
				Annalist = apg,
				TipoSignatario = ts1
			};
			abcContext.RecAdqSet.Add(raAPGsig);

			var raAPGeidas = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Eidas,
				Annalist = apg
			};
			abcContext.RecAdqSet.Add(raAPGeidas);
			
			var raIVRsig = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Signatario,
				Annalist = ivr,
				TipoSignatario = ts1
			};
			abcContext.RecAdqSet.Add(raIVRsig);

			var raIVReidas = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Eidas,
				Annalist = ivr
			};
			abcContext.RecAdqSet.Add(raIVReidas);

			var raCTRsig = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Signatario,
				Annalist = ctr,
				TipoSignatario = ts1
			};
			abcContext.RecAdqSet.Add(raCTRsig);

			var raCTReidas = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Eidas,
				Annalist = ctr
			};
			abcContext.RecAdqSet.Add(raCTReidas);

			var raAMPeidas = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Eidas,
				Annalist = amp
			};
			abcContext.RecAdqSet.Add(raAMPeidas);

			var raJCRsig = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Signatario,
				Annalist = jcr,
				TipoSignatario = ts2
			};
			abcContext.RecAdqSet.Add(raJCRsig);

			var raJCReidas = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Eidas,
				Annalist = jcr
			};
			abcContext.RecAdqSet.Add(raJCReidas);

			var raCELsig = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Signatario,
				Annalist = cel,
				TipoSignatario = ts2
			};
			abcContext.RecAdqSet.Add(raCELsig);

			var raCELeidas = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Eidas,
				Annalist = cel
			};
			abcContext.RecAdqSet.Add(raCELeidas);

			var raDVGsig = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Signatario,
				Annalist = dvg,
				TipoSignatario = ts2
			};
			abcContext.RecAdqSet.Add(raDVGsig);

			var raDVGeidas = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Eidas,
				Annalist = dvg
			};
			abcContext.RecAdqSet.Add(raDVGeidas);

			var raMPHsig = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Signatario,
				Annalist = mph,
				TipoSignatario = ts2
			};
			abcContext.RecAdqSet.Add(raMPHsig);

			var raMPHeidas = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Eidas,
				Annalist = mph
			};
			abcContext.RecAdqSet.Add(raMPHeidas);

			var raJOLeidas = new RecAdq()
			{
				NivelAdquirido = RecAdq.AcquiredLevel.Eidas,
				Annalist = jol
			};
			abcContext.RecAdqSet.Add(raJOLeidas);
			// Fin Reconocimientos Adquiridos
			
			// Reconocimientos Otorgados
			// JIU
			var ro1jiu = new RecOtorg()
	        {
		        Ack = ack1,
		        Enterprise = ent1,
		        Sucursal = suc5,
				Office = office1,
				RecAdq = raJIU
	        };
			ro1jiu.Params.Add(paramAGHUEVPK);
			ro1jiu.Params.Add(paramAGNTK);
			ro1jiu.Params.Add(paramAGCR);
			ro1jiu.Params.Add(paramAGCFNMP);
			ro1jiu.Params.Add(paramAGPTOT);
			ro1jiu.Params.Add(paramAGDBOT);
			ro1jiu.Params.Add(paramAGSOLSED);
			ro1jiu.Params.Add(paramAGAS);
			ro1jiu.Params.Add(paramAGCD);
			ro1jiu.Params.Add(paramAGCU);
			ro1jiu.Params.Add(paramAGNI);
			ro1jiu.Params.Add(paramAGHG);
			ro1jiu.Params.Add(paramAGPB);
			ro1jiu.Params.Add(paramAGZN);
			ro1jiu.Params.Add(paramAGCN);
			ro1jiu.Params.Add(paramAGGYA);
			ro1jiu.Params.Add(paramAGFLUJO);
			ro1jiu.Params.Add(paramAGMFCA);
			ro1jiu.Params.Add(paramAGTEMP);
			ro1jiu.Params.Add(paramAGPHCA);
			ro1jiu.Params.Add(paramAGNO2);
			ro1jiu.Params.Add(paramAGNO3);
			abcContext.RecOtorgSet.Add(ro1jiu);

			var ro11jiu = new RecOtorg()
			{
				Ack = ack11,
				Enterprise = ent11,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raJIU
			};
			ro11jiu.Params.Add(paramAGHUEVPK);
			ro11jiu.Params.Add(paramAGNTK);
			ro11jiu.Params.Add(paramAGCR);
			ro11jiu.Params.Add(paramAGCFNMP);
			ro11jiu.Params.Add(paramAGPTOT);
			ro11jiu.Params.Add(paramAGDBOT);
			ro11jiu.Params.Add(paramAGSOLSED);
			ro11jiu.Params.Add(paramAGAS);
			ro11jiu.Params.Add(paramAGCD);
			ro11jiu.Params.Add(paramAGCU);
			ro11jiu.Params.Add(paramAGNI);
			ro11jiu.Params.Add(paramAGHG);
			ro11jiu.Params.Add(paramAGPB);
			ro11jiu.Params.Add(paramAGZN);
			ro11jiu.Params.Add(paramAGCN);
			ro11jiu.Params.Add(paramAGGYA);
			ro11jiu.Params.Add(paramAGFLUJO);
			ro11jiu.Params.Add(paramAGMFCA);
			ro11jiu.Params.Add(paramAGTEMP);
			ro11jiu.Params.Add(paramAGPHCA);
			ro11jiu.Params.Add(paramAGNO2);
			ro11jiu.Params.Add(paramAGNO3);
			abcContext.RecOtorgSet.Add(ro11jiu);

			var ro17jiu = new RecOtorg()
			{
				Ack = ack17,
				Enterprise = ent17,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raJIU
			};
			ro17jiu.Params.Add(paramAGDBOT);
			ro17jiu.Params.Add(paramAGSOLSED);
			ro17jiu.Params.Add(paramAGAS);
			ro17jiu.Params.Add(paramAGCD);
			ro17jiu.Params.Add(paramAGCU);
			ro17jiu.Params.Add(paramAGNI);
			ro17jiu.Params.Add(paramAGHG);
			ro17jiu.Params.Add(paramAGPB);
			ro17jiu.Params.Add(paramAGZN);
			ro17jiu.Params.Add(paramAGCN);
			ro17jiu.Params.Add(paramAGGYA);
			ro17jiu.Params.Add(paramAGFLUJO);
			ro17jiu.Params.Add(paramAGMFCA);
			ro17jiu.Params.Add(paramAGTEMP);
			ro17jiu.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro17jiu);

			var ro7jiu = new RecOtorg()
			{
				Ack = ack7,
				Enterprise = ent7,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raJIU
			};
			ro7jiu.Params.Add(paramAGPHCA);
			ro7jiu.Params.Add(paramAGNO2);
			ro7jiu.Params.Add(paramAGNO3);
			abcContext.RecOtorgSet.Add(ro7jiu);

			var ro2jiu = new RecOtorg()
			{
				Ack = ack2,
				Enterprise = ent2,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raJIU
			};
			ro2jiu.Params.Add(paramGAGSST);
			abcContext.RecOtorgSet.Add(ro2jiu);

			var ro12jiu = new RecOtorg()
			{
				Ack = ack12,
				Enterprise = ent12,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raJIU
			};
			ro12jiu.Params.Add(paramGAGSST);
			abcContext.RecOtorgSet.Add(ro12jiu);

			var roAjiu = new RecOtorg()
			{
				Ack = ackA,
				Enterprise = entA,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raJIU
			};
			roAjiu.Params.Add(paramAGNO2NO3);
			roAjiu.Params.Add(paramAGNTOT);
			roAjiu.Params.Add(paramAGGYAPP);
			abcContext.RecOtorgSet.Add(roAjiu);

			var roBjiu = new RecOtorg()
			{
				Ack = ackB,
				Enterprise = entB,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raJIU
			};
			roBjiu.Params.Add(paramAGDIGNTK);
			roBjiu.Params.Add(paramAGDIG);
			roBjiu.Params.Add(paramAGDIGPT);
			abcContext.RecOtorgSet.Add(roBjiu);

			// ATS
			var ro1ats = new RecOtorg()
			{
				Ack = ack1,
				Enterprise = ent1,
				Sucursal = suc5,
				Office = office2,
				RecAdq = raATS
			};
			ro1ats.Params.Add(paramAGHUEVPK);
			ro1ats.Params.Add(paramAGNTK);
			ro1ats.Params.Add(paramAGCR);
			ro1ats.Params.Add(paramAGCFNMP);
			ro1ats.Params.Add(paramAGPTOT);
			ro1ats.Params.Add(paramAGDBOT);
			ro1ats.Params.Add(paramAGSOLSED);
			ro1ats.Params.Add(paramAGAS);
			ro1ats.Params.Add(paramAGCD);
			ro1ats.Params.Add(paramAGCU);
			ro1ats.Params.Add(paramAGNI);
			ro1ats.Params.Add(paramAGHG);
			ro1ats.Params.Add(paramAGPB);
			ro1ats.Params.Add(paramAGZN);
			ro1ats.Params.Add(paramAGCN);
			ro1ats.Params.Add(paramAGGYA);
			ro1ats.Params.Add(paramAGFLUJO);
			ro1ats.Params.Add(paramAGMFCA);
			ro1ats.Params.Add(paramAGTEMP);
			ro1ats.Params.Add(paramAGPHCA);
			ro1ats.Params.Add(paramAGNO2);
			ro1ats.Params.Add(paramAGNO3);
			abcContext.RecOtorgSet.Add(ro1ats);

			var ro11ats = new RecOtorg()
			{
				Ack = ack11,
				Enterprise = ent11,
				Sucursal = suc5,
				Office = office2,
				RecAdq = raATS
			};
			ro11ats.Params.Add(paramAGHUEVPK);
			ro11ats.Params.Add(paramAGNTK);
			ro11ats.Params.Add(paramAGCR);
			ro11ats.Params.Add(paramAGCFNMP);
			ro11ats.Params.Add(paramAGPTOT);
			ro11ats.Params.Add(paramAGDBOT);
			ro11ats.Params.Add(paramAGSOLSED);
			ro11ats.Params.Add(paramAGAS);
			ro11ats.Params.Add(paramAGCD);
			ro11ats.Params.Add(paramAGCU);
			ro11ats.Params.Add(paramAGNI);
			ro11ats.Params.Add(paramAGHG);
			ro11ats.Params.Add(paramAGPB);
			ro11ats.Params.Add(paramAGZN);
			ro11ats.Params.Add(paramAGCN);
			ro11ats.Params.Add(paramAGGYA);
			ro11ats.Params.Add(paramAGFLUJO);
			ro11ats.Params.Add(paramAGMFCA);
			ro11ats.Params.Add(paramAGTEMP);
			ro11ats.Params.Add(paramAGPHCA);
			ro11ats.Params.Add(paramAGNO2);
			ro11ats.Params.Add(paramAGNO3);
			abcContext.RecOtorgSet.Add(ro11ats);

			var ro17ats = new RecOtorg()
			{
				Ack = ack17,
				Enterprise = ent17,
				Sucursal = suc5,
				Office = office2,
				RecAdq = raATS
			};
			ro17ats.Params.Add(paramAGDBOT);
			ro17ats.Params.Add(paramAGSOLSED);
			ro17ats.Params.Add(paramAGAS);
			ro17ats.Params.Add(paramAGCD);
			ro17ats.Params.Add(paramAGCU);
			ro17ats.Params.Add(paramAGNI);
			ro17ats.Params.Add(paramAGHG);
			ro17ats.Params.Add(paramAGPB);
			ro17ats.Params.Add(paramAGZN);
			ro17ats.Params.Add(paramAGCN);
			ro17ats.Params.Add(paramAGGYA);
			ro17ats.Params.Add(paramAGFLUJO);
			ro17ats.Params.Add(paramAGMFCA);
			ro17ats.Params.Add(paramAGTEMP);
			ro17ats.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro17ats);

			var ro7ats = new RecOtorg()
			{
				Ack = ack7,
				Enterprise = ent7,
				Sucursal = suc5,
				Office = office2,
				RecAdq = raATS
			};
			ro7ats.Params.Add(paramAGPHCA);
			ro7ats.Params.Add(paramAGNO2);
			ro7ats.Params.Add(paramAGNO3);
			abcContext.RecOtorgSet.Add(ro7ats);

			var ro2ats = new RecOtorg()
			{
				Ack = ack2,
				Enterprise = ent2,
				Sucursal = suc5,
				Office = office2,
				RecAdq = raATS
			};
			ro2ats.Params.Add(paramGAGSST);
			abcContext.RecOtorgSet.Add(ro2ats);

			var ro12ats = new RecOtorg()
			{
				Ack = ack12,
				Enterprise = ent12,
				Sucursal = suc5,
				Office = office2,
				RecAdq = raATS
			};
			ro12ats.Params.Add(paramGAGSST);
			abcContext.RecOtorgSet.Add(ro12ats);

			var roAats = new RecOtorg()
			{
				Ack = ackA,
				Enterprise = entA,
				Sucursal = suc5,
				Office = office2,
				RecAdq = raATS
			};
			roAats.Params.Add(paramAGNO2NO3);
			roAats.Params.Add(paramAGNTOT);
			roAats.Params.Add(paramAGGYAPP);
			abcContext.RecOtorgSet.Add(roAats);

			var roBats = new RecOtorg()
			{
				Ack = ackB,
				Enterprise = entB,
				Sucursal = suc5,
				Office = office2,
				RecAdq = raATS
			};
			roBats.Params.Add(paramAGDIGNTK);
			roBats.Params.Add(paramAGDIG);
			roBats.Params.Add(paramAGDIGPT);
			abcContext.RecOtorgSet.Add(roBats);

			var ro1rrpsig = new RecOtorg()
			{
				Ack = ack1,
				Enterprise = ent1,
				Sucursal = suc5,
				Office = office2,
				RecAdq = raRRPsig
			};
			ro1rrpsig.Params.Add(paramAGCFNMP);
			ro1rrpsig.Params.Add(paramAGDBOT);
			ro1rrpsig.Params.Add(paramAGSOLSED);
			abcContext.RecOtorgSet.Add(ro1rrpsig);

			var ro11rrpsig = new RecOtorg()
			{
				Ack = ack11,
				Enterprise = ent11,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raRRPsig
			};
			ro11rrpsig.Params.Add(paramAGCFNMP);
			ro11rrpsig.Params.Add(paramAGDBOT);
			ro11rrpsig.Params.Add(paramAGSOLSED);
			abcContext.RecOtorgSet.Add(ro11rrpsig);

			var ro17rrpsig = new RecOtorg()
			{
				Ack = ack17,
				Enterprise = ent17,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raRRPsig
			};
			ro17rrpsig.Params.Add(paramAGDBOT);
			ro17rrpsig.Params.Add(paramAGSOLSED);
			abcContext.RecOtorgSet.Add(ro17rrpsig);

			var ro1vsmsig = new RecOtorg()
			{
				Ack = ack1,
				Enterprise = ent1,
				Sucursal = suc5,
				Office = office2,
				RecAdq = raVSMsig
			};
			ro1vsmsig.Params.Add(paramAGCFNMP);
			ro1vsmsig.Params.Add(paramAGDBOT);
			ro1vsmsig.Params.Add(paramAGSOLSED);
			abcContext.RecOtorgSet.Add(ro1vsmsig);

			var ro11vsmsig = new RecOtorg()
			{
				Ack = ack11,
				Enterprise = ent11,
				Sucursal = suc5,
				Office = office2,
				RecAdq = raVSMsig
			};
			ro11vsmsig.Params.Add(paramAGCFNMP);
			ro11vsmsig.Params.Add(paramAGDBOT);
			ro11vsmsig.Params.Add(paramAGSOLSED);
			abcContext.RecOtorgSet.Add(ro11vsmsig);

			var ro17vsmsig = new RecOtorg()
			{
				Ack = ack17,
				Enterprise = ent17,
				Sucursal = suc5,
				Office = office2,
				RecAdq = raVSMsig
			};
			ro17vsmsig.Params.Add(paramAGDBOT);
			ro17vsmsig.Params.Add(paramAGSOLSED);
			abcContext.RecOtorgSet.Add(ro17vsmsig);

			var ro1rrpeidas = new RecOtorg()
			{
				Ack = ack1I,
				Sucursal = suc5,
				RecAdq = raRRPeidas,
				Office = office1,
				Enterprise = ent1I
			};
			ro1rrpeidas.Params.Add(paramAGCFNMP);
			ro1rrpeidas.Params.Add(paramAGDBOT);
			ro1rrpeidas.Params.Add(paramAGSOLSED);
			abcContext.RecOtorgSet.Add(ro1rrpeidas);

			var ro11rrpeidas = new RecOtorg()
			{
				Ack = ack11I,
				Sucursal = suc5,
				RecAdq = raRRPeidas,
				Office = office1,
				Enterprise = ent11I
			};
			ro11rrpeidas.Params.Add(paramAGCFNMP);
			ro11rrpeidas.Params.Add(paramAGDBOT);
			ro11rrpeidas.Params.Add(paramAGSOLSED);
			abcContext.RecOtorgSet.Add(ro11rrpeidas);

			var ro17rrpeidas = new RecOtorg()
			{
				Ack = ack17I,
				Sucursal = suc5,
				RecAdq = raRRPeidas,
				Office = office1,
				Enterprise = ent17I
			};
			ro17rrpeidas.Params.Add(paramAGDBOT);
			ro17rrpeidas.Params.Add(paramAGSOLSED);
			abcContext.RecOtorgSet.Add(ro17rrpeidas);

			var ro1vsmeidas = new RecOtorg()
			{
				Ack = ack1I,
				Sucursal = suc5,
				RecAdq = raVSMeidas,
				Office = office2,
				Enterprise = ent1I
			};
			ro1vsmeidas.Params.Add(paramAGCFNMP);
			ro1vsmeidas.Params.Add(paramAGDBOT);
			ro1vsmeidas.Params.Add(paramAGSOLSED);
			abcContext.RecOtorgSet.Add(ro1vsmeidas);

			var ro11vsmeidas = new RecOtorg()
			{
				Ack = ack11I,
				Sucursal = suc5,
				RecAdq = raVSMeidas,
				Office = office2,
				Enterprise = ent11I
			};
			ro11vsmeidas.Params.Add(paramAGCFNMP);
			ro11vsmeidas.Params.Add(paramAGDBOT);
			ro11vsmeidas.Params.Add(paramAGSOLSED);
			abcContext.RecOtorgSet.Add(ro11vsmeidas);

			var ro17vsmeidas = new RecOtorg()
			{
				Ack = ack17I,
				Sucursal = suc5,
				RecAdq = raVSMeidas,
				Office = office2,
				Enterprise = ent17I
			};
			ro17vsmeidas.Params.Add(paramAGDBOT);
			ro17vsmeidas.Params.Add(paramAGSOLSED);
			abcContext.RecOtorgSet.Add(ro17vsmeidas);

			var ro1epgeidas = new RecOtorg()
			{
				Ack = ack1I,
				Sucursal = suc5,
				RecAdq = raEPGeidas,
				Office = office1,
				Enterprise = ent1I
			};
			ro1epgeidas.Params.Add(paramAGCFNMP);
			ro1epgeidas.Params.Add(paramAGDBOT);
			abcContext.RecOtorgSet.Add(ro1epgeidas);

			var ro11epgeidas = new RecOtorg()
			{
				Ack = ack11I,
				Sucursal = suc5,
				RecAdq = raEPGeidas,
				Office = office1,
				Enterprise = ent11I
			};
			ro11epgeidas.Params.Add(paramAGCFNMP);
			ro11epgeidas.Params.Add(paramAGDBOT);
			abcContext.RecOtorgSet.Add(ro11epgeidas);

			var ro17epgeidas = new RecOtorg()
			{
				Ack = ack17I,
				Sucursal = suc5,
				RecAdq = raEPGeidas,
				Office = office1,
				Enterprise = ent17I
			};
			ro17epgeidas.Params.Add(paramAGDBOT);
			abcContext.RecOtorgSet.Add(ro17epgeidas);

			var ro1cireidas = new RecOtorg()
			{
				Ack = ack1I,
				Sucursal = suc5,
				RecAdq = raCIReidas,
				Office = office1,
				Enterprise = ent1I
			};
			ro1cireidas.Params.Add(paramAGSOLSED);
			abcContext.RecOtorgSet.Add(ro1cireidas);

			var ro11cireidas = new RecOtorg()
			{
				Ack = ack11I,
				Sucursal = suc5,
				RecAdq = raCIReidas,
				Office = office1,
				Enterprise = ent11I
			};
			ro11cireidas.Params.Add(paramAGSOLSED);
			abcContext.RecOtorgSet.Add(ro11cireidas);

			var ro17cireidas = new RecOtorg()
			{
				Ack = ack17I,
				Sucursal = suc5,
				RecAdq = raCIReidas,
				Office = office1,
				Enterprise = ent17I
			};
			ro17cireidas.Params.Add(paramAGSOLSED);
			abcContext.RecOtorgSet.Add(ro17cireidas);

			var ro1apgsig = new RecOtorg()
			{
				Ack = ack1,
				Enterprise = ent1,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raAPGsig
			};
			ro1apgsig.Params.Add(paramAGNTK);
			ro1apgsig.Params.Add(paramAGCN);
			ro1apgsig.Params.Add(paramAGNO2);
			ro1apgsig.Params.Add(paramAGNO3);
			ro1apgsig.Params.Add(paramAGPTOT);
			abcContext.RecOtorgSet.Add(ro1apgsig);

			var ro11apgsig = new RecOtorg()
			{
				Ack = ack11,
				Enterprise = ent11,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raAPGsig
			};
			ro11apgsig.Params.Add(paramAGNTK);
			ro11apgsig.Params.Add(paramAGCN);
			ro11apgsig.Params.Add(paramAGNO2);
			ro11apgsig.Params.Add(paramAGNO3);
			ro11apgsig.Params.Add(paramAGPTOT);
			abcContext.RecOtorgSet.Add(ro11apgsig);

			var ro17apgsig = new RecOtorg()
			{
				Ack = ack17,
				Enterprise = ent17,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raAPGsig
			};
			ro17apgsig.Params.Add(paramAGCN);
			abcContext.RecOtorgSet.Add(ro17apgsig);

			var ro7apgsig = new RecOtorg()
			{
				Ack = ack7,
				Enterprise = ent7,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raAPGsig
			};
			ro7apgsig.Params.Add(paramAGNO2);
			ro7apgsig.Params.Add(paramAGNO3);
			abcContext.RecOtorgSet.Add(ro7apgsig);

			var roAapgsig = new RecOtorg()
			{
				Ack = ackA,
				Enterprise = entA,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raAPGsig
			};
			roAapgsig.Params.Add(paramAGNO2NO3);
			roAapgsig.Params.Add(paramAGNTOT);
			abcContext.RecOtorgSet.Add(roAapgsig);

			var roBapgsig = new RecOtorg()
			{
				Ack = ackB,
				Enterprise = entB,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raAPGsig
			};
			roBapgsig.Params.Add(paramAGDIGNTK);
			roBapgsig.Params.Add(paramAGDIGPT);
			abcContext.RecOtorgSet.Add(roBapgsig);

			var ro1apgeidas = new RecOtorg()
			{
				Ack = ack1I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raAPGeidas,
				Enterprise = ent1I
			};
			ro1apgeidas.Params.Add(paramAGNTK);
			ro1apgeidas.Params.Add(paramAGCN);
			ro1apgeidas.Params.Add(paramAGNO2);
			ro1apgeidas.Params.Add(paramAGNO3);
			ro1apgeidas.Params.Add(paramAGPTOT);
			abcContext.RecOtorgSet.Add(ro1apgeidas);

			var ro11apgeidas = new RecOtorg()
			{
				Ack = ack11I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raAPGeidas,
				Enterprise = ent11I
			};
			ro11apgeidas.Params.Add(paramAGNTK);
			ro11apgeidas.Params.Add(paramAGCN);
			ro11apgeidas.Params.Add(paramAGNO2);
			ro11apgeidas.Params.Add(paramAGNO3);
			ro11apgeidas.Params.Add(paramAGPTOT);
			abcContext.RecOtorgSet.Add(ro11apgeidas);

			var ro17apgeidas = new RecOtorg()
			{
				Ack = ack17I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raAPGeidas,
				Enterprise = ent17I
			};
			ro17apgeidas.Params.Add(paramAGCN);
			abcContext.RecOtorgSet.Add(ro17apgeidas);

			var ro7apgeidas = new RecOtorg()
			{
				Ack = ack7I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raAPGeidas,
				Enterprise = ent7I
			};
			ro7apgeidas.Params.Add(paramAGNO2);
			ro7apgeidas.Params.Add(paramAGNO3);
			abcContext.RecOtorgSet.Add(ro7apgeidas);

			var roAapgeidas = new RecOtorg()
			{
				Ack = ackAI,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raAPGeidas,
				Enterprise = entAI
			};
			roAapgeidas.Params.Add(paramAGNO2NO3);
			roAapgeidas.Params.Add(paramAGNTOT);
			abcContext.RecOtorgSet.Add(roAapgeidas);

			var roBapgeidas = new RecOtorg()
			{
				Ack = ackBI,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raAPGeidas,
				Enterprise = entBI
			};
			roBapgeidas.Params.Add(paramAGDIGNTK);
			roBapgeidas.Params.Add(paramAGDIGPT);
			abcContext.RecOtorgSet.Add(roBapgeidas);

			var ro1jcreidas = new RecOtorg()
			{
				Ack = ack1I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raJCReidas,
				Enterprise = ent1I
			};
			ro1jcreidas.Params.Add(paramAGNTK);
			ro1jcreidas.Params.Add(paramAGCN);
			ro1jcreidas.Params.Add(paramAGNO2);
			ro1jcreidas.Params.Add(paramAGNO3);
			ro1jcreidas.Params.Add(paramAGPTOT);
			ro1jcreidas.Params.Add(paramAGGYA);
			abcContext.RecOtorgSet.Add(ro1jcreidas);

			var ro11jcreidas = new RecOtorg()
			{
				Ack = ack11I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raJCReidas,
				Enterprise = ent11I
			};
			ro11jcreidas.Params.Add(paramAGNTK);
			ro11jcreidas.Params.Add(paramAGCN);
			ro11jcreidas.Params.Add(paramAGNO2);
			ro11jcreidas.Params.Add(paramAGNO3);
			ro11jcreidas.Params.Add(paramAGPTOT);
			ro11jcreidas.Params.Add(paramAGGYA);
			abcContext.RecOtorgSet.Add(ro11jcreidas);

			var ro17jcreidas = new RecOtorg()
			{
				Ack = ack17I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raJCReidas,
				Enterprise = ent17I
			};
			ro17jcreidas.Params.Add(paramAGCN);
			ro17jcreidas.Params.Add(paramAGGYA);
			abcContext.RecOtorgSet.Add(ro17jcreidas);

			var ro7jcreidas = new RecOtorg()
			{
				Ack = ack7I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raJCReidas,
				Enterprise = ent7I
			};
			ro7jcreidas.Params.Add(paramAGNO2);
			ro7jcreidas.Params.Add(paramAGNO3);
			abcContext.RecOtorgSet.Add(ro7jcreidas);

			var roAjcreidas = new RecOtorg()
			{
				Ack = ackAI,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raJCReidas,
				Enterprise = entAI
			};
			roAjcreidas.Params.Add(paramAGNO2NO3);
			roAjcreidas.Params.Add(paramAGNTOT);
			roAjcreidas.Params.Add(paramAGGYAPP);
			abcContext.RecOtorgSet.Add(roAjcreidas);

			var roBjcreidas = new RecOtorg()
			{
				Ack = ackBI,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raJCReidas,
				Enterprise = entBI
			};
			roBjcreidas.Params.Add(paramAGDIGNTK);
			roBjcreidas.Params.Add(paramAGDIGPT);
			abcContext.RecOtorgSet.Add(roBjcreidas);

			var ro1jcrsig = new RecOtorg()
			{
				Ack = ack1,
				Enterprise = ent1,
				Office = office1,
				Sucursal = suc5,
				RecAdq = raJCRsig
			};
			ro1jcrsig.Params.Add(paramAGGYA);
			abcContext.RecOtorgSet.Add(ro1jcrsig);

			var ro11jcrsig = new RecOtorg()
			{
				Ack = ack11,
				Enterprise = ent11,
				Office = office1,
				Sucursal = suc5,
				RecAdq = raJCRsig
			};
			ro11jcrsig.Params.Add(paramAGGYA);
			abcContext.RecOtorgSet.Add(ro11jcrsig);

			var ro17jcrsig = new RecOtorg()
			{
				Ack = ack17,
				Enterprise = ent17,
				Office = office1,
				Sucursal = suc5,
				RecAdq = raJCRsig
			};
			ro17jcrsig.Params.Add(paramAGGYA);
			abcContext.RecOtorgSet.Add(ro17jcrsig);

			var roAjcrsig = new RecOtorg()
			{
				Ack = ackA,
				Enterprise = entA,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raJCRsig
			};
			roAjcrsig.Params.Add(paramAGGYAPP);
			abcContext.RecOtorgSet.Add(roAjcrsig);

			var ro1celsig = new RecOtorg()
			{
				Ack = ack1,
				Enterprise = ent1,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raCELsig
			};
			ro1celsig.Params.Add(paramAGGYA);
			abcContext.RecOtorgSet.Add(ro1celsig);

			var ro11celsig = new RecOtorg()
			{
				Ack = ack11,
				Enterprise = ent11,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raCELsig
			};
			ro11celsig.Params.Add(paramAGGYA);
			abcContext.RecOtorgSet.Add(ro11celsig);

			var ro17celsig = new RecOtorg()
			{
				Ack = ack17,
				Enterprise = ent17,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raCELsig
			};
			ro17celsig.Params.Add(paramAGGYA);
			abcContext.RecOtorgSet.Add(ro17celsig);

			var roAcelsig = new RecOtorg()
			{
				Ack = ackA,
				Enterprise = entA,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raCELsig
			};
			roAcelsig.Params.Add(paramAGGYAPP);
			abcContext.RecOtorgSet.Add(roAcelsig);

			var ro1celeidas = new RecOtorg()
			{
				Ack = ack1I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raCELeidas,
				Enterprise = ent1I
			};
			ro1celeidas.Params.Add(paramAGGYA);
			abcContext.RecOtorgSet.Add(ro1celeidas);

			var ro11celeidas = new RecOtorg()
			{
				Ack = ack11I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raCELeidas,
				Enterprise = ent11I
			};
			ro11celeidas.Params.Add(paramAGGYA);
			abcContext.RecOtorgSet.Add(ro11celeidas);

			var ro17celeidas = new RecOtorg()
			{
				Ack = ack17I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raCELeidas,
				Enterprise = ent17I
			};
			ro17celeidas.Params.Add(paramAGGYA);
			abcContext.RecOtorgSet.Add(ro17celeidas);

			var roAceleidas = new RecOtorg()
			{
				Ack = ackAI,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raCELeidas,
				Enterprise = entAI
			};
			roAceleidas.Params.Add(paramAGGYAPP);
			abcContext.RecOtorgSet.Add(roAceleidas);

			var ro2lbjsig = new RecOtorg()
			{
				Ack = ack2,
				Enterprise = ent2,
				Office = office1,
				Sucursal = suc5,
				RecAdq = raLBJsig
			};
			ro2lbjsig.Params.Add(paramGAGSST);
			abcContext.RecOtorgSet.Add(ro2lbjsig);

			var ro12lbjsig = new RecOtorg()
			{
				Ack = ack12,
				Enterprise = ent12,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raLBJsig
			};
			ro12lbjsig.Params.Add(paramGAGSST);
			abcContext.RecOtorgSet.Add(ro12lbjsig);

			var ro2lbjeidas = new RecOtorg()
			{
				Ack = ack2I,
				Sucursal = suc5,
				RecAdq = raLBJeidas,
				Office = office1,
				Enterprise = ent2I
			};
			ro2lbjeidas.Params.Add(paramGAGSST);
			abcContext.RecOtorgSet.Add(ro2lbjeidas);

			var ro12lbjeidas = new RecOtorg()
			{
				Ack = ack12I,
				Sucursal = suc5,
				RecAdq = raLBJeidas,
				Office = office1,
				Enterprise = ent12I
			};
			ro12lbjeidas.Params.Add(paramGAGSST);
			abcContext.RecOtorgSet.Add(ro12lbjeidas);

			var ro2gareidas = new RecOtorg()
			{
				Ack = ack2I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raGAReidas,
				Enterprise = ent2I
			};
			ro2gareidas.Params.Add(paramGAGSST);
			abcContext.RecOtorgSet.Add(ro2gareidas);

			var ro12gareidas = new RecOtorg()
			{
				Ack = ack12I,
				Sucursal = suc5,
				RecAdq = raGAReidas,
				Office = office1,
				Enterprise = ent12I
			};
			ro12gareidas.Params.Add(paramGAGSST);
			abcContext.RecOtorgSet.Add(ro12gareidas);

			var ro2ceseidas = new RecOtorg()
			{
				Ack = ack2I,
				Sucursal = suc5,
				RecAdq = raCESeidas,
				Office = office1,
				Enterprise = ent2I
			};
			ro2ceseidas.Params.Add(paramGAGSST);
			abcContext.RecOtorgSet.Add(ro2ceseidas);

			var ro12ceseidas = new RecOtorg()
			{
				Ack = ack12I,
				Sucursal = suc5,
				RecAdq = raCESeidas,
				Office = office1,
				Enterprise = ent12I
			};
			ro12ceseidas.Params.Add(paramGAGSST);
			abcContext.RecOtorgSet.Add(ro12ceseidas);

			var ro2clr = new RecOtorg()
			{
				Ack = ack2,
				Enterprise = ent2,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raCLR
			};
			ro2clr.Params.Add(paramGAGSST);
			abcContext.RecOtorgSet.Add(ro2clr);

			var ro12clr = new RecOtorg()
			{
				Ack = ack12,
				Enterprise = ent12,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raCLR
			};
			ro12clr.Params.Add(paramGAGSST);
			abcContext.RecOtorgSet.Add(ro12clr);

			var ro1dvgsig = new RecOtorg()
			{
				Ack = ack1,
				Enterprise = ent1,
				Office = office1,
				Sucursal = suc5,
				RecAdq = raDVGsig
			};
			ro1dvgsig.Params.Add(paramAGFLUJO);
			ro1dvgsig.Params.Add(paramAGMFCA);
			ro1dvgsig.Params.Add(paramAGTEMP);
			ro1dvgsig.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro1dvgsig);

			var ro11dvgsig = new RecOtorg()
			{
				Ack = ack11,
				Office = office1,
				Enterprise = ent11,
				Sucursal = suc5,
				RecAdq = raDVGsig
			};
			ro11dvgsig.Params.Add(paramAGFLUJO);
			ro11dvgsig.Params.Add(paramAGMFCA);
			ro11dvgsig.Params.Add(paramAGTEMP);
			ro11dvgsig.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro11dvgsig);

			var ro17dvgsig = new RecOtorg()
			{
				Ack = ack17,
				Enterprise = ent17,
				Office = office1,
				Sucursal = suc5,
				RecAdq = raDVGsig
			};
			ro17dvgsig.Params.Add(paramAGFLUJO);
			ro17dvgsig.Params.Add(paramAGMFCA);
			ro17dvgsig.Params.Add(paramAGTEMP);
			ro17dvgsig.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro17dvgsig);

			var ro7dvgsig = new RecOtorg()
			{
				Ack = ack7,
				Enterprise = ent7,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raDVGsig
			};
			ro7dvgsig.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro7dvgsig);

			var ro1dvgeidas = new RecOtorg()
			{
				Ack = ack1I,
				Sucursal = suc5,
				RecAdq = raDVGeidas,
				Office = office1,
				Enterprise = ent1I
			};
			ro1dvgeidas.Params.Add(paramAGFLUJO);
			ro1dvgeidas.Params.Add(paramAGMFCA);
			ro1dvgeidas.Params.Add(paramAGTEMP);
			ro1dvgeidas.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro1dvgeidas);

			var ro11dvgeidas = new RecOtorg()
			{
				Ack = ack11I,
				Sucursal = suc5,
				RecAdq = raDVGeidas,
				Office = office1,
				Enterprise = ent11I
			};
			ro11dvgeidas.Params.Add(paramAGFLUJO);
			ro11dvgeidas.Params.Add(paramAGMFCA);
			ro11dvgeidas.Params.Add(paramAGTEMP);
			ro11dvgeidas.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro11dvgeidas);

			var ro17dvgeidas = new RecOtorg()
			{
				Ack = ack17I,
				Sucursal = suc5,
				RecAdq = raDVGeidas,
				Office = office1,
				Enterprise = ent17I
			};
			ro17dvgeidas.Params.Add(paramAGFLUJO);
			ro17dvgeidas.Params.Add(paramAGMFCA);
			ro17dvgeidas.Params.Add(paramAGTEMP);
			ro17dvgeidas.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro17dvgeidas);

			var ro7dvgeidas = new RecOtorg()
			{
				Ack = ack7I,
				Sucursal = suc5,
				RecAdq = raDVGeidas,
				Office = office1,
				Enterprise = ent7I
			};
			ro7dvgeidas.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro7dvgeidas);

			var ro1mphsig = new RecOtorg()
			{
				Ack = ack1,
				Enterprise = ent1,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raMPHsig
			};
			ro1mphsig.Params.Add(paramAGFLUJO);
			ro1mphsig.Params.Add(paramAGMFCA);
			ro1mphsig.Params.Add(paramAGTEMP);
			ro1mphsig.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro1mphsig);

			var ro11mphsig = new RecOtorg()
			{
				Ack = ack11,
				Enterprise = ent11,
				Office = office1,
				Sucursal = suc5,
				RecAdq = raMPHsig
			};
			ro11mphsig.Params.Add(paramAGFLUJO);
			ro11mphsig.Params.Add(paramAGMFCA);
			ro11mphsig.Params.Add(paramAGTEMP);
			ro11mphsig.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro11mphsig);

			var ro17mphsig = new RecOtorg()
			{
				Ack = ack17,
				Enterprise = ent17,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raMPHsig
			};
			ro17mphsig.Params.Add(paramAGFLUJO);
			ro17mphsig.Params.Add(paramAGMFCA);
			ro17mphsig.Params.Add(paramAGTEMP);
			ro17mphsig.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro17mphsig);

			var ro7mphsig = new RecOtorg()
			{
				Ack = ack7,
				Enterprise = ent7,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raMPHsig
			};
			ro7mphsig.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro7mphsig);

			var ro1mpheidas = new RecOtorg()
			{
				Ack = ack1I,
				Sucursal = suc5,
				RecAdq = raMPHeidas,
				Office = office1,
				Enterprise = ent1I
			};
			ro1mpheidas.Params.Add(paramAGFLUJO);
			ro1mpheidas.Params.Add(paramAGMFCA);
			ro1mpheidas.Params.Add(paramAGTEMP);
			ro1mpheidas.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro1mpheidas);

			var ro11mpheidas = new RecOtorg()
			{
				Ack = ack11I,
				Sucursal = suc5,
				RecAdq = raMPHeidas,
				Office = office1,
				Enterprise = ent11I
			};
			ro11mpheidas.Params.Add(paramAGFLUJO);
			ro11mpheidas.Params.Add(paramAGMFCA);
			ro11mpheidas.Params.Add(paramAGTEMP);
			ro11mpheidas.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro11mpheidas);

			var ro17mpheidas = new RecOtorg()
			{
				Ack = ack17I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raMPHeidas,
				Enterprise = ent17I
			};
			ro17mpheidas.Params.Add(paramAGFLUJO);
			ro17mpheidas.Params.Add(paramAGMFCA);
			ro17mpheidas.Params.Add(paramAGTEMP);
			ro17mpheidas.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro17mpheidas);

			var ro7mpheidas = new RecOtorg()
			{
				Ack = ack7I,
				Sucursal = suc5,
				RecAdq = raMPHeidas,
				Office = office1,
				Enterprise = ent7I
			};
			ro7mpheidas.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro7mpheidas);

			var ro1joleidas = new RecOtorg()
			{
				Ack = ack1I,
				Sucursal = suc5,
				RecAdq = raJOLeidas,
				Office = office1,
				Enterprise = ent1I
			};
			ro1joleidas.Params.Add(paramAGFLUJO);
			ro1joleidas.Params.Add(paramAGMFCA);
			ro1joleidas.Params.Add(paramAGTEMP);
			ro1joleidas.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro1joleidas);

			var ro11joleidas = new RecOtorg()
			{
				Ack = ack11I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raJOLeidas,
				Enterprise = ent11I
			};
			ro11joleidas.Params.Add(paramAGFLUJO);
			ro11joleidas.Params.Add(paramAGMFCA);
			ro11joleidas.Params.Add(paramAGTEMP);
			ro11joleidas.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro11joleidas);

			var ro17joleidas = new RecOtorg()
			{
				Ack = ack17I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raJOLeidas,
				Enterprise = ent17I
			};
			ro17joleidas.Params.Add(paramAGFLUJO);
			ro17joleidas.Params.Add(paramAGMFCA);
			ro17joleidas.Params.Add(paramAGTEMP);
			ro17joleidas.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro17joleidas);

			var ro7joleidas = new RecOtorg()
			{
				Ack = ack7I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raJOLeidas,
				Enterprise = ent7I
			};
			ro7joleidas.Params.Add(paramAGPHCA);
			abcContext.RecOtorgSet.Add(ro7joleidas);

			var ro1erdsig = new RecOtorg()
			{
				Ack = ack1,
				Enterprise = ent1,
				Office = office1,
				Sucursal = suc5,
				RecAdq = raERDsig
			};
			ro1erdsig.Params.Add(paramAGHUEVPK);
			abcContext.RecOtorgSet.Add(ro1erdsig);

			var ro11erdsig = new RecOtorg()
			{
				Ack = ack11,
				Enterprise = ent11,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raERDsig
			};
			ro11erdsig.Params.Add(paramAGHUEVPK);
			abcContext.RecOtorgSet.Add(ro11erdsig);

			var ro1erdeidas = new RecOtorg()
			{
				Ack = ack1I,
				Sucursal = suc5,
				RecAdq = raERDeidas,
				Office = office1,
				Enterprise = ent1I
			};
			ro1erdeidas.Params.Add(paramAGHUEVPK);
			abcContext.RecOtorgSet.Add(ro1erdeidas);

			var ro11erdeidas = new RecOtorg()
			{
				Ack = ack11I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raERDeidas,
				Enterprise = ent11I
			};
			ro11erdeidas.Params.Add(paramAGHUEVPK);
			abcContext.RecOtorgSet.Add(ro11erdeidas);

			var ro1vpreidas = new RecOtorg()
			{
				Ack = ack1I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raVPReidas,
				Enterprise = ent1I
			};
			ro1vpreidas.Params.Add(paramAGHUEVPK);
			abcContext.RecOtorgSet.Add(ro1vpreidas);

			var ro11vpreidas = new RecOtorg()
			{
				Ack = ack11I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raVPReidas,
				Enterprise = ent11I
			};
			ro11vpreidas.Params.Add(paramAGHUEVPK);
			abcContext.RecOtorgSet.Add(ro11vpreidas);

			var ro1vprsig = new RecOtorg()
			{
				Ack = ack1,
				Enterprise = ent1,
				Office = office1,
				Sucursal = suc5,
				RecAdq = raVPRsig
			};
			ro1vprsig.Params.Add(paramAGHUEVPK);
			abcContext.RecOtorgSet.Add(ro1vprsig);

			var ro11vprsig = new RecOtorg()
			{
				Ack = ack11,
				Enterprise = ent11,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raVPRsig
			};
			ro11vprsig.Params.Add(paramAGHUEVPK);
			abcContext.RecOtorgSet.Add(ro11vprsig);

			var ro1ivrsig = new RecOtorg()
			{
				Ack = ack1,
				Enterprise = ent1,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raIVRsig
			};
			ro1ivrsig.Params.Add(paramAGCR);
			ro1ivrsig.Params.Add(paramAGAS);
			ro1ivrsig.Params.Add(paramAGCD);
			ro1ivrsig.Params.Add(paramAGCU);
			ro1ivrsig.Params.Add(paramAGNI);
			ro1ivrsig.Params.Add(paramAGHG);
			ro1ivrsig.Params.Add(paramAGPB);
			ro1ivrsig.Params.Add(paramAGZN);
			abcContext.RecOtorgSet.Add(ro1ivrsig);

			var ro11ivrsig = new RecOtorg()
			{
				Ack = ack11,
				Enterprise = ent11,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raIVRsig
			};
			ro11ivrsig.Params.Add(paramAGCR);
			ro11ivrsig.Params.Add(paramAGAS);
			ro11ivrsig.Params.Add(paramAGCD);
			ro11ivrsig.Params.Add(paramAGCU);
			ro11ivrsig.Params.Add(paramAGNI);
			ro11ivrsig.Params.Add(paramAGHG);
			ro11ivrsig.Params.Add(paramAGPB);
			ro11ivrsig.Params.Add(paramAGZN);
			abcContext.RecOtorgSet.Add(ro11ivrsig);

			var ro17ivrsig = new RecOtorg()
			{
				Ack = ack17,
				Office = office1,
				Enterprise = ent17,
				Sucursal = suc5,
				RecAdq = raIVRsig
			};
			ro17ivrsig.Params.Add(paramAGAS);
			ro17ivrsig.Params.Add(paramAGCD);
			ro17ivrsig.Params.Add(paramAGCU);
			ro17ivrsig.Params.Add(paramAGNI);
			ro17ivrsig.Params.Add(paramAGHG);
			ro17ivrsig.Params.Add(paramAGPB);
			ro17ivrsig.Params.Add(paramAGZN);
			abcContext.RecOtorgSet.Add(ro17ivrsig);

			var roBivrsig = new RecOtorg()
			{
				Ack = ackB,
				Enterprise = entB,
				Office = office1,
				Sucursal = suc5,
				RecAdq = raIVRsig
			};
			roBivrsig.Params.Add(paramAGDIG);
			abcContext.RecOtorgSet.Add(roBivrsig);

			var ro1ivreidas = new RecOtorg()
			{
				Ack = ack1I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raIVReidas,
				Enterprise = ent1I
			};
			ro1ivreidas.Params.Add(paramAGCR);
			ro1ivreidas.Params.Add(paramAGAS);
			ro1ivreidas.Params.Add(paramAGCD);
			ro1ivreidas.Params.Add(paramAGCU);
			ro1ivreidas.Params.Add(paramAGNI);
			ro1ivreidas.Params.Add(paramAGHG);
			ro1ivreidas.Params.Add(paramAGPB);
			ro1ivreidas.Params.Add(paramAGZN);
			abcContext.RecOtorgSet.Add(ro1ivreidas);

			var ro11ivreidas = new RecOtorg()
			{
				Ack = ack11I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raIVReidas,
				Enterprise = ent11I
			};
			ro11ivreidas.Params.Add(paramAGCR);
			ro11ivreidas.Params.Add(paramAGAS);
			ro11ivreidas.Params.Add(paramAGCD);
			ro11ivreidas.Params.Add(paramAGCU);
			ro11ivreidas.Params.Add(paramAGNI);
			ro11ivreidas.Params.Add(paramAGHG);
			ro11ivreidas.Params.Add(paramAGPB);
			ro11ivreidas.Params.Add(paramAGZN);
			abcContext.RecOtorgSet.Add(ro11ivreidas);

			var ro17ivreidas = new RecOtorg()
			{
				Ack = ack17I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raIVReidas,
				Enterprise = ent17I
			};
			ro17ivreidas.Params.Add(paramAGAS);
			ro17ivreidas.Params.Add(paramAGCD);
			ro17ivreidas.Params.Add(paramAGCU);
			ro17ivreidas.Params.Add(paramAGNI);
			ro17ivreidas.Params.Add(paramAGHG);
			ro17ivreidas.Params.Add(paramAGPB);
			ro17ivreidas.Params.Add(paramAGZN);
			abcContext.RecOtorgSet.Add(ro17ivreidas);

			var roBivreidas = new RecOtorg()
			{
				Ack = ackBI,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raIVReidas,
				Enterprise = entBI
			};
			roBivreidas.Params.Add(paramAGDIG);
			abcContext.RecOtorgSet.Add(roBivreidas);

			var ro1ctrsig = new RecOtorg()
			{
				Ack = ack1,
				Enterprise = ent1,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raCTRsig
			};
			ro1ctrsig.Params.Add(paramAGCR);
			ro1ctrsig.Params.Add(paramAGAS);
			ro1ctrsig.Params.Add(paramAGCD);
			ro1ctrsig.Params.Add(paramAGCU);
			ro1ctrsig.Params.Add(paramAGNI);
			ro1ctrsig.Params.Add(paramAGHG);
			ro1ctrsig.Params.Add(paramAGPB);
			ro1ctrsig.Params.Add(paramAGZN);
			abcContext.RecOtorgSet.Add(ro1ctrsig);

			var ro11ctrsig = new RecOtorg()
			{
				Ack = ack11,
				Enterprise = ent11,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raCTRsig
			};
			ro11ctrsig.Params.Add(paramAGCR);
			ro11ctrsig.Params.Add(paramAGAS);
			ro11ctrsig.Params.Add(paramAGCD);
			ro11ctrsig.Params.Add(paramAGCU);
			ro11ctrsig.Params.Add(paramAGNI);
			ro11ctrsig.Params.Add(paramAGHG);
			ro11ctrsig.Params.Add(paramAGPB);
			ro11ctrsig.Params.Add(paramAGZN);
			abcContext.RecOtorgSet.Add(ro11ctrsig);

			var ro17ctrsig = new RecOtorg()
			{
				Ack = ack17,
				Enterprise = ent17,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raCTRsig
			};
			ro17ctrsig.Params.Add(paramAGAS);
			ro17ctrsig.Params.Add(paramAGCD);
			ro17ctrsig.Params.Add(paramAGCU);
			ro17ctrsig.Params.Add(paramAGNI);
			ro17ctrsig.Params.Add(paramAGHG);
			ro17ctrsig.Params.Add(paramAGPB);
			ro17ctrsig.Params.Add(paramAGZN);
			abcContext.RecOtorgSet.Add(ro17ctrsig);

			var roBctrsig = new RecOtorg()
			{
				Ack = ackB,
				Enterprise = entB,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raCTRsig
			};
			roBctrsig.Params.Add(paramAGDIG);
			abcContext.RecOtorgSet.Add(roBctrsig);

			var ro1ctreidas = new RecOtorg()
			{
				Ack = ack1I,
				Sucursal = suc5,
				RecAdq = raCTReidas,
				Office = office1,
				Enterprise = ent1I
			};
			ro1ctreidas.Params.Add(paramAGCR);
			ro1ctreidas.Params.Add(paramAGAS);
			ro1ctreidas.Params.Add(paramAGCD);
			ro1ctreidas.Params.Add(paramAGCU);
			ro1ctreidas.Params.Add(paramAGNI);
			ro1ctreidas.Params.Add(paramAGHG);
			ro1ctreidas.Params.Add(paramAGPB);
			ro1ctreidas.Params.Add(paramAGZN);
			abcContext.RecOtorgSet.Add(ro1ctreidas);

			var ro11ctreidas = new RecOtorg()
			{
				Ack = ack11I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raCTReidas,
				Enterprise = ent11I
			};
			ro11ctreidas.Params.Add(paramAGCR);
			ro11ctreidas.Params.Add(paramAGAS);
			ro11ctreidas.Params.Add(paramAGCD);
			ro11ctreidas.Params.Add(paramAGCU);
			ro11ctreidas.Params.Add(paramAGNI);
			ro11ctreidas.Params.Add(paramAGHG);
			ro11ctreidas.Params.Add(paramAGPB);
			ro11ctreidas.Params.Add(paramAGZN);
			abcContext.RecOtorgSet.Add(ro11ctreidas);

			var ro17ctreidas = new RecOtorg()
			{
				Ack = ack17I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raCTReidas,
				Enterprise = ent17I
			};
			ro17ctreidas.Params.Add(paramAGAS);
			ro17ctreidas.Params.Add(paramAGCD);
			ro17ctreidas.Params.Add(paramAGCU);
			ro17ctreidas.Params.Add(paramAGNI);
			ro17ctreidas.Params.Add(paramAGHG);
			ro17ctreidas.Params.Add(paramAGPB);
			ro17ctreidas.Params.Add(paramAGZN);
			abcContext.RecOtorgSet.Add(ro17ctreidas);

			var roBctreidas = new RecOtorg()
			{
				Ack = ackBI,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raCTReidas,
				Enterprise = entBI
			};
			roBctreidas.Params.Add(paramAGDIG);
			abcContext.RecOtorgSet.Add(roBctreidas);

			var ro1ampeidas = new RecOtorg()
			{
				Ack = ack1I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raAMPeidas,
				Enterprise = ent1I
			};
			ro1ampeidas.Params.Add(paramAGCR);
			ro1ampeidas.Params.Add(paramAGAS);
			ro1ampeidas.Params.Add(paramAGCD);
			ro1ampeidas.Params.Add(paramAGCU);
			ro1ampeidas.Params.Add(paramAGNI);
			ro1ampeidas.Params.Add(paramAGHG);
			ro1ampeidas.Params.Add(paramAGPB);
			ro1ampeidas.Params.Add(paramAGZN);
			abcContext.RecOtorgSet.Add(ro1ampeidas);

			var ro11ampeidas = new RecOtorg()
			{
				Ack = ack11I,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raAMPeidas,
				Enterprise = ent11I
			};
			ro11ampeidas.Params.Add(paramAGCR);
			ro11ampeidas.Params.Add(paramAGAS);
			ro11ampeidas.Params.Add(paramAGCD);
			ro11ampeidas.Params.Add(paramAGCU);
			ro11ampeidas.Params.Add(paramAGNI);
			ro11ampeidas.Params.Add(paramAGHG);
			ro11ampeidas.Params.Add(paramAGPB);
			ro11ampeidas.Params.Add(paramAGZN);
			abcContext.RecOtorgSet.Add(ro11ampeidas);

			var ro17ampeidas = new RecOtorg()
			{
				Ack = ack17I,
				Office = office1,
				Sucursal = suc5,
				RecAdq = raAMPeidas,
				Enterprise = ent17I
			};
			ro17ampeidas.Params.Add(paramAGAS);
			ro17ampeidas.Params.Add(paramAGCD);
			ro17ampeidas.Params.Add(paramAGCU);
			ro17ampeidas.Params.Add(paramAGNI);
			ro17ampeidas.Params.Add(paramAGHG);
			ro17ampeidas.Params.Add(paramAGPB);
			ro17ampeidas.Params.Add(paramAGZN);
			abcContext.RecOtorgSet.Add(ro17ampeidas);

			var roBampeidas = new RecOtorg()
			{
				Ack = ackBI,
				Sucursal = suc5,
				Office = office1,
				RecAdq = raAMPeidas,
				Enterprise = entBI
			};
			roBampeidas.Params.Add(paramAGDIG);
			abcContext.RecOtorgSet.Add(roBampeidas);

			// Fin Reconocimientos Otorgados

			// Si quiero que estos objetos se adicionen a la bitacora de cambios:
			//	abcContext.SaveChanges("root");
			abcContext.SaveChanges();

	        groupAGNTOTK.DispParamId = paramAGDIGNTK.Id;
			groupAGMTN01.DispParamId = paramAGDIG.Id;
			groupAGPTOTAL.DispParamId = paramAGDIGPT.Id;
			
			// Esto hay que hacerlo realmente donde mismo se crean los Analistas
			// si se quiere usar esta funcion seed()
	        //AnnalistController().UpdateFromIntelesis();

			abcContext.SaveChanges();

			base.Seed(context);
        }
    }

    // AbcContext contains the tables of the DB described in the models.
    // This approach is called 'Code First'.
    public class AbcContext : DbContext
    {
        public AbcContext()
            : base("name=AbcContextCxnStr")
        {
            Database.SetInitializer<AbcContext>(new AbcWebContextSeeder());
        }

		// Tabla para la bitacora de cambios
		public DbSet<Audit> AuditRecords { get; set; }

		//Models related with the users, rights, roles and sessions.
		public DbSet<User> UserSet { get; set; }
        public DbSet<Right> RightSet { get; set; }
        public DbSet<Role> RoleSet { get; set; }
        public DbSet<SessionLog> SessionLogSet { get; set; }
        public DbSet<Notes> NotesSet { get; set; }
		public DbSet<ParamCol> ParamColSet { get; set; }

		//Models related with the problem itself.
		// Region, Mercado, Oficina y Sucursal
		public DbSet<Region> RegionSet { get; set; }
        public DbSet<Market> MarketSet { get; set; }
        public DbSet<Office> OfficeSet { get; set; }
        public DbSet<Sucursal> SucursalSet { get; set; }
        // Matrices
        public DbSet<BaseMatrix> BaseMatrixSet { get; set; } 
        public DbSet<Matrix> MatrixSet { get; set; }
		//Paramatro base, parámetro, Clasificacion Quimica, unidad de medida
		// familia a la que pertenece el parámetro base (esta se va a borrar)
		public DbSet<BaseParam> BaseParamSet { get; set; }
		public DbSet<ClasificacionQuimica> ClasificacionQuimicaSet { get; set; }
		//public DbSet<BaseParamFamily> BaseParamFamilySet { get; set; }
        public DbSet<Param> ParamSet { get; set; }
        public DbSet<MeasureUnit> MeasureUnitSet { get; set; }
        public DbSet<Currency> CurrencySet { get; set; }
        public DbSet<Price> PriceSet { get; set; }
        public DbSet<MaxPermitedLimit> MaxPermitedLimitSet { get; set; }
        public DbSet<ParamRoute> ParamRouteSet { get; set; }
        public DbSet<ParamPrintResult> ParamPrintResultSet { get; set; }
		public DbSet<Rama> RamaSet { get; set; }
		// Reconocimientos, Acciones y Empresas
		public DbSet<Enterprise> EnterpriseSet { get; set; }
        public DbSet<Accion> AccionSet { get; set; }
        public DbSet<Ack> AckSet { get; set; }
		public DbSet<Alcance> AlcanceSet { get; set; }
		public DbSet<RecAdq> RecAdqSet { get; set; }
		public DbSet<RecOtorg> RecOtorgSet { get; set; }
		//Analista
		public DbSet<Annalist> AnnalistSet { get; set; }
        public DbSet<AnnalistKey> AnnalistKeySet { get; set; }
		public DbSet<Jornada> JornadaSet { get; set; }
		public DbSet<TipoSignatario> TipoSignatarioSet { get; set; }
		public DbSet<AnalyticsMethod> AnalyticsMethodSet { get; set; }
		// Area Analitica
        public DbSet<AnalyticsArea> AnalyticsAreaSet { get; set; }
		public DbSet<CentroCosto> CentroCostoSet { get; set; }
		public DbSet<UnidadAnalitica> UnidadAnaliticaSet { get; set; }
		// Paquete, norma a la que responde el paquete creado y grupo
		public DbSet<Package> PackageSet { get; set; }
        public DbSet<Norm> NormSet { get; set; }
        public DbSet<Group> GroupSet { get; set; }
		public DbSet<TipoServicio> TipoServicioSet { get; set; }
		//public DbSet<ComplexSampling> ComplexSamplingSet { get; set; }
        public DbSet<Status> StatusSet { get; set; }
		// Metodo
		public DbSet<Method> MethodSet { get; set; }
        public DbSet<Container> ContainerSet { get; set; }
        public DbSet<Preserver> PreserverSet { get; set; }
        public DbSet<Residue> ResidueSet { get; set; }
		public DbSet<Material> MaterialSet { get; set; }

		// Solamente para tener constancia de cuando fue la ultima actualizacion de Intelesis
		public DbSet<Intelesis> IntelesisSet { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }

		// ***********************************
		public int SaveChanges(string userName, string ipAddress = "127.0.0.1")
		{
			// Get all Added/Deleted/Modified entities (not Unmodified or Detached)
			foreach (var x in ChangeTracker.Entries()
				.Where(p => p.State == (EntityState)System.Data.EntityState.Added 
				            || p.State == (EntityState)System.Data.EntityState.Deleted 
				            || p.State == (EntityState)System.Data.EntityState.Modified)
				.SelectMany(entry => GetAuditRecordsForChange(entry, userName, ipAddress)))
			{
				AuditRecords.Add(x);
			}

			// Call the original SaveChanges(), which will save both the changes made and the audit records
			return base.SaveChanges();
		}

	    private IEnumerable<Audit> GetAuditRecordsForChange(DbEntityEntry dbEntry, string userName, string ipAddress)
		{
			try
			{
				var result = new List<Audit>();

				var changeTime = DateTime.UtcNow;

				// Get the Table() attribute, if one exists
				var tableAttr = dbEntry.Entity.GetType()
					.GetCustomAttributes(typeof(TableAttribute), true)
					.SingleOrDefault() as TableAttribute;

				// Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
				var tableName = tableAttr != null 
					? tableAttr.Name : dbEntry.Entity.GetType().Name;
				
				switch (dbEntry.State)
				{
					case (EntityState)System.Data.EntityState.Added:
						// For Inserts, just add the whole record
						// If the entity implements IDescribableEntity, use the description from Describe(), otherwise use ToString()

						result.AddRange(dbEntry.CurrentValues.PropertyNames
							.Select(propertyName => new Audit()
							{
								AuditId = Guid.NewGuid(),
								UserName = userName,
								EventDate = changeTime,
								IpAddress = ipAddress,
								EventType = "A", // Added
								TableName = tableName,
								ElementId = (int)dbEntry.CurrentValues.GetValue<object>("Id"),
								ColumnName = propertyName,
								NewValue = dbEntry.CurrentValues.GetValue<object>(propertyName) == null 
									? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString()
							}));
						break;

					case (EntityState)System.Data.EntityState.Deleted:
						// Same with deletes, do the whole record, and use either the description from Describe() or ToString()
						result.Add(new Audit()
						{
							AuditId = Guid.NewGuid(),
							UserName = userName,
							EventDate = changeTime,
							IpAddress = ipAddress,
							EventType = "D", // Deleted
							TableName = tableName,
							ElementId = (int)dbEntry.OriginalValues.GetValue<object>("Id"),
							ColumnName = "*ALL",
							NewValue = dbEntry.OriginalValues.ToObject().ToString() // (dbEntry.OriginalValues.ToObject() is IDescribableEntity) ? (dbEntry.OriginalValues.ToObject() as IDescribableEntity).Describe() : dbEntry.OriginalValues.ToObject().ToString()
						}
							);
						break;

					case (EntityState)System.Data.EntityState.Modified:
						result.AddRange(from propertyName in dbEntry.OriginalValues.PropertyNames
							where !Equals(dbEntry.OriginalValues.GetValue<object>(propertyName), 
								dbEntry.CurrentValues.GetValue<object>(propertyName))
							select new Audit()
							{
								AuditId = Guid.NewGuid(),
								UserName = userName,
								EventDate = changeTime,
								IpAddress = ipAddress,
								EventType = "M", // Modified
								TableName = tableName,
								ElementId = (int)dbEntry.OriginalValues.GetValue<object>("Id"),
								ColumnName = propertyName,
								OriginalValue = dbEntry.OriginalValues.GetValue<object>(propertyName) == null 
									? null : dbEntry.OriginalValues.GetValue<object>(propertyName).ToString(),
								NewValue = dbEntry.CurrentValues.GetValue<object>(propertyName) == null 
									? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString()
							});
						break;
				}
				// Otherwise, don't do anything, we don't care about Unchanged or Detached entities

				return result;
			}
			catch (Exception exception)
			{
				throw new Exception(exception.ToString());
			}
			
		}

		// ***********************************

		//private string LocalIPAddress()
		//{
		//	if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
		//	{
		//		return null;
		//	}
		//	return Request.UserHostName;
		//	//var host = Dns.GetHostEntry(Dns.GetHostName());

		//	//return host
		//	//	.AddressList
		//	//	.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
		//}
	}

	public class Cryptografy
    {
        public static string CalcHash(string data)
        {
            var ret = "";
            try
            {
                using (var mem = new MemoryStream())
                {
                    var bWriter = new BinaryWriter(mem);
                    bWriter.Write(data);
                    mem.Position = 0;
                    MD5 md5 = new MD5CryptoServiceProvider();
                    var res = md5.ComputeHash(mem);
                    bWriter.Close();
                    ret = res.Aggregate(ret, (current, t) => current + (char)t);
                }
            }
            catch { ret = "N/A"; }
            return ret;
        }
    }
}
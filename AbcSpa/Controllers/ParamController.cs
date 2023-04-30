using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AbcPersistent.Models;

namespace AbcSpa.Controllers
{
    public class ParamController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveParam")]
        public JsonResult SaveParam(Param param)
        {
            try
            {
                var par = param.Id == 0
                    ? new Param()
                    {
                        Precio = new Price(),
                        ParamRoutes = new List<ParamRoute>(),
                        Annalists = new List<Annalist>()
                    }
                    : _dbStore.ParamSet.Find(param.Id);

                if (!AssignValues(par, param))
                    throw new Exception("Error al llenar los datos del param");

                if (!CheckAnnalistList(par, param))
                    throw new Exception("Error al llenar la lista de analistas");

                    if (!CheckParamRoutes(par, param))
                        throw new Exception("Error al llenar la lista de límites máximos permisibles");

                foreach (var _pr in from pr in param.ParamRoutes
                                    where pr.Id == 0
                                    select new ParamRoute()
                                    {
                                        MaxPermitedLimit = _dbStore.MaxPermitedLimitSet.Find(pr.MaxPermitedLimit.Id),
                                        //Matrix = pr.Matrix != null ? _dbStore.MatrixSet.Find(pr.Matrix.Id) : null,
                                        Group = pr.Group != null ? _dbStore.GroupSet.Find(pr.Group.Id) : null,
                                        Package = pr.Package != null ? _dbStore.PackageSet.Find(pr.Package.Id) : null,
                                        Value = pr.Value,
                                        DecimalsPoints = pr.DecimalsPoints
                                    })
                {
                    _dbStore.ParamRouteSet.Add(_pr);
                }
                _dbStore.SaveChanges();

                var routes = _dbStore.ParamRouteSet.Where(pr => pr.Parameter == null);
                foreach (var pr in routes.ToList())
                {
                    par.ParamRoutes.Add(_dbStore.ParamRouteSet.Find(pr.Id));
                }

				par.PublishInAutolab = true;
				if (!par.Active || string.IsNullOrEmpty(par.Description)
				    || (par.MaxPermitedLimit == null)
				    || (par.PerTurnCapacity == null)
				    || (par.PerWeekCapacity == null)
				    || (par.DecimalesReporte == null)
				    || (par.AnalyticsMethod == null)
				    || string.IsNullOrEmpty(par.AutolabAssignedAreaName)
				    || string.IsNullOrEmpty(par.ParamUniquekey)
				    || string.IsNullOrEmpty(par.GenericKeyForStatistic)
				   // || string.IsNullOrEmpty(par.GenericKey)
				    || string.IsNullOrEmpty(par.Formula)
				    || (par.Precio == null)
				    || (par.QcObj.HasQc && (par.QcObj.LowerLimit == null || par.QcObj.UpperLimit == null))
				    || (par.Uncertainty.Value == null)
				    || (par.BaseParam == null)
				    || (par.CentroCosto == null)
				    || (par.Rama == null)
				    || (par.UnidadAnalitica == null)
				    || !par.Annalists.Any(a => a.Active)
				    || (par.Metodo == null)
				    || (par.Unit == null)
					|| !par.Matrixes.Any(m => m.Active)
				    || (par.TipoServicio == null)
				    || (par.AnalyticsMethod == null)
				    || ((par.SucursalVende == null) && (par.SucursalRealiza == null))
					|| (par.Residue == null)
					|| (par.AnalysisTime == null)
					|| (par.RequiredVolume == null)
					|| (par.MinimumVolume == null)
					|| (par.ReportLimit == null)
					|| (par.DetectionLimit == null)
					|| (par.CuantificationLimit == null)
					|| (par.DeliverTime == null)
					|| (par.MaxTimeBeforeAnalysis == null)
					|| (par.LabDeliverTime == null)
					|| (par.ReportTime == null)
					|| (par.Container == null)
					|| (par.Preserver == null)
					)
				{
					par.PublishInAutolab = false;
				}

                if (param.Id == 0)
                {
                   _dbStore.PriceSet.Add(par.Precio);
                    _dbStore.ParamSet.Add(par);
                }

                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

		private bool AssignValues(Param par, Param param)
        {
            try
            {
                par.MaxPermitedLimit = param.MaxPermitedLimit;
                par.PerTurnCapacity = param.PerTurnCapacity;
                par.PerWeekCapacity = param.PerWeekCapacity;
                par.AutolabAssignedAreaName = param.AutolabAssignedAreaName;
                par.ParamUniquekey = param.ParamUniquekey;
                par.GenericKeyForStatistic = param.GenericKeyForStatistic;
              //  par.GenericDescription = param.GenericDescription;
				par.InternetPublish = param.InternetPublish;
               // par.GenericKey = param.GenericKey;
                par.Formula = param.Formula;
	            par.TipoFormula = param.TipoFormula;
				par.Rama = param.Rama != null ? _dbStore.RamaSet.Find(param.Rama.Id) : null;
                par.Uncertainty = param.Uncertainty;
                par.QcObj = param.QcObj;
                par.ResiduoPeligroso = param.ResiduoPeligroso;
                par.ReportaCliente = param.ReportaCliente;
                par.SellSeparated = param.SellSeparated;
                par.CuentaEstadistica = param.CuentaEstadistica;
                par.Week = param.Week;
                par.Description = param.Description;
                par.DecimalesReporte = param.DecimalesReporte;
                par.BaseParam = _dbStore.BaseParamSet.Find(param.BaseParam.Id);
                par.AnalyticsMethod = param.AnalyticsMethod != null ? _dbStore.AnalyticsMethodSet.Find(param.AnalyticsMethod.Id) : null;
                par.CentroCosto = (param.CentroCosto != null)
					? _dbStore.CentroCostoSet.Find(param.CentroCosto.Id)
                    : null;
                par.Unit = (param.Unit != null) ? _dbStore.MeasureUnitSet.Find(param.Unit.Id) : null;
                par.Metodo = (param.Metodo != null) ? _dbStore.MethodSet.Find(param.Metodo.Id) : null;
                par.TipoServicio = (param.TipoServicio != null)
                    ? _dbStore.TipoServicioSet.Find(param.TipoServicio.Id)
                    : null;
				par.UnidadAnalitica = (param.UnidadAnalitica != null)
					? _dbStore.UnidadAnaliticaSet.Find(param.UnidadAnalitica.Id)
					: null;
                par.AnnalistKey = (param.AnnalistKey != null)
                    ? _dbStore.AnnalistKeySet.Find(param.AnnalistKey.Id)
                    : null;
                par.Precio.Value = param.Precio.Value;
                par.Precio.Currency = _dbStore.CurrencySet.Find(param.Precio.Currency.Id);
                par.SucursalVende = (param.SucursalVende != null)
                    ? _dbStore.SucursalSet.Find(param.SucursalVende.Id)
                    : null;
                par.SucursalRealiza = (param.SucursalRealiza != null)
                    ? _dbStore.SucursalSet.Find(param.SucursalRealiza.Id)
                    : null;
				par.Residue = param.Residue != null
					? _dbStore.ResidueSet.Find(param.Residue.Id)
					: null;
				par.AnalysisTime = param.AnalysisTime;

				//par.Matrix = param.Matrix != null
				//	? _dbStore.MatrixSet.Find(param.Matrix.Id)
				//	: null;

				par.RequiredVolume = param.RequiredVolume;
				par.MinimumVolume = param.MinimumVolume;

				par.ReportLimit = param.ReportLimit;
				par.DeliverTime = param.DeliverTime;
				par.MaxTimeBeforeAnalysis = param.MaxTimeBeforeAnalysis;
				par.LabDeliverTime = param.LabDeliverTime;
				par.ReportTime = param.ReportTime;
				par.DetectionLimit = param.DetectionLimit;
				par.CuantificationLimit = param.CuantificationLimit;
				par.Container = param.Container != null ? _dbStore.ContainerSet.Find(param.Container.Id) : null;
				par.Preserver = param.Preserver != null ? _dbStore.PreserverSet.Find(param.Preserver.Id) : null;

				if (!checkMatrixes(par, param))
					throw new Exception("Error al llenar la lista de Matrices");
                if (!checkGroups(par, param))
                    throw new Exception("Error al llenar la lista de Grupos");
                if (!checkPackages(par, param))
                    throw new Exception("Error al llenar la lista de Paquetes");

                if (param.ParamPrintResults.FirstOrDefault().Id == 0 || param.Id == 0)
                {
                    _dbStore.ParamPrintResultSet.Add(new ParamPrintResult()
                    {
                        Yes =
                            param.ParamPrintResults.FirstOrDefault(prr => prr.Parameter == null && prr.Group == null)
                                .Yes
                    });
                    _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                    par.ParamPrintResults.Add(_dbStore.ParamPrintResultSet.FirstOrDefault(prr => prr.Group == null && prr.Parameter == null));
                }
                else
                    _dbStore.ParamPrintResultSet.FirstOrDefault(prr => prr.Group == null && prr.Parameter.Id == par.Id).Yes = param.ParamPrintResults.FirstOrDefault().Yes;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool CheckAnnalistList(Param par, Param param)
        {
            try
            {
                var annalisttodelete = par.Annalists.Where(a => a.Active && param.Annalists.All(m => m.Id != a.Id));

                foreach (var a in annalisttodelete.ToList())
                    par.Annalists.Remove(_dbStore.AnnalistSet.Find(a.Id));

                var annalisttoadd = param.Annalists.Where(a => par.Annalists.All(m => m.Id != a.Id));

                foreach (var a in annalisttoadd)
                    par.Annalists.Add(_dbStore.AnnalistSet.Find(a.Id));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool CheckParamRoutes(Param par, Param parameter)
        {
            try
            {
                var parRoutetodelete = par.ParamRoutes.Where(pr => parameter.ParamRoutes.Where(p => p.Id != 0).All(m => m.Id != pr.Id));
                foreach (var pr in parRoutetodelete.ToList())
                {
                    par.ParamRoutes.Remove(_dbStore.ParamRouteSet.Find(pr.Id));
                    _dbStore.ParamRouteSet.Remove(_dbStore.ParamRouteSet.Find(pr.Id));
                }

                var parWithValueChanged = parameter.ParamRoutes.Where(pr => par.ParamRoutes.Any(m => m.Id == pr.Id && m.Value != pr.Value && m.Id != 0));
                foreach (var pr in parWithValueChanged.ToList())
                {
                    _dbStore.ParamRouteSet.Find(pr.Id).Value = pr.Value;
                }

                var paramRoutetoadd = parameter.ParamRoutes.Where(gr => par.ParamRoutes.All(m => m.Id != gr.Id) && gr.Id != 0);
                foreach (var g in paramRoutetoadd.ToList())
                    par.ParamRoutes.Add(_dbStore.ParamRouteSet.Find(g.Id));

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

		private bool checkMatrixes(Param par, Param parameter)
		{
			try
			{
				var matrixestodelete = par.Matrixes.Where(mtrx => parameter.Matrixes.All(m => m.Id != mtrx.Id));
				foreach (var pr in matrixestodelete.ToList())
					par.Matrixes.Remove(_dbStore.MatrixSet.Find(pr.Id));

				var matrixestoadd = parameter.Matrixes.Where(m => par.Matrixes.All(mtrx => mtrx.Id != m.Id));

				foreach (var g in matrixestoadd.ToList())
					par.Matrixes.Add(_dbStore.MatrixSet.Find(g.Id));

				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}

        private bool checkGroups(Param par, Param parameter)
        {
            try
            {
                var groupstodelete = par.Groups.Where(mtrx => parameter.Groups.All(m => m.Id != mtrx.Id));
                foreach (var pr in groupstodelete.ToList())
                    par.Groups.Remove(_dbStore.GroupSet.Find(pr.Id));

                var groupstoadd = parameter.Groups.Where(m => par.Groups.All(mtrx => mtrx.Id != m.Id));

                foreach (var g in groupstoadd.ToList())
                    par.Groups.Add(_dbStore.GroupSet.Find(g.Id));

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private bool checkPackages(Param par, Param parameter)
        {
            try
            {
                var packagestodelete = par.Packages.Where(mtrx => parameter.Packages.All(m => m.Id != mtrx.Id));
                foreach (var pr in packagestodelete.ToList())
                    par.Packages.Remove(_dbStore.PackageSet.Find(pr.Id));

                var packagestoadd = parameter.Packages.Where(m => par.Packages.All(mtrx => mtrx.Id != m.Id));

                foreach (var g in packagestoadd.ToList())
                    par.Packages.Add(_dbStore.PackageSet.Find(g.Id));

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [HttpPost, ActionName("RefreshList")]
        public JsonResult RefreshList(int page, int pageSize, string searchGeneral,
                                      /*string searchGenericKey,*/ string searchDescription, string searchSpecificKey,
                                      int? baseparamfiltered, int? bLine, int? tServId, int? CQ1Id, int? CQ2Id, int? CQ3Id,
                                      int? sucReId, int? sucVenId, int? ramaId, int? CeCId, int? AnalyticMethodId, int? ackId,
									  int? signatariosId, int? EIDASId, int? AnAreaId, int? annalistKeyId,
									  int? envaseId, int? preserverId,
                                      bool alta = true, bool baja = false,
									  bool spcSi = false, bool spcNo = true)
        {
            try
            {
                dynamic currUsr = Session["curr_User"];
                int userId = currUsr.Id;

                var sucursales = User.Identity.Name == "root"
                    ? _dbStore.SucursalSet.Where(s => s.Active)
                    : _dbStore.SucursalSet.Where(s => s.Active &&
                                s.Users.Any(u => u.Id.Equals(userId)));

                //var sucursales = User.Identity.Name == "root"
                //	? _dbStore.SucursalSet.Where(s => s.Active)
                //                : _dbStore.SucursalSet.Where(s => s.Active && 
                //				s.Users.Any(u => u.UserName.Equals(User.Identity.Name)));

				var totalParam = _dbStore.ParamSet.Count(p => ((alta && baja) ||
							(alta && p.Active) || (baja && !p.Active))
								&& ((spcSi && spcNo) ||
							(spcSi && !p.PublishInAutolab) || (spcNo && p.PublishInAutolab))
                                && (sucursales.Any(s => s.AnalyticsArea.Any(aa => aa.Id.Equals(p.CentroCosto.Id))))
                                && (string.IsNullOrEmpty(searchGeneral) ||
                                    (p.ParamUniquekey.ToUpper().Contains(searchGeneral.ToUpper())) ||
                                    (p.BaseParam.Name.ToUpper().Contains(searchGeneral.ToUpper())))
                                && (baseparamfiltered == null || p.BaseParam.Id == baseparamfiltered)
								&& (bLine == null || p.Matrixes.Any(m => m.BaseMatrix.Mercado.Id == bLine))
                                && (tServId == null || p.TipoServicio.Id == tServId)
                                && (CQ1Id == null || p.BaseParam.ClasificacionQuimica1.Id == CQ1Id)
                                && (CQ2Id == null || p.BaseParam.ClasificacionQuimica2.Id == CQ2Id)
                                && (CQ3Id == null || p.BaseParam.ClasificacionQuimica3.Id == CQ3Id)
								&& (envaseId == null || p.Container.Id == envaseId)
								&& (preserverId == null || p.Preserver.Id == preserverId)
                                && (sucReId == null || p.SucursalRealiza.Id == sucReId)
                                && (sucVenId == null || p.SucursalVende.Id == sucVenId)
                                && (ramaId == null || p.Rama.Id == ramaId)
                                && (CeCId == null || p.CentroCosto.Id == CeCId)
                                && (AnalyticMethodId == null || p.AnalyticsMethod.Id == AnalyticMethodId)
                                && (ackId == null || p.RecOtorgs.Any(r => r.Id == ackId))
                                && (signatariosId == null)
                                && (EIDASId == null)
                                && (AnAreaId == null || p.UnidadAnalitica.AreaAnalitica.Id == AnAreaId)
                                && (annalistKeyId==null || p.UnidadAnalitica.AnnalistKeys.Any(ak=>ak.Active && ak.Id== annalistKeyId)));

                //          var activeParamList = _dbStore.ParamSet.Where(p => p.Active.Equals(activeOption)
                //              && (string.IsNullOrEmpty(searchGeneral) ||
                //                  (p.BaseParam.Name.ToUpper().Contains(searchGeneral.ToUpper()))))
                //.OrderBy(p => p.BaseParam.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                //                  .Select(param => param.ToJson());

                var total = _dbStore.BaseParamSet.Count(bp => bp.Parameters.Any(p => (alta && baja ||
                                                                                                    alta && p.Active || baja && !p.Active) && (spcSi && spcNo ||
                                                                                                    spcSi && !p.PublishInAutolab || spcNo && p.PublishInAutolab)
                            && (sucursales.Any(s => s.AnalyticsArea.Any(aa => aa.Id.Equals(p.CentroCosto.Id))))
                            && (string.IsNullOrEmpty(searchGeneral) ||
                                                                                                                    (p.ParamUniquekey.ToUpper().Contains(searchGeneral.ToUpper())) ||
                                                             (bp.Name.ToUpper().Contains(searchGeneral.ToUpper())))
                                                                                                    //&& (string.IsNullOrEmpty(searchGenericKey) ||
                                                                                                    //                (p.GenericKey.ToUpper().Contains(searchGenericKey.ToUpper())))
                                                                                                    && (string.IsNullOrEmpty(searchDescription) ||
                                                                                                                    (p.Description.ToUpper().Contains(searchDescription.ToUpper())))
							&& (bLine == null || p.Matrixes.Any(m => m.BaseMatrix.Mercado.Id == bLine))
                                                                                                    && (tServId == null || p.TipoServicio.Id == tServId)
                                                                                                    && (CQ1Id == null || p.BaseParam.ClasificacionQuimica1.Id == CQ1Id)
                                                                                                    && (CQ2Id == null || p.BaseParam.ClasificacionQuimica2.Id == CQ2Id)
                                                                                                    && (CQ3Id == null || p.BaseParam.ClasificacionQuimica3.Id == CQ3Id)
							&& (envaseId == null || p.Container.Id == envaseId)
							&& (preserverId == null || p.Preserver.Id == preserverId)
                                                                                                    && (sucReId == null || p.SucursalRealiza.Id == sucReId)
                                                                                                    && (sucVenId == null || p.SucursalVende.Id == sucVenId)
                                                                                                    && (ramaId == null || p.Rama.Id == ramaId)
                                                                                                    && (CeCId == null || p.CentroCosto.Id == CeCId)
                                                                                                    && (AnalyticMethodId == null || p.AnalyticsMethod.Id == AnalyticMethodId)
                                                                                                    && (ackId == null || p.RecOtorgs.Any(r => r.Id == ackId))
                                                                                                    && (signatariosId == null)
                                                                                                    && (EIDASId == null)
                                                                                                    && (AnAreaId == null || p.UnidadAnalitica.AreaAnalitica.Id == AnAreaId)
                                                                                                    && (annalistKeyId == null || p.UnidadAnalitica.AnnalistKeys.Any(ak=>ak.Active && ak.Id == annalistKeyId)))
                                                                         && (baseparamfiltered == null || bp.Id == baseparamfiltered));

                var activeParamList = _dbStore.BaseParamSet.Where(bp => bp.Parameters.Any(p => (alta && baja ||
                                                                                                    alta && p.Active || baja && !p.Active) && (spcSi && spcNo ||
                                                                                                    spcSi && !p.PublishInAutolab || spcNo && p.PublishInAutolab)
                    && (sucursales.Any(s => s.AnalyticsArea.Any(aa => aa.Id.Equals(p.CentroCosto.Id))))
                    && (string.IsNullOrEmpty(searchGeneral) ||
                                    (p.ParamUniquekey.ToUpper().Contains(searchGeneral.ToUpper())) ||
                                    (bp.Name.ToUpper().Contains(searchGeneral.ToUpper())))
                    //&& (string.IsNullOrEmpty(searchGenericKey) ||
                    //                (p.GenericKey.ToUpper().Contains(searchGenericKey.ToUpper())))
                                                                                                    && (string.IsNullOrEmpty(searchDescription) ||
                                                                                                                    (p.Description.ToUpper().Contains(searchDescription.ToUpper())))
					&& (bLine == null || p.Matrixes.Any(m => m.BaseMatrix.Mercado.Id == bLine))
                                                                                                    && (tServId == null || p.TipoServicio.Id == tServId)
                                                                                                    && (CQ1Id == null || p.BaseParam.ClasificacionQuimica1.Id == CQ1Id)
                                                                                                    && (CQ2Id == null || p.BaseParam.ClasificacionQuimica2.Id == CQ2Id)
                                                                                                    && (CQ3Id == null || p.BaseParam.ClasificacionQuimica3.Id == CQ3Id)
					&& (envaseId == null || p.Container.Id == envaseId)
					&& (preserverId == null || p.Preserver.Id == preserverId)
                                                                                                    && (sucReId == null || p.SucursalRealiza.Id == sucReId)
                                                                                                    && (sucVenId == null || p.SucursalVende.Id == sucVenId)
                                                                                                    && (ramaId == null || p.Rama.Id == ramaId)
                                                                                                    && (CeCId == null || p.CentroCosto.Id == CeCId)
                                                                                                    && (AnalyticMethodId == null || p.AnalyticsMethod.Id == AnalyticMethodId)
                                                                                                    && (ackId == null || p.RecOtorgs.Any(r => r.Ack.Id == ackId))
					&& (signatariosId == null || p.Annalists.Any(an => an.RecAdqs.Any(ra => ra.NivelAdquirido == RecAdq.AcquiredLevel.Signatario) && an.Id == signatariosId))
                                                                                                    && (EIDASId == null || p.Annalists.Any(an => an.RecAdqs.Any(ra => ra.NivelAdquirido == RecAdq.AcquiredLevel.Eidas) && an.Id == EIDASId))
                                                                                                    && (AnAreaId == null || p.UnidadAnalitica.AreaAnalitica.Id == AnAreaId)
                                                                                                    && (annalistKeyId == null || p.UnidadAnalitica.AnnalistKeys.Any(ak=>ak.Active && ak.Id == annalistKeyId)))
                                                                         && (baseparamfiltered == null || bp.Id == baseparamfiltered))
                        .OrderBy(bp => bp.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
						.Select(bp => bp.ToJsonTree(alta, baja, User.Identity.Name, searchGeneral));

				// 220 ms
                return Json(new { success = true, elements = activeParamList, total, totalParam });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    err = "No se pudo recuperar la info.",
                    success = false,
                    details = ex.Message
                });
            }
        }

        [HttpPost, ActionName("SaveActiveStatus")]
        public JsonResult SaveActiveStatus(int id, bool active)
        {
            try
            {
                var par = _dbStore.ParamSet.Find(id);
                if (par == null)
                    return Json(new { success = false, details = "No se encontró info del parámetro" });

                par.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpPost, ActionName("GetAllMaxPermitedLimit")]
        public JsonResult GetAllMaxPermitedLimit(int Id)
        {
            try
            {
                var LMP = _dbStore.ParamSet.Find(Id).ParamRoutes.Select(pr => new
                {
                    pr.MaxPermitedLimit.Name,
                    pr.Value,
                    pr.DecimalsPoints,
                    Group = pr.Group?.Name,
                    Package = pr.Package?.Name,
                });
                return Json(new { success = true, LMP });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }
		
        [HttpGet, ActionName("CheckParamUniquekey")]
        public JsonResult CheckParamUniquekey(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.ParamSet.Find(id).ParamUniquekey.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);

                var isIndb = _dbStore.ParamSet.FirstOrDefault(p => p.ParamUniquekey.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));

                // returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpPost, ActionName("GetParamCurrency")]
        public JsonResult GetParamCurrency(bool active = true)
        {
            try
            {
                var currency = new CurrencyController();
                return currency.RefreshList(active);
            }
            catch (Exception e)
            {
                return Json(new
                {
                    err = "No se pudo procesar la info",
                    success = false,
                    details = e.Message
                });
            }
        }

        [HttpPost, ActionName("GetMatrixes")]
        public JsonResult GetMatrixes(int Id)
        {
            try
            {
				var Matrixes = _dbStore.ParamSet.Find(Id).Matrixes.Where(m => m.Active)
					.Select(m => new
                                                                {
						m.Name,
						m.Description,
						m.SubMatrix,
						m.SubMtrxDescription,
                                                                    BaseMatrix = new
                                                                    {
                                                                        Mercado = new
                                                                        {
								m.BaseMatrix.Mercado.Name
                                                                        },
							m.BaseMatrix.Name
                                                                    }
					});

				return Json(new { success = true, Matrixes });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    err = "No se pudo procesar la info",
                    success = false,
                    details = e.Message
                });
            }
        }

        [HttpPost, ActionName("GetParamBase")]
        public JsonResult GetParamBase(bool active = true)
        {
            try
            {
                var paramBase = new BaseParamController();
                return paramBase.GetBaseParam(User.Identity.Name, active);
            }
            catch (Exception e)
            {
                return Json(new
                {
                    err = "No se pudo procesar la info",
                    success = false,
                    details = e.Message
                });
            }
        }

		[HttpPost, ActionName("GetBaseMatrix")]
		public JsonResult GetBaseMatrix(bool active = true)
		{
			try
			{
				var baseMatrix = _dbStore.BaseMatrixSet.Where(bm => bm.Active && bm.Matrixes.Any(m => m.Active)).ToList()
					.Select(bm => new
					{
						bm.Id,
						bm.Name,
						Matrixes = bm.Matrixes.Select(m => new { m.Id, m.Name, m.Description, m.SubMatrix, m.SubMtrxDescription, BaseMatrix = new { Mercado = new { m.BaseMatrix.Mercado.Name }, m.BaseMatrix.Name } })

					});
				return Json(new { success = true, baseMatrix });
			}
			catch (Exception e)
			{
				return Json(new
				{
					err = "No se pudo procesar la info",
					success = false,
					details = e.Message
				});
			}
		}

		//[HttpPost, ActionName("GetMethodMatrixes")]
		//public JsonResult GetMethodMatrixes(bool active = true)
		//{
		//	try
		//	{
		//		var elements = _dbStore.MethodSet.Where(bm => bm.Active && bm.Matrixes.Any(m => m.Active)).ToList()
		//			.Select(bm => new
		//			{
		//				bm.Id,
		//				bm.Name,
		//				Matrixes = bm.Matrixes.Select(m => new { m.Id, m.Name, m.Description, m.SubMatrix, m.SubMtrxDescription, BaseMatrix = new { Mercado = new { m.BaseMatrix.Mercado.Name }, m.BaseMatrix.Name } })
		//			});
		//		return Json(new { success = true, elements });
		//	}
		//	catch (Exception e)
		//	{
		//		return Json(new
		//		{
		//			err = "No se pudo procesar la info",
		//			success = false,
		//			details = e.Message
		//		});
		//	}
		//}

		//[HttpPost, ActionName("GetBaseParamMatrixes")]
		//public JsonResult GetBaseParamMatrixes(bool active = true)
		//{
		//	try
		//	{
		//		var elements = _dbStore.BaseParamSet.Where(bm => bm.Active && bm.Matrixes.Any(m => m.Active)).ToList()
		//			.Select(bm => new
		//			{
		//				bm.Id,
		//				bm.Name,
		//				Matrixes = bm.Matrixes.Select(m => new
		//				{
		//					m.Id, m.Name, m.Description, m.SubMatrix, m.SubMtrxDescription,
		//					BaseMatrix = new
		//					{
		//						Mercado = new { m.BaseMatrix.Mercado.Name },
		//						m.BaseMatrix.Name
		//					}
		//				})
		//			});
		//		return Json(new { success = true, elements });
		//	}
		//	catch (Exception e)
		//	{
		//		return Json(new
		//		{
		//			err = "No se pudo procesar la info",
		//			success = false,
		//			details = e.Message
		//		});
		//	}
		//}

        [HttpPost, ActionName("GetFilters")]
        public JsonResult GetFilters()
        {
            try
            {
                dynamic currUsr = Session["curr_User"];
                int userId = currUsr.Id;

                var sucursales = User.Identity.Name == "root"
                    ? _dbStore.SucursalSet.Where(s => s.Active)
                    : _dbStore.SucursalSet.Where(s => s.Active &&
                                s.Users.Any(u => u.Id.Equals(userId)));

                var parameters = _dbStore.ParamSet.Where(p => p.Active && sucursales.Any(s => s.Active &&
                                                                                                    (p.SucursalRealiza != null &&
                                                                                                     s.Id.Equals(p.SucursalRealiza.Id) ||
                                                                                                     p.SucursalVende != null && s.Id.Equals(p.SucursalVende.Id))));

                var sucVenList = parameters.Where(p => p.SucursalVende != null).Select(p => p.SucursalVende).Distinct().Select(s => new { s.Id, s.Name });
                var sucRealList = parameters.Where(p => p.SucursalRealiza != null).Select(p => p.SucursalRealiza).Distinct().Select(s => new { s.Id, s.Name });
				var marketList = parameters.Where(p => p.Matrixes.Any(m => m.Active)).SelectMany(p => p.Matrixes).Select(m => m.BaseMatrix.Mercado).Distinct().Select(s => new { s.Id, s.Name });
                var tServList = parameters.Where(p => p.TipoServicio != null).Select(p => p.TipoServicio).Distinct().Select(ts => new { ts.Id, ts.Name });
                var cq1 = parameters.Where(p => p.BaseParam.ClasificacionQuimica1 != null).Select(p => p.BaseParam.ClasificacionQuimica1).Distinct().Select(q => new { q.Id, q.Name });
                var cq2 = parameters.Where(p => p.BaseParam.ClasificacionQuimica2 != null).Select(p => p.BaseParam.ClasificacionQuimica2).Distinct().Select(q => new { q.Id, q.Name });
                var cq3 = parameters.Where(p => p.BaseParam.ClasificacionQuimica3 != null).Select(p => p.BaseParam.ClasificacionQuimica3).Distinct().Select(q => new { q.Id, q.Name });
                var ramaList = parameters.Where(p => p.Rama != null).Select(p => p.Rama).Distinct().Select(p => new { p.Id, p.Name });
                var CeCList = parameters.Where(p => p.CentroCosto != null).Select(p => p.CentroCosto).Distinct().Select(p => new { p.Id, Name = p.Number });
                var AnalyliticMethodList = parameters.Where(p => p.AnalyticsMethod != null).Select(p => p.AnalyticsMethod).Distinct().Select(p => new { p.Id, p.Name });
                var AckList = parameters.Where(p => p.RecOtorgs.Any()).SelectMany(p => p.RecOtorgs.Select(ro => ro.Ack)).Distinct().Select(ack => new { ack.Id, Name = ack.Key });
                var AnAreaList = parameters.Where(p => p.UnidadAnalitica != null).Select(p => p.UnidadAnalitica.AreaAnalitica).Distinct().Select(aa => new { aa.Id, Name = aa.Key });
				var signatariosList = parameters.Where(p => p.Annalists.Any()).SelectMany(p => p.Annalists.Where(an => an.RecAdqs.Any(ra => ra.NivelAdquirido == RecAdq.AcquiredLevel.Signatario))).Distinct().Select(an => new { an.Id, Name = an.Key });
				var EIDASList = parameters.Where(p => p.Annalists.Any()).SelectMany(p => p.Annalists.Where(an => an.RecAdqs.Any(ra => ra.NivelAdquirido == RecAdq.AcquiredLevel.Eidas))).Distinct().Select(an => new { an.Id, Name = an.Key });

				var envaseList = parameters.Where(m => m.Active && m.Container != null).Select(m => m.Container).Distinct().Select(m => new { m.Id, m.Name });
				var preserverList = parameters.Where(m => m.Active && m.Preserver != null).Select(m => m.Preserver).Distinct().Select(p => new { p.Id, p.Name });

				return Json(new { success = true, marketList, sucVenList, sucRealList, tServList, cq1, cq2, cq3, ramaList, CeCList, AnalyliticMethodList, AckList, AnAreaList, signatariosList, EIDASList, envaseList, preserverList });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    err = "No se pudo recuperar la info.",
                    success = false,
                    details = ex.Message
                });
            }
        }

        [HttpPost, ActionName("GetSucursales")]
        public JsonResult GetSucursales(int mtrxId)
        {
            try
            {
				var matrix = _dbStore.MatrixSet.Find(mtrxId);
				var marketId = _dbStore.BaseMatrixSet.Find(matrix.BaseMatrix.Id).Mercado.Id;

                dynamic currUsr = Session["curr_User"];
                int userId = currUsr.Id;

                var sucursales = User.Identity.Name == "root"
					? _dbStore.SucursalSet.Where(s => s.Active && s.Offices.Any(o => o.Market.Id == marketId))
					: _dbStore.SucursalSet.Where(s => s.Active && s.Offices.Any(o => o.Market.Id == marketId) &&
                                s.Users.Any(u => u.Id.Equals(userId)));

                var vSucursalList =
                    sucursales.Where(s => s.Vende).Select(s => new
                    {
                        s.Id,
                        s.Name,
                        s.Key,
                        Region = new
                        {
                            s.Region.Name
                        }

                    });
                var rSucursalList =
                   sucursales.Where(
                        s => s.Realiza).Select(s => new
                        {
                            s.Id,
                            s.Name,
                            s.Key,
                            Region = new
                            {
                                s.Region.Name
                            }
                        });

                return Json(new { success = true, vSucursalList, rSucursalList });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    err = "No se pudo procesar la info",
                    success = false,
                    details = e.Message
                });
            }
        }

		//[HttpPost, ActionName("GetMethods")]
		//public JsonResult GetMethods(bool activeOption = true)
		//{
		//	try
		//	{
		//		var method = new MethodController();
		//		return method.GetMethods(activeOption);
		//	}
		//	catch (Exception e)
		//	{
		//		return Json(new
		//		{
		//			err = "No se pudo procesar la info",
		//			success = false,
		//			details = e.Message
		//		});
		//	}
		//}

        [HttpPost, ActionName("SaveCcChange")]
        public JsonResult SaveCcChange(int baseParam, int centroCosto, IEnumerable<int> prms)
        {
            try
            {
                if (baseParam == 0 || centroCosto == 0 || prms == null)
                    return Json(new { success = false });

                foreach (var par in prms.Select(p => _dbStore.ParamSet.Find(p)))
                {
					par.CentroCosto = _dbStore.CentroCostoSet.Find(centroCosto);
                }

                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpPost, ActionName("GetRootPrintResults")]
        public JsonResult GetRootPrintResults(int id)
        {
            try
            {
                var paramPrintResult = _dbStore.ParamPrintResultSet.FirstOrDefault(prr => prr.Parameter.Id == id && prr.Group == null)?.ToJson();
                return Json(new { success = true, paramPrintResult, decimalReport = _dbStore.ParamSet.Find(id).DecimalesReporte });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

		[HttpPost, ActionName("GetParamKeys")]
		public JsonResult GetParamKeys()
		{
			try
			{
				var elements = _dbStore.ParamSet.Where(p => p.Active)
					.Select(p => new
					{
						p.ParamUniquekey
						//CantidadMuestreos = p.ComplexSamplings.Any()
						//	? p.ComplexSamplings.FirstOrDefault().CantidadMuestreos : 0
					});
				return Json(new { success = true, elements });
			}
			catch (Exception e)
			{
				return Json(new { success = false, details = e.Message });
			}
		}

		[HttpPost, ActionName("GetParamContainer")]
		public JsonResult GetParamContainer(bool active = true)
		{
			try
			{
				var container = _dbStore.ContainerSet.Where(c => c.Active)
					.Select(c => new { c.Id, c.Name });
				return Json(new { success = true, elements = container });
			}
			catch (Exception e)
			{
				return Json(new
				{
					err = "No se pudo procesar la info",
					success = false,
					details = e.Message
				});
			}
		}

		[HttpPost, ActionName("GetParamPreserver")]
		public JsonResult GetParamPreserver(bool active = true)
		{
			try
			{
				var preserver = _dbStore.PreserverSet.Where(p => p.Active)
					.Select(p => new { p.Id, p.Name });
				return Json(new { success = true, elements = preserver });
			}
			catch (Exception e)
			{
				return Json(new
				{
					err = "No se pudo procesar la info",
					success = false,
					details = e.Message
				});
			}
		}

		[HttpPost, ActionName("GetParamResidue")]
		public JsonResult GetParamResidue(bool active = true)
		{
			try
			{
				var residue = _dbStore.ResidueSet.Where(r => r.Active)
					.Select(r => new { r.Id, r.Name });
				return Json(new { success = true, elements = residue });
				//var residue = new ResidueController();
				//return residue.RefreshList(active);
			}
			catch (Exception e)
			{
				return Json(new
				{
					err = "No se pudo procesar la info",
					success = false,
					details = e.Message
				});
			}
		}

	}
}
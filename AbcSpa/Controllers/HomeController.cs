using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AbcPersistent.Models;

namespace AbcSpa.Controllers
{
	public class HomeController : Controller
	{
		// variable to do queries on the DB
		private readonly AbcContext _dbStore = new AbcContext();

		// Esta action es la 1ra que se lanza para cargar la pagina!
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost, ActionName("GetAllCount")]
		public JsonResult GetAllCount(bool activeOption = true)
		{
			try
			{
				var countDict = new Dictionary<string, int>
				{
					["ackCount"] = _dbStore.AckSet.Count(e => e.Active == activeOption),
                    ["actionsCount"] = _dbStore.AccionSet.Count(e => e.Active == activeOption),
                    ["analyticsAreaCount"] = _dbStore.AnalyticsAreaSet.Count(e => e.Active == activeOption),
                    ["annalistKeyCount"] = _dbStore.AnnalistKeySet.Count(e => e.Active == activeOption),
                    ["analyticsMethodCount"] = _dbStore.AnalyticsMethodSet.Count(e => e.Active == activeOption),
                    ["annalistCount"] = _dbStore.AnnalistSet.Count(e => e.Active == activeOption),
                    ["baseParamCount"] = _dbStore.BaseParamSet.Count(e => e.Active == activeOption),
                    ["containerCount"] = _dbStore.ContainerSet.Count(e => e.Active == activeOption),
                    ["currencyCount"] = _dbStore.CurrencySet.Count(e => e.Active == activeOption),
                    ["enterpriseCount"] = _dbStore.EnterpriseSet.Count(e => e.Active == activeOption),
                    ["marketCount"] = _dbStore.MarketSet.Count(e => e.Active == activeOption),
                    ["matrixCount"] = _dbStore.MatrixSet.Count(e => e.Active == activeOption),
                    ["basematrixCount"] = _dbStore.BaseMatrixSet.Count(e => e.Active == activeOption),
                    ["measureUnitCount"] = _dbStore.MeasureUnitSet.Count(e => e.Active == activeOption),
					//["measureUnitReportCount"] = _dbStore.MeasureUnitSet.Count(e => e.Active == activeOption && e.Reporte),
					["methodCount"] = _dbStore.MethodSet.Count(e => e.Active == activeOption),
                    ["officeCount"] = _dbStore.OfficeSet.Count(e => e.Active == activeOption),
                    ["preserverCount"] = _dbStore.PreserverSet.Count(e => e.Active == activeOption),
                    ["regionCount"] = _dbStore.RegionSet.Count(e => e.Active == activeOption),
                    ["residueCount"] = _dbStore.ResidueSet.Count(e => e.Active == activeOption),
                    ["roleCount"] = _dbStore.RoleSet.Count(e => e.Active == activeOption),
                    ["userCount"] = _dbStore.UserSet.Count(e => e.Active == activeOption),
                    ["groupCount"] = _dbStore.GroupSet.Count(e => e.Active == activeOption),
                    ["normCount"] = _dbStore.NormSet.Count(e => e.Active == activeOption),
                    ["statusCount"] = _dbStore.StatusSet.Count(e => e.Active == activeOption),
                    //["baseParamFamilyCount"] = _dbStore.BaseParamFamilySet.Count(e => e.Active == activeOption),
                    ["paramCount"] = _dbStore.ParamSet.Count(e => e.Active == activeOption),
                    ["packageCount"] = _dbStore.PackageSet.Count(e => e.Active == activeOption),
                    ["materialCount"] = _dbStore.MaterialSet.Count(e => e.Active == activeOption),
                    ["sucursalCount"] = _dbStore.SucursalSet.Count(e => e.Active == activeOption),
					["maxPermitedLimitCount"] = _dbStore.MaxPermitedLimitSet.Count(e => e.Active == activeOption),
					["clasificacionQuimica1Count"] = _dbStore.ClasificacionQuimicaSet.Count(e => e.Active.Equals(activeOption) && e.Level.Equals(1)),
					["clasificacionQuimica2Count"] = _dbStore.ClasificacionQuimicaSet.Count(e => e.Active.Equals(activeOption) && e.Level.Equals(2)),
					["clasificacionQuimica3Count"] = _dbStore.ClasificacionQuimicaSet.Count(e => e.Active.Equals(activeOption) && e.Level.Equals(3)),
					["tiposervicioCount"] = _dbStore.TipoServicioSet.Count(e => e.Active == activeOption),
					["centroCostoCount"] = _dbStore.CentroCostoSet.Count(e => e.Active == activeOption),
					["unidadAnaliticaCount"] = _dbStore.UnidadAnaliticaSet.Count(e => e.Active == activeOption),
					["ramaCount"] = _dbStore.RamaSet.Count(e => e.Active == activeOption),
					["alcancesCount"] = _dbStore.AlcanceSet.Count(e => e.Active == activeOption)
				};
				
				return Json(new { success = true, elements = countDict });
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

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AbcPersistent;
using AbcPersistent.Models;
using Microsoft.Ajax.Utilities;

namespace AbcSpa.Controllers
{
    public class GroupController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveGroup")]
        public JsonResult SaveGroup(Group group)
        {
            try
            {
                var bg = group.Id == 0
					? new Group()
					{
						Parameters = new List<Param>(),
						ParamRoutes = new List<ParamRoute>(),
						ParamPrintResults = new List<ParamPrintResult>(),
						//ComplexSamplings = new List<ComplexSampling>(),
						Matrixes = new List<Matrix>()
					}
                    : _dbStore.GroupSet.Find(group.Id);

                bg.Name = group.Name;
                bg.Description = group.Description;
                bg.Active = group.Active;
				bg.SellSeparated = group.SellSeparated;
				bg.CuentaEstadistica = group.CuentaEstadistica;
	            bg.MostrarLista = group.MostrarLista;
				bg.Week = group.Week;
               // bg.DecimalesReporte = group.DecimalesReporte;
                bg.ImpResultado = group.ImpResultado;

				bg.TipoServicio = (group.TipoServicio != null)
					? _dbStore.TipoServicioSet.Find(group.TipoServicio.Id)
					: null;

				bg.ClasificacionQuimica1 = group.ClasificacionQuimica1 != null ?
											  _dbStore.ClasificacionQuimicaSet
											  .Find(group.ClasificacionQuimica1.Id) : null;

				bg.ClasificacionQuimica2 = group.ClasificacionQuimica2 != null ?
											  _dbStore.ClasificacionQuimicaSet
											  .Find(group.ClasificacionQuimica2.Id) : null;

				bg.ClasificacionQuimica3 = group.ClasificacionQuimica3 != null ?
											  _dbStore.ClasificacionQuimicaSet
											  .Find(group.ClasificacionQuimica3.Id) : null;

				//if (!CheckComplexSampling(bg, group))
				//	throw new Exception("Error al llenar la lista de Muestreos Compuestos");

                bg.DispParamId = group.DispParamId;

                if (!CheckParamList(bg, group))
                    throw new Exception("Error al llenar la lista de Parámetros");

                if (!checkMatrixes(bg, group))
                    throw new Exception("Error al llenar la lista de Matrices");

                if (!checkPackages(bg, group))
                    throw new Exception("Error al llenar la lista de Paquetes");

	            bg.Sucursal = group.Sucursal != null ? _dbStore.SucursalSet.Find(group.Sucursal.Id) : null;

	            bg.PublishInAutolab = true;
	            if (!bg.Active
					|| string.IsNullOrEmpty(bg.Name)
					|| string.IsNullOrEmpty(bg.Description)
					//|| (bg.DecimalesReporte == null)
					|| !bg.Parameters.Any(p => p.Active && p.PublishInAutolab)
					|| !bg.Matrixes.Any(m => m.Active)
					|| (bg.TipoServicio == null)
					|| (bg.Sucursal == null)
					|| ((bg.ClasificacionQuimica1 == null) && (bg.ClasificacionQuimica2 == null) && (bg.ClasificacionQuimica3 == null))
					)
	            {
		            bg.PublishInAutolab = false;
	            }

				if (group.Id == 0)
                {
                    _dbStore.GroupSet.Add(bg);
                }

                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                if (!CheckParamPrintResult(bg, group))
                    throw new Exception("Error al llenar la lista de Permisos de Impresión");
                
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        private bool CheckParamList(Group bg, Group group)
        {
            try
            {
                var paramtodelete = bg.Parameters.Where(par => group.Parameters.All(m => m.Id != par.Id) && par.Active);

                foreach (var pd in paramtodelete.ToList())
                    bg.Parameters.Remove(_dbStore.ParamSet.Find(pd.Id));

                var paramtoadd = group.Parameters.Where(par => bg.Parameters.All(m => m.Id != par.Id));

                foreach (var pa in paramtoadd.ToList())
                    bg.Parameters.Add(_dbStore.ParamSet.Find(pa.Id));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

		//private bool CheckComplexSampling(Group bg, Group group)
		//{
		//	try
		//	{
		//		var cstodelete = bg.ComplexSamplings.Where(cs => group.ComplexSamplings.All(m => m.Id != cs.Id));

		//		foreach (var csd in cstodelete.ToList())
		//		{
		//			bg.ComplexSamplings.Remove(_dbStore.ComplexSamplingSet.Find(csd.Id));
		//			_dbStore.ComplexSamplingSet.Remove(_dbStore.ComplexSamplingSet.Find(csd.Id));
		//		}
					
		//		foreach (var complex in group.ComplexSamplings)
		//		{
		//			var csampling = _dbStore.ComplexSamplingSet.FirstOrDefault(cs =>
		//				cs.Group.Id.Equals(group.Id) && cs.Param.Id.Equals(complex.Param.Id)) ??
		//			                new ComplexSampling()
		//			                {
		//				                Param = _dbStore.ParamSet.Find(complex.Param.Id)
		//			                };

		//			csampling.CantidadMuestreos = complex.CantidadMuestreos;

		//			if (csampling.Id == 0)
		//				_dbStore.ComplexSamplingSet.Add(csampling);
					
		//			_dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

		//			if (group.ComplexSamplings.All(c => c.Id != csampling.Id))
		//				bg.ComplexSamplings.Add(_dbStore.ComplexSamplingSet.Find(csampling.Id));
		//		}

		//		//var cstoadd = group.ComplexSamplings.Where(cs => bg.ComplexSamplings.All(m => m.Id != cs.Id));

		//		//foreach (var csa in cstoadd.ToList())
		//		//	bg.ComplexSamplings.Add(_dbStore.ComplexSamplingSet.Find(csa.Id));

		//		bg.MuestreosCompuestos = bg.ComplexSamplings.Any();

		//		return true;
		//	}
		//	catch (Exception)
		//	{
		//		return false;
		//	}
		//}

        private bool CheckParamPrintResult(Group bg, Group group)
        {
            try
            {
                var printResulttodelete = bg.ParamPrintResults.Where(par => group.ParamPrintResults.All(m => m.Id != par.Id) && par.Active);

                foreach (var pd in printResulttodelete.ToList())
                {
                    bg.ParamPrintResults.Remove(_dbStore.ParamPrintResultSet.Find(pd.Id));
                    _dbStore.ParamPrintResultSet.Remove(_dbStore.ParamPrintResultSet.Find(pd.Id));
                }

				var printResulttoupdate = group.ParamPrintResults.Where(par => bg.ParamPrintResults.Any(m => m.Id == par.Id && m.Yes != par.Yes));

                foreach (var pd in printResulttoupdate.ToList())
                {
					_dbStore.ParamPrintResultSet.Find(pd.Id).Yes = pd.Yes;
                    //_dbStore.ParamPrintResultSet.Remove(_dbStore.ParamPrintResultSet.Find(pd.Id));
                }

                var printResultToCreate = group.ParamPrintResults.Where(prr => prr.Id == 0);
                foreach (var prr in printResultToCreate.ToList())
                {
                    _dbStore.ParamPrintResultSet.Add(new ParamPrintResult()
                    {
                        Yes = prr.Yes,
                        Parameter = _dbStore.ParamSet.Find(prr.Parameter.Id),
                        Group = _dbStore.GroupSet.Find(bg.Id)
                    });
                }
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool checkMatrixes(Group bg, Group group)
        {
            try
            {
                var matrixestodelete = bg.Matrixes.Where(mtrx => group.Matrixes.All(m => m.Id != mtrx.Id));
                foreach (var pr in matrixestodelete.ToList())
                    bg.Matrixes.Remove(_dbStore.MatrixSet.Find(pr.Id));

                var matrixestoadd = group.Matrixes.Where(m => bg.Matrixes.All(mtrx => mtrx.Id != m.Id));

                foreach (var g in matrixestoadd.ToList())
                    bg.Matrixes.Add(_dbStore.MatrixSet.Find(g.Id));

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private bool checkPackages(Group bg, Group group)
        {
            try
            {
                var packagestodelete = bg.Packages.Where(mtrx => group.Packages.All(m => m.Id != mtrx.Id));
                foreach (var pr in packagestodelete.ToList())
                    bg.Packages.Remove(_dbStore.PackageSet.Find(pr.Id));

                var packagestoadd = group.Packages.Where(m => bg.Packages.All(mtrx => mtrx.Id != m.Id));

                foreach (var g in packagestoadd.ToList())
                    bg.Packages.Add(_dbStore.PackageSet.Find(g.Id));

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [HttpPost, ActionName("RefreshList")]
        public JsonResult RefreshList(int page, int pageSize, string searchGeneral, string searchDescription,
                                            int? sucId, int? bLine, int? tServId,
                                            int? CQ1Id, int? CQ2Id, int? CQ3Id,
			bool alta = true, bool baja = false, bool spcSi = false, bool spcNo = true)
        {
            try
            {

                dynamic currUsr = Session["curr_User"];
                int userId = currUsr.Id;

                var sucursales = User.Identity.Name == "root"
                    ? _dbStore.SucursalSet.Where(s => s.Active)
                    : _dbStore.SucursalSet.Where(s => s.Active &&
                                s.Users.Any(u => u.Id.Equals(userId)));


                var total = _dbStore.GroupSet.Count(g => ((alta && baja) ||
							(alta && g.Active) || (baja && !g.Active))
							&& ((spcSi && spcNo) ||
							(spcSi && !g.PublishInAutolab) || (spcNo && g.PublishInAutolab)) &&
                                                         g.Parameters.Any(p => p.Active &&
                                                        sucursales.Any(s => s.Active &&
                                                        (p.SucursalRealiza != null && s.Id.Equals(p.SucursalRealiza.Id) ||
                                                        p.SucursalVende != null && s.Id.Equals(p.SucursalVende.Id))))
                    && (string.IsNullOrEmpty(searchGeneral) || (g.Name.ToUpper().Contains(searchGeneral.ToUpper())))
                    && (string.IsNullOrEmpty(searchDescription) || (g.Name.ToUpper().Contains(searchDescription.ToUpper())))
                    && (sucId == null || g.Sucursal.Id == sucId) && (bLine == null || g.Matrixes.Any(m => m.BaseMatrix.Mercado.Id == bLine))
                    && (tServId == null || g.TipoServicio != null && g.TipoServicio.Id == tServId)
                    && (CQ1Id == null || g.ClasificacionQuimica1 != null && g.ClasificacionQuimica1.Id == CQ1Id)
                    && (CQ2Id == null || g.ClasificacionQuimica2 != null && g.ClasificacionQuimica2.Id == CQ2Id)
                    && (CQ3Id == null || g.ClasificacionQuimica3 != null && g.ClasificacionQuimica3.Id == CQ3Id));



                var activeGroupList = _dbStore.GroupSet.Where(g => ((alta && baja) ||
							(alta && g.Active) || (baja && !g.Active))
							&& ((spcSi && spcNo) ||
							(spcSi && !g.PublishInAutolab) || (spcNo && g.PublishInAutolab)) &&
                                                         g.Parameters.Any(p => p.Active &&
                                                        sucursales.Any(s => s.Active &&
                                                        (p.SucursalRealiza != null && s.Id.Equals(p.SucursalRealiza.Id) ||
                                                        p.SucursalVende != null && s.Id.Equals(p.SucursalVende.Id))))
                    && (string.IsNullOrEmpty(searchGeneral) || (g.Name.ToUpper().Contains(searchGeneral.ToUpper())))
                    && (string.IsNullOrEmpty(searchDescription) || (g.Name.ToUpper().Contains(searchDescription.ToUpper())))
                    && (sucId == null || g.Sucursal.Id == sucId) && (bLine == null || g.Matrixes.Any(m => m.BaseMatrix.Mercado.Id == bLine))
                    && (tServId == null || g.TipoServicio != null && g.TipoServicio.Id == tServId)
                    && (CQ1Id == null || g.ClasificacionQuimica1 != null && g.ClasificacionQuimica1.Id == CQ1Id)
                    && (CQ2Id == null || g.ClasificacionQuimica2 != null && g.ClasificacionQuimica2.Id == CQ2Id)
                    && (CQ3Id == null || g.ClasificacionQuimica3 != null && g.ClasificacionQuimica3.Id == CQ3Id))
                        .OrderBy(g => g.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                        .Select(gr => gr.ToJson());

                //var sucList = _dbStore.GroupSet.Where(g => g.Active &&
                //                                           g.Parameters.Any(p => p.Active &&
                //                                                                 sucursales.Any(s => s.Active &&
                //                                                                                     (p.SucursalRealiza != null &&
                //                                                                                      s.Id.Equals(p.SucursalRealiza.Id) ||
                //                                                                                      p.SucursalVende !=null &&
                //                                                                                      s.Id.Equals(p.SucursalVende.Id)))))
                //                                                                                        .Select(s => new
                //                                                                                        {
                //                                                                                            s.Sucursal.Id,
                //                                                                                            s.Sucursal.Name
                //                                                                                        });

                return Json(new { success = true, elements = activeGroupList, total });
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

                var groups = _dbStore.GroupSet.Where(g => g.Active &&
                                                          g.Parameters.Any(p => p.Active &&
                                                                                sucursales.Any(s => s.Active &&
                                                                                                    (p.SucursalRealiza !=
                                                                                                     null &&
                                                                                                     s.Id.Equals(
                                                                                                         p
                                                                                                             .SucursalRealiza
                                                                                                             .Id) ||
                                                                                                     p.SucursalVende !=
                                                                                                     null &&
                                                                                                     s.Id.Equals(
                                                                                                         p.SucursalVende
                                                                                                             .Id)))));

                var sucList = groups.Select(g => g.Sucursal).Distinct().Select(s => new { s.Id, s.Name });
                var marketList = groups.SelectMany(g => g.Matrixes.Select(m => m.BaseMatrix.Mercado)).Distinct().Select(s => new { s.Id, s.Name });
                var tipoServicioList = groups.Select(g => g.TipoServicio).Distinct().Select(t => new { t.Id, t.Name });
                var cq1 = groups.Where(g => g.ClasificacionQuimica1 != null).Select(g => g.ClasificacionQuimica1).Distinct().Select(q => new { q.Id, q.Name });
                var cq2 = groups.Where(g => g.ClasificacionQuimica2 != null).Select(g => g.ClasificacionQuimica2).Distinct().Select(q => new { q.Id, q.Name });
                var cq3 = groups.Where(g => g.ClasificacionQuimica3 != null).Select(g => g.ClasificacionQuimica3).Distinct().Select(q => new { q.Id, q.Name });

                return Json(new { success = true, sucList, marketList, tipoServicioList, cq1, cq2, cq3 });
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
                var mar = _dbStore.GroupSet.Find(id);
                if (mar == null)
                    return Json(new { success = false, details = "No se encontró info del mercado" });

                mar.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }


        [HttpPost, ActionName("GetClasificacionesQuimicas")]
        public JsonResult GetClasificacionesQuimicas(int level)
        {
            try
            {
                var elements = _dbStore.ClasificacionQuimicaSet.Where(cq => cq.Active && cq.Level == level).Select(cq=>new {cq.Id, cq.Name});
                return Json(new { success = true, elements });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckGroupName")]
        public JsonResult CheckGroupName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.GroupSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);

                var isIndb = _dbStore.GroupSet.FirstOrDefault(g => g.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));

                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpPost, ActionName("GetMatrixes")]
        public JsonResult GetMatrixes(int Id)
        {
            try
            {
                var Matrixes = _dbStore.GroupSet.Find(Id).Matrixes.Where(m => m.Active).Select(m => new { m.Name, m.Description, m.SubMatrix, m.SubMtrxDescription, BaseMatrix = new { m.BaseMatrix.Name, Mercado = new { m.BaseMatrix.Mercado.Name } } });
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

        [HttpPost, ActionName("GetParamsForGroup")]
        public JsonResult GetParamsForGroup(int id, int page = 1, int pageSize = 10, string searchGeneral = null)
        {
            try
            {
                var PermitedLimits = _dbStore.MaxPermitedLimitSet.Where(lmp => lmp.Active).ToList().Select(lmp => lmp.ToMiniJson());

                dynamic currUsr = Session["curr_User"];
                int userId = currUsr.Id;

                var sucursales = User.Identity.Name == "root"
                    ? _dbStore.SucursalSet.Where(s => s.Active)
                    : _dbStore.SucursalSet.Where(s => s.Active &&
                                s.Users.Any(u => u.Id.Equals(userId)));

                var total = _dbStore.ParamSet.Count(p => p.Active &&
                                                         sucursales.Any(s => s.Active &&
                                                                             (p.SucursalRealiza != null && s.Id == p.SucursalRealiza.Id ||
                                                                              p.SucursalVende != null && s.Id == p.SucursalVende.Id) &&
                                                                              (string.IsNullOrEmpty(searchGeneral) ||
                                                                            p.Description.ToLower().Contains(searchGeneral.ToLower()) ||
                                                                            p.ParamUniquekey.ToLower().Contains(searchGeneral.ToLower()))));

                var activeParamList = _dbStore.ParamSet.Where(p => p.Active &&
                                                                   sucursales.Any(s => s.Active &&
                                                                                       (p.SucursalRealiza != null &&
                                                                                        s.Id == p.SucursalRealiza.Id ||
                                                                                        p.SucursalVende != null &&
                                                                                        s.Id == p.SucursalVende.Id)) &&
                                                                                        (string.IsNullOrEmpty(searchGeneral) ||
                                                                                        p.Description.ToLower().Contains(searchGeneral.ToLower()) ||
                                                                                        p.ParamUniquekey.ToLower().Contains(searchGeneral.ToLower())))
                                                        .OrderBy(p => p.ParamUniquekey).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                    .Select(p => p.ToJson(grupoId: id));
                /*_dbStore.ParamSet.Where(p => p.Active).ToList()
                .Select(p => p.ToJson(grupoId: id));*/
                return Json(new { success = true, paramList = activeParamList, PermitedLimits, total });
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

		[HttpPost, ActionName("GetSucursales")]
		public JsonResult GetSucursales(int bMatrixId)
		{
			try
			{
				var marketId = _dbStore.BaseMatrixSet.Find(bMatrixId).Mercado.Id;
				
				var sucursals = _dbStore.SucursalSet.Where(s => s.Active &&
							s.Offices.Any(o => o.Market.Id == marketId))
					.OrderBy(s => s.Name).ToList()
					.Select(s => new
					{
						s.Id,
						s.Name,
						Region = s.Region.Name
					});

				return Json(new { success = true, elements = sucursals });
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
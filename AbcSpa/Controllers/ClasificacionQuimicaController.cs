using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using AbcPersistent.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.Office.Interop.Excel;

namespace AbcSpa.Controllers
{
    public class ClasificacionQuimicaController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveClasificacionQuimica")]
        public JsonResult SaveClasificacionQuimica(ClasificacionQuimica clasificacionquimica)
        {
            try
            {
	            var cq = clasificacionquimica.Id == 0 ? 
		            new ClasificacionQuimica() : 
		            _dbStore.ClasificacionQuimicaSet.Find(clasificacionquimica.Id);

				cq.Name = clasificacionquimica.Name;
				cq.Active = clasificacionquimica.Active;
				cq.Description = clasificacionquimica.Description;
	            cq.Level = clasificacionquimica.Level;

				if (clasificacionquimica.Id == 0)
					_dbStore.ClasificacionQuimicaSet.Add(cq);

				_dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpPost, ActionName("RefreshList")]
        public JsonResult RefreshList(int page, int pageSize, string searchGeneral, 
                                      int level, bool alta = true, bool baja = false)
        {
            try
            {
	            var total = _dbStore.ClasificacionQuimicaSet.Count(e => (alta && baja ||
							alta && e.Active || baja && !e.Active)
							&& e.Level.Equals(level) && (string.IsNullOrEmpty(searchGeneral) ||
                            e.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())));

                var cq = _dbStore.ClasificacionQuimicaSet.Where(e => (alta && baja ||
                            alta && e.Active || baja && !e.Active)
                            && e.Level.Equals(level) && (string.IsNullOrEmpty(searchGeneral) ||
                            e.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())))
                    .OrderBy(bp => bp.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                    .Select(b => b.ToJson());
                
                return Json(new { success = true, elements = cq, total });
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
                var cq = _dbStore.ClasificacionQuimicaSet.Find(id);
                if (cq == null)
                    return Json(new { success = false, details = "No se encontró info de la clasificación química" });

                cq.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckClasificacionQuimicaName")]
        public JsonResult CheckClasificacionQuimicaName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.ClasificacionQuimicaSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);

                var isIndb = _dbStore.ClasificacionQuimicaSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));

                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

		[HttpPost, ActionName("GetClasificacionQuimica")]
		public JsonResult GetClasificacionQuimica(int level)
		{
			try
			{
				var cq = _dbStore.ClasificacionQuimicaSet.Where(c => c.Active && c.Level.Equals(level))
					.Select(c => new {c.Id, c.Name});

				return Json(new { success = true, elements = cq });
			}
			catch (Exception e)
			{
				return Json(new { success = false, details = e.Message });
			}
		}

		[HttpPost, ActionName("GetClasQuimToFilter")]
        public JsonResult GetClasQuimToFilter()
		{
			try
			{
				dynamic currUsr = Session["curr_User"];
				int userId = currUsr.Id;

				var sucursales = _dbStore.SucursalSet.Where(s => s.Active
					&& (userId == 0 || s.Users.Any(x => x.Id.Equals(userId)))
					&& s.SucursalRealizaParams.Any(p => p.Active)
					|| s.SucursalVendeParams.Any(p => p.Active));

				var parameters = _dbStore.ParamSet.Where(p => p.Active &&
							sucursales.Any(s => (p.SucursalRealiza != null
											&& s.Id.Equals(p.SucursalRealiza.Id))
											|| (p.SucursalVende != null
											&& s.Id.Equals(p.SucursalVende.Id))));

				//var groups = _dbStore.GroupSet.Where(g => g.Active &&
				//			g.Parameters.Any(p => parameters.Any(p1 => p1.Id.Equals(p.Id))));
				var groups = _dbStore.GroupSet.Where(g => g.Active &&
							sucursales.Any(s => g.Sucursal != null && s.Id.Equals(g.Sucursal.Id)));

				var clasquims = _dbStore.ClasificacionQuimicaSet.Where(cq => cq.Active &&
							((cq.BaseParamsCq1.Any(bp => bp.Parameters.Any(p =>
								parameters.Any(p1 => p1.Id.Equals(p.Id)))) ||
							cq.BaseParamsCq2.Any(bp => bp.Parameters.Any(p =>
								parameters.Any(p1 => p1.Id.Equals(p.Id)))) ||
							cq.BaseParamsCq3.Any(bp => bp.Parameters.Any(p =>
								parameters.Any(p1 => p1.Id.Equals(p.Id))))) ||
							(cq.GroupsCq1.Any(g => groups.Any(g1 => g1.Id.Equals(g.Id))) ||
							cq.GroupsCq2.Any(g => groups.Any(g1 => g1.Id.Equals(g.Id))) ||
							cq.GroupsCq3.Any(g => groups.Any(g1 => g1.Id.Equals(g.Id))))));

				var clasquims1 = clasquims.Where(cq => cq.Level == 1)
					.OrderBy(cq => cq.Name).ToList()
					.Select(cq => new
					{
						cq.Id,
						name = cq.Name
					});

				var clasquims2 = clasquims.Where(cq => cq.Level == 2)
					.OrderBy(cq => cq.Name).ToList()
					.Select(cq => new
					{
						cq.Id,
						name = cq.Name
					});

				var clasquims3 = clasquims.Where(cq => cq.Level == 3)
					.OrderBy(cq => cq.Name).ToList()
					.Select(cq => new
					{
						cq.Id,
						name = cq.Name
					});

				return Json(new { success = true, clasquims1, clasquims2, clasquims3 });
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
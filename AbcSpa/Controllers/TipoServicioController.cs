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

namespace AbcSpa.Controllers
{
    public class TipoServicioController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveTipoServicio")]
        public JsonResult SaveTipoServicio(TipoServicio tiposervicio)
        {
            try
            {
	            var tserv = tiposervicio.Id == 0
		            ? new TipoServicio()
		            : _dbStore.TipoServicioSet.Find(tiposervicio.Id);

				tserv.Name = tiposervicio.Name;
				tserv.Active = tiposervicio.Active;
				tserv.Description = tiposervicio.Description;

				if (tiposervicio.Id == 0)
                    _dbStore.TipoServicioSet.Add(tserv);
                
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }
         
        [HttpPost, ActionName("RefreshList")]
        public JsonResult RefreshList(bool alta = true, bool baja = false)
        {
            try
            {
	            var total = _dbStore.TipoServicioSet.Count(e => ((alta && baja) ||
							(alta && e.Active) || (baja && !e.Active)));

                var activeTipoServicioList = _dbStore.TipoServicioSet
					.Where(e => ((alta && baja) ||
							(alta && e.Active) || (baja && !e.Active)))
		            .OrderBy(e => e.Name).ToList()
					.Select(tiposervicio => tiposervicio.ToJson());
				
                return Json(new { success = true, elements = activeTipoServicioList, total });
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
                var tserv = _dbStore.TipoServicioSet.Find(id);
                if (tserv == null)
                    return Json(new { success = false, details = "No se encontró info del residuo" });

                tserv.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckTipoServicioName")]
        public JsonResult CheckTipoServicioName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.TipoServicioSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);
                
                var isIndb = _dbStore.TipoServicioSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));
                
                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }
	}
}
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
    public class StatusController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveStatus")]
        public JsonResult SaveStatus(Status status)
        {
            try
            {
	            var st = status.Id == 0
		            ? new Status()
		            {
			            //Params = new List<Param>()
		            }
		            : _dbStore.StatusSet.Find(status.Id);

				st.Tipo = status.Tipo;
				st.Active = status.Active;
				st.Description = status.Description;

				if (status.Id == 0)
                    _dbStore.StatusSet.Add(st);
                
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
            bool alta = true, bool baja = false)
        {
            try
            {
	            var total = _dbStore.StatusSet.Count(e => (alta && baja ||
							alta && e.Active || baja && !e.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) || e.Tipo.ToUpper().Contains(searchGeneral.ToUpper())||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())));

                var activeStatusList = _dbStore.StatusSet
					.Where(e => (alta && baja ||
                            alta && e.Active || baja && !e.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) || e.Tipo.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())))
                    .OrderBy(m => m.Tipo).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                    .Select(status => status.ToJson());
				
                return Json(new { success = true, elements = activeStatusList, total });
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
                var st = _dbStore.StatusSet.Find(id);
                if (st == null)
                    return Json(new { success = false, details = "No se encontró info del Status" });

                st.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckStatusName")]
        public JsonResult CheckStatusName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.StatusSet.Find(id).Tipo.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);
                
                var isIndb = _dbStore.StatusSet.FirstOrDefault(p => p.Tipo.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));
                
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
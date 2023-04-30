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
    public class AnnalistKeyController : Controller

    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveAnnalistKey")]
        public JsonResult AnnalistKey(AnnalistKey annalistKey)
        {
            try
            {
                var ak = annalistKey.Id == 0
                    ? new AnnalistKey()
                    {
                        Parameters = new List<Param>()
                    }
                    : _dbStore.AnnalistKeySet.Find(annalistKey.Id);

                ak.Clave = annalistKey.Clave;
                ak.Active = annalistKey.Active;
                ak.Description = annalistKey.Description;

                if (annalistKey.Id == 0)
                    _dbStore.AnnalistKeySet.Add(ak);

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
	            var total = _dbStore.AnnalistKeySet.Count(e => (alta && baja ||
							alta && e.Active || baja && !e.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) || e.Clave.ToUpper().Contains(searchGeneral.ToUpper())||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())));

                var activeAnnalistKeyList = _dbStore.AnnalistKeySet
					.Where(e => (alta && baja ||
                            alta && e.Active || baja && !e.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) || e.Clave.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())))
                    .OrderBy(m => m.Clave).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                    .Select(status => status.ToJson());
				
                return Json(new { success = true, elements = activeAnnalistKeyList, total });
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


        [HttpPost, ActionName("GetAnnalistKeys")]
        public JsonResult GetAnnalistKeys(int page, int pageSize, int? id, string searchGeneral, 
           bool alta = true, bool baja = false)
        {
            try
            {
                var total = _dbStore.AnnalistKeySet.Count(e => (alta && baja ||
                            alta && e.Active || baja && !e.Active) && (e.UnidadAnalitica==null || e.UnidadAnalitica.Id == id) &&
                            (string.IsNullOrEmpty(searchGeneral) || e.Clave.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())));

                var activeAnnalistKeyList = _dbStore.AnnalistKeySet
                    .Where(e => (alta && baja ||
                            alta && e.Active || baja && !e.Active) && (e.UnidadAnalitica == null || e.UnidadAnalitica.Id == id) &&
                            (string.IsNullOrEmpty(searchGeneral) || e.Clave.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())))
                    .OrderBy(m => m.Clave).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                    .Select(status => status.ToJson());

                return Json(new { success = true, elements = activeAnnalistKeyList, total });
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
                var ak = _dbStore.AnnalistKeySet.Find(id);
                if (ak == null)
                    return Json(new { success = false, details = "No se encontró info de la clave de analistas" });

                ak.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckAnnalistKeyName")]
        public JsonResult CheckAnnalistKeyName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.AnnalistKeySet.Find(id).Clave.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);
                
                var isIndb = _dbStore.AnnalistKeySet.FirstOrDefault(p => p.Clave.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));
                
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
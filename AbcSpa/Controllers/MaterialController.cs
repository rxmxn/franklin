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
    public class MaterialController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveMaterial")]
        public JsonResult SaveMaterial(Material material)
        {
            try
            {
                var mat = material.Id == 0
                    ? new Material()
                    : _dbStore.MaterialSet.Find(material.Id);

                mat.Name = material.Name;
                mat.Description = material.Description;
                mat.Active = material.Active;
                
                if(material.Id == 0)
                    _dbStore.MaterialSet.Add(mat);

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
	            var total = _dbStore.MaterialSet.Count(e => (alta && baja ||
                            alta && e.Active || baja && !e.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) || e.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())));

                var materials = _dbStore.MaterialSet.Where(e => (alta && baja ||
                            alta && e.Active || baja && !e.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) || e.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())))
                        .OrderBy(m => m.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                    .Select(m => m.ToJson());

                return Json(new { success = true, elements = materials, total });
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
                var mat = _dbStore.MaterialSet.Find(id);
                if (mat == null)
                    return Json(new { success = false, details = "No se encontró info del material" });

                mat.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckMaterialName")]
        public JsonResult CheckMaterialName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.MaterialSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);
                
                var isIndb = _dbStore.MaterialSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));
                
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
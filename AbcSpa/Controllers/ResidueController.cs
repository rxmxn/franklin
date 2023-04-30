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
    public class ResidueController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveResidue")]
        public JsonResult SaveResidue(Residue residue)
        {
            try
            {
	            var res = residue.Id == 0
		            ? new Residue()
		            {
			            Params = new List<Param>()
		            }
		            : _dbStore.ResidueSet.Find(residue.Id);

				res.Name = residue.Name;
				res.Active = residue.Active;
				res.Description = residue.Description;

				if (residue.Id == 0)
                    _dbStore.ResidueSet.Add(res);
                
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
	            var total = _dbStore.ResidueSet.Count(e => (alta && baja ||
							alta && e.Active || baja && !e.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) || e.Name.ToUpper().Contains(searchGeneral.ToUpper())||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())));

                var activeResidueList = _dbStore.ResidueSet
					.Where(e => (alta && baja ||
                            alta && e.Active || baja && !e.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) || e.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())))
                    .OrderBy(m => m.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                    .Select(residue => residue.ToJson());
				
                return Json(new { success = true, elements = activeResidueList, total });
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
                var res = _dbStore.ResidueSet.Find(id);
                if (res == null)
                    return Json(new { success = false, details = "No se encontró info del residuo" });

                res.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckResidueName")]
        public JsonResult CheckResidueName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.ResidueSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);
                
                var isIndb = _dbStore.ResidueSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));
                
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
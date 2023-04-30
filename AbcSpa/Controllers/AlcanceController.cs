using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AbcPersistent.Models;

namespace AbcSpa.Controllers
{
    public class AlcanceController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveAlcance")]
        public JsonResult SaveAlcance(Alcance alcance)
        {
            try
            {
                var alc = alcance.Id == 0
                    ? new Alcance() 
                    : _dbStore.AlcanceSet.Find(alcance.Id);

                alc.Name = alcance.Name;
                alc.Description = alcance.Description;
                alc.Active = alcance.Active;
	            alc.ZonaGeografica = alcance.ZonaGeografica;

				if (alcance.Id == 0)
                    _dbStore.AlcanceSet.Add(alc);

                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }
        
        [HttpPost, ActionName("RefreshList")]
        public JsonResult RefreshList(int page, int pageSize, bool alta = true, bool baja = false)
        {
            try
            {
				var total = _dbStore.AlcanceSet.Count(a => ((alta && baja) ||
							(alta && a.Active) || (baja && !a.Active)));

				var alcance = _dbStore.AlcanceSet.Where(a => ((alta && baja) ||
							(alta && a.Active) || (baja && !a.Active)))
					.OrderBy(e => e.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
					.Select(a => a.ToJson());

                return Json(new { success = true, elements = alcance, total });
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
                var alc = _dbStore.AlcanceSet.Find(id);
                if (alc == null)
                    return Json(new { success = false, details = "No se encontró info del alcance" });

                alc.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckAlcanceName")]
        public JsonResult CheckAlcanceName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.AlcanceSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);
                
                var isIndb = _dbStore.AlcanceSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));
                
                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }
		
		[HttpPost, ActionName("GetAllAlcances")]
		public JsonResult GetAllAlcances(bool activeOption = true)
		{
			try
			{
				var total = _dbStore.AlcanceSet.Count(a => a.Active.Equals(activeOption));

				var alcance = _dbStore.AlcanceSet.Where(e => e.Active == activeOption)
					.OrderBy(e => e.Name).ToList()
					.Select(a => a.ToJson());

				return Json(new { success = true, elements = alcance, total });
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
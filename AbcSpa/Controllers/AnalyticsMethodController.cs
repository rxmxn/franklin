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
    public class AnalyticsMethodController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveAnalyticsMethod")]
        public JsonResult SaveAnalyticsMethod(AnalyticsMethod analyticsMethod)
        {
            try
            {
                AnalyticsMethod am;

                if (analyticsMethod.Id == 0)
                {
                    am = new AnalyticsMethod()
                    {
                        Name = analyticsMethod.Name,
                        Active = analyticsMethod.Active,
                        Description = analyticsMethod.Description,
                        Methods = new List<Method>()    
                    };
                    _dbStore.AnalyticsMethodSet.Add(am);
                }
                else
                {
                    am = _dbStore.AnalyticsMethodSet.Find(analyticsMethod.Id);
                    am.Name = analyticsMethod.Name;
                    am.Active = analyticsMethod.Active;
                    am.Description = analyticsMethod.Description;
                }
                
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
	            var total = _dbStore.AnalyticsMethodSet.Count(e => (alta && baja ||
                            alta && e.Active || baja && !e.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) || e.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())));

                var aaMethod = _dbStore.AnalyticsMethodSet.Where(e => (alta && baja ||
                            alta && e.Active || baja && !e.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) || e.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())))
                        .OrderBy(m => m.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                        .Select(aam => aam.ToJson());
                
                return Json(new { success = true, elements = aaMethod, total });
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
                var am = _dbStore.AnalyticsMethodSet.Find(id);
                if (am == null)
                    return Json(new { success = false, details = "No se encontro info del método analítico" });

                am.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckAnalyticsMethodName")]
        public JsonResult CheckAnalyticsMethodName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.AnalyticsMethodSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);
                
                var isIndb = _dbStore.AnalyticsMethodSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));
                
                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

		[HttpPost, ActionName("GetAnalyticsMethods")]
		public JsonResult GetAnalyticsMethods()
		{
			try
			{
				var elements = _dbStore.AnalyticsMethodSet.Where(a => a.Active)
					.Select(a => new {a.Id, a.Name});
				
				return Json(new { success = true, elements });
			}
			catch (Exception e)
			{
				return Json(new { success = false, details = e.Message });
			}
		}
	}
}
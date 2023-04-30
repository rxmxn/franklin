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
    public class PreserverController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SavePreserver")]
        public JsonResult SavePreserver(Preserver preserver)
        {
            try
            {
                Preserver pre;

                if (preserver.Id == 0)
                {
                    pre = new Preserver()
                    {
                        Name = preserver.Name,
                        Active = preserver.Active,
                        Description = preserver.Description,
                        //Methods = new List<Method>()    
						Params = new List<Param>()
                    };
                    _dbStore.PreserverSet.Add(pre);
                }
                else
                {
                    pre = _dbStore.PreserverSet.Find(preserver.Id);
                    pre.Name = preserver.Name;
                    pre.Active = preserver.Active;
                    pre.Description = preserver.Description;
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
	            var total = _dbStore.PreserverSet.Count(e => (alta && baja ||
							alta && e.Active || baja && !e.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) ||
                            e.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())));

                var preserver = _dbStore.PreserverSet.Where(e => (alta && baja ||
                            alta && e.Active || baja && !e.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) ||
                            e.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())))
                     .OrderBy(m => m.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                    .Select(p => p.ToJson());

                return Json(new { success = true, elements = preserver, total });
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
                var pre = _dbStore.PreserverSet.Find(id);
                if (pre == null)
                    return Json(new { success = false, details = "No se encontro info del preservador" });

                pre.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckPreserverName")]
        public JsonResult CheckPreserverName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.PreserverSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);
                
                var isIndb = _dbStore.PreserverSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));
                
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
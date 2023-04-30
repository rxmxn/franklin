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
    public class CurrencyController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

       [HttpPost, ActionName("SaveCurrency")]
        public JsonResult SaveCurrency(Currency currency)
        {
            try
            {
                Currency cur;

                if (currency.Id == 0)
                {
                    cur = new Currency()
                    {
                        Name = currency.Name,
                        Description = currency.Description,
                        Active = currency.Active
                    };
                    _dbStore.CurrencySet.Add(cur);
                }
                else
                {
                    cur = _dbStore.CurrencySet.Find(currency.Id);
                    cur.Name = currency.Name;
                    cur.Description = currency.Description;
                    cur.Active = currency.Active;
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
        public JsonResult RefreshList(bool alta = true, bool baja = false)
        {
            try
            {
	            var total = _dbStore.CurrencySet.Count(a => ((alta && baja) ||
							(alta && a.Active) || (baja && !a.Active)));

                var currencies = _dbStore.CurrencySet.Where(a => ((alta && baja) ||
							(alta && a.Active) || (baja && !a.Active)))
					.OrderBy(e => e.Name).ToList()
					.Select(c => c.ToJson());
				
                return Json(new { success = true, elements = currencies, total });
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
                var cur = _dbStore.CurrencySet.Find(id);
                if (cur == null)
                    return Json(new { success = false, details = "No se encontro info de la sucursal" });

                cur.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckCurrencyName")]
        public JsonResult CheckCurrencyName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.CurrencySet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);
                
                var isIndb = _dbStore.CurrencySet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));
                
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
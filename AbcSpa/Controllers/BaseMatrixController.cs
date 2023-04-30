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
    public class BaseMatrixController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveBaseMatrix")]
        public JsonResult SaveBaseMatrix(BaseMatrix basematrix)
        {
            try
            {
                BaseMatrix bm;

                if (basematrix.Id == 0)
                {
                    bm = new BaseMatrix()
                    {
                        Name = basematrix.Name,
                        Active = basematrix.Active,
                        Description = basematrix.Description,
                        Mercado = _dbStore.MarketSet.Find(basematrix.Mercado.Id)

                    };

                    _dbStore.BaseMatrixSet.Add(bm);
                }
                else
                {
                    bm = _dbStore.BaseMatrixSet.Find(basematrix.Id);
                    bm.Name = basematrix.Name;
                    bm.Active = basematrix.Active;
                    bm.Description = basematrix.Description;
                    bm.Mercado = _dbStore.MarketSet.Find(basematrix.Mercado.Id);
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
        public JsonResult RefreshList(int? marketId, string searchGeneral,
                                        bool alta = true, bool baja = false)
        {
            try
            {
                var total = _dbStore.BaseMatrixSet.Count(a => (alta && baja ||
                            alta && a.Active || baja && !a.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) || a.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            a.Description.ToUpper().Contains(searchGeneral.ToUpper())) &&
                            (marketId == null || a.Mercado.Id == marketId));

                var bm = _dbStore.BaseMatrixSet.Where(a => ((alta && baja) ||
                            (alta && a.Active) || (baja && !a.Active)) &&
                            (string.IsNullOrEmpty(searchGeneral) || a.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            a.Description.ToUpper().Contains(searchGeneral.ToUpper())) &&
                            (marketId == null || a.Mercado.Id == marketId))
                    .OrderBy(e => e.Name).ToList()
                    .Select(e => e.ToJson());

                return Json(new { success = true, elements = bm, total });
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

        [HttpPost, ActionName("GetMarkets")]
        public JsonResult GetMarkets()
        {
            try
            {
                var marketList = _dbStore.MarketSet.Where(m => m.Active).ToList().Select(m => m.ToJson());
                return Json(new { success = true, elements = marketList });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    err = "No se pudo procesar la info",
                    success = false,
                    details = e.Message
                });
            }
        }

        [HttpPost, ActionName("GetFilters")]
        public JsonResult GetFilters()
        {
            try
            {

                var marketList = _dbStore.BaseMatrixSet.Select(bm => bm.Mercado).Distinct().Select(mk => new { mk.Id, mk.Name });

                return Json(new { success = true, marketList });
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
                var bm = _dbStore.BaseMatrixSet.Find(id);
                if (bm == null)
                    return Json(new { success = false, details = "No se encontró info de la matriz base" });

                bm.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckBaseMatrixName")]
        public JsonResult CheckBaseMatrixName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.BaseMatrixSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);

                var isIndb = _dbStore.BaseMatrixSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));

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
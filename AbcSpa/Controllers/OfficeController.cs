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
    public class OfficeController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveOffice")]
        public JsonResult SaveOffice(Office office)
        {
            try
            {
                var offi = office.Id == 0
                    ? new Office()
                    {
                        Sucursales = new List<Sucursal>(),
                        Regions = new List<Region>()
                    }
                    : _dbStore.OfficeSet.Find(office.Id);

                offi.Name = office.Name;
                offi.Description = office.Description;
                offi.Active = office.Active;
                offi.Market = _dbStore.MarketSet.Find(office.Market.Id);

                if (office.Id == 0)
                    _dbStore.OfficeSet.Add(offi);

                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpPost, ActionName("RefreshList")]
        public JsonResult RefreshList(int page, int pageSize,
                                    string searchGeneral, int? bLine,
                                    bool alta = true, bool baja = false)
        {
            try
            {
                var total = _dbStore.OfficeSet.Count(e => (alta && baja ||
                            alta && e.Active || baja && !e.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) || e.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())) &&
                            (bLine == null || e.Market.Id == bLine));

                var offices = _dbStore.OfficeSet.Where(e => (alta && baja ||
                            alta && e.Active || baja && !e.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) || e.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())) &&
                            (bLine == null || e.Market.Id == bLine))
                    .OrderBy(e => e.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                    .Select(o => o.ToJson());

                return Json(new { success = true, elements = offices, total });
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
                var offi = _dbStore.OfficeSet.Find(id);
                if (offi == null)
                    return Json(new { success = false, details = "No se encontró info de la sucursal" });

                offi.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckOfficeName")]
        public JsonResult CheckOfficeName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.OfficeSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);

                var isIndb = _dbStore.OfficeSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));

                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpPost, ActionName("GetOfficeMarket")]
        public JsonResult GetOfficeMarket(bool active = true)
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
    }
}
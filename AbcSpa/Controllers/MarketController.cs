using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AbcPersistent.Models;
using Microsoft.Ajax.Utilities;

namespace AbcSpa.Controllers
{
    public class MarketController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveMarket")]
        public JsonResult SaveMarket(Market market)
        {
            try
            {
                var mar = market.Id == 0
                    ? new Market()
                    {
                        Offices = new List<Office>()
                    }
                    : _dbStore.MarketSet.Find(market.Id);

                mar.Name = market.Name;
                mar.Description = market.Description;
                mar.Active = market.Active;

                if (market.Id == 0)
                    _dbStore.MarketSet.Add(mar);

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
                                    string searchGeneral,
                                    bool alta = true, bool baja = false)
        {
            try
            {
                var total = _dbStore.MarketSet.Count(m => (alta && baja ||
                                                        alta && m.Active || baja && !m.Active) &&
                                                        (string.IsNullOrEmpty(searchGeneral) || m.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                                                        m.Description.ToUpper().Contains(searchGeneral.ToUpper())));

                var markets = _dbStore.MarketSet.Where(m => (alta && baja ||
                                                        alta && m.Active || baja && !m.Active) &&
                                                        (string.IsNullOrEmpty(searchGeneral) || m.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                                                        m.Description.ToUpper().Contains(searchGeneral.ToUpper())))
                    .OrderBy(m => m.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                    .Select(m => m.ToJson());

                return Json(new { success = true, elements = markets, total });
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
                var mar = _dbStore.MarketSet.Find(id);
                if (mar == null)
                    return Json(new { success = false, details = "No se encontró info del mercado" });

                mar.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckMarketName")]
        public JsonResult CheckMarketName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.MarketSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);

                var isIndb = _dbStore.MarketSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));

                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpPost, ActionName("GetMarketsToFilter")]
        public JsonResult GetMarketsToFilter()
        {
            try
            {
                dynamic currUsr = Session["curr_User"];
                int userId = currUsr.Id;

                var sucursales = _dbStore.SucursalSet.Where(s => s.Active
                    && (userId == 0 || s.Users.Any(x => x.Id.Equals(userId)))
                    && s.SucursalRealizaParams.Any(p => p.Active)
                    || s.SucursalVendeParams.Any(p => p.Active));

                var markets = sucursales.Where(a => a.Offices.Any(o => o.Market.Active)).ToList()
                    .OrderBy(a => a.Name)
					.SelectMany(a => a.Offices)
                    .Select(a => new { a.Market.Id, name = a.Market.Name }).Distinct();

                return Json(new { success = true, elements = markets });
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
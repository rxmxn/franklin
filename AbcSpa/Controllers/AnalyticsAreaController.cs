using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AbcPersistent.Models;

namespace AbcSpa.Controllers
{
    public class AnalyticsAreaController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        //[Audit]
        [HttpPost, ActionName("SaveAnalyticsArea")]
        public JsonResult SaveAnalyticsArea(AnalyticsArea analyticsArea)
        {
            try
            {
                var aa = analyticsArea.Id == 0
                    ? new AnalyticsArea()
                    {
                        UnidadesAnaliticas = new List<UnidadAnalitica>()
                        
                    }
                    : _dbStore.AnalyticsAreaSet.Find(analyticsArea.Id);

                aa.Key = analyticsArea.Key;
                aa.Active = analyticsArea.Active;
                aa.Description = analyticsArea.Description;

                aa.Sucursal = analyticsArea.Sucursal != null
                    ? _dbStore.SucursalSet.Find(analyticsArea.Sucursal.Id)
                    : null;

                aa.CentroCosto = analyticsArea.CentroCosto != null
                    ? _dbStore.CentroCostoSet.Find(analyticsArea.CentroCosto.Id)
                    : null;

                aa.TipoServicio = analyticsArea.TipoServicio != null
                    ? _dbStore.TipoServicioSet.Find(analyticsArea.TipoServicio.Id)
                    : null;

                if (!CheckUnidadesAnaliticasList(aa, analyticsArea))
                    throw new Exception("Error al llenar la lista de unidades analíticas");

                if (analyticsArea.Id == 0)
                    _dbStore.AnalyticsAreaSet.Add(aa);

                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        private bool CheckUnidadesAnaliticasList(AnalyticsArea aa, AnalyticsArea analyticsArea)
        {
            try
            {
                var uatodelete = aa.UnidadesAnaliticas.Where(a => a.Active &&
                    analyticsArea.UnidadesAnaliticas.All(m => m.Id != a.Id));

                foreach (var a in uatodelete.ToList())
                    aa.UnidadesAnaliticas.Remove(_dbStore.UnidadAnaliticaSet.Find(a.Id));

                var uatoadd = analyticsArea.UnidadesAnaliticas.Where(a =>
                    aa.UnidadesAnaliticas.All(m => m.Id != a.Id));

                foreach (var a in uatoadd)
                    aa.UnidadesAnaliticas.Add(_dbStore.UnidadAnaliticaSet.Find(a.Id));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpPost, ActionName("RefreshList")]
        public JsonResult RefreshList(int page, int pageSize, string searchGeneral,
                                      int? CeCId, int? tServId, int? sucursalId, int? uAnaliticaId,
                                      bool alta = true, bool baja = false)
        {
            try
            {
                var total = _dbStore.AnalyticsAreaSet.Count(a => (alta && baja ||
                            alta && a.Active || baja && !a.Active) && (string.IsNullOrEmpty(searchGeneral) ||
                                    (a.Description.ToUpper().Contains(searchGeneral.ToUpper())) ||
                                    (a.Key.ToUpper().Contains(searchGeneral.ToUpper())))
                                    && (CeCId == null || a.CentroCosto.Id == CeCId)
                                    && (tServId == null || a.TipoServicio.Id == tServId)
                                    && (sucursalId==null || a.Sucursal.Id== sucursalId)
                                    && (uAnaliticaId==null || a.UnidadesAnaliticas.Any(ua=>ua.Active && ua.Id== uAnaliticaId)));

                var analyticsArea = _dbStore.AnalyticsAreaSet.Where(a => (alta && baja ||
                            alta && a.Active || baja && !a.Active) && (string.IsNullOrEmpty(searchGeneral) ||
                                    (a.Description.ToUpper().Contains(searchGeneral.ToUpper())) ||
                                    (a.Key.ToUpper().Contains(searchGeneral.ToUpper())))
                                    && (CeCId == null || a.CentroCosto.Id == CeCId)
                                    && (tServId == null || a.TipoServicio.Id == tServId)
                                    && (sucursalId == null || a.Sucursal.Id == sucursalId)
                                    && (uAnaliticaId == null || a.UnidadesAnaliticas.Any(ua => ua.Active && ua.Id == uAnaliticaId)))
                    .OrderBy(aa => aa.Key).Skip((page - 1) * pageSize).Take(pageSize).ToList().Select(aa => aa.ToJson());

                return Json(new { success = true, elements = analyticsArea, total });
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
                var aa = _dbStore.AnalyticsAreaSet.Find(id);
                if (aa == null)
                    return Json(new { success = false, details = "No se encontro info del área analítica" });

                aa.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckAnalyticsAreaName")]
        public JsonResult CheckAnalyticsAreaName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.AnalyticsAreaSet.Find(id).Key.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);

                var isIndb = _dbStore.AnalyticsAreaSet.FirstOrDefault(p =>
                    p.Key.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));

                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }
        [HttpPost, ActionName("GetFilters")]
        public JsonResult GetFilters()
        {
            try
            {

                //dynamic currUsr = Session["curr_User"];
                //int userId = currUsr.Id;

                //var sucursales = User.Identity.Name == "root"
                //    ? _dbStore.SucursalSet.Where(s => s.Active)
                //    : _dbStore.SucursalSet.Where(s => s.Active &&
                //                s.Users.Any(u => u.Id.Equals(userId)));

                var uAnaliticasList =
                    _dbStore.AnalyticsAreaSet.Where(aa => aa.Active )
                        .SelectMany(aa => aa.UnidadesAnaliticas.Where(ua=>ua.Active))
                        .Distinct()
                        .Select(ua => new {ua.Id, Name=ua.Key});
                var tServList =
                    _dbStore.AnalyticsAreaSet.Where(aa => aa.Active && aa.TipoServicio != null)
                        .Select(aa => aa.TipoServicio)
                        .Distinct()
                        .Select(ts => new {ts.Id, ts.Name});
                var sucList= _dbStore.AnalyticsAreaSet.Where(aa => aa.Active && aa.Sucursal != null).Select(aa=>aa.Sucursal).Distinct()
                        .Select(suc => new { suc.Id, suc.Name });
                var CeCList= _dbStore.AnalyticsAreaSet.Where(aa => aa.Active && aa.CentroCosto!=null).Select(aa => aa.CentroCosto).Distinct()
                     .Select(cec => new { cec.Id, Name=cec.Number });

                //var CeCList = parameters.Where(p => p.CentroCosto != null).Select(p => p.CentroCosto).Distinct().Select(p => new { p.Id, Name = p.Number });

                return Json(new { success = true, uAnaliticasList, tServList, sucList, CeCList });
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
        [HttpPost, ActionName("GetAnalyticsAreaSucursal")]
        public JsonResult GetAnalyticsAreaSucursal(bool active = true)
        {
            try
            {
                var sucursal = new SucursalController();
                return sucursal.GetSucursales(active);
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
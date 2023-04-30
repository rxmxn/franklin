using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AbcPersistent.Models;

namespace AbcSpa.Controllers
{
    public class UnidadAnaliticaController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        //[Audit]
        [HttpPost, ActionName("SaveUnidadAnalitica")]
        public JsonResult SaveUnidadAnalitica(UnidadAnalitica unidadAnalitica)
        {
            try
            {
                var ua = unidadAnalitica.Id == 0
                    ? new UnidadAnalitica()
                    {
                        // AreaAnalitica = new List<AnalyticsArea>()
                    }
                    : _dbStore.UnidadAnaliticaSet.Find(unidadAnalitica.Id);

                ua.Key = unidadAnalitica.Key;
                ua.Active = unidadAnalitica.Active;
                ua.Description = unidadAnalitica.Description;

                if (!CheckAnnalistKeys(ua, unidadAnalitica))
                    throw new Exception("Error al llenar la lista de Claves de analista");

                //if (!CheckAreasAnalitica(ua, unidadAnalitica))
                //    throw new Exception("Error al llenar la lista de Claves de analista");


                if (unidadAnalitica.Id == 0)
                    _dbStore.UnidadAnaliticaSet.Add(ua);

                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        private bool CheckAnnalistKeys(UnidadAnalitica ua, UnidadAnalitica uanalitica)
        {
            try
            {
                var aktodelete = ua.AnnalistKeys.Where(a => a.Active && uanalitica.AnnalistKeys.All(m => m.Id != a.Id));

                foreach (var a in aktodelete.ToList())
                    ua.AnnalistKeys.Remove(_dbStore.AnnalistKeySet.Find(a.Id));

                var aktoadd = uanalitica.AnnalistKeys.Where(a => ua.AnnalistKeys.All(m => m.Id != a.Id));

                foreach (var a in aktoadd)
                    ua.AnnalistKeys.Add(_dbStore.AnnalistKeySet.Find(a.Id));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //private bool CheckAreasAnalitica(UnidadAnalitica ua, UnidadAnalitica uanalitica)
        //{
        //    try
        //    {
        //        var aatodelete = ua.AreaAnalitica.Where(a => a.Active && uanalitica.AreaAnalitica.All(m => m.Id != a.Id));

        //        foreach (var a in aatodelete.ToList())
        //            ua.AreaAnalitica.Remove(_dbStore.AnalyticsAreaSet.Find(a.Id));

        //        var aatoadd = uanalitica.AreaAnalitica.Where(a => ua.AreaAnalitica.All(m => m.Id != a.Id));

        //        foreach (var a in aatoadd)
        //            ua.AreaAnalitica.Add(_dbStore.AnalyticsAreaSet.Find(a.Id));

        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        [HttpPost, ActionName("RefreshList")]
        public JsonResult RefreshList(int page, int pageSize, string searchGeneral,
                                      int? annalistKeyId, bool alta = true, bool baja = false)
        {
            try
            {
                var total = _dbStore.UnidadAnaliticaSet.Count(a => (alta && baja ||
                            alta && a.Active || baja && !a.Active) &&
                           (string.IsNullOrEmpty(searchGeneral) || a.Key.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            a.Description.ToUpper().Contains(searchGeneral.ToUpper())) &&
                            (annalistKeyId == null || a.AnnalistKeys.Any(ak => ak.Id == annalistKeyId)));

                var unidadAnalitica = _dbStore.UnidadAnaliticaSet.Where(a => (alta && baja ||
                            alta && a.Active || baja && !a.Active) &&
                           (string.IsNullOrEmpty(searchGeneral) || a.Key.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            a.Description.ToUpper().Contains(searchGeneral.ToUpper())) &&
                            (annalistKeyId == null || a.AnnalistKeys.Any(ak => ak.Id == annalistKeyId)))
                    .OrderBy(ua => ua.Key).Skip((page - 1) * pageSize).Take(pageSize).ToList().Select(ua => ua.ToJson());

                return Json(new { success = true, elements = unidadAnalitica, total });
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

        [HttpPost, ActionName("GetFilters")]
        public JsonResult GetFilters()
        {
            try
            {

                var annalistKeyList =
                    _dbStore.UnidadAnaliticaSet.Where(ua => ua.Active && ua.AnnalistKeys.Any())
                        .SelectMany(ua => ua.AnnalistKeys.Where(ak => ak.Active).Select(ak => ak)).Distinct().ToList().Select(ak => ak.ToJson());


                return Json(new { success = true, annalistKeyList });
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
                var ua = _dbStore.UnidadAnaliticaSet.Find(id);
                if (ua == null)
                    return Json(new { success = false, details = "No se encontro info del área analítica" });

                ua.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckUnidadAnaliticaName")]
        public JsonResult CheckUnidadAnaliticaName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (String.Equals(_dbStore.UnidadAnaliticaSet.Find(id).Key, uniqueInput, StringComparison.CurrentCultureIgnoreCase))
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);

                var isIndb = _dbStore.UnidadAnaliticaSet.FirstOrDefault(p =>
                    p.Key.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));

                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpPost, ActionName("GetUnidadesAnaliticas")]
        public JsonResult GetUnidadesAnaliticas()
        {
            try
            {
                var ua = _dbStore.UnidadAnaliticaSet.Where(cc => cc.Active)
                    .OrderBy(cc => cc.Key).ToList().Select(cc => new
                    {
                        cc.Id,
                        cc.Key,
                        AreaAnalitica = cc.AreaAnalitica != null ? new { cc.AreaAnalitica.Id } : null
                    });

                return Json(new { success = true, elements = ua });
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
        
 [HttpPost, ActionName("GetAnnalistKey")]
        public JsonResult GetAnnalistKey(int id)
        {
            try
            {
                var akList = _dbStore.UnidadAnaliticaSet.Find(id)?.AnnalistKeys.Select(ak => ak.ToJson());
                   

                return Json(new { success = true, elements = akList });
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
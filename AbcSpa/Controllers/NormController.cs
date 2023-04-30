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
    public class NormController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveNorm")]
        public JsonResult SaveNorm(Norm norm)
        {
            try
            {
                var nor = norm.Id == 0
                    ? new Norm()
                    {
                        Packages = new List<Package>(),
                        Matrixes = new List<Matrix>()
                    }
                    : _dbStore.NormSet.Find(norm.Id);

                nor.Name = norm.Name;
                nor.Active = norm.Active;
                nor.Description = norm.Description;
                nor.Alcance = norm.Alcance;
                nor.FechaEntVigor = norm.FechaEntVigor;
                if (nor.Estado != norm.Estado)
                {
                    nor.FechaCambioStatus= DateTime.Now;
                    nor.Estado = norm.Estado != null ? _dbStore.StatusSet.Find(norm.Estado.Id) : null;
                }
                //nor.Estado = norm.Estado ?? null;

                if (norm.Id==0)
                {
                    _dbStore.NormSet.Add(nor);
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
        public JsonResult RefreshList(int page, int pageSize, bool alta = true, bool baja = false)
        {
            try
            {
                var total = _dbStore.NormSet.Count(e => ((alta && baja) ||
                            (alta && e.Active) || (baja && !e.Active)));

                var norms = _dbStore.NormSet.Where(e => ((alta && baja) ||
                            (alta && e.Active) || (baja && !e.Active)))
                    .OrderBy(e => e.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                    .Select(n => n.ToJson());

                return Json(new { success = true, elements = norms, total });
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
                var nor = _dbStore.NormSet.Find(id);
                if (nor == null)
                    return Json(new { success = false, details = "No se encontro info de la norma" });

                nor.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckNormName")]
        public JsonResult CheckNormName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.NormSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);

                var isIndb = _dbStore.NormSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));

                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }
        [HttpPost, ActionName("GetNormInfo")]
        public JsonResult GetNormInfo(int Id)
        {
            try
            {
                if (Id == 0) return Json(new {success = false});
                var norm = _dbStore.NormSet.Find(Id).ToJson();
                return Json(new { success = true, norm});
                
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AbcPersistent.Models;

namespace AbcSpa.Controllers
{
    public class MeasureUnitController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveMeasureUnit")]
        public JsonResult SaveMeasureUnit(MeasureUnit measureunit)
        {
            try
            {
                var unit = measureunit.Id == 0 ?
					new MeasureUnit()
					{
						BaseParams = new List<BaseParam>()
					} : 
					_dbStore.MeasureUnitSet.Find(measureunit.Id);

				unit.Name = measureunit.Name;
				unit.Active = measureunit.Active;
				unit.Description = measureunit.Description;

				if (measureunit.Id == 0)
					_dbStore.MeasureUnitSet.Add(unit);
                
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpPost, ActionName("RefreshList")]
        public JsonResult RefreshList(int page, int pageSize, string searchGeneral, bool alta = true, bool baja = false)
        {
            try
            {
	            var total = _dbStore.MeasureUnitSet.Count(e => (alta && baja ||
							alta && e.Active || baja && !e.Active) && 
                            (string.IsNullOrEmpty(searchGeneral) || 
                            e.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())));

                var mu = _dbStore.MeasureUnitSet.Where(e => (alta && baja ||
                            alta && e.Active || baja && !e.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) ||
                            e.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())))
                    .OrderBy(bp => bp.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                    .Select(u => u.ToJson());
                
                return Json(new { success = true, elements = mu, total });
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
                var unit = _dbStore.MeasureUnitSet.Find(id);
                if (unit == null)
                    return Json(new { success = false, details = "No se encontro info del unidad de medida" });

                unit.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }
		
        [HttpGet, ActionName("CheckMeasureUnitName")]
        public JsonResult CheckMeasureUnitName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.MeasureUnitSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);

                var isIndb = _dbStore.MeasureUnitSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AbcPersistent.Models;

namespace AbcSpa.Controllers
{
    public class CentroCostoController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

		//[Audit]
		[HttpPost, ActionName("SaveCentroCosto")]
        public JsonResult SaveCentroCosto(CentroCosto centroCosto)
        {
            try
            {
				var cc = centroCosto.Id == 0
					? new CentroCosto()
					{
						ParameterToAnalyze = new List<Param>(),
						AreasAnaliticas = new List<AnalyticsArea>()
					}
					: _dbStore.CentroCostoSet.Find(centroCosto.Id);
                
                cc.Number = centroCosto.Number;
                cc.Active = centroCosto.Active;
                cc.Description = centroCosto.Description;
	            cc.Tipo = centroCosto.Tipo;

				if (centroCosto.Id == 0)
					_dbStore.CentroCostoSet.Add(cc);

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
	            var total = _dbStore.CentroCostoSet.Count(a => (alta && baja ||
							alta && a.Active || baja && !a.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) || a.Number.Contains(searchGeneral.ToUpper()) ||
                            a.Description.ToUpper().Contains(searchGeneral.ToUpper()))/*&&
                            (tipoCeC==null || a.Tipo==(int)CentroCosto.TipoCentroCosto(tipoCeC))*/);

                var centroCosto = _dbStore.CentroCostoSet.Where(a => (alta && baja ||
                            alta && a.Active || baja && !a.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) || a.Number.Contains(searchGeneral.ToUpper()) ||
                            a.Description.ToUpper().Contains(searchGeneral.ToUpper())))
                    .OrderBy(cc => cc.Number).Skip((page - 1) * pageSize).Take(pageSize).ToList().Select(cc => cc.ToJson());
                
                return Json(new { success = true, elements = centroCosto, total });
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
                var cc = _dbStore.CentroCostoSet.Find(id);
                if (cc == null)
                    return Json(new { success = false, details = "No se encontro info del área analítica" });

                cc.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckCentroCostoName")]
        public JsonResult CheckCentroCostoName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.CentroCostoSet.Find(id).Number.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);
                
                var isIndb = _dbStore.CentroCostoSet.FirstOrDefault(p => 
					p.Number.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));
                
                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

		[HttpPost, ActionName("GetCentrosCosto")]
		public JsonResult GetCentrosCosto()
		{
			try
			{
				var centroCosto = _dbStore.CentroCostoSet.Where(cc => cc.Active)
					.OrderBy(cc => cc.Number).ToList().Select(cc => new
					{
						cc.Id,
						cc.Number,
						cc.Description
					});

				return Json(new { success = true, elements = centroCosto });
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
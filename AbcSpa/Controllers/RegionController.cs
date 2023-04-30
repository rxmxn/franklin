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
    public class RegionController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveRegion")]
        public JsonResult SaveRegion(Region region, IEnumerable<int> offices)
        {
            try
            {
                var reg = region.Id == 0
                    ? new Region()
                    {
                        Offices = new List<Office>(),
						Sucursales = new List<Sucursal>()
					}
                    : _dbStore.RegionSet.Find(region.Id);

                reg.Name = region.Name;
                reg.Active = region.Active;
                reg.Description = region.Description;
	            reg.Key = region.Key;
				
	            if (region.Id != 0)
	            {
					if (!CheckOffices(reg, offices))
						throw new Exception("Error al llenar la lista de empresas");
				}
	            else
	            {
		            foreach (var office in offices)
			            reg.Offices.Add(_dbStore.OfficeSet.Find(office));

					_dbStore.RegionSet.Add(reg);
	            }	
				
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

		private bool CheckOffices(Region reg, IEnumerable<int> offices)
		{
			try
			{
				var officeToDelete = reg.Offices.Where(m => m.Active &&
				                                            offices.All(o => o != m.Id));

				// Todos los elementos que esten en BD que no vengan de la vista se borran.
				foreach (var office in officeToDelete.ToList())
					reg.Offices.Remove(_dbStore.OfficeSet.Find(office.Id));

				// Todos los elementos que vengan y no esten en BD se adicionan.
				var officeToAdd = offices.Where(s => reg.Offices.All(a => a.Id != s));

				foreach (var office in officeToAdd)
					reg.Offices.Add(_dbStore.OfficeSet.Find(office));

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		[HttpPost, ActionName("RefreshList")]
        public JsonResult RefreshList(int page, int pageSize,
                            string searchGeneral, int? bLine, int? OfficeId,
                            bool alta = true, bool baja = false)
        {
            try
            {
	            var total = _dbStore.RegionSet.Count(e => (alta && baja ||
							alta && e.Active || baja && !e.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) || e.Name.ToUpper().Contains(searchGeneral.ToUpper()) || 
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())) &&
                            (bLine==null || e.Offices.Any(o=>o.Market.Id== bLine)) &&
                            (OfficeId==null || e.Offices.Any(o=>o.Id== OfficeId)));

                var regions = _dbStore.RegionSet.Where(e => (alta && baja ||
                            alta && e.Active || baja && !e.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) || e.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())) &&
                            (bLine == null || e.Offices.Any(o => o.Market.Id == bLine)) &&
                            (OfficeId == null || e.Offices.Any(o => o.Id == OfficeId)))
                    .OrderBy(e => e.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                    .Select(region => region.ToJson());

                return Json(new { success = true, elements = regions, total });
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

        //[HttpPost, ActionName("GetFilters")]
        //public JsonResult GetFilters()
        //{
        //    try
        //    {

        //        dynamic currUsr = Session["curr_User"];
        //        int userId = currUsr.Id;

        //        var sucursales = User.Identity.Name == "root"
        //            ? _dbStore.SucursalSet.Where(s => s.Active)
        //            : _dbStore.SucursalSet.Where(s => s.Active &&
        //                        s.Users.Any(u => u.Id.Equals(userId)));

        //       var officeList = sucursales.Select(s => s.Office).Distinct().Select(o => new { o.Id, o.Name });
        //        var marketList = sucursales.Select(s => s.Office.Market).Distinct().Select(mkt => new { mkt.Id, mkt.Name });
        //        //var recList =
        //        //    sucursales.SelectMany(s => s.RecOtorgs.Select(ro => ro))
        //        //        .Distinct()
        //        //        .Select(ro => new {ro.Id, ro.Ack.Name});
                

        //        return Json(new { success = true, officeList, marketList});
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            err = "No se pudo recuperar la info.",
        //            success = false,
        //            details = ex.Message
        //        });
        //    }
        //}

        [HttpPost, ActionName("SaveActiveStatus")]
        public JsonResult SaveActiveStatus(int id, bool active)
        {
            try
            {
                var reg = _dbStore.RegionSet.Find(id);
                if (reg == null)
                    return Json(new { success = false, details = "No se encontró info del región" });

                reg.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckRegionName")]
        public JsonResult CheckRegionName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.RegionSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);

                var isIndb = _dbStore.RegionSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));

                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpPost, ActionName("GetRegionOffices")]
        public JsonResult GetRegionOffices()
        {
            try
            {
	            var total = _dbStore.OfficeSet.Count();
                var offices = _dbStore.OfficeSet.ToList().Select(o => o.ToJson());
                
                return Json(new { success = true, elements = offices, total });
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
		
		[HttpPost, ActionName("GetAllRegions")]
		public JsonResult GetAllRegions()
		{
			try
			{
				var total = _dbStore.RegionSet.Count();
				var regions = _dbStore.RegionSet.Where(r => r.Active)
					.OrderBy(r => r.Name).ToList().Select(r => r.ToJson());

				return Json(new { success = true, elements = regions, total });
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
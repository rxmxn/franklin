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
    public class ContainerController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveContainer")]
        public JsonResult SaveContainer(Container container)
        {
            try
            {
                var con = container.Id == 0
					? new Container()
					{
						//Methods = new List<Method>()
						Params = new List<Param>()
					}
					: _dbStore.ContainerSet.Find(container.Id);

				con.Name = container.Name;
				con.Active = container.Active;
				con.Description = container.Description;
	            con.Capacity = container.Capacity;
				con.Material = _dbStore.MaterialSet.Find(container.Material.Id);

				if (container.Id == 0)
                    _dbStore.ContainerSet.Add(con);
                
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
            int? materialId, bool alta = true, bool baja = false)
        {
            try
            {
	            var total = _dbStore.ContainerSet.Count(a => (alta && baja ||
							alta && a.Active || baja && !a.Active) && (string.IsNullOrEmpty(searchGeneral) ||
                                                                    a.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                                                                    a.Description.ToUpper().Contains(searchGeneral.ToUpper())) &&
                                                                    (materialId==null || a.Material.Id== materialId));

                var containers = _dbStore.ContainerSet.Where(a => (alta && baja ||
                            alta && a.Active || baja && !a.Active) && (string.IsNullOrEmpty(searchGeneral) ||
                                                                    a.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                                                                    a.Description.ToUpper().Contains(searchGeneral.ToUpper())) &&
                                                                    (materialId == null || a.Material.Id == materialId))
                     .OrderBy(m => m.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                    .Select(c => c.ToJson());

                return Json(new { success = true, elements = containers, total });
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

                var materialList = _dbStore.ContainerSet.Where(c => c.Active && c.Material != null).Select(c => c.Material).Distinct().Select(c => new { c.Id, c.Name });
               
                return Json(new { success = true, materialList});
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
                var cont = _dbStore.ContainerSet.Find(id);
                if (cont == null)
                    return Json(new { success = false, details = "No se encontro info del contenedor" });

                cont.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckContainerName")]
        public JsonResult CheckContainerName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.ContainerSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);
                
                var isIndb = _dbStore.ContainerSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));
                
                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

		[HttpPost, ActionName("GetContainerMaterial")]
		public JsonResult GetContainerMaterial(bool active = true)
		{
			try
			{

			    var material = _dbStore.MaterialSet.Where(mat => mat.Active).Select(mat => new {mat.Id, mat.Name});
			    
                return Json(new {success = true, elements = material});
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AbcPersistent.Models;

namespace AbcSpa.Controllers
{
    public class RamaController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

		//[Audit]
		[HttpPost, ActionName("SaveRama")]
        public JsonResult SaveRama(Rama rama)
        {
            try
            {
				var ram = rama.Id == 0
					? new Rama()
					{
						Params = new List<Param>(),
						Matrixes = new List<Matrix>(),
						Enterprises = new List<Enterprise>()
					}
					: _dbStore.RamaSet.Find(rama.Id);
                
                ram.Name = rama.Name;
                ram.Active = rama.Active;
                ram.Description = rama.Description;

				if (rama.Matrixes != null && !CheckMatrixesList(ram, rama))
					throw new Exception("Error al llenar la lista de matrices");

				if (rama.Enterprises != null && !CheckEnterprisesList(ram, rama))
					throw new Exception("Error al llenar la lista de instituciones");

				if (rama.Id == 0)
					_dbStore.RamaSet.Add(ram);

				_dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

		private bool CheckMatrixesList(Rama ram, Rama rama)
		{
			try
			{
				var todelete = ram.Matrixes.Where(a => a.Active && rama.Matrixes.All(m => m.Id != a.Id));

				foreach (var a in todelete.ToList())
					ram.Matrixes.Remove(_dbStore.MatrixSet.Find(a.Id));

				var toadd = rama.Matrixes.Where(a => ram.Matrixes.All(m => m.Id != a.Id));

				foreach (var a in toadd)
					ram.Matrixes.Add(_dbStore.MatrixSet.Find(a.Id));

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private bool CheckEnterprisesList(Rama ram, Rama rama)
		{
			try
			{
				var todelete = ram.Enterprises.Where(a => a.Active && rama.Enterprises.All(m => m.Id != a.Id));

				foreach (var a in todelete.ToList())
					ram.Enterprises.Remove(_dbStore.EnterpriseSet.Find(a.Id));

				var toadd = rama.Enterprises.Where(a => ram.Enterprises.All(m => m.Id != a.Id));

				foreach (var a in toadd)
					ram.Enterprises.Add(_dbStore.EnterpriseSet.Find(a.Id));

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		[HttpPost, ActionName("RefreshList")]
        public JsonResult RefreshList(string searchGeneral, string searchMatrix, 
                                        int? institutionId, bool alta = true, 
                                        bool baja = false)
        {
            try
            {
	            var total = _dbStore.RamaSet.Count(ram => (alta && baja ||
                            alta && ram.Active || baja && !ram.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) || ram.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            ram.Description.ToUpper().Contains(searchGeneral.ToUpper())) &&
                            (string.IsNullOrEmpty(searchMatrix) || ram.Matrixes.Any(m => m.Name.ToUpper().Contains(searchMatrix.ToUpper()))) &&
                            (institutionId == null || ram.Enterprises.Any(e => e.Id == institutionId)));

                var rama = _dbStore.RamaSet.Where(ram => (alta && baja ||
							alta && ram.Active || baja && !ram.Active) && 
                            (string.IsNullOrEmpty(searchGeneral) || ram.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            ram.Description.ToUpper().Contains(searchGeneral.ToUpper())) &&
                            (string.IsNullOrEmpty(searchMatrix) || ram.Matrixes.Any(m=>m.Name.ToUpper().Contains(searchMatrix.ToUpper()))) &&
                            (institutionId==null || ram.Enterprises.Any(e=>e.Id== institutionId)))
		                    .OrderBy(ram => ram.Name).ToList().Select(ram => ram.ToJson());
                
                return Json(new { success = true, elements = rama, total });
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
                var ram = _dbStore.RamaSet.Find(id);
                if (ram == null)
                    return Json(new { success = false, details = "No se encontro info del área analítica" });

                ram.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckRamaName")]
        public JsonResult CheckRamaName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.RamaSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);
                
                var isIndb = _dbStore.RamaSet.FirstOrDefault(p => 
					p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));
                
                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

		[HttpPost, ActionName("GetRamas")]
		public JsonResult GetRamas()
		{
			try
			{
				var rama = _dbStore.RamaSet.Where(ram => ram.Active)
					.OrderBy(ram => ram.Name).ToList().Select(ram => new
					{
						ram.Id,
						ram.Name,
						ram.Description
					});

				return Json(new { success = true, elements = rama });
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

                var matrixList = _dbStore.RamaSet.Where(r=>r.Active && r.Matrixes.Any()).SelectMany(r => r.Matrixes.Select(m=>new {m.Name})).Distinct();
                var InstitutionList =
                    _dbStore.RamaSet.Where(r => r.Active && r.Enterprises.Any())
                        .SelectMany(r => r.Enterprises.Select(e => e))
                        .Distinct()
                        .Select(e => new {e.Id, e.Name});

                return Json(new { success = true, matrixList, InstitutionList });
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AbcPersistent.Models;

namespace AbcSpa.Controllers
{
	public class EnterpriseController : Controller
	{
		// variable to do queries on the DB
		private readonly AbcContext _dbStore = new AbcContext();

		//[Audit]
		[HttpPost, ActionName("SaveEnterprise")]
		public JsonResult SaveEnterprise(Enterprise enterprise)
		{
			try
			{
				var en = enterprise.Id == 0
					? new Enterprise() { RecOtorgs = new List<RecOtorg>() }
					: _dbStore.EnterpriseSet.Find(enterprise.Id);

				en.Name = enterprise.Name;
				en.Description = enterprise.Description;
				en.Active = enterprise.Active;
				en.Sede = enterprise.Sede;
				en.Tipo = enterprise.Tipo;
				
				if (enterprise.Id == 0)
					_dbStore.EnterpriseSet.Add(en);

				_dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

				return Json(new { success = true });
			}
			catch (Exception e)
			{
				return Json(new { success = false, details = e.Message });
			}
		}

		[HttpPost, ActionName("RefreshList")]
		public JsonResult RefreshList(int page, int pageSize, string searchGeneral, bool? tipo, bool alta = true, bool baja = false)
		{
			try
			{
				var total = _dbStore.EnterpriseSet.Count(e => (alta && baja ||
							alta && e.Active || baja && !e.Active) && 
                            (tipo==null || e.Tipo==tipo) && 
                            (string.IsNullOrEmpty(searchGeneral)|| e.Name.ToUpper().Contains(searchGeneral.ToUpper()) || 
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())));

                var enterprises = _dbStore.EnterpriseSet.Where(e => (alta && baja ||
                            alta && e.Active || baja && !e.Active) &&
                            (tipo == null || e.Tipo == tipo) &&
                            (string.IsNullOrEmpty(searchGeneral) || e.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            e.Description.ToUpper().Contains(searchGeneral.ToUpper())))
                            .OrderBy(e => e.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
					        .Select(e => e.ToJson());

				return Json(new { success = true, elements = enterprises, total });
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
				var ent = _dbStore.EnterpriseSet.Find(id);
				if (ent == null)
					return Json(new { success = false, details = "No se encontró info de la empresa" });

				ent.Active = active;
				_dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
				return Json(new { success = true });
			}
			catch (Exception e)
			{
				return Json(new { success = false, details = e.Message });
			}
		}

		[HttpGet, ActionName("CheckEnterpriseName")]
		public JsonResult CheckEnterpriseName(string uniqueInput, int id)
		{
			try
			{
				if (id != 0)
					if (_dbStore.EnterpriseSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
						return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);

				var isIndb = _dbStore.EnterpriseSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));

				//it returns true if it exists
				return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				return Json(new { success = false, details = e.Message });
			}
		}
		
		[HttpPost, ActionName("GetEnterprises")]
		public JsonResult GetEnterprises(bool activeOption = true)
		{
			try
			{
				var activeEnterpriseList = _dbStore.EnterpriseSet
					.Where(e => e.Active == activeOption)
					.OrderBy(e => e.Name).ToList()
					.Select(ent => ent.ToJson());

				return Json(new { success = true, elements = activeEnterpriseList });
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

		[HttpPost, ActionName("GetAllEnterprises")]
		public JsonResult GetAllEnterprises()
		{
			try
			{
                var enterprises = _dbStore.EnterpriseSet.Where(e => e.Active)
					.OrderBy(e => e.Name).ToList()
					.Select(e => new { e.Id, e.Name, e.Description });

				return Json(new { success = true, elements = enterprises });
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
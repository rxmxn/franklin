using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AbcPersistent.Models;

namespace AbcSpa.Controllers
{
    public class ActionsController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveActions")]
        public JsonResult SaveActions(Accion actions)
        {
            try
            {
                var act = actions.Id == 0
                    ? new Accion()
                    {
                        Acks = new List<Ack>()
                    }
                    : _dbStore.AccionSet.Find(actions.Id);

                act.Name = actions.Name;
                act.Description = actions.Description;
                act.Active = actions.Active;

                if (actions.Id == 0)
                    _dbStore.AccionSet.Add(act);

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
				var total = _dbStore.AccionSet.Count(a => (alta && baja ||
							alta && a.Active || baja && !a.Active) && 
                            (string.IsNullOrEmpty(searchGeneral)||a.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            a.Description.ToUpper().Contains(searchGeneral.ToUpper())));

				var actions = _dbStore.AccionSet.Where(a => (alta && baja ||
                            alta && a.Active || baja && !a.Active) &&
                            (string.IsNullOrEmpty(searchGeneral) || a.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            a.Description.ToUpper().Contains(searchGeneral.ToUpper())))
                            .OrderBy(e => e.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
					        .Select(a => a.ToJson());

                return Json(new { success = true, elements = actions, total });
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
                var act = _dbStore.AccionSet.Find(id);
                if (act == null)
                    return Json(new { success = false, details = "No se encontró info de la acción" });

                act.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckActionsName")]
        public JsonResult CheckActionsName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.AccionSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);
                
                var isIndb = _dbStore.AccionSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));
                
                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }
		
		[HttpPost, ActionName("GetAllActions")]
		public JsonResult GetAllActions(bool activeOption = true)
		{
			try
			{
				var total = _dbStore.AccionSet.Count(a => a.Active.Equals(activeOption));

				var actions = _dbStore.AccionSet.Where(e => e.Active == activeOption)
					.OrderBy(e => e.Name).ToList()
					.Select(a => a.ToJson());

				return Json(new { success = true, elements = actions, total });
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
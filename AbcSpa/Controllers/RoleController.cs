using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AbcPersistent.Models;

namespace AbcSpa.Controllers
{
    public class RoleController : Controller
    {
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost]
        [ActionName("SaveRoleData")]
        public JsonResult SaveRoleData(Role role)
        {
            try
            {
                var rol = role.Id == 0 
					? new Role()
					{
						RightColl = new List<Right>(),
						ParamCols = new List<ParamCol>()
					} 
					: _dbStore.RoleSet.Find(role.Id);
                
                rol.Name = role.Name;
                rol.Description = role.Description;
                rol.Active = role.Active;

                if (role.Id != 0)
                {
                    if (!ResetLists(rol.Id))
                        throw new Exception("Error al resetear las listas de permisos y columnas");
				}

                foreach (var p in role.RightColl)
                    rol.RightColl.Add(_dbStore.RightSet.Find(p.Id));

				foreach (var c in role.ParamCols)
					rol.ParamCols.Add(_dbStore.ParamColSet.Find(c.Id));

				if (role.Id == 0)
                    _dbStore.RoleSet.Add(rol);

                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
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

        private bool ResetLists(int roleid)
        {
            try
            {
                var role = _dbStore.RoleSet.Find(roleid);
                if (role == null) return false;

                var rightList = role.RightColl.ToList();
                foreach (var p in rightList)
                    role.RightColl.Remove(p);

	            var paramColsList = role.ParamCols.ToList();
				foreach (var c in paramColsList)
					role.ParamCols.Remove(c);

				return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpPost]
        [ActionName("UpdateRoleActive")]
        public JsonResult UpdateRoleActive(int id, bool active)
        {
            try
            {
                var rol = _dbStore.RoleSet.Find(id);
                if (rol == null)
                    return Json(new {success = false, details = "No se encontro el elemento"});

                rol.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
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

        [HttpPost]
        [ActionName("GetRoles")]
        public JsonResult GetRoles(bool alta = true, bool baja = false)
        {
            try
            {
	            var total = _dbStore.RoleSet.Count(e => ((alta && baja) ||
							(alta && e.Active) || (baja && !e.Active)));

                var roles = _dbStore.RoleSet.Where(e => ((alta && baja) ||
							(alta && e.Active) || (baja && !e.Active)))
					.OrderBy(r => r.Name).ToList()
					.Select(r => r.ToJson());

                return Json(new { success = true, elements = roles, total });
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
		
        [HttpPost]
        [ActionName("GetAccessList")]
        public JsonResult GetAccessList()
        {
            try
            {
                var pList = _dbStore.RightSet.OrderBy(r => r.Name).ToList()
					.Select(r => r.ToJson());

                return Json(new { success = true, permList = pList });
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

		[HttpPost]
		[ActionName("GetColumnList")]
		public JsonResult GetColumnList()
		{
			try
			{
				var pList = _dbStore.ParamColSet.OrderBy(p => p.Id).ToList()
					.Select(p => p.ToJson());

				return Json(new { success = true, columnList = pList });
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

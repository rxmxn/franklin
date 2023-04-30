using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using AbcPersistent.Models;

namespace AbcSpa.Controllers
{
    public class UserController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        public static dynamic GhostAdmin()
        {
            var e = new User
            {
                Id = 0,
                Name = "Súper Administrador",
                Gender = false,
                UserName = "root",
                Photo = "/Content/img/root.png"
            };

            return new
            {
                e.Id,
                e.Name,
                e.UserName,
                e.Active,
                e.Photo
            };
        }

	    [HttpPost, ActionName("RefreshList")]
        public JsonResult RefreshList(int page, int pageSize, string searchGeneral,
			string fromDate, string untilDate, bool alta = true, bool baja = false)
        {
            try
            {
				DateTime fromD = new DateTime(), untilD = new DateTime();
				if (!fromDate.IsEmpty() && !untilDate.IsEmpty())
				{
					var dateData = fromDate.Substring(0, 10).Split('/');
					fromD = new DateTime(Int32.Parse(dateData[2]), Int32.Parse(dateData[1]), Int32.Parse(dateData[0]));
					dateData = untilDate.Substring(0, 10).Split('/');
					untilD = new DateTime(Int32.Parse(dateData[2]), Int32.Parse(dateData[1]), Int32.Parse(dateData[0]));
					untilD = untilD.AddHours(23).AddMinutes(59).AddSeconds(59);
				}

	            var total = _dbStore.UserSet.Count(u => ((alta && baja) ||
							(alta && u.Active) || (baja && !u.Active))
					&& (string.IsNullOrEmpty(fromDate) && string.IsNullOrEmpty(untilDate) ||
							((u.UserCreateDate >= fromD) && (u.UserCreateDate <= untilD)))
					&& (string.IsNullOrEmpty(searchGeneral) ||
	                    (u.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
	                    u.LastNameFather.ToUpper().Contains(searchGeneral.ToUpper()) ||
						u.LastNameMother.ToUpper().Contains(searchGeneral.ToUpper()) ||
						u.UserName.ToUpper().Contains(searchGeneral.ToUpper()) ||
						u.Phone.Contains(searchGeneral) ||
						u.Email.ToUpper().Contains(searchGeneral.ToUpper()))));
				
				var activeUserList = _dbStore.UserSet.Where(u => ((alta && baja) ||
							(alta && u.Active) || (baja && !u.Active))
					&& (string.IsNullOrEmpty(fromDate) && string.IsNullOrEmpty(untilDate) ||
							((u.UserCreateDate >= fromD) && (u.UserCreateDate <= untilD)))
					&& (string.IsNullOrEmpty(searchGeneral) ||
						(u.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
						u.LastNameFather.ToUpper().Contains(searchGeneral.ToUpper()) ||
						u.LastNameMother.ToUpper().Contains(searchGeneral.ToUpper()) ||
						u.UserName.ToUpper().Contains(searchGeneral.ToUpper()) ||
						u.Phone.Contains(searchGeneral) ||
						u.Email.ToUpper().Contains(searchGeneral.ToUpper()))))
					.OrderBy(u => u.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
					.Select(u => u.ToJson());
				
                return Json(new { success = true, elements = activeUserList, total });
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
		
		[HttpPost, ActionName("SaveEmployee")]
        public JsonResult SaveEmployee(User employee)
        {
            try
            {
                var empleado = employee.Id == 0
                    ? new User
                    {
                        Notes = new List<Notes>(),
						SessionLogColl = new List<SessionLog>(),
						Sucursales = new List<Sucursal>()
                    }
					: _dbStore.UserSet.Find(employee.Id);

                /*Ver si darle privilegio al administrador para modificar las contraseñas. sinó quitar instrucción*/
                empleado.UserPassword = Cryptografy.CalcHash(employee.UserPassword);

                empleado.Name = employee.Name;
                empleado.LastNameFather = employee.LastNameFather;
				empleado.LastNameMother = employee.LastNameMother;
				empleado.Gender = employee.Gender; 
                empleado.Photo = employee.Photo;
                empleado.UserName = employee.UserName;
                empleado.Phone = employee.Phone;
                empleado.Email = employee.Email;
                empleado.Role = _dbStore.RoleSet.Find(employee.Role.Id);

				if (employee.Id != 0)
					if (!ResetSucursalesList(employee.Id))
						throw new Exception("Error al resetear la lista de sucursales");

				if (!RefillSucursalesList(employee, empleado))
					throw new Exception("Error al llenar la lista de sucursales");

				if (employee.Id == 0)
                    _dbStore.UserSet.Add(empleado);

				_dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

	    private bool ResetSucursalesList(int userId)
	    {
			try
			{
				var user = _dbStore.UserSet.Find(userId);
				if (user == null) return false;

				foreach (var route in user.Sucursales.ToList())
				{
					user.Sucursales.Remove(route);
				}

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private bool RefillSucursalesList(User employee, User emp)
		{
			try
			{
				if (employee.Sucursales == null) return true;
				foreach (var e in employee.Sucursales.ToList())
				{
					//emp.Sucursales.Add(_dbStore.SucursalSet.FirstOrDefault(s => (s.Region.Id == e.Region.Id) && (s.Office.Id == e.Office.Id)));
					emp.Sucursales.Add(_dbStore.SucursalSet.Find(e.Id));
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		[HttpPost, ActionName("GetUserSucursal")]
	    public JsonResult GetUserSucursal(int userId, bool active = true)
	    {
		    try
		    {
				var user = userId == 0 ? null : _dbStore.UserSet.Find(userId);
				var activeSucursalesList = _dbStore.SucursalSet.Where(e => e.Active == active).ToList();
				var sucursalList = new List<dynamic>();

			    foreach (var sucursal in activeSucursalesList)
			    {
					//var route = user?.Sucursales.FirstOrDefault(u => (u.Region.Id == sucursal.Region.Id)
					//												&& (u.Office.Id == sucursal.Office.Id));

					var route = user?.Sucursales.FirstOrDefault(u => u.Id.Equals(sucursal.Id));

					var check = route != null;

					sucursalList.Add(new
					{
						sucursal.Id,
						sucursal.Name,
						Region = sucursal.Region.Name,
						check
					});
				}
				
				return Json(new { success = true, elements = sucursalList });
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
		
		//--------------------------------------------------------------------------------------------------
		[HttpPost, ActionName("SaveActiveStatus")]
        public JsonResult SaveActiveStatus(int id, bool active)
        {
            try
            {
                var emp = _dbStore.UserSet.Find(id);
                if (emp == null)
                    return Json(new {success = false, details = "No se encontro info del empleado"});

                emp.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        //--------------------------------------------------------------------------------------------------

        [HttpGet, ActionName("CheckUserName")]
        public JsonResult CheckUserName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.UserSet.Find(id).UserName.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);

                var isIndb = _dbStore.UserSet.FirstOrDefault(p => p.UserName.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));

                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpPost, ActionName("GetUserRoles")]
        public JsonResult GetUserRoles(bool active = true)
        {
            try
            {
                var rc = new RoleController();
                return rc.GetRoles(active);
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
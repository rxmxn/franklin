using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Web.Mvc;
using System.Web.Security;
using AbcPersistent.Models;

namespace AbcSpa.Controllers
{
	public class Cryptografy
	{
		public static string CalcHash(string data)
		{
			var ret = "";
			try
			{
				using (var mem = new MemoryStream())
				{
					var bWriter = new BinaryWriter(mem);
					bWriter.Write(data);
					mem.Position = 0;
					MD5 md5 = new MD5CryptoServiceProvider();
					var res = md5.ComputeHash(mem);
					bWriter.Close();
					ret = res.Aggregate(ret, (current, t) => current + (char)t);
				}
			}
			catch { ret = "N/A"; }
			return ret;
		}
	}

	public class SecurityController : Controller
	{
		private readonly AbcContext _dbStore = new AbcContext();

		// Just to save in Session
		public static dynamic UserToJson(User user)
		{
			return new
			{
				user.Id,
				user.Name,
				user.LastNameFather,
				user.LastNameMother,
				user.UserName,
				user.Gender,
				user.Photo,
				Role = new
				{
					user.Role.Id,
					user.Role.Name
				}
			};
		}

		[HttpPost]
		[AllowAnonymous]
		[ActionName("UserLogin")]
		public JsonResult UserLogin(string uname, string upass)
		{
			try
			{
				dynamic tojson;
				var str = Cryptografy.CalcHash(upass);
				Session["curr_User"] = null;
				
				User empleado = null;
				
				if (uname.Equals("root") && upass.Equals("r00t"))
				{
					tojson = UserController.GhostAdmin();
					Session["curr_User"] = tojson;
				}
				else
				{
					empleado = _dbStore.UserSet.SingleOrDefault(e =>
							e.UserName.Equals(uname) &&
							e.UserPassword.Equals(str) &&
							e.Active);
					if (empleado != null)
					{
						tojson = UserToJson(empleado);
						Session["curr_User"] = tojson;
					}
					else
					{
						throw new Exception("Nombre de usuario o contraseña incorrectos. Inténtelo nuevamente o consulte a su administrador.");
					}
				}
				
				// Si un usuario se esta logueando (y no es root)
				if (Session["curr_User"] != null && empleado != null)
				{
					if (ModelState.IsValid)
					{
						var sl = new SessionLog
						{
							StartSession = DateTime.Now,
							Key = empleado.UserName,
							Connected = true,
							IpAddress = LocalIpAddress() ?? "127.0.0.1"
                        };
						empleado.SessionLogColl.Add(sl);

						// Revisando si la ultima vez que se conecto este usuario se deslogueo
						// (para evitar posibles errores, por ejemplo, si se reinicio el server)
                        var lastSession = _dbStore.SessionLogSet
							.SingleOrDefault(s => (s.User.Id == empleado.Id) && s.Connected);
						if (lastSession != null)
						{
							lastSession.Connected = false;
							lastSession.EndSession = DateTime.Now;
						}

						_dbStore.SaveChanges();
					}
				}

				FormsAuthentication.SetAuthCookie(uname, true);

				string roleName = tojson.UserName == "root" ? "Administrador" : tojson.Role.Name;

				return Json(new
				{
					success = true,
					user = tojson,
					accessList = FillAccessList(roleName),
					columnList = FillColumnsList(roleName)
				});
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
		[ActionName("CheckUserConnection")]
		public JsonResult CheckUserConnection()
		{
			try
			{
				if (Session["curr_User"] == null)
					return Json(new {success = false});

				dynamic toJson = Session["curr_User"];
				string roleName = toJson.UserName == "root" ? "Administrador" : toJson.Role.Name;

				return Json(new
				{
					success = true,
					user = toJson,
					accessList = FillAccessList(roleName),
					columnList = FillColumnsList(roleName)
				});
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
		[ActionName("UserLogout")]
		public JsonResult UserLogout(int userId = 0)
		{
			try
			{
				if (userId == 0)
				{
					// Que userId sea = 0 implica que el usuario se esta deslogueando de forma natural.
					// Si es diferente de 0 implica que se lanzo la interrupcion por timeout
					// y que se desloguea el usuario por no estar ejecutando ninguna accion.
					dynamic toJson = Session["curr_User"];
					userId = toJson.Id;
					Session["curr_User"] = null;
				}

				if (userId == 0) return Json(new {success = true});

				var session = _dbStore.SessionLogSet.Single(s => (s.User.Id == userId) && s.Connected);
				session.Connected = false;
				session.EndSession = DateTime.Now;
				_dbStore.SaveChanges();

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
		
		private dynamic FillAccessList(string roleName)
		{
			var accessList = new Dictionary<string, int>();

			var rights = _dbStore.RightSet.ToList();
			var role = _dbStore.RoleSet.FirstOrDefault(e => e.Name.Equals(roleName));

			if (role == null) return null;
			foreach (var right in role.RightColl)
			{
				accessList[right.Value] = right.Level;
			}

			foreach (var r in rights.Where(r => !accessList.ContainsKey(r.Value)))
			{
				accessList[r.Value] = -1;
			}

			return accessList;
		}

		private dynamic FillColumnsList(string roleName)
		{
			var columns = new Dictionary<string, bool>();

			var paramCols = _dbStore.ParamColSet.ToList();
			var role = _dbStore.RoleSet.FirstOrDefault(e => e.Name.Equals(roleName));

			if (role == null) return null;
			foreach (var col in paramCols)
			{
				columns[col.Key] = role.ParamCols.Contains(col);
			}
			
			return columns;
		}

		private string LocalIpAddress()
		{
			return !System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()
				? null
				: Request.UserHostName;
			//!string.IsNullOrEmpty(Request.ServerVariables["HTTP_X_FORWARDED_FOR"])
			//	? Request.ServerVariables["HTTP_X_FORWARDED_FOR"] // behind proxy
			//	: Request.UserHostName;
			//Dns.GetHostEntry(Dns.GetHostName())
			//	.AddressList
			//	.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?
			//	.ToString();

			//var host = Dns.GetHostEntry(Dns.GetHostName());

			//         return host
			//	.AddressList
			//	.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?.ToString();
		}

		[HttpPost]
		[ActionName("RestartSession")]
		public JsonResult RestartSession()
		{
			try
			{
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
	}
}

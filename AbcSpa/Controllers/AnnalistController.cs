using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AbcPersistent.Models;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AbcSpa.Controllers
{
	public class AnnalistController : Controller
	{
		// variable to do queries on the DB
		private readonly AbcContext _dbStore = new AbcContext();

		[HttpPost, ActionName("SaveAnnalist")]
		public JsonResult SaveAnnalist(Annalist annalist)
		{
			try
			{
				var an = annalist.Id == 0 ?
					new Annalist()
					{
						Params = new List<Param>(),
						Sucursales = new List<Sucursal>(),
						RecAdqs = new List<RecAdq>()
					}
					: _dbStore.AnnalistSet.Find(annalist.Id);

				an.Name = annalist.Name;
				an.Active = annalist.Active;
				an.Description = annalist.Description;
				an.Photo = annalist.Photo;
				an.Gender = annalist.Gender;
				an.Email = annalist.Email;
				an.Phone = annalist.Phone;
				an.LastNameFather = annalist.LastNameFather;
				an.LastNameMother = annalist.LastNameMother;
			    an.Key = annalist.Key;//an.AnnalistKey = annalist.AnnalistKey!=null? _dbStore.AnnalistKeySet.Find(annalist.AnnalistKey.Id):null;
			    an.FechaAlta = annalist.FechaAlta;
			    an.Curriculum = annalist.Curriculum;
			    an.NoEmpleado = annalist.NoEmpleado;
			    an.Firma = annalist.Firma;
			    an.Puesto = annalist.Puesto;

				if (annalist.Id != 0)
					if (!ResetSucursalesList(annalist.Id))
						throw new Exception("Error al resetear la lista de sucursales");

				if (!RefillSucursalesList(annalist, an))
					throw new Exception("Error al llenar la lista de sucursales");

				if (annalist.Id == 0)
					_dbStore.AnnalistSet.Add(an);

				_dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

				return Json(new { success = true });
			}
			catch (Exception e)
			{
				return Json(new { success = false, details = e.Message });
			}
		}

		private bool ResetSucursalesList(int annalistId)
		{
			try
			{
				var annalist = _dbStore.AnnalistSet.Find(annalistId);
				if (annalist == null) return false;

				foreach (var route in annalist.Sucursales.ToList())
				{
					annalist.Sucursales.Remove(route);
				}

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private bool RefillSucursalesList(Annalist annalist, Annalist an)
		{
			try
			{
				if (annalist.Sucursales == null) return true;
				foreach (var e in annalist.Sucursales.ToList())
				{
					//an.Sucursales.Add(_dbStore.SucursalSet
					//	.FirstOrDefault(s => (s.Region.Id == e.Region.Id)
					//	&& (s.Office.Id == e.Office.Id)));
					an.Sucursales.Add(_dbStore.SucursalSet.Find(e.Id));
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		[HttpPost, ActionName("RefreshList")]
		public JsonResult RefreshList(int page, int pageSize, bool alta = true, bool baja = false, bool signatario = false)
		{
			try
			{
				var total = _dbStore.AnnalistSet.Count(a => ((alta && baja) ||
							(alta && a.Active) || (baja && !a.Active))
														&& (!signatario || a.RecAdqs.Any(ra => ra.TipoSignatario != null)));

				var annalists = _dbStore.AnnalistSet.Where(a => ((alta && baja) ||
							(alta && a.Active) || (baja && !a.Active))
														&& (!signatario || a.RecAdqs.Any(ra => ra.TipoSignatario != null)))
					.OrderBy(a => a.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
					.Select(a => a.ToJson());

				return Json(new { success = true, elements = annalists, total });
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
				var an = _dbStore.AnnalistSet.Find(id);
				if (an == null)
					return Json(new { success = false, details = "No se encontro info del analista" });

				an.Active = active;
				_dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
				return Json(new { success = true });
			}
			catch (Exception e)
			{
				return Json(new { success = false, details = e.Message });
			}
		}

        [HttpGet, ActionName("CheckAnnalistKey")]
        public JsonResult CheckAnnalistKey(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.AnnalistSet.Find(id).Key.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);

                var isIndb = _dbStore.AnnalistSet.FirstOrDefault(p => p.Key.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));

                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpPost, ActionName("GetAnnalist")]
		public JsonResult GetAnnalist(bool activeOption = true, bool signatario = false)
		{
			try
			{
				var annalists = _dbStore.AnnalistSet.Where(a => a.Active.Equals(activeOption)
								&& (!signatario || a.RecAdqs.Any(ra => ra.TipoSignatario != null)))
					.OrderBy(a => a.Name).ToList()
					.Select(a => a.ToJson());

				return Json(new { success = true, elements = annalists });
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

		[HttpPost, ActionName("GetAnnalistSucursal")]
		public JsonResult GetAnnalistSucursal(int annalistId, bool active = true)
		{
			try
			{
				var annalist = annalistId == 0 ? null : _dbStore.AnnalistSet.Find(annalistId);
				var activeSucursalesList = _dbStore.SucursalSet.Where(e => e.Active == active).ToList();
				var sucursalList = new List<dynamic>();

				foreach (var sucursal in activeSucursalesList)
				{
					//var route = annalist?.Sucursales.FirstOrDefault(u => (u.Region.Id == sucursal.Region.Id)
					//												&& (u.Office.Id == sucursal.Office.Id));
					var route = annalist?.Sucursales.FirstOrDefault(u => u.Id.Equals(sucursal.Id));

					var check = route != null;

					sucursalList.Add(new
					{
						sucursal.Id,
						sucursal.Name,
						//Office = sucursal.Office.Name,
						//Market = sucursal.Office.Market.Name,
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

		[HttpPost, ActionName("GetAnnalistInfo")]
		public JsonResult GetAnnalistInfo(string annalistKey)
		{
			try
			{
				var annalist = string.IsNullOrEmpty(annalistKey) ? null 
					: _dbStore.AnnalistSet/*.Where(a=>a.Active && a.AnnalistKey!=null)*/.FirstOrDefault(a => a.Key/*a.AnnalistKey.Clave*/.ToUpper().Equals(annalistKey.ToUpper(), StringComparison.CurrentCultureIgnoreCase));
				var elements = annalist?.ToJson();

				return Json(new { success = true, elements });
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

		[HttpPost, ActionName("UpdateFromIntelesis")]
		public JsonResult UpdateFromIntelesis()
		{
			try
			{
				var response = new List<dynamic>();

				#region From INTELISIS

				const string sql = "SELECT" +
					"Personal.Personal," +
					"Personal.Nombre," +
					"Personal.ApellidoPaterno," +
					"Personal.ApellidoMaterno," +
					"Personal.FechaAlta," +
					"Personal.Jornada," +
					"Personal.Puesto," +
					"Personal.Telefono," +
					"Personal.eMail," +
					"Personal.NivelAcademico," +
					"Personal.Departamento," +
					"Personal.Estatus," +
					"Jornada.Jornada," +
					"Jornada.Domingo," +
					"Jornada.Lunes," +
					"Jornada.Martes," +
					"Jornada.Miercoles," +
					"Jornada.Jueves," +
					"Jornada.Viernes," +
					"Jornada.Sabado," +
					"jornada.Descripcion," +
					"jornada.SaleSiguientedia," +
					"jornada.EsRotativo," +
					"CentroCostos.CentroCostos," +
					"CentroCostos.Descripcion," +
					"CentroCostos.Grupo," +
					"CentroCostos.Estatus " +
					"FROM (personal INNER JOIN Jornada ON personal.jornada = jornada.jornada) " +
					"INNER JOIN CentroCostos ON personal.CentroCostos = CentroCostos.CentroCostos";

				using (var con = new SqlConnection(
						System.Configuration.ConfigurationManager.ConnectionStrings["IntelisisCxnStr"]
						.ConnectionString))
				using (var cmd = con.CreateCommand())
				{
					con.Open();
					cmd.CommandText = sql;

					using (var result = cmd.ExecuteReader())
					{
						while (result.Read())
						{
							dynamic resp = new
							{
								Personal = result.IsDBNull(result.GetOrdinal("Personal")) ? 0
										: result.GetInt32(result.GetOrdinal("Personal")),

								Nombre = result.IsDBNull(result.GetOrdinal("Nombre")) ? ""
										: result.GetString(result.GetOrdinal("Nombre")),

								ApellidoPaterno = result.IsDBNull(result.GetOrdinal("ApellidoPaterno")) ? ""
										: result.GetString(result.GetOrdinal("ApellidoPaterno")),

								ApellidoMaterno = result.IsDBNull(result.GetOrdinal("ApellidoMaterno")) ? ""
										: result.GetString(result.GetOrdinal("ApellidoMaterno")),

								FechaAlta = result.IsDBNull(result.GetOrdinal("FechaAlta")) ? new DateTime()
										: result.GetDateTime(result.GetOrdinal("FechaAlta")),

								Puesto = result.IsDBNull(result.GetOrdinal("Puesto")) ? ""
										: result.GetString(result.GetOrdinal("Puesto")),

								Telefono = result.IsDBNull(result.GetOrdinal("Telefono")) ? ""
										: result.GetString(result.GetOrdinal("Telefono")),

								eMail = result.IsDBNull(result.GetOrdinal("eMail")) ? ""
										: result.GetString(result.GetOrdinal("eMail")),

								NivelAcademico = result.IsDBNull(result.GetOrdinal("NivelAcademico")) ? ""
										: result.GetString(result.GetOrdinal("NivelAcademico")),

								Departamento = result.IsDBNull(result.GetOrdinal("Departamento")) ? ""
										: result.GetString(result.GetOrdinal("Departamento")),
								// Duda con respecto a Estatus que se repite en personal y centrocosto.
								Estatus = result.IsDBNull(result.GetOrdinal("Estatus")) ? ""
										: result.GetString(result.GetOrdinal("Estatus")),
								// Duda con respecto a Jornada que se repite en personal y jornada.
								Jornada = result.IsDBNull(result.GetOrdinal("Jornada")) ? 0
										: result.GetInt32(result.GetOrdinal("Jornada")),
								// Duda con respecto a Descripcion que se repite en jornada y centrocosto.
								Descripcion = result.IsDBNull(result.GetOrdinal("Descripcion")) ? ""
										: result.GetString(result.GetOrdinal("Descripcion")),
								Domingo = !result.IsDBNull(result.GetOrdinal("Domingo")) && result.GetBoolean(result.GetOrdinal("Domingo")),
								Lunes = !result.IsDBNull(result.GetOrdinal("Lunes")) && result.GetBoolean(result.GetOrdinal("Lunes")),
								Martes = !result.IsDBNull(result.GetOrdinal("Martes")) && result.GetBoolean(result.GetOrdinal("Martes")),
								Miercoles = !result.IsDBNull(result.GetOrdinal("Miercoles")) && result.GetBoolean(result.GetOrdinal("Miercoles")),
								Jueves = !result.IsDBNull(result.GetOrdinal("Jueves")) && result.GetBoolean(result.GetOrdinal("Jueves")),
								Viernes = !result.IsDBNull(result.GetOrdinal("Viernes")) && result.GetBoolean(result.GetOrdinal("Viernes")),
								Sabado = !result.IsDBNull(result.GetOrdinal("Sabado")) && result.GetBoolean(result.GetOrdinal("Sabado")),
								SaleSiguientedia = !result.IsDBNull(result.GetOrdinal("SaleSiguientedia")) && result.GetBoolean(result.GetOrdinal("SaleSiguientedia")),
								EsRotativo = !result.IsDBNull(result.GetOrdinal("EsRotativo")) && result.GetBoolean(result.GetOrdinal("EsRotativo")),
								CentroCostos = result.IsDBNull(result.GetOrdinal("CentroCostos")) ? 0
										: result.GetInt32(result.GetOrdinal("CentroCostos")),
								Grupo = result.IsDBNull(result.GetOrdinal("Grupo")) ? ""
										: result.GetString(result.GetOrdinal("Grupo"))
							};

							response.Add(resp);

							#region GuardandoEnABC

							var an = resp.Personal == 0 ?
								new Annalist()
								{
									Params = new List<Param>(),
									Sucursales = new List<Sucursal>(),
									RecAdqs = new List<RecAdq>()
								}
								: _dbStore.AnnalistSet.Find(resp.Personal);

							an.Name = resp.Name;
							an.Active = resp.Estatus == "True"; // no se con que comparar
							an.Email = resp.eMail;
							an.Phone = resp.Telefono;
							an.LastNameFather = resp.ApellidoPaterno;
							an.LastNameMother = resp.ApellidoMaterno;
							an.FechaAlta = resp.FechaAlta;
							an.Jornada = _dbStore.JornadaSet.Find(resp.Jornada);
							an.Puesto = resp.Puesto;
							an.NivelAcademico = resp.NivelAcademico;
							an.Departamento = resp.Departamento;

							// Al final de la consulta se relaciona personal (analistas) con centroCostos.
							// Nosotros tenemos relacionados analistas con sucursales, asi que podriamos
							// cambiar las sucursales en dependencia de los centroCostos que llegan aqui.
							//if (annalist.Id != 0)
							//	if (!ResetSucursalesList(annalist.Id))
							//		throw new Exception("Error al resetear la lista de sucursales");

							//if (!RefillSucursalesList(annalist, an))
							//	throw new Exception("Error al llenar la lista de sucursales");

							if (resp.Personal == 0)
								_dbStore.AnnalistSet.Add(an);

							var jor = resp.Jornada == 0 ?
								new Jornada()
								{
									Description = resp.Descripcion, // duda pq descripcion se repite
									Sunday = resp.Domingo,
									Monday = resp.Lunes,
									Tuesday = resp.Martes,
									Wednesday = resp.Miercoles,
									Thursday = resp.Jueves,
									Friday = resp.Viernes,
									Saturday = resp.Sabado,
									SaleSiguienteDia = resp.SaleSiguientedia,
									EsRotativo = resp.EsRotativo,
									Annalist = an
								}
								: _dbStore.JornadaSet.Find(resp.Jornada);

							if (resp.Jornada == 0)
								_dbStore.JornadaSet.Add(jor);

							var cc = resp.CentroCostos == 0
								? new CentroCosto()
								{
									ParameterToAnalyze = new List<Param>(),
									AreasAnaliticas = new List<AnalyticsArea>()
								}
								: _dbStore.CentroCostoSet.Find(resp.CentroCostos);

							cc.Active = resp.Estatus == "True"; // no se con que comparar, ademas de que esta repetido
							cc.Description = resp.Descripcion;
							cc.Tipo = resp.Grupo == "Mixto" ? 0 : resp.Grupo == "DeGasto" ? 1 : 2;

							if (resp.CentroCostos == 0)
								_dbStore.CentroCostoSet.Add(cc);

							_dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

							#endregion
						}
					}
					con.Close();
				}

				#endregion

				if (!response.Any())
					return Json(new { success = false, details = "Error recuperando los datos de Intelesis." });

				_dbStore.IntelesisSet.Add(new Intelesis());
				_dbStore.SaveChanges();

				return Json(new { success = true });
			}
			catch (Exception e)
			{
				return Json(new { success = false, details = e.Message });
			}
		}

	}
}
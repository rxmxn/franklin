using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using AbcPersistent.Models;
using Microsoft.Ajax.Utilities;
using WebGrease.Css.Extensions;

namespace AbcSpa.Controllers
{
	public class SheetTableController : Controller
	{
		// variable to do queries on the DB
		private readonly AbcContext _dbStore = new AbcContext();

		[HttpPost, ActionName("RefreshList")]
		public JsonResult RefreshList(int page, int pageSize, string searchSpecificKey,
			string specificDescription, string searchMethod, string searchGenericKey,
			bool viewParameters = true, bool viewGroups = true, bool viewPackages = true,
			int marketId = 0, int matrixId = 0, int sucursalId = 0,
			int clasquim1Id = 0, int clasquim2Id = 0, int clasquim3Id = 0)
		{
			try
			{
				dynamic currUsr = Session["curr_User"];
				int userId = currUsr.Id;

				int pkListCount = 0, gListCount = 0;

				var elemList = new List<dynamic>();

				//var sucursales = _dbStore.SucursalSet.Where(s => s.Active
				//	&& (userId == 0 || s.Users.Any(x => x.Id.Equals(userId)))
				//	&& s.SucursalRealizaParams.Any(p => p.Active)
				//	|| s.SucursalVendeParams.Any(p => p.Active));

				// suponiendo que en una sucursal pueden haber solamente grupos o paquetes que contengan
				// parametros de otras sucursales
				var sucursales = _dbStore.SucursalSet.Where(s => s.Active
					&& (userId == 0 || s.Users.Any(x => x.Id.Equals(userId))));

				// Filtrando por elementos que tienen que ver con las sucursales
				if (sucursalId != 0)
					sucursales = sucursales.Where(s => s.Id == sucursalId);

				if (marketId != 0)
					sucursales = sucursales.Where(s => s.Offices.Any(o => o.Market.Id == marketId));

				//IEnumerable<Sucursal> suc = sucursales as IList<Sucursal> ?? sucursales.ToList();

				// Tomando todos los parametros de las sucursales del usuario activo
				var parameters = _dbStore.ParamSet.Where(p => p.Active &&
							sucursales.Any(s => (p.SucursalRealiza != null
											&& s.Id.Equals(p.SucursalRealiza.Id))
											|| (p.SucursalVende != null
											&& s.Id.Equals(p.SucursalVende.Id))));

				// No se debe filtrar en parameters pq grupos y paquetes dependen tambien de esa
				// variable para saber si estan o no en una de las sucursales del usuario,
				// y puede ser q el grupo o el paquete tengan matrices diferentes a los parametros 
				// que contienen.
				var parametros = parameters.Where(p => ((matrixId == 0) ||
								p.Matrixes.Any(m => m.Id.Equals(matrixId))) &&
								((clasquim1Id == 0) ||
								p.BaseParam.ClasificacionQuimica1.Id.Equals(clasquim1Id)) &&
								((clasquim2Id == 0) ||
								p.BaseParam.ClasificacionQuimica2.Id.Equals(clasquim2Id)) &&
								((clasquim3Id == 0) ||
								p.BaseParam.ClasificacionQuimica3.Id.Equals(clasquim3Id)) &&
								!p.Packages.Any() && !p.Groups.Any() &&
								(string.IsNullOrEmpty(searchSpecificKey) || p.ParamUniquekey.ToUpper().Contains(searchSpecificKey.ToUpper())) &&
								(string.IsNullOrEmpty(specificDescription) || p.Description.ToUpper().Contains(specificDescription.ToUpper())) &&
								(string.IsNullOrEmpty(searchMethod) || p.Metodo.Name.ToUpper().Contains(searchMethod.ToUpper())) /*&&
								(string.IsNullOrEmpty(searchGenericKey) || p.GenericKey.ToUpper().Contains(searchGenericKey.ToUpper()))*/);

				var paramCount = viewParameters ? parametros.Count() : 0;

				// Tomando todos los grupos que tengan alguno de los parametros anteriores
				//var groups = _dbStore.GroupSet.Where(g => g.Active &&
				//			g.Parameters.Any(p => parameters.Any(p1 => p1.Id.Equals(p.Id))) &&
				//			((matrixId == 0) || g.Matrixes.Any(m => m.Id.Equals(matrixId))) &&
				//			((clasquim1Id == 0) ||
				//			g.ClasificacionQuimica1.Id.Equals(clasquim1Id)) &&
				//			((clasquim2Id == 0) ||
				//			g.ClasificacionQuimica2.Id.Equals(clasquim2Id)) &&
				//			((clasquim3Id == 0) ||
				//			g.ClasificacionQuimica3.Id.Equals(clasquim3Id)));

				// Tomando todos los grupos que tengan su sucursal entre las sucursales del usuario
				var groups = _dbStore.GroupSet.Where(g => g.Active &&
					sucursales.Any(s => (g.Sucursal != null && s.Id.Equals(g.Sucursal.Id))) &&
					((matrixId == 0) || g.Matrixes.Any(m => m.Id.Equals(matrixId))) &&
					((clasquim1Id == 0) ||
					(g.ClasificacionQuimica1.Id.Equals(clasquim1Id) || g.Parameters.Any(p => p.BaseParam.ClasificacionQuimica1.Id.Equals(clasquim1Id)))) 
					&& ((clasquim2Id == 0) ||
					(g.ClasificacionQuimica2.Id.Equals(clasquim2Id) || g.Parameters.Any(p => p.BaseParam.ClasificacionQuimica2.Id.Equals(clasquim2Id)))) &&
					((clasquim3Id == 0) ||
					(g.ClasificacionQuimica3.Id.Equals(clasquim3Id) || g.Parameters.Any(p => p.BaseParam.ClasificacionQuimica3.Id.Equals(clasquim3Id)))) &&
					!g.Packages.Any() &&
					(string.IsNullOrEmpty(searchSpecificKey) || g.Name.ToUpper().Contains(searchSpecificKey.ToUpper()) ||
					g.Parameters.Any(p => p.Active && p.ParamUniquekey.ToUpper().Contains(searchSpecificKey.ToUpper()))) &&
					(string.IsNullOrEmpty(specificDescription) || g.Description.ToUpper().Contains(specificDescription.ToUpper()) ||
					g.Parameters.Any(p => p.Description.ToUpper().Contains(specificDescription.ToUpper()))) &&
					(string.IsNullOrEmpty(searchMethod) || g.Parameters.Any(p => p.Active && p.Metodo.Name.ToUpper().Contains(searchMethod.ToUpper()))) /*&&
					(string.IsNullOrEmpty(searchGenericKey) || g.Parameters.Any(p => p.Active && p.GenericKey.ToUpper().Contains(searchGenericKey.ToUpper())))*/);

				var groupCount = viewGroups ? groups.Count() : 0;

				// Tomando todos los paquetes que tengan alguno de los parametros o grupos anteriores
				//var packages = _dbStore.PackageSet.Where(pk => pk.Active &&
				//			(pk.Parameters.Any(p => parameters.Any(p1 => p1.Id.Equals(p.Id)))
				//			|| pk.Groups.Any(g => groups.Any(g1 => g1.Id.Equals(g.Id)))) &&
				//			((matrixId == 0) || pk.Matrixes.Any(m => m.Id.Equals(matrixId))));

				// Tomando todos los paquetes que tengan su sucursal entre las sucursales del usuario
				var packages = _dbStore.PackageSet.Where(pk => pk.Active &&
							sucursales.Any(s => (pk.Sucursal != null && s.Id.Equals(pk.Sucursal.Id))) &&
							(matrixId == 0 || pk.Matrixes.Any(m => m.Id.Equals(matrixId))) &&
							(string.IsNullOrEmpty(searchSpecificKey) || pk.Name.ToUpper().Contains(searchSpecificKey.ToUpper()) ||
							pk.Groups.Any(g => g.Active && (g.Name.ToUpper().Contains(searchSpecificKey.ToUpper()) ||
							g.Parameters.Any(p => p.Active && p.ParamUniquekey.ToUpper().Contains(searchSpecificKey.ToUpper())))) ||
							pk.Parameters.Any(p => p.Active && p.ParamUniquekey.ToUpper().Contains(searchSpecificKey.ToUpper()))) &&
							(string.IsNullOrEmpty(specificDescription) || pk.Description.ToUpper().Contains(specificDescription.ToUpper()) ||
							pk.Groups.Any(g => g.Active && (g.Description.ToUpper().Contains(specificDescription.ToUpper()) ||
							g.Parameters.Any(p => p.Active && p.Description.ToUpper().Contains(specificDescription.ToUpper())))) ||
							pk.Parameters.Any(p => p.Active && p.Description.ToUpper().Contains(specificDescription.ToUpper()))) &&
							((clasquim1Id == 0) ||
							pk.Parameters.Any(p => p.BaseParam.ClasificacionQuimica1.Id.Equals(clasquim1Id)) ||
                            pk.Groups.Any(g => g.ClasificacionQuimica1.Id.Equals(clasquim1Id) || g.Parameters.Any(p => p.BaseParam.ClasificacionQuimica1.Id.Equals(clasquim1Id))))
							&& ((clasquim2Id == 0) ||
							pk.Parameters.Any(p => p.BaseParam.ClasificacionQuimica2.Id.Equals(clasquim2Id)) ||
							pk.Groups.Any(g => g.ClasificacionQuimica2.Id.Equals(clasquim2Id) || g.Parameters.Any(p => p.BaseParam.ClasificacionQuimica2.Id.Equals(clasquim2Id)))) &&
							((clasquim3Id == 0) ||
							pk.Parameters.Any(p => p.BaseParam.ClasificacionQuimica3.Id.Equals(clasquim3Id)) ||
							pk.Groups.Any(g => g.ClasificacionQuimica3.Id.Equals(clasquim3Id) || g.Parameters.Any(p => p.BaseParam.ClasificacionQuimica3.Id.Equals(clasquim3Id)))) &&
                            (string.IsNullOrEmpty(searchMethod) || pk.Groups.Any(g => g.Active && (g.Parameters.Any(p => p.Active && p.Metodo.Name.ToUpper().Contains(searchMethod.ToUpper())))) ||
							pk.Parameters.Any(p => p.Active && p.Metodo.Name.ToUpper().Contains(searchMethod.ToUpper()))) /*&&
							(string.IsNullOrEmpty(searchGenericKey) || pk.Groups.Any(g => g.Active && g.Parameters.Any(p => p.Active && p.GenericKey.ToUpper().Contains(searchGenericKey.ToUpper()))) ||
							pk.Parameters.Any(p => p.Active && p.GenericKey.ToUpper().Contains(searchGenericKey.ToUpper()))) */
							);

				var packageCount = viewPackages ? packages.Count() : 0;

				// Armando las Listas para enviar al cliente
				int skip = (page - 1) * pageSize;

				if (packageCount - skip > 0 && viewPackages)
				{
					var packageList = packages.ToList()
							.Select(pk => pk.ToJson(0, true))
							.Skip(skip).Take(pageSize);

					var pkgs = packageList as dynamic[] ?? packageList.ToArray();

					if (pkgs.Any())
						elemList.AddRange(pkgs);

					pkListCount = pkgs.Count();
				}

				int skipGroup = skip - packageCount < 0 ? 0 : skip - packageCount;
				int takeGroup = pageSize - pkListCount;

				if (takeGroup > 0 && viewGroups && groupCount > 0)
				{
					var groupList = groups.ToList()
							.Select(g => g.ToJson(0, true))
							.Skip(skipGroup).Take(takeGroup);

					var grps = groupList as dynamic[] ?? groupList.ToArray();

					if (grps.Any())
						elemList.AddRange(grps);

					gListCount = grps.Count();
				}

				int skipParam = skip - packageCount - groupCount < 0 ? 0 : skip - packageCount - groupCount;
				int takeParam = pageSize - pkListCount - gListCount;

				// Inverted if to improve performance
				if (takeParam <= 0 || !viewParameters)
					return Json(new
					{
						success = true,
						elements = elemList,
						total = (viewPackages ? packageCount : 0)
								+ (viewGroups ? groupCount : 0)
								+ (viewParameters ? paramCount : 0)
					});

				var paramList = parametros.ToList()
							.Select(p => p.ToJson(sheet: true))
							.Skip(skipParam).Take(takeParam);

				var prms = paramList as dynamic[] ?? paramList.ToArray();

				if (prms.Any())
					elemList.AddRange(prms);
				// 1.875ms
				return Json(new
				{
					success = true,
					elements = elemList,
					total = (viewPackages ? packageCount : 0)
							+ (viewGroups ? groupCount : 0)
							+ paramCount
				});
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

		[HttpPost, ActionName("GetSpecificKeySug")]
		public JsonResult GetSpecificKeySug(string suggestSpecificKey, string specificDescription, string searchMethod, string searchGenericKey)
		{
			try
			{
				dynamic currUsr = Session["curr_User"];
				int userId = currUsr.Id;

				var sucursales = User.Identity.Name == "root"
					? _dbStore.SucursalSet.Where(s => s.Active)
					: _dbStore.SucursalSet.Where(s => s.Active &&
								s.Users.Any(u => u.Id.Equals(userId)));

				var paramKeys = _dbStore.ParamSet.Where(p => p.Active &&
							  sucursales.Any(s => (p.SucursalRealiza != null
									&& s.Id.Equals(p.SucursalRealiza.Id))
									|| (p.SucursalVende != null
									&& s.Id.Equals(p.SucursalVende.Id))) &&
									(string.IsNullOrEmpty(specificDescription) 
									|| p.Description.ToUpper().Contains(specificDescription.ToUpper())) &&
									(string.IsNullOrEmpty(suggestSpecificKey) 
									|| p.ParamUniquekey.ToUpper().Contains(suggestSpecificKey.ToUpper())) &&
									(string.IsNullOrEmpty(searchMethod) 
									|| p.Metodo.Name.ToUpper().Contains(searchMethod.ToUpper())) /*&&
									(string.IsNullOrEmpty(searchGenericKey) 
									|| p.GenericKey.ToUpper().Contains(searchGenericKey.ToUpper()))*/)
									.Select(p => new
									{
										key = p.ParamUniquekey,
										metodo = p.Metodo.Name,
										//p.GenericKey,
										p.Description
									});

				var groupKeys = _dbStore.GroupSet.Where(g => g.Active && g.Parameters.Any(p => p.Active &&
																							sucursales.Any(s => (p.SucursalRealiza != null
																												 && s.Id.Equals(p.SucursalRealiza.Id))
																												 || (p.SucursalVende != null
																												 && s.Id.Equals(p.SucursalVende.Id)))) &&
																												 (string.IsNullOrEmpty(specificDescription) || g.Description.ToUpper().Contains(specificDescription.ToUpper())) &&
																												 (string.IsNullOrEmpty(suggestSpecificKey) || g.Name.ToUpper().Contains(suggestSpecificKey.ToUpper()))).Select(g => new { key = g.Name, g.Description });

				var pktKeys = _dbStore.PackageSet.Where(pk => pk.Active && (pk.Parameters.Any(p => p.Active &&
																							sucursales.Any(s => (p.SucursalRealiza != null
																												 && s.Id.Equals(p.SucursalRealiza.Id))
																												 || (p.SucursalVende != null
																												 && s.Id.Equals(p.SucursalVende.Id)))) ||
																			pk.Groups.Any(g => g.Active && g.Parameters.Any(p => p.Active &&
																							sucursales.Any(s => (p.SucursalRealiza != null
																												 && s.Id.Equals(p.SucursalRealiza.Id))
																												 || (p.SucursalVende != null
																												 && s.Id.Equals(p.SucursalVende.Id)))))) &&
																												 (string.IsNullOrEmpty(specificDescription) || pk.Description.ToUpper().Contains(specificDescription.ToUpper())) &&
																												 (string.IsNullOrEmpty(suggestSpecificKey) || pk.Name.ToUpper().Contains(suggestSpecificKey.ToUpper()))).Select(pk => new { key = pk.Name, pk.Description });

				var suggestedSpecificKeys = paramKeys.Select(p => p.key).Union(groupKeys.Select(g => g.key)).Union(pktKeys.Select(pk => pk.key)).Distinct().Take(10);
				var suggestedSpecificDescription = paramKeys.Select(p => p.Description).Union(groupKeys.Select(g => g.Description)).Union(pktKeys.Select(pk => pk.Description)).Distinct().Take(10);
				var suggestedMethods = paramKeys.Select(p => p.metodo).Distinct().Take(10);
				//var suggestedGenericKeyList = paramKeys.Select(p => p.GenericKey).Distinct().Take(10);

				return Json(new { success = true, suggestedSpecificKeys, suggestedSpecificDescription, suggestedMethods/*, suggestedGenericKeyList*/ });
			}
			catch (Exception e)
			{
				return Json(new { success = false, details = e.Message });
			}
		}

	}
}

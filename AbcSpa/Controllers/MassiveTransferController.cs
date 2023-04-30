using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AbcPersistent.Models;

namespace AbcSpa.Controllers
{
	public class MassiveTransferController : Controller
	{
		// variable to do queries on the DB
		private readonly AbcContext _dbStore = new AbcContext();

		[HttpPost, ActionName("TransferElement")]
		public JsonResult TransferElement(int suc1, IEnumerable<int> s1Pkgs, IEnumerable<int> s1Grps, 
			IEnumerable<int> s1Prms, int suc2, IEnumerable<int> s2Pkgs, IEnumerable<int> s2Grps, 
			IEnumerable<int> s2Prms)
		{
			try
			{
				var sucursal1 = _dbStore.SucursalSet.Find(suc1);
				var sucursal2 = _dbStore.SucursalSet.Find(suc2);

				// De suc1 a suc2
				if (s1Pkgs != null)
					foreach (var pkg in s1Pkgs.Select(pk => _dbStore.PackageSet.Find(pk)))
						pkg.Sucursal = sucursal2;

				if (s1Grps != null)
					foreach (var grp in s1Grps.Select(g => _dbStore.GroupSet.Find(g)))
					{
						if (grp.Packages.Any())
						{
							SaveGroup(grp, sucursal2);
						}
						else
						{
							grp.Sucursal = sucursal2;
						}
					}
						
				if (s1Prms != null)
					foreach (var prm in s1Prms.Select(p => _dbStore.ParamSet.Find(p)))
					{
						if (prm.Packages.Any() || prm.Groups.Any())
						{
							var newprm = prm;
							newprm.Packages.Clear();
							newprm.Groups.Clear();
							newprm.SucursalVende = prm.SucursalVende != null ? sucursal2 : null;
							newprm.SucursalRealiza = prm.SucursalRealiza != null ? sucursal2 : null;

							_dbStore.ParamSet.Add(newprm);
						}
						else
						{
							prm.SucursalVende = prm.SucursalVende != null ? sucursal2 : null;
							prm.SucursalRealiza = prm.SucursalRealiza != null ? sucursal2 : null;
						}
					}

				// De suc2 a suc1
				if (s2Pkgs != null)
					foreach (var pkg in s2Pkgs.Select(pk => _dbStore.PackageSet.Find(pk)))
						pkg.Sucursal = sucursal1;

				if (s2Grps != null)
					foreach (var grp in s2Grps.Select(g => _dbStore.GroupSet.Find(g)))
					{
						if (grp.Packages.Any())
						{
							SaveGroup(grp, sucursal1);
						}
						else
						{
							grp.Sucursal = sucursal1;
						}
					}

				if (s2Prms != null)
					foreach (var prm in s2Prms.Select(p => _dbStore.ParamSet.Find(p)))
					{
						if (prm.Packages.Any() || prm.Groups.Any())
						{
							var newprm2 = prm;
							newprm2.Packages.Clear();
							newprm2.Groups.Clear();
							newprm2.SucursalVende = prm.SucursalVende != null ? sucursal1 : null;
							newprm2.SucursalRealiza = prm.SucursalRealiza != null ? sucursal1 : null;

							_dbStore.ParamSet.Add(newprm2);
						}
						else
						{
							prm.SucursalVende = prm.SucursalVende != null ? sucursal1 : null;
							prm.SucursalRealiza = prm.SucursalRealiza != null ? sucursal1 : null;
						}
					}

				_dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

				return Json(new { success = true });
			}
			catch (Exception e)
			{
				return Json(new { success = false, details = e.Message });
			}
		}

		// Esta funcion es necesaria para grupos pq es un elemento muy complejo.
		private void SaveGroup(Group grp, Sucursal sucursal)
		{
			var newgrp = new Group()
			{
				Parameters = new List<Param>(),
				ParamRoutes = new List<ParamRoute>(),
				ParamPrintResults = new List<ParamPrintResult>(),
				//ComplexSamplings = new List<ComplexSampling>(),
				Matrixes = new List<Matrix>(),
				Name = grp.Name,
				Description = grp.Description,
				Active = grp.Active,
				PublishInAutolab = grp.PublishInAutolab,
				SellSeparated = grp.SellSeparated,
				CuentaEstadistica = grp.CuentaEstadistica,
				MostrarLista = grp.MostrarLista,
				Week = grp.Week,
				//DecimalesReporte = grp.DecimalesReporte,
				TipoServicio = (grp.TipoServicio != null)
									? _dbStore.TipoServicioSet.Find(grp.TipoServicio.Id)
									: null,
				ClasificacionQuimica1 = grp.ClasificacionQuimica1 != null ?
											  _dbStore.ClasificacionQuimicaSet
											  .Find(grp.ClasificacionQuimica1.Id) : null,
				ClasificacionQuimica2 = grp.ClasificacionQuimica2 != null ?
											  _dbStore.ClasificacionQuimicaSet
											  .Find(grp.ClasificacionQuimica2.Id) : null,
				ClasificacionQuimica3 = grp.ClasificacionQuimica3 != null ?
											  _dbStore.ClasificacionQuimicaSet
											  .Find(grp.ClasificacionQuimica3.Id) : null
			};

			// adding complex samplings
			//foreach (var csampling in grp.ComplexSamplings.Select(complex => new ComplexSampling
			//{
			//	Param = _dbStore.ParamSet.Find(complex.Param.Id),
			//	CantidadMuestreos = complex.CantidadMuestreos
			//}))
			//{
			//	_dbStore.ComplexSamplingSet.Add(csampling);

			//	_dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

			//	if (grp.ComplexSamplings.All(c => c.Id != csampling.Id))
			//		newgrp.ComplexSamplings.Add(_dbStore.ComplexSamplingSet.Find(csampling.Id));
			//}

			//newgrp.MuestreosCompuestos = newgrp.ComplexSamplings.Any();
			// end adding complex samplings

			newgrp.DispParamId = grp.DispParamId;

			foreach (var pa in grp.Parameters.ToList())
				newgrp.Parameters.Add(_dbStore.ParamSet.Find(pa.Id));

			foreach (var g in grp.Matrixes.ToList())
				newgrp.Matrixes.Add(_dbStore.MatrixSet.Find(g.Id));

			newgrp.Sucursal = sucursal;

			_dbStore.GroupSet.Add(newgrp);
			_dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

			foreach (var prr in grp.ParamPrintResults.ToList())
			{
				_dbStore.ParamPrintResultSet.Add(new ParamPrintResult()
				{
					Yes = prr.Yes,
					Parameter = _dbStore.ParamSet.Find(prr.Parameter.Id),
					Group = _dbStore.GroupSet.Find(newgrp.Id)
				});
			}
			_dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
		}
		
		[HttpPost, ActionName("RefreshList")]
		public JsonResult RefreshList(int page, int pageSize,
			bool viewParameters = true, bool viewGroups = true, bool viewPackages = true,
			int sucursalId = 0)
		{
			try
			{
				int pkListCount = 0, gListCount = 0;

				var elemList = new List<dynamic>();

				//var sucursales = _dbStore.SucursalSet.Where(s => s.Active
				//	&& s.SucursalRealizaParams.Any(p => p.Active)
				//	|| s.SucursalVendeParams.Any(p => p.Active));
				
				// suponiendo que en una sucursal pueden haber solamente grupos o paquetes que contengan
				// parametros de otras sucursales
				var sucursales = _dbStore.SucursalSet.Where(s => s.Active);

				// Filtrando por elementos que tienen que ver con las sucursales
				if (sucursalId != 0)
					sucursales = sucursales.Where(s => s.Id == sucursalId);
				
				// Tomando todos los parametros de la sucursal seleccionada
				var parameters = _dbStore.ParamSet.Where(p => p.Active &&
							sucursales.Any(s => (p.SucursalRealiza != null
											&& s.Id.Equals(p.SucursalRealiza.Id))
											|| (p.SucursalVende != null
											&& s.Id.Equals(p.SucursalVende.Id)))
											&& !p.Packages.Any() && !p.Groups.Any());
				
				var paramCount = viewParameters ? parameters.Count() : 0;
				
				var groups = _dbStore.GroupSet.Where(g => g.Active &&
							sucursales.Any(s => g.Sucursal != null && s.Id.Equals(g.Sucursal.Id))
							&& !g.Packages.Any());

				var groupCount = viewGroups ? groups.Count() : 0;
				
				var packages = _dbStore.PackageSet.Where(pk => pk.Active &&
							sucursales.Any(s => pk.Sucursal != null && s.Id.Equals(pk.Sucursal.Id)));
				
				var packageCount = viewPackages ? packages.Count() : 0;

				// Armando las Listas para enviar al cliente
				var skip = (page - 1) * pageSize;

				if (packageCount - skip > 0 && viewPackages)
				{
					var packageList = packages.ToList()
							.Select(pk => pk.ToMainInfoJson())
							.Skip(skip).Take(pageSize);

					var pkgs = packageList as dynamic[] ?? packageList.ToArray();

					if (pkgs.Any())
						elemList.AddRange(pkgs);

					pkListCount = pkgs.Count();
				}

				var skipGroup = skip - packageCount < 0 ? 0 : skip - packageCount;
				var takeGroup = pageSize - pkListCount;

				if (takeGroup > 0 && viewGroups && groupCount > 0)
				{
					var groupList = groups.ToList()
							.Select(g => g.ToMainInfoJson())
							.Skip(skipGroup).Take(takeGroup);

					var grps = groupList as dynamic[] ?? groupList.ToArray();

					if (grps.Any())
						elemList.AddRange(grps);

					gListCount = grps.Count();
				}

				var skipParam = skip - packageCount - groupCount < 0 ? 0 : skip - packageCount - groupCount;
				var takeParam = pageSize - pkListCount - gListCount;

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

				var paramList = parameters.ToList()
							.Select(p => p.ToMainInfoJson())
							.Skip(skipParam).Take(takeParam);

				var prms = paramList as dynamic[] ?? paramList.ToArray();

				if (prms.Any())
					elemList.AddRange(prms);
				
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
	}
}
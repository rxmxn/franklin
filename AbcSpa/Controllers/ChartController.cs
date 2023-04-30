using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using AbcPersistent.Models;
using Microsoft.Ajax.Utilities;

namespace AbcSpa.Controllers
{
	public class ChartController : Controller
	{
		// variable to do queries on the DB
		private readonly AbcContext _dbStore = new AbcContext();

		// Charts
		[HttpPost, ActionName("TotalMethodsAcknowledged")]
		public JsonResult TotalMethodsAcknowledged(string regionName)
		{
			try
			{
				// Metodos Reconocidos
				var methods = _dbStore.MethodSet.Where(m => m.Active && m.Parameters.Any(p => p.Active &&
							((regionName == null) || p.SucursalRealiza.Region.Name.Equals(regionName) ||
							p.SucursalVende.Region.Name.Equals(regionName)) &&
							p.RecOtorgs.Any(ro => ro.Enterprise != null)));

				// Aqui se podria pensar que hay un error entre la cantidad de reconocimientos que se muestran
				// y los que se enumeran mas abajo en la columna de reconocimientos.
				// Esto esta dado debido a que la cantidad de reconocimientos se esta calculando en dependencia
				// de las empresas/instituciones que reconocen el metodo, pero en la columna se muestran solamente los 
				// reconocimientos (Acks) de ese metodo, y varias empresas/instituciones pudieran tener el mismo Ack.

				var data = methods.Select(m => new
				{
					label = m.Name,
					value = m.Parameters.Where(p => p.Active &&
					((regionName == null) || p.SucursalRealiza.Region.Name.Equals(regionName) ||
						p.SucursalVende.Region.Name.Equals(regionName)))
								.Sum(p => p.RecOtorgs.Where(ro => ro.Enterprise != null)
								.Select(ro => ro.Enterprise.Id).Distinct().Count()),
					info = m.Parameters.Where(p => p.Active &&
						((regionName == null) || p.SucursalRealiza.Region.Name.Equals(regionName) ||
						p.SucursalVende.Region.Name.Equals(regionName)) &&
						p.RecOtorgs.Any(ro => ro.Enterprise != null))
							.Select(x => new
							{
								Param = x.ParamUniquekey,
								Annalists = x.Annalists/*.Where(a => a.Active && a.AnnalistKey!=null)*/.Select(a => a.Key/*a.AnnalistKey.Clave*/),
								// ReSharper disable once MergeConditionalExpression
								SucursalRealiza = x.SucursalRealiza == null ? null : x.SucursalRealiza.Name,
								// ReSharper disable once MergeConditionalExpression
								SucursalVende = x.SucursalVende == null ? null : x.SucursalVende.Name,
								RecOtorgs = x.RecOtorgs.Where(ro => ro.Enterprise != null)
										.Select(ro => ro.Ack.Key).Distinct()
							})

				});
				// 13ms region == null
				// 2ms region != null
				return Json(new { success = true, data });
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

		[HttpPost, ActionName("MethodsAckPerEnterprise")]
		public JsonResult MethodsAckPerEnterprise(string methodName)
		{
			try
			{
				var methods = _dbStore.MethodSet.Where(m => m.Active &&
									m.Parameters.Any(p => p.Active)
									&& ((methodName == null) || m.Name.Equals(methodName)));
				// Lo quite de la consulta para evitar algunos ToList
				// (p.SucursalRealiza != null) || (p.SucursalVende != null)
				// Anyways, para crear un parametro es obligatorio ponerle una Sucursal.
				// Aqui por ejemplo, no preguntamos si tiene ParamBase, ya que es obligatorio.

				var data = _dbStore.EnterpriseSet.Where(e => e.Active && e.RecOtorgs.Any() &&
								methods.Any(m => m.Parameters
								.Any(p => p.RecOtorgs.Any(w => w.Enterprise.Id == e.Id) && p.Active)))
					.Select(x => new
					{
						label = x.Name,
						value = methodName == null ? 
						methods.Count(m => m.Parameters
								.Any(p => p.RecOtorgs.Any(w => w.Enterprise.Id == x.Id) && p.Active)) :
                        methods.Sum(m => m.Parameters
								.Count(p => p.RecOtorgs.Any(w => w.Enterprise.Id == x.Id) && p.Active)),
						info = methods.Where(m => m.Parameters
						.Any(p => p.RecOtorgs.Any(w => w.Enterprise.Id == x.Id) && p.Active))
						.Select(m => new
						{
                            Method = m.Name,
							Param = m.Parameters.Where(p => (p.RecOtorgs.Count > 0) && p.Active &&
															(p.RecOtorgs.Count(pe => pe.Enterprise.Id == x.Id) > 0))
									.Select(y => new
									{
										Name = y.ParamUniquekey,
										// ReSharper disable once MergeConditionalExpression
										Analistas = y.Annalists/*.Where(a=>a.AnnalistKey!=null)*/.Select(a => a.Key/*a.AnnalistKey.Clave*/),
										// ReSharper disable once MergeConditionalExpression
										SucursalRealiza = y.SucursalRealiza != null ? y.SucursalRealiza.Name : null,
										// ReSharper disable once MergeConditionalExpression
										SucursalVende = y.SucursalVende != null ? y.SucursalVende.Name : null,
										RecOtorgs = y.RecOtorgs.Where(w => w.Enterprise.Id == x.Id).Select(ro => ro.Ack.Key).Distinct()
									})
						})
					});

				// 29ms (methodName != null)
				// 2ms (methodName == null)
				return Json(new { success = true, data });
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

		[HttpPost, ActionName("SignatariosPerEnterprise")]
		public JsonResult SignatariosPerEnterprise()
		{
			try
			{
				var annalist = _dbStore.AnnalistSet.Where(a => a.Active &&
								a.RecAdqs.Any(ra => ra.TipoSignatario != null));

				var data = _dbStore.EnterpriseSet.Where(e => e.RecOtorgs.Any() && e.Active)
					.Select(x => new
					{
						label = x.Name,
						value = annalist.Count(p => p.RecAdqs.Any(w => w.RecOtorgs.Any(r => r.Enterprise.Id.Equals(x.Id))) && p.Active),
						info = annalist.Where(p => p.RecAdqs.Any(w => w.RecOtorgs.Any(r => r.Enterprise.Id.Equals(x.Id))) && p.Active)
							.Select(a => new
							{
								Name = a.Name + " " + a.LastNameFather + " " + a.LastNameMother,
								a.Photo,
								a.Gender,
								acks = a.RecAdqs.SelectMany(z => z.RecOtorgs
									.Where(r => r.Enterprise != null)
									.Select(r => r.Ack.Key)).Distinct(),
								sucursales = a.Sucursales.Select(z => z.Name + "/" + z.Region.Name)
							})
					});
				// 2ms
				return Json(new { success = true, data });
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

		// Metodos reconocidos por region
		[HttpPost, ActionName("MethodAckPerRegion")]
		public JsonResult MethodAckPerRegion(string ackName)
		{
			try
			{
				//1->Buscar metodos activos.
				//2->Buscar los parametros que los activan.
				//3->Buscar las matrices de esos parametros.
				//4->Buscar las sucursales de esas matrices.
				//5->Coger las regiones diferentes.

				var methods = _dbStore.MethodSet.Where(e => e.Active &&
					e.Parameters.Any(a => a.Active &&
					((ackName == null) || a.RecOtorgs.Any(w => w.Enterprise.Name.Equals(ackName)))));

				var data = _dbStore.RegionSet.Where(r => r.Active && methods.Any(m => m.Parameters
										.Where(p => p.RecOtorgs.Any(ro => ro.Enterprise != null) &&
												p.Active)
										.Any(p => p.SucursalRealiza.Region.Id == r.Id ||
										p.SucursalVende.Region.Id == r.Id)))
							.Select(r => new
							{
								label = r.Name,
								value = methods.Count(m => m.Parameters
										.Where(p => p.RecOtorgs.Any(ro => ro.Enterprise != null) &&
												p.Active)
										.Any(p => p.SucursalRealiza.Region.Id == r.Id ||
										p.SucursalVende.Region.Id == r.Id)),
								info = methods.Where(m => m.Parameters.Any(p => p.Active &&
												p.RecOtorgs.Any(ro => ro.Enterprise != null)
												&& (p.SucursalRealiza.Region.Id == r.Id ||
												p.SucursalVende.Region.Id == r.Id)))
									.Select(m => new
									{
										Method = m.Name,
										Param = m.Parameters.Where(p => p.Active &&
											p.RecOtorgs.Any(ro => ro.Enterprise != null)
											&& (p.SucursalRealiza.Region.Id == r.Id ||
											p.SucursalVende.Region.Id == r.Id))
											.Select(y => new
											{
												Param = y.ParamUniquekey,
												// ReSharper disable once MergeConditionalExpression
												Analistas = y.Annalists/*.Where(a=>a.AnnalistKey!=null)*/.Select(a => a.Key/*a.AnnalistKey.Clave*/),
												// ReSharper disable once MergeConditionalExpression
												SucursalRealiza = y.SucursalRealiza != null ? y.SucursalRealiza.Name : null,
												// ReSharper disable once MergeConditionalExpression
												SucursalVende = y.SucursalVende != null ? y.SucursalVende.Name : null,
												acks = y.RecOtorgs.Where(ro => ro.Enterprise != null)
													.Select(ro => ro.Ack.Key).Distinct()
											})
									})
							});
				// 4ms ackname == null
				// 2ms ackname != null
				return Json(new { success = true, data });
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

		// Signatarios por region 
		[HttpPost, ActionName("SignatariosPerRegion")]
		public JsonResult SignatariosPerRegion(string ackName)
		{
			try
			{
				//1->Buscar signatarios activos.
				//2->Buscar las sucursales de esos signatarios.
				//3->Coger las regiones diferentes.

				var annalist = _dbStore.AnnalistSet.Where(a => a.Active &&
						a.RecAdqs.Any(ra => ra.RecOtorgs.Any(ro => (ackName == null) ||
							ro.Enterprise.Name.Equals(ackName))));

				var data = _dbStore.RegionSet.Where(e => e.Active).ToList()
							.Select(r => new
							{
								label = r.Name,
								value = annalist.Count(p => p.Sucursales
									.Any(w => w.Region.Id == r.Id) && p.Active),
								info = annalist.ToList().Where(p => p.Sucursales
									.Any(w => w.Region.Id == r.Id) && p.Active)
									.Select(m => new
									{
										Name = m.Name + " " + m.LastNameFather + " " + m.LastNameMother,
										m.Photo,
										m.Gender,
										acks = m.RecAdqs.SelectMany(ra => ra.RecOtorgs
											.Where(ro => ro.Enterprise != null)
											.Select(ro => ro.Ack.Key)).Distinct(),
										sucursales = m.Sucursales
											.Select(z => z.Name + "/" + z.Region.Name)
									})
							});
				// 5ms
				return Json(new { success = true, data });
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

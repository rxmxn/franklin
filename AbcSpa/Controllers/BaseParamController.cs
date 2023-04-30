using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using AbcPersistent.Models;

namespace AbcSpa.Controllers
{
	public class BaseParamController : Controller
	{
		// variable to do queries on the DB
		private readonly AbcContext _dbStore = new AbcContext();

		[HttpPost, ActionName("SaveBaseParam")]
		public JsonResult SaveBaseParam(BaseParam baseparam)
		{
			try
			{
				var baseP = baseparam.Id == 0 ?
					new BaseParam()
					{
						Units = new List<MeasureUnit>(),
						Parameters = new List<Param>(),
						Matrixes = new List<Matrix>()
					} : _dbStore.BaseParamSet.Find(baseparam.Id);

				baseP.Name = baseparam.Name;
				baseP.Active = baseparam.Active;
				baseP.Description = baseparam.Description;

				baseP.ClasificacionQuimica1 = baseparam.ClasificacionQuimica1 != null ?
											  _dbStore.ClasificacionQuimicaSet
											  .Find(baseparam.ClasificacionQuimica1.Id) : null;

				baseP.ClasificacionQuimica2 = baseparam.ClasificacionQuimica2 != null ?
											  _dbStore.ClasificacionQuimicaSet
											  .Find(baseparam.ClasificacionQuimica2.Id) : null;

				baseP.ClasificacionQuimica3 = baseparam.ClasificacionQuimica3 != null ?
											  _dbStore.ClasificacionQuimicaSet
											  .Find(baseparam.ClasificacionQuimica3.Id) : null;

				if (!CheckMeasureUnitList(baseP, baseparam))
					throw new Exception("Error al llenar la lista de Unidades de Medida");

				if (!checkMatrixes(baseP, baseparam))
					throw new Exception("Error al llenar la lista de Matrices");

				if (baseparam.Id == 0)
					_dbStore.BaseParamSet.Add(baseP);

				_dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

				return Json(new { success = true });
			}
			catch (Exception e)
			{
				return Json(new { success = false, details = e.Message });
			}
		}

		private bool CheckMeasureUnitList(BaseParam baseP, BaseParam baseparam)
		{
			try
			{
				var unittodelete = baseP.Units.Where(u => baseparam.Units.All(m => m.Id != u.Id) && u.Active);

				foreach (var ud in unittodelete.ToList())
					baseP.Units.Remove(_dbStore.MeasureUnitSet.Find(ud.Id));

				var unittoadd = baseparam.Units.Where(u => baseP.Units.All(m => m.Id != u.Id));

				foreach (var ua in unittoadd.ToList())
					baseP.Units.Add(_dbStore.MeasureUnitSet.Find(ua.Id));

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private bool checkMatrixes(BaseParam baseP, BaseParam baseparam)
		{
			try
			{
				var matrixestodelete = baseP.Matrixes.Where(mtrx => baseparam.Matrixes.All(m => m.Id != mtrx.Id));
				foreach (var pr in matrixestodelete.ToList())
					baseP.Matrixes.Remove(_dbStore.MatrixSet.Find(pr.Id));

				var matrixestoadd = baseparam.Matrixes.Where(m => baseP.Matrixes.All(mtrx => mtrx.Id != m.Id));

				foreach (var g in matrixestoadd.ToList())
					baseP.Matrixes.Add(_dbStore.MatrixSet.Find(g.Id));

				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}

		[HttpPost, ActionName("RefreshList")]
		public JsonResult RefreshList(int page, int pageSize, string searchGeneral, int? CQ1Id,
									  int? CQ2Id, int? CQ3Id, bool alta = true, bool baja = false)
		{
			try
			{
				var total = _dbStore.BaseParamSet.Count(bp => (alta && baja ||
							alta && bp.Active || baja && !bp.Active) &&
							(string.IsNullOrEmpty(searchGeneral) || bp.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
							bp.Description.ToUpper().Contains(searchGeneral.ToUpper())) &&
							(CQ1Id == null || bp.ClasificacionQuimica1 != null && bp.ClasificacionQuimica1.Id == CQ1Id) &&
							(CQ2Id == null || bp.ClasificacionQuimica2 != null && bp.ClasificacionQuimica2.Id == CQ2Id) &&
							(CQ3Id == null || bp.ClasificacionQuimica3 != null && bp.ClasificacionQuimica3.Id == CQ3Id));

				var baseParams = _dbStore.BaseParamSet.Where(bp => (alta && baja ||
							alta && bp.Active || baja && !bp.Active) &&
							(string.IsNullOrEmpty(searchGeneral) || bp.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
							bp.Description.ToUpper().Contains(searchGeneral.ToUpper())) &&
							(CQ1Id == null || bp.ClasificacionQuimica1 != null && bp.ClasificacionQuimica1.Id == CQ1Id) &&
							(CQ2Id == null || bp.ClasificacionQuimica2 != null && bp.ClasificacionQuimica2.Id == CQ2Id) &&
							(CQ3Id == null || bp.ClasificacionQuimica3 != null && bp.ClasificacionQuimica3.Id == CQ3Id))
					.OrderBy(bp => bp.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
					.Select(bp => bp.ToJson());

				return Json(new { success = true, elements = baseParams, total });
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
				var baseP = _dbStore.BaseParamSet.Find(id);
				if (baseP == null)
					return Json(new { success = false, details = "No se encontro info del parámetro base" });

				baseP.Active = active;
				_dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
				return Json(new { success = true });
			}
			catch (Exception e)
			{
				return Json(new { success = false, details = e.Message });
			}
		}

		[HttpGet, ActionName("CheckBaseParamName")]
		public JsonResult CheckBaseParamName(string uniqueInput, int id)
		{
			try
			{
				if (id != 0)
					if (_dbStore.BaseParamSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
						return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);

				var isIndb = _dbStore.BaseParamSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));

				//it returns true if it exists
				return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				return Json(new { success = false, details = e.Message });
			}
		}

		[HttpPost, ActionName("GetUnits")]
		public JsonResult GetUnits(bool activeOption = true)
		{
			try
			{
				//  var units = _dbStore.MeasureUnitSet.Where(e => e.Active && (e.BaseParam == null)).ToList()
				//.Select(u => u.ToJson());

				var units = _dbStore.MeasureUnitSet.Where(e => e.Active).ToList()
					.Select(u => u.ToJson());

				return Json(new { success = true, elements = units });
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
				var cq1 = _dbStore.BaseParamSet.Where(bp => bp.Active && bp.ClasificacionQuimica1 != null).Select(bp => bp.ClasificacionQuimica1).Distinct().Select(q => new { q.Id, q.Name });
				var cq2 = _dbStore.BaseParamSet.Where(bp => bp.Active && bp.ClasificacionQuimica2 != null).Select(bp => bp.ClasificacionQuimica2).Distinct().Select(q => new { q.Id, q.Name });
				var cq3 = _dbStore.BaseParamSet.Where(bp => bp.Active && bp.ClasificacionQuimica3 != null).Select(bp => bp.ClasificacionQuimica3).Distinct().Select(q => new { q.Id, q.Name });

				return Json(new { success = true, cq1, cq2, cq3, });
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

		[HttpPost, ActionName("GetBaseParam")]
		public JsonResult GetBaseParam(string userName, bool activeOption = true)
		{
			try
			{
				var activeBaseParams = _dbStore.BaseParamSet.Where(e => e.Active.Equals(activeOption))
										 .OrderBy(e => e.Name).ToList()
										 .Select(bp => bp.ToJson());

				//            var sucursales = userName == "root"
				//	? _dbStore.SucursalSet.Where(s => s.Active)
				//	: _dbStore.SucursalSet.Where(s => s.Active &&
				//				s.Users.Any(u => u.UserName.Equals(userName)));

				//var activeBaseParams = _dbStore.BaseParamSet
				//	.Where(bp => bp.Parameters.Any(p => p.Active.Equals(activeOption)
				//			&& (sucursales.Any(s => s.AnalyticsArea.Any(aa => aa.Id.Equals(p.CentroCosto.Id))))))
				//	.OrderBy(e => e.Name).ToList()
				//	.Select(bp => bp.ToJson());

				return Json(new { success = true, elements = activeBaseParams });
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

		[HttpPost, ActionName("GetBaseMatrix")]
		public JsonResult GetBaseMatrix(bool active = true)
		{
			try
			{
				var baseMatrix = _dbStore.BaseMatrixSet.Where(bm => bm.Active && bm.Matrixes.Any(m => m.Active)).ToList()
					.Select(bm => new
					{
						bm.Id,
						bm.Name,
						Matrixes = bm.Matrixes.Select(m => new { m.Id, m.Name, m.Description, m.SubMatrix, m.SubMtrxDescription, BaseMatrix = new { Mercado = new { m.BaseMatrix.Mercado.Name }, m.BaseMatrix.Name } })

					});
				return Json(new { success = true, baseMatrix });
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

		[HttpPost, ActionName("GetMatrixes")]
		public JsonResult GetMatrixes(int Id)
		{
			try
			{
				var Matrixes = _dbStore.BaseParamSet.Find(Id).Matrixes.Where(m => m.Active)
					.Select(m => new
					{
						m.Name,
						m.Description,
						m.SubMatrix,
						m.SubMtrxDescription,
						BaseMatrix = new
						{
							Mercado = new
							{
								m.BaseMatrix.Mercado.Name
							},
							m.BaseMatrix.Name
						}
					});
				return Json(new { success = true, Matrixes });
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
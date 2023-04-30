using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using AbcPersistent.Models;
using Param = AbcPersistent.Models.Param;

namespace AbcSpa.Controllers
{
    public class MethodController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveMethod")]
        public JsonResult SaveMethod(Method method)
        {
            try
            {
                var met = method.Id == 0
                    ? new Method()
                    {
                        Parameters = new List<Param>(),
						//Matrixes = new List<Matrix>(),
						Norms = new List<Norm>(),
                        TiposServicios = new LinkedList<TipoServicio>()
                    }
                    : _dbStore.MethodSet.Find(method.Id);

                if (!AssignValues(method, met))
                    throw new Exception("Error al llenar los datos del método");

				//if (!checkMatrixes(met, method))
				//	throw new Exception("Error al llenar la lista de Matrices");

                if (!CheckNorms(met, method))
                    throw new Exception("Error al llenar la lista de Normas");

                if (!CheckTiposServicios(met, method))
                    throw new Exception("Error al llenar la lista de Tipos de Servicios");

				if (method.Id == 0)
					_dbStore.MethodSet.Add(met);
				
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

		private bool AssignValues(Method method, Method met)
        {
            try
            {
                met.Name = method.Name;
                met.Active = method.Active;
                met.Description = method.Description;
                //met.RequiredVolume = method.RequiredVolume;
                //met.MinimumVolume = method.MinimumVolume;
                //met.Formula = method.Formula;
	            met.EntradaEnVigor = method.EntradaEnVigor;

				//met.ReportLimit = method.ReportLimit;
    //            met.DeliverTime = method.DeliverTime;
    //            met.MaxTimeBeforeAnalysis = method.MaxTimeBeforeAnalysis;
    //            met.LabDeliverTime = method.LabDeliverTime;
    //            met.ReportTime = method.ReportTime;
    //            met.DetectionLimit = method.DetectionLimit;
    //            met.CuantificationLimit = method.CuantificationLimit;
    //            //met.Uncertainty = method.Uncertainty;
    //            //met.QcObj = method.QcObj;
    //            met.Container = method.Container != null ? _dbStore.ContainerSet.Find(method.Container.Id) : null;
    //            met.Preserver = method.Preserver != null ? _dbStore.PreserverSet.Find(method.Preserver.Id) : null;
                
                // met.AnalyticsMethod = method.AnalyticsMethod != null ? _dbStore.AnalyticsMethodSet.Find(method.AnalyticsMethod.Id) : null;

                if (met.Estado == method.Estado) return true;
                met.FechaCambioStatus = DateTime.Now;
                met.Estado = method.Estado != null ? _dbStore.StatusSet.Find(method.Estado.Id) : null;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

		//private bool checkMatrixes(Method met, Method method)
		//{
		//	try
		//	{
		//		var matrixestodelete = met.Matrixes.Where(mtrx => method.Matrixes.All(m => m.Id != mtrx.Id));
		//		foreach (var pr in matrixestodelete.ToList())
		//			met.Matrixes.Remove(_dbStore.MatrixSet.Find(pr.Id));

		//		var matrixestoadd = method.Matrixes.Where(m => met.Matrixes.All(mtrx => mtrx.Id != m.Id));

		//		foreach (var g in matrixestoadd.ToList())
		//			met.Matrixes.Add(_dbStore.MatrixSet.Find(g.Id));

		//		return true;
		//	}
		//	catch (Exception e)
		//	{
		//		return false;
		//	}
		//}

        private bool CheckNorms(Method met, Method method)
        {
            try
            {
                var todelete = met.Norms.Where(nrm => method.Norms.All(m => m.Id != nrm.Id));
                foreach (var pr in todelete.ToList())
                    met.Norms.Remove(_dbStore.NormSet.Find(pr.Id));

                var toadd = method.Norms.Where(m => met.Norms.All(nrm => nrm.Id != m.Id));

                foreach (var g in toadd.ToList())
                    met.Norms.Add(_dbStore.NormSet.Find(g.Id));

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private bool CheckTiposServicios(Method met, Method method)
        {
            try
            {
                var todelete = met.TiposServicios.Where(nrm => method.TiposServicios.All(m => m.Id != nrm.Id));
                foreach (var pr in todelete.ToList())
                    met.TiposServicios.Remove(_dbStore.TipoServicioSet.Find(pr.Id));

                var toadd = method.TiposServicios.Where(m => met.TiposServicios.All(nrm => nrm.Id != m.Id));

                foreach (var g in toadd.ToList())
                    met.TiposServicios.Add(_dbStore.TipoServicioSet.Find(g.Id));

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

		[HttpPost, ActionName("RefreshList")]
        public JsonResult RefreshList(int page, int pageSize, string searchGeneral,
			bool alta = true, bool baja = false)
        {
            try
            {
	            var total = _dbStore.MethodSet.Count(e => (alta && baja ||
							alta && e.Active || baja && !e.Active) && (string.IsNullOrEmpty(searchGeneral) ||
	                                                                e.Name.ToUpper().Contains(searchGeneral.ToUpper()) || 
                                                                    e.Description.ToUpper().Contains(searchGeneral.ToUpper()))
                                                                    );

                var methods = _dbStore.MethodSet.Where(e => (alta && baja ||
                            alta && e.Active || baja && !e.Active) && (string.IsNullOrEmpty(searchGeneral) ||
                                                                    e.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                                                                    e.Description.ToUpper().Contains(searchGeneral.ToUpper())))
                    .OrderBy(m => m.Name).Skip((page - 1)*pageSize).Take(pageSize).ToList()
		            .Select(m => m.ToJson());
				
                return Json(new { success = true, elements = methods, total });
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

        //[HttpPost, ActionName("GetFilters")]
        //public JsonResult GetFilters()
        //{
        //    try
        //    {

        //        var envaseList = _dbStore.MethodSet.Where(m => m.Active && m.Container!=null).Select(m=>m.Container).Distinct().Select(m=>new {m.Id, m.Name});
        //        var preserverList = _dbStore.MethodSet.Where(m => m.Active && m.Preserver!=null).Select(m => m.Preserver).Distinct().Select(p => new { p.Id, p.Name });
        //        //var residueList = _dbStore.MethodSet.Where(m => m.Active && m.Residue!=null).Select(m => m.Residue).Distinct().Select(r => new { r.Id, r.Name });

        //        return Json(new { success = true, envaseList, preserverList});
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            err = "No se pudo recuperar la info.",
        //            success = false,
        //            details = ex.Message
        //        });
        //    }
        //}

        [HttpPost, ActionName("SaveActiveStatus")]
        public JsonResult SaveActiveStatus(int id, bool active)
        {
            try
            {
                var method = _dbStore.MethodSet.Find(id);
                if (method == null)
                    return Json(new { success = false, details = "No se encontro info del método" });

	            var hasparams = method.Parameters.Any(p => p.Active);
	            if (hasparams)
		            return Json(new { success = false, details = "El método está siendo usado, por lo que no puede cambiar su estado."});

				method.Active = active;
	            _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
	            return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckMethodName")]
        public JsonResult CheckMethodName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.MethodSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);
                
                var isIndb = _dbStore.MethodSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));
                
                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

		//   [HttpPost, ActionName("GetMethodContainer")]
		//   public JsonResult GetMethodContainer(bool active = true)
		//   {
		//       try
		//       {
		//           var container = _dbStore.ContainerSet.Where(c=>c.Active)
		//.Select(c => new {c.Id, c.Name});
		//           return Json(new { success = true, elements = container });
		//       }
		//       catch (Exception e)
		//       {
		//           return Json(new
		//           {
		//               err = "No se pudo procesar la info",
		//               success = false,
		//               details = e.Message
		//           });
		//       }
		//   }

		//   [HttpPost, ActionName("GetMethodPreserver")]
		//   public JsonResult GetMethodPreserver(bool active = true)
		//   {
		//       try
		//       {
		//           var preserver = _dbStore.PreserverSet.Where(p => p.Active)
		//.Select(p => new {p.Id, p.Name});
		//           return Json(new { success = true, elements = preserver });
		//       }
		//       catch (Exception e)
		//       {
		//           return Json(new
		//           {
		//               err = "No se pudo procesar la info",
		//               success = false,
		//               details = e.Message
		//           });
		//       }
		//   }

		//[HttpPost, ActionName("GetMethodResidue")]
		//public JsonResult GetMethodResidue(bool active = true)
        //{
		//	try
		//	{
		//		var residue = _dbStore.ResidueSet.Where(r => r.Active)
		//			.Select(r => new { r.Id, r.Name });
		//		return Json(new { success = true, elements = residue });
		//		//var residue = new ResidueController();
		//		//return residue.RefreshList(active);
		//	}
		//	catch (Exception e)
		//	{
		//		return Json(new
		//		{
		//			err = "No se pudo procesar la info",
		//			success = false,
		//			details = e.Message
		//		});
		//	}
		//}

		////[HttpPost, ActionName("GetMethodAnalyticsMethod")]
		////public JsonResult GetMethodAnalyticsMethod(bool active = true)
		////{
		////    try
		////    {
		////        var analyticsMethod = new AnalyticsMethodController();
		////        return analyticsMethod.RefreshList(active);
		////    }
		////    catch (Exception e)
		////    {
		////        return Json(new
		////        {
		////            err = "No se pudo procesar la info",
		////            success = false,
		////            details = e.Message
		////        });
		////    }
        //}
        
        [HttpPost, ActionName("GetMethods")]
        public JsonResult GetMethods(bool activeOption = true)
        {
            try
            {
	            var methods = _dbStore.MethodSet.Where(e => e.Active == activeOption)
		            .OrderBy(e => e.Name).ToList().Select(method => new
		            {
			            method.Id,
			            method.Name,
			            method.Description
		            });

                return Json(new { success = true, elements = methods });
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

		//[HttpPost, ActionName("GetBaseMatrix")]
		//public JsonResult GetBaseMatrix(bool active = true)
		//{
		//	try
		//	{
		//		var baseMatrix = _dbStore.BaseMatrixSet.Where(bm => bm.Active && bm.Matrixes.Any(m => m.Active)).ToList()
		//			.Select(bm => new
		//			{
		//				bm.Id,
		//				bm.Name,
		//				Matrixes = bm.Matrixes.Select(m => new { m.Id, m.Name, m.Description, m.SubMatrix, m.SubMtrxDescription, BaseMatrix = new { Mercado = new { m.BaseMatrix.Mercado.Name }, m.BaseMatrix.Name } })

		//			});
		//		return Json(new { success = true, baseMatrix });
		//	}
		//	catch (Exception e)
		//	{
		//		return Json(new
		//		{
		//			err = "No se pudo procesar la info",
		//			success = false,
		//			details = e.Message
		//		});
		//	}
		//}

		//[HttpPost, ActionName("GetMatrixes")]
		//public JsonResult GetMatrixes(int Id)
		//{
		//	try
		//	{
		//		var Matrixes = _dbStore.MethodSet.Find(Id).Matrixes.Where(m => m.Active)
		//														.Select(m => new
		//														{
		//															m.Name,
		//															m.Description,
		//															m.SubMatrix,
		//															m.SubMtrxDescription,
		//															BaseMatrix = new
		//															{
		//																Mercado = new
		//																{
		//																	m.BaseMatrix.Mercado.Name
		//																},
		//																m.BaseMatrix.Name
		//															}
		//														});
		//		return Json(new { success = true, Matrixes });
		//	}
		//	catch (Exception e)
		//	{
		//		return Json(new
		//		{
		//			err = "No se pudo procesar la info",
		//			success = false,
		//			details = e.Message
		//		});
		//	}
		//}

	}
}
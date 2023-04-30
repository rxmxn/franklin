using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using AbcPersistent.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.Office.Interop.Excel;

namespace AbcSpa.Controllers
{
    public class MatrixController : Controller
    {
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("RefreshList")]
        public JsonResult RefreshList(int page, int pageSize, string searchGeneral,
                                        string searchMatrix, string searchSubmatrix, 
                                        int? bMatrixId, int? marketId, string searchfokey,
                                        string fromDate, string untilDate,
                                        bool alta = true, bool baja = false)
        {
            try
            {
                // recordar filtrar por sucursal
                DateTime fromD = new DateTime(), untilD = new DateTime();
                if (!fromDate.IsEmpty() && !untilDate.IsEmpty())
                {
                    var dateData = fromDate.Substring(0, 10).Split('/');
                    fromD = new DateTime(Int32.Parse(dateData[2]), Int32.Parse(dateData[1]), Int32.Parse(dateData[0]));
                    dateData = untilDate.Substring(0, 10).Split('/');
                    untilD = new DateTime(Int32.Parse(dateData[2]), Int32.Parse(dateData[1]), Int32.Parse(dateData[0]));
                    untilD = untilD.AddHours(23).AddMinutes(59).AddSeconds(59);
                }

                var total = _dbStore.MatrixSet.Count(m => ((alta && baja) ||
                            (alta && m.Active) || (baja && !m.Active))
                        && (string.IsNullOrEmpty(fromDate) &&
                            string.IsNullOrEmpty(untilDate) ||
                            ((m.MatrixCreateDate >= fromD) &&
                            (m.MatrixCreateDate <= untilD)))
                        && (string.IsNullOrEmpty(searchGeneral) ||
                            m.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            m.SubMatrix.ToUpper().Contains(searchGeneral.ToUpper())) &&
                            (string.IsNullOrEmpty(searchMatrix) ||
                            m.Name.ToUpper().Contains(searchMatrix.ToUpper())) &&
                            (string.IsNullOrEmpty(searchSubmatrix) ||
                            m.SubMatrix.ToUpper().Contains(searchSubmatrix.ToUpper())) &&
                            (bMatrixId == null || m.BaseMatrix.Id == bMatrixId) &&
                            (marketId == null || m.BaseMatrix.Mercado.Id == marketId));


                var matrixes = _dbStore.MatrixSet.Where(m => ((alta && baja) ||
                            (alta && m.Active) || (baja && !m.Active))
                        && (string.IsNullOrEmpty(fromDate) &&
                            string.IsNullOrEmpty(untilDate) ||
                            ((m.MatrixCreateDate >= fromD) &&
                            (m.MatrixCreateDate <= untilD)))
                        && (string.IsNullOrEmpty(searchGeneral) ||
                            m.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            m.SubMatrix.ToUpper().Contains(searchGeneral.ToUpper())) &&
                            /*(string.IsNullOrEmpty(searchfokey)|| searchfokey[0]==m.BaseMatrix.Mercado.Name[0] && searchfokey.) &&*/
                            (string.IsNullOrEmpty(searchMatrix) ||
                            m.Name.ToUpper().Contains(searchMatrix.ToUpper())) &&
                            (string.IsNullOrEmpty(searchSubmatrix) ||
                            m.SubMatrix.ToUpper().Contains(searchSubmatrix.ToUpper())) &&
                            (bMatrixId==null || m.BaseMatrix.Id==bMatrixId) &&
                            (marketId==null || m.BaseMatrix.Mercado.Id==marketId))

                    .OrderBy(m => m.MatrixCreateDate).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                    .Select(m => m.ToMiniJson());

                return Json(new { success = true, elements = matrixes, total });
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

        [HttpPost, ActionName("SaveMatrix")]
        public JsonResult SaveMatrix(Matrix matrix)
        {
            try
            {
                Matrix mtrx;
                if (matrix.Id == 0)
                {
                    mtrx = new Matrix()
                    {
                        BaseMatrix = _dbStore.BaseMatrixSet.Find(matrix.BaseMatrix.Id),
                        Description = matrix.Description,
                        SubMatrix = matrix.SubMatrix,
                        SubMtrxDescription = matrix.SubMtrxDescription,
                        Name = matrix.Name,
                        MatrixCreateDate = DateTime.Now,
                        Parameters = new List<Param>(),
                        Groups = new List<Group>(),
                        Packages = new List<Package>(),
						BaseParams = new List<BaseParam>()
                    };


                    _dbStore.MatrixSet.Add(mtrx);
                }
                else
                {
                    mtrx = _dbStore.MatrixSet.Find(matrix.Id);

                    mtrx.BaseMatrix = _dbStore.BaseMatrixSet.Find(matrix.BaseMatrix.Id);
                    mtrx.Name = matrix.Name;
                    mtrx.SubMatrix = matrix.SubMatrix;
                    mtrx.SubMtrxDescription = matrix.SubMtrxDescription;
                    mtrx.Description = matrix.Description;
                    mtrx.MatrixCreateDate = DateTime.Now;
                    mtrx.Active = matrix.Active;
                }
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        //     private bool ResetAccessoriesList(Matrix mtrx, 
        //IEnumerable<int> pks, IEnumerable<int> grps, IEnumerable<int> prms)
        //     {
        //         try
        //         {
        //          if (prms != null)
        //          {
        //		var paramtodelete = mtrx.Parameters.Where(par => prms.All(m => m != par.Id) && par.Active);

        //		foreach (var pd in paramtodelete.ToList())
        //			mtrx.Parameters.Remove(_dbStore.ParamSet.Find(pd.Id));

        //		var paramtoadd = prms.Where(par => mtrx.Parameters.All(m => m.Id != par));

        //		foreach (var pa in paramtoadd)
        //			mtrx.Parameters.Add(_dbStore.ParamSet.Find(pa));
        //	}

        //          if (grps != null)
        //          {
        //		var grouptodelete = mtrx.Groups.Where(par => grps.All(m => m != par.Id) && par.Active);

        //		foreach (var gd in grouptodelete.ToList())
        //			mtrx.Groups.Remove(_dbStore.GroupSet.Find(gd.Id));

        //		var grouptoadd = grps.Where(par => mtrx.Groups.All(m => m.Id != par));

        //		foreach (var ga in grouptoadd)
        //			mtrx.Groups.Add(_dbStore.GroupSet.Find(ga));
        //	}

        //          if (pks == null) return true;

        //          var packtodelete = mtrx.Packages.Where(par => pks.All(m => m != par.Id) && par.Active);

        //          foreach (var pk in packtodelete.ToList())
        //           mtrx.Packages.Remove(_dbStore.PackageSet.Find(pk.Id));

        //          var packtoadd = pks.Where(par => mtrx.Packages.All(m => m.Id != par));

        //          foreach (var ga in packtoadd)
        //           mtrx.Packages.Add(_dbStore.PackageSet.Find(ga));

        //          return true;
        //         }
        //         catch (Exception)
        //         {
        //             return false;
        //         }
        //     }

        [HttpPost, ActionName("SaveActiveStatus")]
        public JsonResult SaveActiveStatus(int id, bool active)
        {
            try
            {
                var mtrx = _dbStore.MatrixSet.Find(id);
                if (mtrx == null)
                    return Json(new { success = false, details = "No se encontro info de la matriz" });

                mtrx.Active = active;
                mtrx.MatrixCreateDate = DateTime.Now;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpPost, ActionName("GetMatrixName")]
        public JsonResult GetMatrixName(string key)
        {
            try
            {
                /*IEnumerable<string>*/
                var matrixNameList = _dbStore.MatrixSet.Where(m => m.Name.Contains(key)).OrderBy(m => m.Name).Select(m => m.Name).Distinct().Take(20);
                return !matrixNameList.Any() ? Json(new { success = false }) : Json(new { success = true, matrixNameList });

            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message, err = "Error en el servidor" });
            }
        }

        [HttpPost, ActionName("GetElems")]
        public JsonResult GetGroups(int id)
        {
            try
            {
                dynamic currUsr = Session["curr_User"];
                int userId = currUsr.Id;

                var sucursales = User.Identity.Name == "root"
                    ? _dbStore.SucursalSet.Where(s => s.Active)
                    : _dbStore.SucursalSet.Where(s => s.Active &&
                                s.Users.Any(u => u.Id.Equals(userId)));

                var baseMatrixList = _dbStore.BaseMatrixSet.Where(bm => bm.Active).Select(bm => new { bm.Id, bm.Name, Mercado = new { bm.Mercado.Id, bm.Mercado.Name } });
                var matrixList = _dbStore.MatrixSet.Where(m => m.Active).Select(m => new { m.Name, m.Description });
                var submatrixList = _dbStore.MatrixSet.Where(m => m.Active).Select(m => new { Name = m.SubMatrix, Description = m.SubMtrxDescription });

                return Json(new { success = true, baseMatrixList, matrixList, submatrixList });
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

        [HttpPost, ActionName("GetFilters")]
        public JsonResult GetFilters()
        {
            try
            {

                dynamic currUsr = Session["curr_User"];
                int userId = currUsr.Id;

                var sucursales = User.Identity.Name == "root"
                    ? _dbStore.SucursalSet.Where(s => s.Active)
                    : _dbStore.SucursalSet.Where(s => s.Active &&
                                s.Users.Any(u => u.Id.Equals(userId)));

                var matrixes = _dbStore.MatrixSet.Where(m => m.Active /*&& m.Parameters.Any(p => p.Active && sucursales.Any(s => s.Active &&
                                                                                                        (p.SucursalRealiza != null &&
                                                                                                        s.Id.Equals(p.SucursalRealiza.Id) ||
                                                                                                         p.SucursalVende != null && s.Id.Equals(p.SucursalVende.Id))))*/);
                var matrixList = matrixes.Select(m => new {m.Name }).Distinct();
                var baseMatrixList = matrixes.Select(m => m.BaseMatrix).Distinct().Select(sm => new { sm.Id, sm.Name });
                var submatrixList = matrixes.Select(m => new { Name=m.SubMatrix==""?"N/A": m.SubMatrix }).Distinct();
                var marketList = matrixes.Select(m => m.BaseMatrix.Mercado).Distinct().Select(mk => new { mk.Id, mk.Name });

                return Json(new { success = true, matrixList, baseMatrixList, submatrixList, marketList });
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


        [HttpPost, ActionName("GetPackages")]
        public JsonResult GetPackages(int id)
        {
            try
            {
                var packageList = _dbStore.PackageSet.Where(pk => pk.Active)
                    .ToList().Select(pk => pk.ToJson());

                return Json(new { success = true, elements = packageList });
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


        //[HttpPost, ActionName("GetSucursal")]
        //public JsonResult GetSucursal(bool active = true)
        //{
        //    try
        //    {
        //        dynamic currUsr = Session["curr_User"];
        //        int userId = currUsr.Id;

        //        var sucursales = User.Identity.Name == "root"
        //            ? _dbStore.SucursalSet.Where(s => s.Active)
        //            : _dbStore.SucursalSet.Where(s => s.Active &&
        //                        s.Users.Any(u => u.Id.Equals(userId)));

        //        var mtrxL = _dbStore.MatrixSet.Where(m => m.Active &&
        //                                                  m.Parameters.Any(p => p.Active &&
        //                                                                        sucursales.Any(
        //                                                                            s =>
        //                                                                                s.Active &&
        //                                                                                (p.SucursalRealiza != null &&
        //                                                                                 p.SucursalRealiza.Id == s.Id ||
        //                                                                                 p.SucursalVende != null &&
        //                                                                                 p.SucursalVende.Id == s.Id))))
        //            .Select(m => new
        //            {
        //                m.BaseMatrix.Name,
        //                m.Description
        //            });



        //            sucursalList.Add(new
        //            {
        //                sucursal.Id,
        //                Name = sucursal.ToString(),
        //                matrixes = mtrxList
        //            });

        //        return Json(new { success = true, sucursalList });
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new
        //        {
        //            err = "No se pudo procesar la info",
        //            success = false,
        //            details = e.Message
        //        });
        //    }
        //}

        [HttpPost, ActionName("GetMatrixforSucursal")]
        public JsonResult GetMatrixforSucursal(int id)
        {
            try
            {
                var suc = _dbStore.SucursalSet.Find(id);
                var mtrxList = _dbStore.BaseMatrixSet.Where(mtrx => mtrx.Active 
				&& suc.Offices.Any(o => o.Market.Id.Equals(mtrx.Mercado.Id))).Select(bm => new
                {
                    bm.Id,
                    bm.Name
                });

                return Json(new { success = true, elements = mtrxList });
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

        [HttpPost, ActionName("GetMatrixesToFilter")]
        public JsonResult GetMatrixesToFilter()
        {
            try
            {
                dynamic currUsr = Session["curr_User"];
                int userId = currUsr.Id;

                var sucursales = _dbStore.SucursalSet.Where(s => s.Active
                    && (userId == 0 || s.Users.Any(x => x.Id.Equals(userId)))
                    && s.SucursalRealizaParams.Any(p => p.Active)
                    || s.SucursalVendeParams.Any(p => p.Active));

                var parameters = _dbStore.ParamSet.Where(p => p.Active &&
                            sucursales.Any(s => (p.SucursalRealiza != null
                                            && s.Id.Equals(p.SucursalRealiza.Id))
                                            || (p.SucursalVende != null
                                            && s.Id.Equals(p.SucursalVende.Id))));

                //var groups = _dbStore.GroupSet.Where(g => g.Active &&
                //			g.Parameters.Any(p => parameters.Any(p1 => p1.Id.Equals(p.Id))));
                var groups = _dbStore.GroupSet.Where(g => g.Active &&
                            sucursales.Any(s => g.Sucursal != null && s.Id.Equals(g.Sucursal.Id)));

                //var packages = _dbStore.PackageSet.Where(pk => pk.Active &&
                //			(pk.Parameters.Any(p => parameters.Any(p1 => p1.Id.Equals(p.Id)))
                //			|| pk.Groups.Any(g => groups.Any(g1 => g1.Id.Equals(g.Id)))));
                var packages = _dbStore.PackageSet.Where(pk => pk.Active &&
                            sucursales.Any(s => pk.Sucursal != null && s.Id.Equals(pk.Sucursal.Id)));

                var matrixes = _dbStore.MatrixSet.Where(m => m.Active &&
                    m.Parameters.Any(p => parameters.Any(p1 => p1.Id.Equals(p.Id))) ||
                    m.Groups.Any(g => groups.Any(g1 => g1.Id.Equals(g.Id))) ||
                    m.Packages.Any(pk => packages.Any(pk1 => pk1.Id.Equals(pk.Id))))
                    .OrderBy(m => m.Name).ToList()
                    .Select(m => new
                    {
                        m.Id,
                        name = m.Name
                    });

                //var baseMatrixes = suc.SelectMany(m => m.Matrixes
                //	.Where(a => a.Active)
                //	.DistinctBy(a => a.BaseMatrix)
                //	.OrderBy(a => a.BaseMatrix.Name).ToList()
                //	.Select(a => new {a.BaseMatrix.Id, name = a.BaseMatrix.Name}));

                return Json(new { success = true, elements = matrixes });
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

        [HttpPost, ActionName("GetMatrixes")]
        public JsonResult GetMatrixes()
        {
            try
            {
                var mtrx = _dbStore.MatrixSet.Where(m => m.Active)
                    .OrderBy(m => m.Name).ToList().Select(m => new
                    {
                        m.Id,
                        m.Name
                    });

                return Json(new { success = true, elements = mtrx });
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
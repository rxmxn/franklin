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

namespace AbcSpa.Controllers
{
    public class PackageController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SavePackage")]
        public JsonResult SavePackage(Package pack,
            IEnumerable<int> grps, IEnumerable<int> prms)
        {
            try
            {
                var pk = pack.Id == 0
                    ? new Package()
                    {
                        Parameters = new List<Param>(),
                        Groups = new List<Group>()
                    }
                    : _dbStore.PackageSet.Find(pack.Id);

                pk.Name = pack.Name;
                pk.Description = pack.Description;
                pk.Active = pack.Active;
                pk.SellSeparated = pack.SellSeparated;
                pk.CuentaEstadistica = pack.CuentaEstadistica;
                //pk.Norm = _dbStore.NormSet.Find(pack.Norm?.Id);
                pk.DecimalesReporte = pack.DecimalesReporte;
                pk.TipoServicio = (pack.TipoServicio != null)
                    ? _dbStore.TipoServicioSet.Find(pack.TipoServicio.Id)
                    : null;

                pk.Sucursal = pack.Sucursal != null ? _dbStore.SucursalSet.Find(pack.Sucursal.Id) : null;

                if (!checkMatrixes(pk, pack))
                    throw new Exception("Error al llenar la lista de Matrices");

                if (!CheckNorms(pk, pack))
                    throw new Exception("Error al llenar la lista de Normas");

                if (pack.Id != 0)
                    ResetAccessoriesList(pk, grps, prms);
                else
                {
                    if (prms != null)
                        foreach (var param in prms)
                            pk.Parameters.Add(_dbStore.ParamSet.Find(param));

                    if (grps != null)
                        foreach (var group in grps)
                            pk.Groups.Add(_dbStore.GroupSet.Find(group));
                }

                pk.PublishInAutolab = true;
                if (!pk.Active
                    || string.IsNullOrEmpty(pk.Name)
                    || string.IsNullOrEmpty(pk.Description)
                    || (pk.DecimalesReporte == null)
                   // || (pk.Norm == null)
                    || (!pk.Parameters.Any(p => p.Active && p.PublishInAutolab) && !pk.Groups.Any(g => g.Active && g.PublishInAutolab))
                    || !pk.Matrixes.Any(m => m.Active)
                    || (pk.TipoServicio == null)
                    || (pk.Sucursal == null)
                    )
                {
                    pk.PublishInAutolab = false;
                }

                if (pack.Id == 0)
                    _dbStore.PackageSet.Add(pk);

                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        private bool ResetAccessoriesList(Package pack,
            IEnumerable<int> grps, IEnumerable<int> prms)
        {
            try
            {
                //var pack = _dbStore.PackageSet.Find(pkId);

                if (prms != null)
                {
                    var paramtodelete = pack.Parameters
                    .Where(par => prms.All(m => m != par.Id) && par.Active);

                    foreach (var pd in paramtodelete.ToList())
                        pack.Parameters.Remove(_dbStore.ParamSet.Find(pd.Id));

                    var paramtoadd = prms.Where(par => pack.Parameters.All(m => m.Id != par));

                    foreach (var pa in paramtoadd)
                        pack.Parameters.Add(_dbStore.ParamSet.Find(pa));
                }

                if (grps == null) return true;

                // quitando todos los que estan en el pk que no vinieron marcados
                var grouptodelete = pack.Groups
                    .Where(par => grps.All(m => m != par.Id) && par.Active);

                foreach (var gd in grouptodelete.ToList())
                    pack.Groups.Remove(_dbStore.GroupSet.Find(gd.Id));

                // adicionando todos los que no estan en el pk que vinieron marcados
                var grouptoadd = grps.Where(par => pack.Groups.All(m => m.Id != par));

                foreach (var ga in grouptoadd)
                    pack.Groups.Add(_dbStore.GroupSet.Find(ga));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool checkMatrixes(Package pk, Package pack)
        {
            try
            {
                var matrixestodelete = pk.Matrixes.Where(mtrx => pack.Matrixes.All(m => m.Id != mtrx.Id));
                foreach (var pr in matrixestodelete.ToList())
                    pk.Matrixes.Remove(_dbStore.MatrixSet.Find(pr.Id));

                var matrixestoadd = pack.Matrixes.Where(m => pk.Matrixes.All(mtrx => mtrx.Id != m.Id));

                foreach (var g in matrixestoadd.ToList())
                    pk.Matrixes.Add(_dbStore.MatrixSet.Find(g.Id));

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private bool CheckNorms(Package pk, Package pack)
        {
            try
            {
                var todelete = pk.Norms.Where(nrm => pack.Norms.All(m => m.Id != nrm.Id));
                foreach (var pr in todelete.ToList())
                    pk.Norms.Remove(_dbStore.NormSet.Find(pr.Id));

                var toadd = pack.Norms.Where(m => pk.Norms.All(nrm => nrm.Id != m.Id));

                foreach (var g in toadd.ToList())
                    pk.Norms.Add(_dbStore.NormSet.Find(g.Id));

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [HttpPost, ActionName("RefreshList")]
        public JsonResult RefreshList(int page, int pageSize, string searchGeneral,
                                      string searchDescription, int? sucId, int? bLine,
                                      bool alta = true, bool baja = false,
                                      bool spcSi = false, bool spcNo = true)
        {
            try
            {
                dynamic currUsr = Session["curr_User"];
                int userId = currUsr.Id;

                var sucursales = User.Identity.Name == "root"
                    ? _dbStore.SucursalSet.Where(s => s.Active)
                    : _dbStore.SucursalSet.Where(s => s.Active &&
                                s.Users.Any(u => u.Id.Equals(userId)));

                var total = _dbStore.PackageSet.Count(pk => ((alta && baja) ||
                            (alta && pk.Active) || (baja && !pk.Active))
                            && ((spcSi && spcNo) ||
                            (spcSi && !pk.PublishInAutolab) || (spcNo && pk.PublishInAutolab)) &&
                            (pk.Groups.Any(g => g.Active &&
                                                g.Parameters.Any(p => p.Active &&
                                                sucursales.Any(s => s.Active &&
                                                (p.SucursalRealiza != null && p.SucursalRealiza.Id == s.Id ||
                                                    p.SucursalVende != null && p.SucursalVende.Id == s.Id)))) ||
                            pk.Parameters.Any(p => p.Active &&
                                                sucursales.Any(s => s.Active &&
                                                (p.SucursalRealiza != null && p.SucursalRealiza.Id == s.Id ||
                                                p.SucursalVende != null && p.SucursalVende.Id == s.Id)))) &&
                            (string.IsNullOrEmpty(searchGeneral) ||
                            (pk.Name.ToUpper().Contains(searchGeneral.ToUpper()))) &&
                            (string.IsNullOrEmpty(searchDescription) ||
                            (pk.Description.ToUpper().Contains(searchDescription.ToUpper()))) &&
                            (sucId == null || pk.Sucursal.Id == sucId) && (bLine == null || pk.Matrixes.Any(m=>m.BaseMatrix.Id==bLine)));

                var activePackageList = _dbStore.PackageSet.Where(pk => ((alta && baja) ||
                            (alta && pk.Active) || (baja && !pk.Active))
                            && ((spcSi && spcNo) ||
                            (spcSi && !pk.PublishInAutolab) || (spcNo && pk.PublishInAutolab)) &&
                            (pk.Groups.Any(g => g.Active &&
                                                g.Parameters.Any(p => p.Active &&
                                                sucursales.Any(s => s.Active &&
                                                (p.SucursalRealiza != null && p.SucursalRealiza.Id == s.Id ||
                                                    p.SucursalVende != null && p.SucursalVende.Id == s.Id)))) ||
                            pk.Parameters.Any(p => p.Active &&
                                                sucursales.Any(s => s.Active &&
                                                (p.SucursalRealiza != null && p.SucursalRealiza.Id == s.Id ||
                                                p.SucursalVende != null && p.SucursalVende.Id == s.Id)))) &&
                            (string.IsNullOrEmpty(searchGeneral) ||
                            (pk.Name.ToUpper().Contains(searchGeneral.ToUpper()))) &&
                            (string.IsNullOrEmpty(searchDescription) ||
                            (pk.Description.ToUpper().Contains(searchDescription.ToUpper()))) &&
                            (sucId == null || pk.Sucursal.Id == sucId) && (bLine == null || pk.Matrixes.Any(m => m.BaseMatrix.Id == bLine)))
                        .OrderBy(pkt => pkt.Name)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize).ToList()
                        .Select(pk => pk.ToJson());

                return Json(new { success = true, elements = activePackageList, total });
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

                dynamic currUsr = Session["curr_User"];
                int userId = currUsr.Id;

                var sucursales = User.Identity.Name == "root"
                    ? _dbStore.SucursalSet.Where(s => s.Active)
                    : _dbStore.SucursalSet.Where(s => s.Active &&
                                s.Users.Any(u => u.Id.Equals(userId)));

                var packages = _dbStore.PackageSet.Where(pk => pk.Active &&
                                                          (pk.Parameters.Any(p => p.Active &&
                                                                                sucursales.Any(s => s.Active &&
                                                                                                    (p.SucursalRealiza != null &&
                                                                                                     s.Id.Equals(p.SucursalRealiza.Id) ||
                                                                                                     p.SucursalVende !=null && s.Id.Equals(p.SucursalVende.Id)))) || 
                                                           pk.Groups.Any(g=>g.Parameters.Any(p => p.Active &&
                                                                                sucursales.Any(s => s.Active &&
                                                                                                    (p.SucursalRealiza != null &&
                                                                                                     s.Id.Equals(p.SucursalRealiza.Id) ||
                                                                                                     p.SucursalVende != null && s.Id.Equals(p.SucursalVende.Id)))))));

                var sucList = packages.Select(g => g.Sucursal).Distinct().Select(s => new { s.Id, s.Name });
                var marketList = packages.SelectMany(pk => pk.Matrixes.Select(m => m.BaseMatrix.Mercado)).Distinct().Select(s => new { s.Id, s.Name });
                
                return Json(new { success = true, sucList, marketList});
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
                var mar = _dbStore.PackageSet.Find(id);
                if (mar == null)
                    return Json(new { success = false, details = "No se encontró info del mercado" });

                mar.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckPackageName")]
        public JsonResult CheckPackageName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.PackageSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);

                var isIndb = _dbStore.PackageSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));

                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        //[HttpPost, ActionName("GetParams")]
        //public JsonResult GetParams(int id, int page, int pageSize, string searchGeneral)
        //{
        //    try
        //    {
        //        dynamic currUsr = Session["curr_User"];
        //        int userId = currUsr.Id;

        //        var sucursales = User.Identity.Name == "root"
        //            ? _dbStore.SucursalSet.Where(s => s.Active)
        //            : _dbStore.SucursalSet.Where(s => s.Active &&
        //                        s.Users.Any(u => u.Id.Equals(userId)));

        //        var total = _dbStore.ParamSet.Count(p => p.Active &&
        //                                                 sucursales.Any(s => s.Active &&
        //                                                                     (p.SucursalRealiza != null && s.Id == p.SucursalRealiza.Id ||
        //                                                                      p.SucursalVende != null && s.Id == p.SucursalVende.Id) &&
        //                                                                      (string.IsNullOrEmpty(searchGeneral) ||
        //                                                                    p.Description.ToLower().Contains(searchGeneral.ToLower()) ||
        //                                                                    p.ParamUniquekey.ToLower().Contains(searchGeneral.ToLower()))));

        //        var activeParamList = _dbStore.ParamSet.Where(p => p.Active &&
        //                                                           sucursales.Any(s => s.Active &&
        //                                                                               (p.SucursalRealiza != null &&
        //                                                                                s.Id == p.SucursalRealiza.Id ||
        //                                                                                p.SucursalVende != null &&
        //                                                                                s.Id == p.SucursalVende.Id)) &&
        //                                                                                (string.IsNullOrEmpty(searchGeneral) ||
        //                                                                                p.Description.ToLower().Contains(searchGeneral.ToLower()) ||
        //                                                                                p.ParamUniquekey.ToLower().Contains(searchGeneral.ToLower())))
        //                                                .OrderBy(p => p.ParamUniquekey).Skip((page - 1) * pageSize).Take(pageSize).ToList()
        //            .Select(p => p.ToJson(grupoId: id));

        //        return Json(new { success = true, paramList = activeParamList, total });
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

        private dynamic GetParams(int gId, int page, int pageSize, string searchGeneral)
        {
            try
            {
                dynamic currUsr = Session["curr_User"];
                int userId = currUsr.Id;

                var sucursales = User.Identity.Name == "root"
                    ? _dbStore.SucursalSet.Where(s => s.Active)
                    : _dbStore.SucursalSet.Where(s => s.Active &&
                                s.Users.Any(u => u.Id.Equals(userId)));

                var total = _dbStore.ParamSet.Count(p => p.Active &&
                                                         sucursales.Any(s => s.Active &&
                                                                             (p.SucursalRealiza != null && s.Id == p.SucursalRealiza.Id ||
                                                                              p.SucursalVende != null && s.Id == p.SucursalVende.Id) &&
                                                                              (string.IsNullOrEmpty(searchGeneral) ||
                                                                            p.Description.ToLower().Contains(searchGeneral.ToLower()) ||
                                                                            p.ParamUniquekey.ToLower().Contains(searchGeneral.ToLower()))));

                var parameters = _dbStore.ParamSet.Where(p => p.Active &&
                                              sucursales.Any(s => s.Active &&
                                                                  (p.SucursalRealiza != null &&
                                                                   s.Id == p.SucursalRealiza.Id ||
                                                                   p.SucursalVende != null &&
                                                                   s.Id == p.SucursalVende.Id)) &&
                                              (string.IsNullOrEmpty(searchGeneral) ||
                                               p.Description.ToLower().Contains(searchGeneral.ToLower()) ||
                                               p.ParamUniquekey.ToLower().Contains(searchGeneral.ToLower())))
                    .OrderBy(p => p.ParamUniquekey).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                    .Select(p => p.ToJson(grupoId: gId));

                return new
                {
                    parameters,
                    total
                };
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private dynamic GetGroups(int page, int pageSize, string searchGeneral)
        {
            try
            {
                dynamic currUsr = Session["curr_User"];
                int userId = currUsr.Id;

                var sucursales = User.Identity.Name == "root"
                    ? _dbStore.SucursalSet.Where(s => s.Active)
                    : _dbStore.SucursalSet.Where(s => s.Active &&
                                s.Users.Any(u => u.Id.Equals(userId)));

                var total = _dbStore.GroupSet.Count(g => g.Active && g.Parameters.Any(p => p.Active && sucursales.Any(s => s.Active &&
                                                                               (p.SucursalRealiza != null && s.Id == p.SucursalRealiza.Id ||
                                                                                p.SucursalVende != null && s.Id == p.SucursalVende.Id))) &&
                                                                                (string.IsNullOrEmpty(searchGeneral) ||
                                                                                g.Description.ToLower().Contains(searchGeneral.ToLower()) ||
                                                                                g.Name.ToLower().Contains(searchGeneral.ToLower())));

                var groups = _dbStore.GroupSet.Where(g => g.Active && g.Parameters.Any(p => p.Active && sucursales.Any(s => s.Active &&
                                                                                 (p.SucursalRealiza != null && s.Id == p.SucursalRealiza.Id ||
                                                                                  p.SucursalVende != null && s.Id == p.SucursalVende.Id))) &&
                                                                                  (string.IsNullOrEmpty(searchGeneral) ||
                                                                                  g.Description.ToLower().Contains(searchGeneral.ToLower()) ||
                                                                                  g.Name.ToLower().Contains(searchGeneral.ToLower())))
                                                         .OrderBy(g => g.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                                                         .Select(g => g.ToJson());
                return new { groups, total };

            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpPost, ActionName("GetAccessories")]
        public JsonResult GetAccessories(int id)
        {
            try
            {
                var parameters = GetParams(0, 1, 10, "");
                var groups = GetGroups(1, 10, "");
                var PermitedLimits = _dbStore.MaxPermitedLimitSet.Where(lmp => lmp.Active).ToList().Select(lmp => lmp.ToMiniJson());

                return Json(new { success = true, paramList = parameters.@parameters, groupList = groups.@groups, pTotal = parameters.total, gTotal = groups.total, PermitedLimits });
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

        [HttpPost, ActionName("GetParamPage")]
        public JsonResult GetParamPage(int page, int pageSize, string searchGeneral)
        {
            try
            {
                var parameters = GetParams(0, page, pageSize, searchGeneral);
                return Json(new { success = true, paramList = parameters.@parameters, pTotal = parameters.total });
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


        [HttpPost, ActionName("GetGroupPage")]
        public JsonResult GetGroupPage(int page, int pageSize, string searchGeneral)
        {
            try
            {
                var groups = GetGroups(page, pageSize, searchGeneral);
                return Json(new { success = true, groupList = groups.@groups, gTotal = groups.total });
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
        public JsonResult GetMatrixes(int Id)
        {
            try
            {
                var Matrixes = _dbStore.PackageSet.Find(Id).Matrixes.Where(m => m.Active).Select(m => new { m.Name, m.Description, m.SubMatrix, m.SubMtrxDescription, BaseMatrix = new { m.BaseMatrix.Name, Mercado = new { m.BaseMatrix.Mercado.Name } } });
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


        //[HttpPost, ActionName("GetBaseMatrix")]
        //public JsonResult GetBaseMatrix(bool active = true)
        //{
        //    try
        //    {
        //        var baseMatrix = _dbStore.BaseMatrixSet.Where(bm => bm.Active && bm.Matrixes.Any(m => m.Active)).ToList()
        //            .Select(bm => new
        //            {
        //                bm.Id,
        //                bm.Name,
        //                Matrixes = bm.Matrixes.Select(m => new { m.Id, m.Name, m.Description, m.SubMatrix, m.SubMtrxDescription, BaseMatrix = new { Mercado = new { m.BaseMatrix.Mercado.Name }, m.BaseMatrix.Name } })

        //            });
        //        return Json(new { success = true, baseMatrix });
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

        //[HttpPost, ActionName("GetNorms")]
        //public JsonResult GetNorms(bool activeOption = true)
        //{
        //    try
        //    {
        //        var norms = _dbStore.NormSet.Where(e => e.Active == activeOption)
        //            .OrderBy(e => e.Name).ToList()
        //            .Select(n => n.ToJson());

        //        return Json(new { success = true, elements = norms });
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
    }
}
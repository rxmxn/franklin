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
using Microsoft.Office.Interop.Excel;

namespace AbcSpa.Controllers
{
    public class MaxPermitedLimitController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        //[Audit]
        [HttpPost, ActionName("SaveMaxPermitedLimit")]
        public JsonResult SaveMaxPermitedLimit(MaxPermitedLimit maxPermitedLimit)
        {
            try
            {
                var lmp = maxPermitedLimit.Id == 0
                    ? new MaxPermitedLimit()
                    {
                        Name = maxPermitedLimit.Name,
                        Description = maxPermitedLimit.Description,
                        ParamRoutes = new List<ParamRoute>()

                    }
                    : _dbStore.MaxPermitedLimitSet.Find(maxPermitedLimit.Id);

                if (lmp.Id != 0)
                    if (!Checkgrauppackage(lmp, maxPermitedLimit))
                        Json(new { success = false, details = "Error al actualizar los paquetes, grupos o parámetros" });


                foreach (var _pr in from pr in maxPermitedLimit.ParamRoutes
                                    where pr.Id == 0 || maxPermitedLimit.Id == 0
                                    select new ParamRoute()
                                    {
                                        // Matrix = pr.Matrix != null ? _dbStore.MatrixSet.Find(pr.Matrix.Id) : null,
                                        Group = pr.Group != null ? _dbStore.GroupSet.Find(pr.Group.Id) : null,
                                        Package = pr.Package != null ? _dbStore.PackageSet.Find(pr.Package.Id) : null,
                                        Parameter = _dbStore.ParamSet.Find(pr.Parameter.Id),
                                        Value = pr.Value,
                                        DecimalsPoints = pr.DecimalsPoints
                                    })
                {
                    _dbStore.ParamRouteSet.Add(_pr);

                }
                _dbStore.SaveChanges();

                var routes = _dbStore.ParamRouteSet.Where(pr => pr.MaxPermitedLimit == null);
                foreach (var pr in routes)
                {
                    lmp.ParamRoutes.Add(_dbStore.ParamRouteSet.Find(pr.Id));
                }

                if (maxPermitedLimit.Id != 0)
                {

                    lmp.Name = maxPermitedLimit.Name;
                    lmp.Description = maxPermitedLimit.Description;
                    lmp.Active = maxPermitedLimit.Active;
                }
                else
                {

                    _dbStore.MaxPermitedLimitSet.Add(lmp);
                }


                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        private bool Checkgrauppackage(MaxPermitedLimit lmp, MaxPermitedLimit maxPermitedLimit)
        {
            try
            {
                var parRoutetodelete = lmp.ParamRoutes.Where(pr => maxPermitedLimit.ParamRoutes.Where(p => p.Id != 0).All(m => m.Id != pr.Id));
                foreach (var pr in parRoutetodelete.ToList())
                {
                    lmp.ParamRoutes.Remove(_dbStore.ParamRouteSet.Find(pr.Id));
                    _dbStore.ParamRouteSet.Remove(_dbStore.ParamRouteSet.Find(pr.Id));
                }

                var parWithValueChanged = maxPermitedLimit.ParamRoutes.Where(pr => lmp.ParamRoutes.Any(m => m.Id == pr.Id && (m.Value != pr.Value || m.DecimalsPoints != pr.DecimalsPoints) && m.Id != 0));
                foreach (var pr in parWithValueChanged.ToList())
                {
                    _dbStore.ParamRouteSet.Find(pr.Id).Value = pr.Value;
                    _dbStore.ParamRouteSet.Find(pr.Id).DecimalsPoints = pr.DecimalsPoints;
                }

                var paramRoutetoadd = maxPermitedLimit.ParamRoutes.Where(gr => lmp.ParamRoutes.All(m => m.Id != gr.Id) && gr.Id != 0);
                foreach (var g in paramRoutetoadd.ToList())
                    lmp.ParamRoutes.Add(_dbStore.ParamRouteSet.Find(g.Id));

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [HttpPost, ActionName("RefreshList")]
        public JsonResult RefreshList(int page, int pageSize, string searchGeneral, bool alta = true, bool baja = false)
        {
            try
            {

                dynamic currUsr = Session["curr_User"];
                int userId = currUsr.Id;

                var sucursales = User.Identity.Name == "root"
                    ? _dbStore.SucursalSet.Where(s => s.Active)
                    : _dbStore.SucursalSet.Where(s => s.Active &&
                                s.Users.Any(u => u.Id.Equals(userId)));

                var total = _dbStore.MaxPermitedLimitSet.Count(e => (alta && baja || alta && e.Active || baja && !e.Active) &&
                                                                    (string.IsNullOrEmpty(searchGeneral) || (e.Name.ToUpper().Contains(searchGeneral.ToUpper()))) &&
                                                                    e.ParamRoutes.Any(pr => sucursales.Any(suc => (suc.Id == pr.Parameter.SucursalRealiza.Id ||
                                                                    pr.Parameter.SucursalVende != null && suc.Id == pr.Parameter.SucursalVende.Id))));

                var maxPermitedLimit = _dbStore.MaxPermitedLimitSet.Where(e => ((alta && baja) || (alta && e.Active) || (baja && !e.Active)) &&
                                                                    (string.IsNullOrEmpty(searchGeneral) || (e.Name.ToUpper().Contains(searchGeneral.ToUpper()))) &&
                                                                    e.ParamRoutes.Any(pr => sucursales.Any(suc => (pr.Parameter.SucursalRealiza != null && suc.Id == pr.Parameter.SucursalRealiza.Id ||
                                                                    pr.Parameter.SucursalVende != null && suc.Id == pr.Parameter.SucursalVende.Id))))
                                                                    .OrderBy(e => e.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                                                                    .Select(e => e.ToJson());

                return Json(new { success = true, elements = maxPermitedLimit, total });
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
                var lmp = _dbStore.MaxPermitedLimitSet.Find(id);
                if (lmp == null)
                    return Json(new { success = false, details = "No se encontró info de la empresa" });

                lmp.Active = active;


                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckMaxPermitedLimitName")]
        public JsonResult CheckMaxPermitedLimitName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.MaxPermitedLimitSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);

                var isIndb = _dbStore.MaxPermitedLimitSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));

                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }



        [HttpPost, ActionName("GetAllElements")]
        public JsonResult GetAllElements(int page, int pageSize, int Id, string searchGeneral, bool viewPackages, bool viewGroups, bool viewParameters)
        {
            try
            {
                var elemList = new List<dynamic>();
                int pkListCount = 0, gListCount = 0;
                dynamic currUsr = Session["curr_User"];
                int userId = currUsr.Id;
                var sucursales = _dbStore.SucursalSet.Where(s => s.Active
                    && (userId == 0 || s.Users.Any(x => x.Id.Equals(userId)))
                    && s.SucursalRealizaParams.Any(p => p.Active)
                    || s.SucursalVendeParams.Any(p => p.Active));



                //// Tomando todos los parametros de las sucursales del usuario activo
                //var parameters = _dbStore.ParamSet.Where(p => p.Active &&
                //            sucursales.Any(s => (p.SucursalRealiza != null
                //                            && s.Id.Equals(p.SucursalRealiza.Id))
                //                            || (p.SucursalVende != null
                //                            && s.Id.Equals(p.SucursalVende.Id))));

                //// Tomando todos los grupos que tengan alguno de los parametros anteriores
                //var groups = _dbStore.GroupSet.Where(g => g.Active &&
                //            g.Parameters.Any(p => parameters.Any(p1 => p1.Id.Equals(p.Id))));

                //var package =
                //    _dbStore.PackageSet.Where(
                //        pk => pk.Active && (
                //        pk.Parameters.Any(p => parameters.Any(p1 => p1.Id.Equals(p.Id) ||
                //        pk.Groups.Any(g => (groups.Any(g1 => g1.Id.Equals(g.Id))))))));


                var packageCount = _dbStore.PackageSet.Count(pk => pk.Active && viewPackages && (pk.Parameters.Any(p => p.Active && (p.SucursalVende != null &&
                                                                                                                  sucursales.Any(s => s.Id.Equals(p.SucursalVende.Id)) ||
                                                                                                     p.SucursalRealiza != null && sucursales.Any(s => s.Id.Equals(p.SucursalRealiza.Id)))) ||
                                                                   pk.Groups.Any(g => g.Active && g.Parameters.Any(p => p.Active && (p.SucursalVende != null &&
                                                                                                                                     sucursales.Any(s => s.Id.Equals(p.SucursalVende.Id))) ||
                                                                                                                        p.SucursalRealiza != null && sucursales.Any(s => s.Id.Equals(p.SucursalRealiza.Id))))) &&
                                                                   (string.IsNullOrEmpty(searchGeneral) || pk.Name.ToLower().Contains(searchGeneral.ToLower()) ||
                                                                   pk.Description.ToUpper().Contains(searchGeneral.ToUpper())) && viewPackages);


                var groupCount = _dbStore.GroupSet.Count(g => g.Active && viewGroups &&/*!g.Packages.Any() &&*/
                                                         g.Parameters.Any(p => p.Active &&
                                                         (p.SucursalVende != null && sucursales.Any(s => s.Id.Equals(p.SucursalVende.Id)) ||
                                                          p.SucursalRealiza != null && sucursales.Any(s => s.Id.Equals(p.SucursalRealiza.Id)))) &&
                                                            (string.IsNullOrEmpty(searchGeneral) || g.Name.ToLower().Contains(searchGeneral.ToLower())) && viewGroups);

                var paramCount =
                    _dbStore.ParamSet.Count(
                        p =>
                            p.Active && viewParameters &&/*!p.Groups.Any() && !p.Packages.Any() &&*/
                            (p.SucursalVende != null && sucursales.Any(s => s.Id.Equals(p.SucursalVende.Id)) ||
                             p.SucursalRealiza != null && sucursales.Any(s => s.Id.Equals(p.SucursalRealiza.Id))) &&
                             (string.IsNullOrEmpty(searchGeneral) ||
                              p.BaseParam.Name.ToLower().Contains(searchGeneral.ToLower())) && viewParameters);


                var lmpElem = new List<dynamic>();

                if (page == 1 && Id > 0)
                {
                    var pkList =
                         _dbStore.ParamRouteSet.Where(pr => pr.MaxPermitedLimit.Id == Id && pr.Package != null)
                                                    .Select(pr => pr.Package).Distinct().ToList()
                                                    .Select(pk => pk.ToMiniJson(_dbStore.ParamRouteSet.Where(pro => pro.MaxPermitedLimit.Id == Id && pro.Package != null)));

                    var gList =
                        _dbStore.ParamRouteSet.Where(pr => pr.MaxPermitedLimit.Id == Id && pr.Package == null && pr.Group != null)
                                            .Select(pr => pr.Group).Distinct().ToList()
                                            .Select(g => g.ToMiniJson(_dbStore.ParamRouteSet.Where(pro => pro.MaxPermitedLimit.Id == Id && pro.Package == null && pro.Group != null)));


                    var pList =
                        _dbStore.ParamRouteSet.Where(
                            pr => pr.MaxPermitedLimit.Id == Id && pr.Package == null && pr.Group == null)
                                            .Select(pr => pr.Parameter).Distinct().ToList()
                                            .Select(p => p.ToMiniJson(_dbStore.ParamRouteSet.FirstOrDefault(pro => pro.MaxPermitedLimit.Id == Id && pro.Package == null && pro.Group == null && pro.Parameter.Id == p.Id)));

                    lmpElem.AddRange(pkList);
                    lmpElem.AddRange(gList);
                    lmpElem.AddRange(pList);
                }

                int skip = (page - 1) * pageSize;

                if (packageCount - skip >= 0 && viewPackages)
                {
                    var packageList = _dbStore.PackageSet.Where(pk => pk.Active && (pk.Parameters.Any(p => p.Active && (p.SucursalVende != null &&
                                                                                                                  sucursales.Any(s => s.Id.Equals(p.SucursalVende.Id))) ||
                                                                                                     p.SucursalRealiza != null && (sucursales.Any(s => s.Id.Equals(p.SucursalRealiza.Id)))) ||
                                                                   pk.Groups.Any(g => g.Active && g.Parameters.Any(p => p.Active && (p.SucursalVende != null &&
                                                                                                                                     sucursales.Any(s => s.Id.Equals(p.SucursalVende.Id))) ||
                                                                                                                        p.SucursalRealiza != null && (sucursales.Any(s => s.Id.Equals(p.SucursalRealiza.Id)))))) &&
                                                                   (string.IsNullOrEmpty(searchGeneral) || pk.Name.ToLower().Contains(searchGeneral.ToLower())))
                                                         .OrderBy(pk => pk.Name)
                                                         .Skip(skip)
                                                         .Take(pageSize).ToList()
                                                         .Select(pk => pk.ToMiniJson(Id == 0
                                                                                              ? null
                                                                                              : _dbStore.ParamRouteSet.Where(pr => pr.MaxPermitedLimit.Id == Id &&
                                                                                                                                   pr.Package != null &&
                                                                                                                                   pk.Id == pr.Package.Id).AsEnumerable()));


                    if (packageList.Any())
                    {

                        if (packageList.Count() == pageSize)
                            return Json(new { success = true, elements = packageList, elemUsedByLMP = lmpElem, total = packageCount + groupCount + paramCount });

                        elemList.AddRange(packageList.ToList());
                    }
                    pkListCount = packageList.Count();
                }

                int skipGroup = skip - packageCount < 0 ? 0 : skip - packageCount;
                int takeGroup = pageSize - pkListCount;

                if (takeGroup >= 0 && viewGroups)
                {

                    var groupList = _dbStore.GroupSet.Where(g => g.Active && /*!g.Packages.Any() &&*/
                                                         g.Parameters.Any(p => p.Active &&
                                                         (p.SucursalVende != null && sucursales.Any(s => s.Id.Equals(p.SucursalVende.Id)) ||
                                                          p.SucursalRealiza != null && sucursales.Any(s => s.Id.Equals(p.SucursalRealiza.Id)))) &&
                                                            (string.IsNullOrEmpty(searchGeneral) || g.Name.ToLower().Contains(searchGeneral.ToLower())))
                                                     .OrderBy(g => g.Name)
                                                     .Skip(skipGroup)
                                                     .Take(takeGroup).ToList()
                                                     .Select(g => g.ToMiniJson((Id == 0)
                                                                                        ? null
                                                                                        : _dbStore.ParamRouteSet.Where(pro => pro.MaxPermitedLimit.Id == Id &&
                                                                                                                             (pro.Package == null) &&
                                                                                                                             (pro.Group != null) &&
                                                                                                                              pro.Group.Id == g.Id).AsEnumerable()) /*g.ToJson(0, true)*/);


                    if (groupList.Any())
                    {

                        elemList.AddRange(groupList);
                        if (elemList.Count == pageSize)
                            return Json(new { success = true, elements = elemList, elemUsedByLMP = lmpElem, total = packageCount + groupCount + paramCount });
                    }
                    gListCount = groupList.Count();
                }

                int skipParam = skip - packageCount - groupCount < 0 ? 0 : skip - packageCount - groupCount;
                int takeParam = pageSize - pkListCount - gListCount;
                if (takeParam <= 0 && viewParameters)
                    return
                        Json(
                            new
                            {
                                success = true,
                                elements = elemList,
                                elemUsedByLMP = lmpElem,
                                total = packageCount + groupCount + paramCount
                            });
                var paramList = _dbStore.ParamSet.Where(p => p.Active && /*!p.Groups.Any() && !p.Packages.Any() &&*/
                                                             (p.SucursalVende != null && sucursales.Any(s => s.Id.Equals(p.SucursalVende.Id)) ||
                                                              p.SucursalRealiza != null && sucursales.Any(s => s.Id.Equals(p.SucursalRealiza.Id))) &&
                                                             (string.IsNullOrEmpty(searchGeneral) || p.ParamUniquekey.ToLower().Contains(searchGeneral.ToLower())))
                    .OrderBy(p => p.BaseParam.Name).Skip(skipParam).Take(takeParam).ToList()
                    .Select(p => p.ToMiniJson(_dbStore.ParamRouteSet.FirstOrDefault(pr => pr.MaxPermitedLimit.Id == Id &&
                                                                                          pr.Package == null &&
                                                                                          pr.Group == null &&
                                                                                          pr.Parameter.Id == p.Id), 0));

                if (paramList.Any())
                    elemList.AddRange(paramList);
                return Json(new { success = true, elements = elemList, elemUsedByLMP = lmpElem, total = packageCount + groupCount + paramCount });
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

        internal dynamic PackageToJason(int matrix, int level, Package pk)
        {
            var packChildren = new List<dynamic>();


            return new
            {
                //check = _dbStore.ParamRouteSet.Any(r => r.Matrix.Id == matrix && r.Package == pk.Id),
                //pk.Id,
                //elemType = "package",
                //pk.Name,
                //level,
                //children = packChildren.Concat(pk.Groups.Where(g => g.Active).Select(g => GroupToJson(matrix, pk.Id, level + 1, g)))
                //.Concat(pk.Parameters.Select(p => ParamToJson(matrix, pk.Id, 0, level + 1, p)))
            };
        }

        internal dynamic GroupToJson(int matrixId, int packId, int level, Group g)
        {
            var groupChildren = new List<dynamic>();
            return new
            {
                //check = _dbStore.ParamRouteSet.Any(r => r.Matrix == matrixId && r.Package == 0 && r.Group == g.Id),
                //g.Id,
                //elemType = "group",
                //g.Name,
                //level,
                //children = groupChildren.Concat(g.Parameters.Select(p => ParamToJson(matrixId, packId, level + 1, g.Id, p)))
            };
        }

        internal dynamic ParamToJson(int matrixId, int packId, int groupId, int level, Param par)
        {
            //var value = (_dbStore.ParamRouteSet.FirstOrDefault(r => r.Matrix == matrixId && r.Package == packId && r.Group == groupId && par.Id == r.Parameter.Id) == null) ? 0
            //            : _dbStore.ParamRouteSet.FirstOrDefault(r => r.Matrix == matrixId && r.Package == packId && r.Group == groupId && par.Id == r.Parameter.Id).Value;



            return new
            {
                //matrixId,
                //packId,
                //groupId,
                //elemType = "parameter",
                //level,
                //check = _dbStore.ParamRouteSet.Any(r => r.Matrix == matrixId && r.Package == 0 && r.Group == 0 && r.Parameter.Id == par.Id),
                //par.Id,
                //par.BaseParam.Name,
                //MaxPermitedLimit = value

            };
        }
    }
}
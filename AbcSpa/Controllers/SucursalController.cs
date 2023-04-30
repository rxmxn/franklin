using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AbcPersistent.Models;
namespace AbcSpa.Controllers
{
    public class SucursalController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveSucursal")]
        public JsonResult SaveSucursal(Sucursal sucursal, int regionId)
        {
            try
            {
                var suc = sucursal.Id == 0 ?
                    new Sucursal()
                    {
                        Users = new List<User>(),
                        Annalists = new List<Annalist>(),
                        AnalyticsArea = new List<AnalyticsArea>(),
                        RecOtorgs = new List<RecOtorg>()
                    }
                    : _dbStore.SucursalSet.Find(sucursal.Id);

                suc.Name = sucursal.Name;
                suc.Active = sucursal.Active;
                suc.Description = sucursal.Description;
                suc.Key = sucursal.Key;
                suc.Vende = sucursal.Vende;
                suc.Realiza = sucursal.Realiza;
                suc.SucursalIntelesis = sucursal.SucursalIntelesis;
                suc.SucursalAutolab = sucursal.SucursalAutolab;

                // Office: Empresa a la que pertenece
                //suc.Office = _dbStore.OfficeSet.Find(officeId);
                suc.Region = _dbStore.RegionSet.Find(regionId);

				if (!checkOffices(sucursal, suc))
					throw new Exception("Error al llenar la lista de empresas");

				if (sucursal.Id != 0)
                {
                    if (!CheckAnalyticsAreas(sucursal, suc))
                        throw new Exception("Error al llenar la lista de centros de costo");
                }
                else
                {
                    foreach (var aa in sucursal.AnalyticsArea)
                        suc.AnalyticsArea.Add(_dbStore.AnalyticsAreaSet.Find(aa.Id));

                    _dbStore.SucursalSet.Add(suc);
                }

                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

		private bool checkOffices(Sucursal sucursal, Sucursal suc)
		{
			try
			{
				var todelete = suc.Offices.Where(nrm => sucursal.Offices.All(m => m.Id != nrm.Id));
				foreach (var pr in todelete.ToList())
					suc.Offices.Remove(_dbStore.OfficeSet.Find(pr.Id));

				var toadd = sucursal.Offices.Where(m => suc.Offices.All(nrm => nrm.Id != m.Id));

				foreach (var g in toadd.ToList())
					suc.Offices.Add(_dbStore.OfficeSet.Find(g.Id));

				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}

		private bool CheckAnalyticsAreas(Sucursal sucursal, Sucursal suc)
        {
            try
            {
                // La diferencia entre sucursal y suc es que sucursal es el que 
                // viene de la vista y suc es el que esta guardado en BD que se 
                // esta editando.
                var aaDelete = suc.AnalyticsArea
                    .Where(s => s.Active && sucursal.AnalyticsArea.All(a => a.Id != s.Id));

                // Todos los elementos que esten en BD que no vengan de la vista se borran.
                foreach (var aa in aaDelete.ToList())
                    suc.AnalyticsArea.Remove(_dbStore.AnalyticsAreaSet.Find(aa.Id));

                // Todos los elementos que vengan y no esten en BD se adicionan.
                var aaAdd = sucursal.AnalyticsArea
                    .Where(s => s.Active && suc.AnalyticsArea.All(a => a.Id != s.Id));

                foreach (var ar in aaAdd)
                    suc.AnalyticsArea.Add(_dbStore.AnalyticsAreaSet.Find(ar.Id));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpPost, ActionName("RefreshList")]
        public JsonResult RefreshList(int page, int pageSize,
                            string searchGeneral, int? bLine, int? regionId,
                            int? OfficeId, int? ackId, int? AnAreaId,
                            bool alta = true, bool baja = false,
                            bool vende = true, bool realiza = true)
        {
            try
            {
                var total = _dbStore.SucursalSet.Count(a => (alta && baja ||
                            alta && a.Active || baja && !a.Active) &&
                            (vende && realiza || vende && a.Vende.Equals(vende) || realiza && a.Realiza.Equals(realiza)) &&
                            (string.IsNullOrEmpty(searchGeneral) || a.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            a.Description.ToUpper().Contains(searchGeneral.ToUpper())) &&
                            (bLine == null || a.Offices.Any(o => o.Market.Id == bLine)) &&
                            (regionId == null || a.Region.Id == regionId) &&
                            (OfficeId == null || a.Offices.Any(o => o.Id == OfficeId)) &&
                            (ackId == null) &&
                            (AnAreaId == null || a.AnalyticsArea.Any(aa => aa.Id == AnAreaId)));

                var sucursals = _dbStore.SucursalSet.Where(a => (alta && baja ||
                            alta && a.Active || baja && !a.Active) &&
                            (vende && realiza || vende && a.Vende.Equals(vende) || realiza && a.Realiza.Equals(realiza)) &&
                            (string.IsNullOrEmpty(searchGeneral) || a.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            a.Description.ToUpper().Contains(searchGeneral.ToUpper())) &&
                            (bLine == null || a.Offices.Any(o => o.Market.Id == bLine)) &&
                            (regionId == null || a.Region.Id == regionId) &&
                            (OfficeId == null || a.Offices.Any(o => o.Id == OfficeId)) &&
                            (ackId == null) &&
                            (AnAreaId == null || a.AnalyticsArea.Any(aa => aa.Id == AnAreaId)))
                    .OrderBy(a => a.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList()
                    .Select(a => a.ToJson());

                return Json(new { success = true, elements = sucursals, total });
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

                var regionList = sucursales.Select(s => s.Region).Distinct().Select(r => new { r.Id, r.Name });
                var officeList = sucursales.SelectMany(s => s.Offices).Distinct().Select(o => new { o.Id, o.Name });
                var marketList = sucursales.SelectMany(s => s.Offices).Select(o => o.Market).Distinct().Select(mkt => new { mkt.Id, mkt.Name });
                //var recList =
                //    sucursales.SelectMany(s => s.RecOtorgs.Select(ro => ro))
                //        .Distinct()
                //        .Select(ro => new {ro.Id, ro.Ack.Name});
                var annAreaList =
                    sucursales.Where(s => s.AnalyticsArea.Any())
                        .SelectMany(s => s.AnalyticsArea.Where(aa => aa.Active))
                        .Distinct()
                        .Select(aa => new { aa.Id, aa.Key });


                return Json(new { success = true, regionList, officeList, marketList, annAreaList });
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
                var suc = _dbStore.SucursalSet.Find(id);
                if (suc == null)
                    return Json(new { success = false, details = "No se encontro info del sucursal" });

                suc.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckSucursalName")]
        public JsonResult CheckSucursalName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.SucursalSet.Find(id).Key.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);

                var isIndb = _dbStore.SucursalSet.FirstOrDefault(p => p.Key.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));

                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpPost, ActionName("GetSucursales")]
        public JsonResult GetSucursales(bool activeOption = true,
            bool vende = true, bool realiza = true)
        {
            try
            {
                var total = _dbStore.SucursalSet.Count(a => a.Active.Equals(activeOption) &&
                            ((vende && realiza) || (vende && a.Vende.Equals(vende)) || (realiza && a.Realiza.Equals(realiza))));

                var sucursals = _dbStore.SucursalSet.Where(a => a.Active.Equals(activeOption) &&
                            ((vende && realiza) || (vende && a.Vende.Equals(vende)) || (realiza && a.Realiza.Equals(realiza))))
                    .OrderBy(a => a.Name).ToList()
                    .Select(a => a.ToJson());

                return Json(new { success = true, elements = sucursals, total });
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

        [HttpPost, ActionName("GetAnalyticsArea")]
        public JsonResult GetAnalyticsArea(int sucursalId, bool activeOption = true)
        {
            try
            {
                var total = _dbStore.AnalyticsAreaSet.Count(aa => aa.Active.Equals(activeOption)
                    && (aa.Sucursal.Id.Equals(sucursalId) || (aa.Sucursal == null)));

                var analyticsArea = _dbStore.AnalyticsAreaSet.Where(aa => aa.Active.Equals(activeOption)
                    && (aa.Sucursal.Id.Equals(sucursalId) || (aa.Sucursal == null)))
                    .OrderBy(aa => aa.Key).ToList().Select(aa => aa.ToJson());

                return Json(new { success = true, elements = analyticsArea, total });
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

        [HttpPost, ActionName("GetSucursalesToFilter")]
        public JsonResult GetSucursalesToFilter(bool x, bool y, bool recVr)
        {
            try
            {
                dynamic currUsr = Session["curr_User"];
                int userId = currUsr.Id;

                var sucursales = _dbStore.SucursalSet.Where(s => s.Active
                    && (userId == 0 || s.Users.Any(u => u.Id.Equals(userId)))
                    && s.SucursalRealizaParams.Any(p => p.Active)
                    || s.SucursalVendeParams.Any(p => p.Active));

                // Si recVr == false, entonces se hara el filtro por reconocimiento, 
                // else por vende/realiza.
                var sucursals = sucursales.Where(a => (recVr ||
                    ((x && y) || (x && a.RecOtorgs.Any(ro => ro.Enterprise != null)) ||
                    (y && a.RecOtorgs.All(ro => ro.Enterprise == null)))) &&
                    (!recVr || ((x && y) || (x && a.Vende) || (y && a.Realiza))))
                    .OrderBy(a => a.Name).ToList()
                    .Select(a => new { a.Id, name = a.ToString() });

                var sucursalsX = sucursales.Where(a => (recVr || (x &&
                    a.RecOtorgs.Any(ro => ro.Enterprise != null))) &&
                    (!recVr || (x && a.Vende)))
                    .OrderBy(a => a.Name).ToList()
                    .Select(a => new { a.Id, name = a.ToString() });

                var sucursalsY = sucursales.Where(a => (recVr || (y &&
                    a.RecOtorgs.All(ro => ro.Enterprise == null))) &&
                    (!recVr || (y && a.Realiza)))
                    .OrderBy(a => a.Name).ToList()
                    .Select(a => new { a.Id, name = a.ToString() });

                return Json(new
                {
                    success = true,
                    elements = sucursals,
                    elementsX = sucursalsX,
                    elementsY = sucursalsY
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

        // obtiene los reconocimientos que le han sido otorgados a una sucursal determinada.
        [HttpPost, ActionName("GetSucursalRecOtorg")]
        public JsonResult GetSucursalRecOtorg(int sucursalId)
        {
            try
            {
                var total = _dbStore.RecOtorgSet.Count(ro => ro.Sucursal.Id.Equals(sucursalId));

                var rotorg = _dbStore.RecOtorgSet.Where(ro => ro.Sucursal.Id.Equals(sucursalId))
                    .ToList().Select(ro => ro.ToJson());

                return Json(new { success = true, elements = rotorg, total });
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

        [HttpPost, ActionName("GetAnnalistParams")]
        public JsonResult GetAnnalistParams(int recOtorgId)
        {
            try
            {
                var total = _dbStore.RecOtorgSet.Count(ro => ro.Id.Equals(recOtorgId));

                var paramList = _dbStore.RecOtorgSet.Where(ro => ro.Id.Equals(recOtorgId))
                    .SelectMany(ro => ro.Params.Where(p => p.Active).Select(p => new
                    {
                        Param = p.ParamUniquekey,
                        Method = p.Metodo.Name
                    }));

                return Json(new { success = true, elements = paramList, total });
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

        [HttpPost, ActionName("GetElementsToAddRec")]
        public JsonResult GetElementsToAddRec(int sucId)
        {
            try
            {
                var acks = _dbStore.AckSet.Where(a => a.Active &&
                    (!a.VigenciaFinal.HasValue || (a.VigenciaFinal > DateTime.Now)) &&
                    a.Enterprise != null)
                    .OrderBy(a => a.Name).ToList().Select(a => a.ToJson());
                var enterprises = _dbStore.EnterpriseSet.Where(a => a.Active && 
					a.Acks.Any(ak => (ak.VigenciaFinal == null) || (ak.VigenciaFinal > DateTime.Now)))
                    .OrderBy(a => a.Name).ToList().Select(a => a.ToJson());
	            var offices = _dbStore.OfficeSet.Where(o => o.Active &&
	                o.Sucursales.Any(s => s.Id.Equals(sucId)))
					.OrderBy(o => o.Name).ToList().Select(o => new {o.Id, o.Name, Mercado = o.Market.Name});

                return Json(new { success = true, acks, enterprises, offices });
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

        [HttpPost, ActionName("GetAnnalistToAddRec")]
        public JsonResult GetAnnalistToAddRec(int sucursalId)
        {
            try
            {
                var total = _dbStore.SucursalSet.Where(s => s.Active && s.Id.Equals(sucursalId))
                    .Sum(s => s.Annalists.Count(a => a.Active));

                var annalists = _dbStore.SucursalSet.Where(s => s.Active && s.Id.Equals(sucursalId)).ToList()
                    .SelectMany(s => s.Annalists.Where(a => a.Active).ToList()
                    .Select(a => new
                    {
                        a.Id,
                        Name = a.ToString(),
                        a.Key,//Key=a.AnnalistKey?.Clave,
                        a.Gender,
                        a.Photo,
                        Sucursales = a.Sucursales.Select(u => u.ToJson()),
                        RecAdqs = a.RecAdqs.Select(ra => ra.ToJson()),
                        Params = a.Params.Where(p => p.Active).Select(p => new
                        {
                            p.Id,
                            p.ParamUniquekey,
                            Metodo = p.Metodo.Name
                        })
                    }));

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

        [HttpPost, ActionName("GetTiposSignatario")]
        public JsonResult GetTiposSignatario()
        {
            try
            {
                var total = _dbStore.TipoSignatarioSet.Count();

                var tss = _dbStore.TipoSignatarioSet.ToList().Select(ts => ts.ToJson());

                return Json(new { success = true, elements = tss, total });
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

        [HttpPost, ActionName("GetAnnalistRecAdqs")]
        public JsonResult GetAnnalistRecAdqs(int annalistId)
        {
            try
            {
                var annalist = _dbStore.AnnalistSet.FirstOrDefault(a => a.Id.Equals(annalistId));
                if (annalist == null) return Json(new { success = true, total = 0 });

                var total = annalist.RecAdqs.Count;

                var recAdqs = _dbStore.AnnalistSet.Where(a => a.Id.Equals(annalistId)).ToList()
                    .SelectMany(a => a.RecAdqs.Select(ra => ra.ToJson()));

                return Json(new { success = true, elements = recAdqs, total });
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

        [HttpPost, ActionName("AddReconocimiento")]
        public JsonResult AddReconocimiento(RecOtorg recOtorg, IEnumerable<RecAdq> recAdqs)
        {
            try
            {
                foreach (var recAdq in recAdqs)
                {
                    // hay que analizar aqui si se modifica uno de estos valores, que se crearia
                    // un objeto nuevo, y no se si eso es lo que se desea. De ser asi, lo que hay
                    // que hacer es sacar la asignacion de TipoSignatario fuera del new para que 
                    // se modifique en caso de ser el mismo Annalist con el mismo NivelAdquirido.
                    // Para esto, quitar de la consulta (tS == 0 || ra.TipoSignatario.Id == tS).
                    var tS = 0;
                    if (recAdq.TipoSignatario != null)
                        tS = recAdq.TipoSignatario.Id;

                    var rAdq = _dbStore.RecAdqSet.FirstOrDefault(ra =>
                        ra.Annalist.Id == recAdq.Annalist.Id &&
                        ra.NivelAdquirido == recAdq.NivelAdquirido &&
                        (tS == 0 || ra.TipoSignatario.Id == tS)) ??
                        new RecAdq
                        {
                            RecOtorgs = new List<RecOtorg>(),
                            NivelAdquirido = recAdq.NivelAdquirido,
                            TipoSignatario = tS != 0 ? _dbStore.TipoSignatarioSet.Find(tS) : null,
                            Annalist = _dbStore.AnnalistSet.Find(recAdq.Annalist.Id)
                        };

                    var ro = _dbStore.RecOtorgSet.FirstOrDefault(r =>
                        r.RecAdq.Id.Equals(rAdq.Id) &&
                        recOtorg.Sucursal.Id.Equals(r.Sucursal.Id) &&
						recOtorg.Office.Id.Equals(r.Office.Id) &&
						recOtorg.Enterprise.Id.Equals(r.Enterprise.Id) &&
                        recOtorg.Ack.Id.Equals(r.Ack.Id)) ??
                        new RecOtorg
                        {
                            Params = new List<Param>(),
                            Sucursal = _dbStore.SucursalSet.Find(recOtorg.Sucursal.Id),
							Office = _dbStore.OfficeSet.Find(recOtorg.Office.Id),
							Enterprise = recOtorg.Enterprise.Id != 0
                            ? _dbStore.EnterpriseSet.Find(recOtorg.Enterprise.Id) : null,
                            Ack = _dbStore.AckSet.Find(recOtorg.Ack.Id)
                        };

                    foreach (var par in recAdq.Annalist.Params)
                        ro.Params.Add(_dbStore.ParamSet.Find(par.Id));

                    var flag = false;
                    if (ro.Id == 0)
                    {
                        _dbStore.RecOtorgSet.Add(ro);
                        flag = true;
                    }

                    _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                    if (flag)
                        rAdq.RecOtorgs.Add(_dbStore.RecOtorgSet.Find(ro.Id));

                    if (rAdq.Id == 0)
                        _dbStore.RecAdqSet.Add(rAdq);
                }

                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using AbcPersistent.Models;
using Microsoft.Ajax.Utilities;

namespace AbcSpa.Controllers
{
    public class AckController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();
		
		[HttpPost, ActionName("SaveAck")]
        public JsonResult SaveAck(Ack ack, string fromDate, string untilDate)
        {
            try
            {
				var ac = ack.Id == 0 
					? new Ack()
					{
						RecOtorgs = new List<RecOtorg>()
					}
					: _dbStore.AckSet.Find(ack.Id);

				ac.Name = ack.Name;
				ac.Active = ack.Active;
				ac.Description = ack.Description;
				ac.Key = ack.Key;
	            //ac.Estado = ack.Estado;
	            ac.Pdf = ack.Pdf;
	            ac.AlertaDias = ack.AlertaDias;
				ac.Enterprise = ack.Enterprise != null ? _dbStore.EnterpriseSet.Find(ack.Enterprise.Id) : null;
				ac.Alcance = ack.Alcance != null ? _dbStore.AlcanceSet.Find(ack.Alcance.Id) : null;
				ac.Accion = ack.Accion != null ? _dbStore.AccionSet.Find(ack.Accion.Id) : null;
				
				if (!fromDate.IsEmpty())
				{
					var dateData = fromDate.Substring(0, 10).Split('/');
					ac.VigenciaInicial = new DateTime(Int32.Parse(dateData[2]), Int32.Parse(dateData[1]), Int32.Parse(dateData[0]));
				}

	            if (!untilDate.IsEmpty())
	            {
					var dateData = untilDate.Substring(0, 10).Split('/');
					ac.VigenciaFinal = new DateTime(Int32.Parse(dateData[2]), Int32.Parse(dateData[1]), Int32.Parse(dateData[0]));
					ac.VigenciaFinal = ac.VigenciaFinal.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
				}

				//if (!CheckEnterprisesList(ac, ack))
				//	throw new Exception("Error al llenar la lista de Instituciones");

				if (ack.Enterprise != null)
				{
					var rama = ack.Enterprise.Rama != null && ack.Enterprise.Rama.Id != 0
						? _dbStore.RamaSet.Find(ack.Enterprise.Rama.Id) : null;
					if (ac.Enterprise != null && rama == null)
					{
						rama = _dbStore.RamaSet.Find(ac.Enterprise.Rama.Id);
						rama.Enterprises.Remove(_dbStore.EnterpriseSet.Find(ac.Enterprise.Id));
						ac.Enterprise.Rama = null;
					} else if (ac.Enterprise != null && rama != null)
					{
						ac.Enterprise.Rama = rama;
					}
				}

					//ac.Enterprises.First().Rama = ack.Enterprises.First().Rama != null && ack.Enterprises.First().Rama.Id != 0 
					//	? _dbStore.RamaSet.Find(ack.Enterprises.First().Rama.Id) : null;
				
				if (ack.Id == 0)
                    _dbStore.AckSet.Add(ac);

				_dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

		//private bool CheckEnterprisesList(Ack ac, Ack ack)
		//{
		//	try
		//	{
		//		var enterprisetodelete = ac.Enterprises.Where(e => ack.Enterprises.All(m => m.Id != e.Id) && e.Active);

		//		foreach (var ed in enterprisetodelete.ToList())
		//			ac.Enterprises.Remove(_dbStore.EnterpriseSet.Find(ed.Id));

		//		var enterprisetoadd = ack.Enterprises.Where(e => ac.Enterprises.All(m => m.Id != e.Id));

		//		foreach (var ea in enterprisetoadd.ToList())
		//			ac.Enterprises.Add(_dbStore.EnterpriseSet.Find(ea.Id));

		//		return true;
		//	}
		//	catch (Exception)
		//	{
		//		return false;
		//	}
		//}

		[HttpPost, ActionName("RefreshList")]
        public JsonResult RefreshList(int page, int pageSize, string searchGeneral,
                                        string ackKey, int? institutionId, 
			bool alta = true, bool baja = false,
            bool recVigentes = true, bool recExpirados = true)
        {
            try
            {
	            var total = _dbStore.AckSet.Count(a => (alta && baja ||
							alta && a.Active || baja && !a.Active) &&
					((recVigentes && recExpirados) || recVigentes.Equals(a.VigenciaFinal > DateTime.Now)
					|| recExpirados.Equals(a.VigenciaFinal < DateTime.Now)) &&
                    (string.IsNullOrEmpty(searchGeneral) || a.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            a.Description.ToUpper().Contains(searchGeneral.ToUpper())) &&
                            (string.IsNullOrEmpty(ackKey) || a.Key.ToUpper().Contains(ackKey.ToUpper())) &&
                            (institutionId==null || a.Enterprise.Id == institutionId));

                var acks = _dbStore.AckSet.Where(a => (alta && baja ||
                            alta && a.Active || baja && !a.Active) &&
					((recVigentes && recExpirados) || recVigentes.Equals(a.VigenciaFinal > DateTime.Now)
                    || recExpirados.Equals(a.VigenciaFinal < DateTime.Now)) &&
                    (string.IsNullOrEmpty(searchGeneral) || a.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
                            a.Description.ToUpper().Contains(searchGeneral.ToUpper())) &&
                            (string.IsNullOrEmpty(ackKey) || a.Key.ToUpper().Equals(ackKey.ToUpper())) &&
                            (institutionId == null || a.Enterprise.Id==institutionId))
                    .OrderBy(a => a.Name).Skip((page - 1)*pageSize).Take(pageSize).ToList()
		            .Select(a => a.ToJson());
				
                return Json(new { success = true, elements = acks, total });
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
                var ackKeyList = _dbStore.AckSet.Where(ak => ak.Active).Select(ak => new { ak.Key }).Distinct();
                var institutionList =
                    _dbStore.EnterpriseSet.Where(e => e.Active && e.Acks.Any())
                        .Select(e => new {e.Id, e.Name});

                return Json(new { success = true, ackKeyList, institutionList });
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
                var ac = _dbStore.AckSet.Find(id);
                if (ac == null)
                    return Json(new { success = false, details = "No se encontró info del reconocimiento" });

                ac.Active = active;
                _dbStore.SaveChanges(User.Identity.Name, Request.UserHostName);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpGet, ActionName("CheckAckName")]
        public JsonResult CheckAckName(string uniqueInput, int id)
        {
            try
            {
                if (id != 0)
                    if (_dbStore.AckSet.Find(id).Name.ToUpper() == uniqueInput.ToUpper())
                        return Json(new { success = true, data = false }, JsonRequestBehavior.AllowGet);
                
                var isIndb = _dbStore.AckSet.FirstOrDefault(p => p.Name.ToUpper().Equals(uniqueInput.ToUpper(), StringComparison.CurrentCultureIgnoreCase));
                
                //it returns true if it exists
                return Json(new { success = true, data = (isIndb != null) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        // Hace lo mismo basicamente que RefreshList pero no recibe page para el paginador
        // Si no se usara paginador por parte del server analizar en borrar esta funcion y quedarnos con RefreshList
        // Fijarme en lo que hice en RegionController y MarketController.
        [HttpPost, ActionName("GetAck")]
        public JsonResult GetAck(bool activeOption = true)
        {
            try
            {
	            var total = _dbStore.AckSet.Count(r => r.Active == activeOption);

                var ack = _dbStore.AckSet.Where(r => r.Active == activeOption)
					.OrderBy(a => a.Name).ToList()
					.Select(a => a.ToJson());
                
                return Json(new { success = true, elements = ack, total });
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

		[HttpPost, ActionName("GetAckParams")]
		public JsonResult GetAckParams(int ackId)
		{
			try
			{
				var paramList = _dbStore.RecOtorgSet.Where(ro => ro.Ack.Id.Equals(ackId)).ToList()
					.SelectMany(rot => rot.Params.Where(p => p.Active)
					.Select(par => new
					{
						Param = par.ParamUniquekey,
						Signatarios = par.Annalists.Where(a => a.Active && a.RecAdqs
							.Any(ra => ra.NivelAdquirido.Equals(RecAdq.AcquiredLevel.Signatario)
							&& ra.RecOtorgs.Any(ro => ro.Enterprise.Tipo
							&& ro.Ack.Id.Equals(ackId)
							&& ro.Params.Any(p => p.ParamUniquekey.Equals(par.ParamUniquekey))))/* && a.AnnalistKey != null*/)
							.Select(a => a.Key/*a.AnnalistKey.Clave*/),
						Eidas = par.Annalists.Where(a => a.Active && a.RecAdqs
							.Any(ra => ra.NivelAdquirido.Equals(RecAdq.AcquiredLevel.Eidas)
							&& ra.RecOtorgs.Any(ro => !ro.Enterprise.Tipo
							&& ro.Ack.Id.Equals(ackId)
							&& ro.Params.Any(p => p.ParamUniquekey.Equals(par.ParamUniquekey)))) /*&& a.AnnalistKey != null*/)
							.Select(a => a.Key/*a.AnnalistKey.Clave*/)
					})).DistinctBy(p => p.Param);
				
				return Json(new { success = true, elements = paramList });
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

		[HttpPost, ActionName("GetAckInfo")]
		public JsonResult GetAckInfo(string ackKey)
		{
			try
			{
				var ack = string.IsNullOrEmpty(ackKey) ? null
					: _dbStore.AckSet.FirstOrDefault(a => a.Key.ToUpper().Equals(ackKey.ToUpper(), StringComparison.CurrentCultureIgnoreCase));
				var elements = ack?.ToJson();

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
	}
}
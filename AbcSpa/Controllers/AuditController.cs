using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using AbcPersistent.Models;

namespace AbcSpa.Controllers
{
    public class AuditController : Controller
    {
		private readonly AbcContext _dbStore = new AbcContext();

		[HttpPost, ActionName("RefreshAudit")]
		public JsonResult RefreshAudit(int page, int pageSize, string searchGeneral,
			string fromDate, string untilDate, string searchIp)
		{
			try
			{
				DateTime fromD = new DateTime(), untilD = new DateTime();
				if (!fromDate.IsEmpty() && !untilDate.IsEmpty())
				{
					var dateData = fromDate.Substring(0, 10).Split('/');
					fromD = new DateTime(Int32.Parse(dateData[2]), Int32.Parse(dateData[1]), Int32.Parse(dateData[0]));
					dateData = untilDate.Substring(0, 10).Split('/');
					untilD = new DateTime(Int32.Parse(dateData[2]), Int32.Parse(dateData[1]), Int32.Parse(dateData[0]));
					untilD = untilD.AddHours(23).AddMinutes(59).AddSeconds(59);
				}

				// para que no importe si esta en mayusculas (la forma mas optima (mas que ToUpper))
				// (u.User.Name.IndexOf(searchGeneral, StringComparison.OrdinalIgnoreCase) >= 0)
				// pero se lanza una excepcion por usarlo dentro de un LINQ

				var total = _dbStore.AuditRecords.Where(u =>
							(string.IsNullOrEmpty(fromDate) && string.IsNullOrEmpty(untilDate) ||
							((u.EventDate >= fromD) && (u.EventDate <= untilD)))
							&& ((searchGeneral == "") ||
								(u.TableName.ToUpper().Contains(searchGeneral.ToUpper()) ||
								u.UserName.ToUpper().Contains(searchGeneral.ToUpper())))
							&& ((searchIp == "") || (u.IpAddress.Contains(searchIp))))
							.GroupBy(u => u.TableName).Count();

				var auditList = _dbStore.AuditRecords.Where(u =>
							(string.IsNullOrEmpty(fromDate) && string.IsNullOrEmpty(untilDate) ||
							((u.EventDate >= fromD) && (u.EventDate <= untilD)))
							&& ((searchGeneral == "") ||
								(u.TableName.ToUpper().Contains(searchGeneral.ToUpper()) ||
								u.UserName.ToUpper().Contains(searchGeneral.ToUpper())))
							&& ((searchIp == "") || (u.IpAddress.Contains(searchIp))))
					.GroupBy(u => u.TableName)
					.OrderBy(u => u.Key).Skip((page - 1) * pageSize).Take(pageSize).ToList()
					.Select(u =>
					{
						var last = u.OrderBy(d => d.EventDate).Last();
						return new
						{
							TableName = u.Key,
							Info = new
							{
								last.UserName,
								last.IpAddress,
								EventDate = last.EventDate.ToString("dd/MM/yyyy HH:mm")
							}
						};
					}).ToList();

				return Json(new { success = true, elements = auditList, total });
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

		[HttpPost, ActionName("RefreshAuditInfo")]
		public JsonResult RefreshAuditInfo(int page, int pageSize, string tableName,
			string fromDate, string untilDate, string searchGeneral, string searchIp,
			bool viewAdded, bool viewModified)
		{
			try
			{
				DateTime fromD = new DateTime(), untilD = new DateTime();
				if (!fromDate.IsEmpty() && !untilDate.IsEmpty())
				{
					var dateData = fromDate.Substring(0, 10).Split('/');
					fromD = new DateTime(Int32.Parse(dateData[2]), Int32.Parse(dateData[1]), Int32.Parse(dateData[0]));
					dateData = untilDate.Substring(0, 10).Split('/');
					untilD = new DateTime(Int32.Parse(dateData[2]), Int32.Parse(dateData[1]), Int32.Parse(dateData[0]));
					untilD = untilD.AddHours(23).AddMinutes(59).AddSeconds(59);
				}

				dynamic infoList = null;
				var total = 0;

				if (!tableName.IsEmpty())
				{
					total = _dbStore.AuditRecords.Count(u => (u.TableName == tableName)
						&& (string.IsNullOrEmpty(fromDate) && string.IsNullOrEmpty(untilDate) ||
							((u.EventDate >= fromD) && (u.EventDate <= untilD)))
						&& ((searchGeneral == "") ||
								(u.ColumnName.ToUpper().Contains(searchGeneral.ToUpper()) ||
								u.UserName.ToUpper().Contains(searchGeneral.ToUpper())))
						&& ((searchIp == "") || (u.IpAddress.Contains(searchIp)))
						&& ((viewAdded && viewModified) || 
							(viewAdded && u.EventType.Equals("A")) ||
							(viewModified && u.EventType.Equals("M"))));

					infoList = _dbStore.AuditRecords.Where(u => (u.TableName == tableName)
						&& (string.IsNullOrEmpty(fromDate) && string.IsNullOrEmpty(untilDate) ||
							((u.EventDate >= fromD) && (u.EventDate <= untilD)))
						&& ((searchGeneral == "") ||
								(u.ColumnName.ToUpper().Contains(searchGeneral.ToUpper()) ||
								u.UserName.ToUpper().Contains(searchGeneral.ToUpper())))
						&& ((searchIp == "") || (u.IpAddress.Contains(searchIp)))
						&& ((viewAdded && viewModified) ||
							(viewAdded && u.EventType.Equals("A")) ||
							(viewModified && u.EventType.Equals("M"))))
					.GroupBy(u => u.TableName).ToList()
					.SelectMany(u => u.Select(i => new
					{
						i.UserName,
						i.IpAddress,
						EventDate = i.EventDate.ToString("dd/MM/yyyy HH:mm"),
						i.ColumnName,
						i.ElementId,
						i.EventType,
						i.OriginalValue,
						i.NewValue
					}).ToList()
					).OrderByDescending(i => i.EventDate)
					.Skip((page - 1) * pageSize).Take(pageSize).ToList();
				}

				return Json(new { success = true, elements = infoList, total });
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
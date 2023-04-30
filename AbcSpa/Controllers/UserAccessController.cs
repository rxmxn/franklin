using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using AbcPersistent.Models;

namespace AbcSpa.Controllers
{
    public class UserAccessController : Controller
    {
		private readonly AbcContext _dbStore = new AbcContext();

		[HttpPost, ActionName("RefreshUserAccess")]
		public JsonResult RefreshUserAccess(int page, int pageSize, string searchGeneral,
			string fromDate, string untilDate, bool alta, bool baja, string searchIp, bool conected)
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
				//(u.User.Name.IndexOf(searchGeneral, StringComparison.OrdinalIgnoreCase) >= 0)
				// pero se lanza una excepcion por usarlo dentro de un LINQ

				var total = _dbStore.SessionLogSet.Where(u => ((alta && baja) ||
							(alta && u.User.Active) || (baja && !u.User.Active)) &&
							(string.IsNullOrEmpty(fromDate) && string.IsNullOrEmpty(untilDate) ||
							((u.StartSession >= fromD) && (u.StartSession <= untilD)))
							&& ((searchGeneral == "") ||
								(u.User.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
								u.User.LastNameFather.ToUpper().Contains(searchGeneral.ToUpper()) ||
								u.User.LastNameMother.ToUpper().Contains(searchGeneral.ToUpper()) ||
								u.User.UserName.ToUpper().Contains(searchGeneral.ToUpper())))
							&& ((searchIp == "") || (u.IpAddress.Contains(searchIp)))
							&& ((conected == false) || (u.Connected == conected)))
							.GroupBy(u => u.User.Id).Count();

				var userAccessList = _dbStore.SessionLogSet.Where(u => ((alta && baja) ||
							(alta && u.User.Active) || (baja && !u.User.Active))
					&& (string.IsNullOrEmpty(fromDate) && string.IsNullOrEmpty(untilDate) ||
							((u.StartSession >= fromD) && (u.StartSession <= untilD)))
					&& ((searchGeneral == "") ||
						(u.User.Name.ToUpper().Contains(searchGeneral.ToUpper()) ||
						u.User.LastNameFather.ToUpper().Contains(searchGeneral.ToUpper()) ||
						u.User.LastNameMother.ToUpper().Contains(searchGeneral.ToUpper()) ||
						u.User.UserName.ToUpper().Contains(searchGeneral.ToUpper())))
					&& ((searchIp == "") || (u.IpAddress.Contains(searchIp)))
					&& ((conected == false) || (u.Connected == conected)))
					.GroupBy(u => new
					{
						u.User.Id,
						UserName = u.Key,
						FullName = u.User.Name + " " + u.User.LastNameFather + " " + u.User.LastNameMother,
						u.User.Photo,
						u.User.Gender
					}).OrderBy(u => u.Key.UserName).Skip((page - 1) * pageSize).Take(pageSize).ToList()
					.Select(u =>
					{
						var last = u.Last();
						return new
						{
							User = u.Key,
							Info = new
							{
								last.IpAddress,
								StartSession = last.StartSession.ToString(),
								EndSession = last.EndSession.ToString(),
								last.Connected
							}
						};
					}).ToList();

				return Json(new { success = true, elements = userAccessList, total });
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

		[HttpPost, ActionName("RefreshUserInfo")]
		public JsonResult RefreshUserInfo(int page, int pageSize, int userId,
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

				dynamic infoList = null;
				var total = 0;

				if (userId != 0)
				{
					total = _dbStore.SessionLogSet.Count(u => (u.User.Id == userId)
						&& (string.IsNullOrEmpty(fromDate) && string.IsNullOrEmpty(untilDate) ||
							((u.StartSession >= fromD) && (u.StartSession <= untilD)))
						&& ((searchIp == "") || (u.IpAddress.Contains(searchIp))));

					infoList = _dbStore.SessionLogSet.Where(u => (u.User.Id == userId)
						&& (string.IsNullOrEmpty(fromDate) && string.IsNullOrEmpty(untilDate) ||
							((u.StartSession >= fromD) && (u.StartSession <= untilD)))
						&& ((searchIp == "") || (u.IpAddress.Contains(searchIp))))
					.GroupBy(u => u.User.Id).ToList()
					.SelectMany(u => u.Select(i => new
					{
						i.IpAddress,
						// ReSharper disable once MergeConditionalExpression
						StartSession = i.StartSession.HasValue ? i.StartSession.Value.ToString("dd/MM/yyyy HH:mm") : "",
						// ReSharper disable once MergeConditionalExpression
						EndSession = i.EndSession.HasValue ? i.EndSession.Value.ToString("dd/MM/yyyy HH:mm") : "",
						i.Connected
					}).ToList()
					).OrderByDescending(i => i.StartSession)
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
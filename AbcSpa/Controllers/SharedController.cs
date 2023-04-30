using System.Web.Mvc;

namespace AbcSpa.Controllers
{
    public class SharedController : Controller
    {
        //
        // GET: /Shared/
        
        public ActionResult Header()
        {
            return View();
        }

        public ActionResult SidebarFilter()
        {
            return View();
        }
        public ActionResult Sidebar()
        {
            return View();
        }
        public ActionResult EditParam()
        {
            return View();
        }
        public ActionResult EditGroup()
        {
            return View();
        }
        public ActionResult EditPackage()
        {
            return View();
        }
        public ActionResult EditMatrix()
        {
            return View();
        }

        public ActionResult ShowReportDialog()
        {
            return View();
        }

		public ActionResult EditNote()
		{
			return View();
		}

        public ActionResult EditLMP()
        {
            return View();
        }

		public ActionResult ChangeCC()
		{
			return View();
		}

        public ActionResult ShowLMP()
        {
            return View();
        }

		public ActionResult ShowParamsMethods()
		{
			return View();
		}

		public ActionResult AddReconocimientos()
		{
			return View();
		}

		public ActionResult ShowAnnalistAcks()
		{
			return View();
		}

		public ActionResult RecOtorgados()
		{
			return View();
		}

        public ActionResult ShowMatrixesDlg()
        {
            return View();
        }

		public ActionResult ShowAckParams()
		{
			return View();
		}

		public ActionResult ShowAnnalistInfo()
		{
			return View();
		}

		public ActionResult ShowAckInfo()
		{
			return View();
		}

        public ActionResult ShowNormInfo()
        {
            return View();
        }
    }

}

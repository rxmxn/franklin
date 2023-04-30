using System.Web.Mvc;

namespace AbcSpa.Controllers
{
    public class PartialViewController : Controller
    {
        //
        // GET: /PartialView/

        public ActionResult Login()
        {
            return View();
        }
        public ActionResult LoadExcel()
        {
            return View();
        }
        // Relacionados con usuarios y roles
        public ActionResult Users()
        {
            return View();
        }
        public ActionResult Role()
        {
            return View();
        }
        // Relacionados con empresas
        public ActionResult Enterprises()
        {
            return View();
        }
        public ActionResult Actions()
        {
            return View();
        }
        public ActionResult Ack()
        {
            return View();
        }

        // Relacionados con matrices
        public ActionResult BaseMatrix()
        {
            return View();
        }
        public ActionResult Matrixes()
        {
            return View();
        }

        public ActionResult Matrixhistory()
        {
            return View();
        }

        public ActionResult Norm()
        {
            return View();
        }
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Help()
        {
            return View();
        }

        //Relacionados con metodo
        public ActionResult Container()
        {
            return View();
        }
        public ActionResult Preserver()
        {
            return View();
        }
        public ActionResult Residue()
        {
            return View();
        }
		public ActionResult Material()
		{
			return View();
		}
		//public ActionResult BaseParamFamily()
  //      {
  //          return View();
  //      }
        public ActionResult AnalyticsMethod()
        {
            return View();
        }
        public ActionResult Method()
        {
            return View();
        }

        //Analista
        public ActionResult Annalist()
        {
            return View();
        }
        public ActionResult AnalyticsArea()
        {
            return View();
        }
        public ActionResult BaseParam()
        {
            return View();
        }
        public ActionResult Currency()
        {
            return View();
        }
        public ActionResult MeasureUnits()
        {
            return View();
        }

        // Relacionados con Region, Mercado y Sucursales
        public ActionResult Region()
        {
            return View();
        }
        public ActionResult Sucursal()
        {
            return View();
        }
        
        public ActionResult Market()
        {
           return View();
        }
        public ActionResult Office()
        {
           return View();
        }

        public ActionResult Group()
        {
            return View();
        }
        public ActionResult Param()
        {
            return View();
        }
        public ActionResult Report()
        {
            return View();
        }
        public ActionResult Package()
        {
            return View();
        }
        public ActionResult SheetTable()
        {
            return View();
        }

		public ActionResult Charts()
		{
			return View();
		}

		public ActionResult UserAccess()
		{
			return View();
		}

		public ActionResult Audit()
		{
			return View();
		}

        public ActionResult MaxPermitedLimit()
        {
            return View();
        }

		public ActionResult ClasificacionQuimica()
		{
			return View();
		}

		public ActionResult TipoServicio()
		{
			return View();
		}

		public ActionResult MassiveTransfer()
		{
			return View();
		}

		public ActionResult CentroCosto()
		{
			return View();
		}

		public ActionResult UnidadAnalitica()
		{
			return View();
		}

		public ActionResult Rama()
		{
			return View();
		}

        public ActionResult Alcance()
        {
            return View();
        }

        public ActionResult Status()
        {
            return View();
        }

        public ActionResult AnnalistKey()
        {
            return View();
        }
    }

}

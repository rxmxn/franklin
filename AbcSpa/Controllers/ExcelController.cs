using System;
using System.IO;
using System.Web.Mvc;

namespace AbcSpa.Controllers
{
    public class ExcelController : Controller
    {
        [HttpPost, ActionName("UploadExcel")]
        public JsonResult UploadExcel()
        {
            string message;

            if (Request.Files == null)
                return new JsonResult
                {
                    Data = new {details = "No se encuentra el archivo", success = false, codeName = string.Empty, realName = string.Empty}
                };

            var file = Request.Files[0];

            if (file == null)
                return new JsonResult
                {
                    Data = new {details = "No se encuentra el archivo", success = false, codeName = string.Empty, realName = string.Empty}
                };

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            try
            {
                file.SaveAs(Path.Combine(Server.MapPath("~/Content/uploads"), fileName));
                message = "Subida exitosa!";
            }
            catch (Exception)
            {
                message = "Falló la subida del documento. Por favor intentelo de nuevo.";
            }
            return new JsonResult { Data = new { details = message, success = true, codeName = fileName, realName = file.FileName } };
        }
    }
}
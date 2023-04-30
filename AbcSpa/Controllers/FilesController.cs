using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web.Mvc;
using System.Web.WebPages;
using AbcPersistent.Models;

namespace AbcSpa.Controllers
{
    public class FilesController : Controller
    {
        [HttpPost, ActionName("SaveImage")]
        public JsonResult SaveImage()
        {
            string fileName;
            var message = fileName = String.Empty;

            if (Request.Files == null)
                return new JsonResult { Data = new { details = message, success = false, name = fileName } };

            var file = Request.Files[0];
            if (file == null)
                return new JsonResult { Data = new { details = message, success = false, name = fileName } };

            fileName = /*Guid.NewGuid() +*/file.FileName +  Path.GetExtension(file.FileName);//TODO:ojo buscar qué implicaciones trae quitar el Guid

            try
            {
                file.SaveAs(Path.Combine(Server.MapPath("~/Content/uploads"), fileName));
                message = "Listo!";
            }
            catch (Exception)
            {
                message = "Falló la subida del archivo. Por favor intentelo de nuevo.";
            }
            return new JsonResult { Data = new { details = message, success = true, name = fileName } };
        }
        
        [HttpPost, ActionName("RemoveImage")]
        public JsonResult RemoveImage(string name)
        {
            try
            {
                var fullpath = Server.MapPath("~/Content/uploads/" + name);
                if (System.IO.File.Exists(fullpath))
                {
                    System.IO.File.Delete(fullpath);
                }
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }
        
    }
}
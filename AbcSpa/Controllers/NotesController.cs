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
    public class NotesController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();

        [HttpPost, ActionName("SaveNote")]
        public JsonResult SaveNote(Notes note)
        {
            try
            {
                dynamic currUsr = Session["curr_User"];

                var n = note.Id == 0
                    ? new Notes()
                    {
                        User = _dbStore.UserSet.Find(currUsr.Id)
                    }
                    : _dbStore.NotesSet.Find(note.Id);

                n.Description = note.Description;

                if (note.Id == 0)
                    _dbStore.NotesSet.Add(n);

                _dbStore.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpPost, ActionName("RefreshList")]
        public JsonResult RefreshList(bool done = false)
        {
            try
            {
                dynamic currUsr = Session["curr_User"];
                int? userId = currUsr.Id == 0 ? null : currUsr.Id;
                var todoList = _dbStore.NotesSet.Where(e => e.Done == done && e.User.Id == userId).ToList();
                todoList = done == false
                    ? todoList.OrderBy(e => e.CreatedTime).ToList()
                    : todoList.OrderBy(e => e.DoneTime).ToList();

                var notesList = new List<dynamic>();
                
                foreach(var note in todoList)
                {
                    notesList.Add(new {
                        note.Id,
                        note.Done,
                        note.Description
                    });
                }

                return Json(new { success = true, elements = notesList, total = todoList.Count });
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

        [HttpPost, ActionName("SaveDoneStatus")]
        public JsonResult SaveDoneStatus(int id, bool done)
        {
            try
            {
                var note = _dbStore.NotesSet.Find(id);
                if (note == null)
                    return Json(new { success = false, details = "No se encontró info de la nota" });

                note.Done = done;
                if (done)
                    note.DoneTime = DateTime.Now;
                else
                    note.CreatedTime = DateTime.Now;

                _dbStore.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, details = e.Message });
            }
        }

        [HttpPost]
        [ActionName("DeleteNote")]
        public JsonResult DeleteNote(int id)
        {
            try
            {
                var n = _dbStore.NotesSet.Find(id);
                
                _dbStore.NotesSet.Remove(n);
                _dbStore.SaveChanges();

                return Json(new { success = true });
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
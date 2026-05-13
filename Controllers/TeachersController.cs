using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static Controllers.AccessControl;

namespace Controllers
{
    [UserAccess(Access.View)]
    public class TeachersController : Controller
    {
        const string IllegalAccessUrl = "/Accounts/Login?message=Tentative d'accès illégal!&success=false";

        private void InitSessionVariables()
        {
            if (Session["CurrentTeacherId"] == null) Session["CurrentTeacherId"] = 0;
            if (Session["TeacherSearch"] == null) Session["TeacherSearch"] = false;
            if (Session["TeacherSearchString"] == null) Session["TeacherSearchString"] = "";
        }

        public ActionResult List()
        {
            InitSessionVariables();
            return View();
        }

        public ActionResult Search()
        {
            InitSessionVariables();
            Session["TeacherSearch"] = !(bool)Session["TeacherSearch"];
            return RedirectToAction("List");
        }

        public ActionResult SetSearchString(string searchString)
        {
            InitSessionVariables();
            Session["TeacherSearchString"] = searchString == null ? "" : searchString.ToLower();
            return null;
        }

        public ActionResult GetTeachers(bool forceRefresh = false)
        {
            try
            {
                InitSessionVariables();

                if (DB.Teachers.HasChanged || DB.Allocations.HasChanged || forceRefresh)
                {
                    IEnumerable<Teacher> teachers = DB.Teachers.ToList();
                    bool search = (bool)Session["TeacherSearch"];
                    string searchString = (string)Session["TeacherSearchString"];

                    if (search && searchString != "")
                    {
                        teachers = teachers.Where(t => t.Caption.ToLower().Contains(searchString));
                    }

                    return PartialView(teachers.OrderBy(t => t.LastName).ThenBy(t => t.FirstName));
                }

                return null;
            }
            catch (Exception ex)
            {
                return Content("Erreur interne " + ex.Message, "text/html");
            }
        }

        public ActionResult Details(int id)
        {
            InitSessionVariables();
            Teacher teacher = DB.Teachers.Get(id);

            if (teacher == null)
            {
                return RedirectToAction("List");
            }

            Session["CurrentTeacherId"] = id;
            return View(teacher);
        }

        public ActionResult GetTeacherDetails(bool forceRefresh = false)
        {
            try
            {
                InitSessionVariables();

                int id = (int)Session["CurrentTeacherId"];
                Teacher teacher = DB.Teachers.Get(id);

                if (teacher == null)
                {
                    return null;
                }

                if (DB.Teachers.HasChanged || forceRefresh)
                {
                    return PartialView(teacher);
                }

                return null;
            }
            catch (Exception ex)
            {
                return Content("Erreur interne " + ex.Message, "text/html");
            }
        }

        public ActionResult GetTeacherAllocations(bool forceRefresh = false)
        {
            try
            {
                InitSessionVariables();

                int id = (int)Session["CurrentTeacherId"];
                Teacher teacher = DB.Teachers.Get(id);

                if (teacher == null)
                {
                    return null;
                }

                if (DB.Teachers.HasChanged || DB.Courses.HasChanged || DB.Allocations.HasChanged || forceRefresh)
                {
                    return PartialView(teacher);
                }

                return null;
            }
            catch (Exception ex)
            {
                return Content("Erreur interne " + ex.Message, "text/html");
            }
        }

        [UserAccess(Access.Write)]
        public ActionResult Create()
        {
            InitSessionVariables();
            Teacher teacher = new Teacher();
            return View(teacher);
        }

        [HttpPost]
        [UserAccess(Access.Write)]
        public ActionResult Create(Teacher teacher)
        {
            if (teacher.IsValid())
            {
                int id = DB.Teachers.Add(teacher);
                return RedirectToAction("Details", new { id });
            }

            return Redirect(IllegalAccessUrl);
        }

        [UserAccess(Access.Write)]
        public ActionResult Edit(int id)
        {
            InitSessionVariables();

            Teacher teacher = DB.Teachers.Get(id);

            if (teacher == null)
            {
                return RedirectToAction("List");
            }

            Session["CurrentTeacherId"] = teacher.Id;
            Session["CurrentTeacherCode"] = teacher.Code;
            ViewBag.Allocations = teacher.NextSessionCoursesToSelectList;
            ViewBag.Courses = DB.Courses.NextSessionAvailableToSelectList(teacher.Id);

            return View(teacher);
        }

        [HttpPost]
        [UserAccess(Access.Write)]
        public ActionResult Edit(Teacher teacher, List<int> selectedCoursesId)
        {
            if (teacher.IsValid())
            {
                teacher.Id = (int)Session["CurrentTeacherId"];
                teacher.Code = (string)Session["CurrentTeacherCode"];
                DB.Teachers.Update(teacher, selectedCoursesId);
                return RedirectToAction("Details", new { id = teacher.Id });
            }

            return Redirect(IllegalAccessUrl);
        }

        [UserAccess(Access.Write)]
        public ActionResult Delete(int id)
        {
            Teacher teacher = DB.Teachers.Get(id);

            if (teacher != null)
            {
                DB.Teachers.Delete(id);
            }

            return RedirectToAction("List");
        }
    }
}
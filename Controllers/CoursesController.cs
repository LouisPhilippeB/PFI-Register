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
    public class CoursesController : Controller
    {
        const string IllegalAccessUrl = "/Accounts/Login?message=Tentative d'accès illégal!&success=false";

        private void InitSessionVariables()
        {
            if (Session["CurrentCourseId"] == null) Session["CurrentCourseId"] = 0;
            if (Session["CourseSearch"] == null) Session["CourseSearch"] = false;
            if (Session["CourseSearchString"] == null) Session["CourseSearchString"] = "";
            Session["CoursesSessionsList"] = DB.Courses.ToList().Select(c => c.Session).Distinct().ToList();
        }

        public ActionResult List()
        {
            InitSessionVariables();
            return View();
        }

        public ActionResult Search()
        {
            InitSessionVariables();
            Session["CourseSearch"] = !(bool)Session["CourseSearch"];
            return RedirectToAction("List");
        }

        public ActionResult SetSearchString(string searchString)
        {
            InitSessionVariables();
            Session["CourseSearchString"] = searchString == null ? "" : searchString.ToLower();
            return null;
        }

        public ActionResult GetCourses(bool forceRefresh = false)
        {
            try
            {
                InitSessionVariables();

                if (DB.Courses.HasChanged || DB.Registrations.HasChanged || DB.Allocations.HasChanged || forceRefresh)
                {
                    IEnumerable<Course> courses = DB.Courses.ToList();
                    bool search = (bool)Session["CourseSearch"];
                    string searchString = (string)Session["CourseSearchString"];

                    if (search && searchString != "")
                    {
                        courses = courses.Where(c => c.Caption.ToLower().Contains(searchString));
                    }

                    Session["CoursesSessionsList"] = courses.Select(c => c.Session).Distinct().OrderBy(s => s).ToList();

                    return PartialView(courses.OrderBy(c => c.Session).ThenBy(c => c.Code));
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
            Course course = DB.Courses.Get(id);

            if (course == null)
            {
                return RedirectToAction("List");
            }

            Session["CurrentCourseId"] = id;
            return View(course);
        }

        public ActionResult GetCourseDetails(bool forceRefresh = false)
        {
            try
            {
                InitSessionVariables();

                int id = (int)Session["CurrentCourseId"];
                Course course = DB.Courses.Get(id);

                if (course == null)
                {
                    return null;
                }

                if (DB.Courses.HasChanged || forceRefresh)
                {
                    return PartialView(course);
                }

                return null;
            }
            catch (Exception ex)
            {
                return Content("Erreur interne " + ex.Message, "text/html");
            }
        }

        public ActionResult GetCourseRegistrations(bool forceRefresh = false)
        {
            try
            {
                InitSessionVariables();

                int id = (int)Session["CurrentCourseId"];
                Course course = DB.Courses.Get(id);

                if (course == null)
                {
                    return null;
                }

                if (DB.Courses.HasChanged || DB.Students.HasChanged || DB.Registrations.HasChanged || DB.Allocations.HasChanged || forceRefresh)
                {
                    return PartialView(course);
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
            Course course = new Course();
            return View(course);
        }

        [HttpPost]
        [UserAccess(Access.Write)]
        public ActionResult Create(Course course)
        {
            if (course.IsValid())
            {
                int id = DB.Courses.Add(course);
                return RedirectToAction("Details", new { id });
            }

            return Redirect(IllegalAccessUrl);
        }

        [UserAccess(Access.Write)]
        public ActionResult Edit(int id)
        {
            InitSessionVariables();

            Course course = DB.Courses.Get(id);

            if (course == null)
            {
                return RedirectToAction("List");
            }

            Session["CurrentCourseId"] = course.Id;
            Session["CurrentCourseCode"] = course.Code;
            ViewBag.Registrations = course.NextSessionStudentsToSelectList;
            ViewBag.Students = DB.Students.ToSelectList;

            return View(course);
        }

        [HttpPost]
        [UserAccess(Access.Write)]
        public ActionResult Edit(Course course, List<int> selectedStudentsId)
        {
            if (course.IsValid())
            {
                course.Id = (int)Session["CurrentCourseId"];
                course.Code = (string)Session["CurrentCourseCode"];
                DB.Courses.Update(course, selectedStudentsId);
                return RedirectToAction("Details", new { id = course.Id });
            }

            return Redirect(IllegalAccessUrl);
        }

        [UserAccess(Access.Write)]
        public ActionResult Delete(int id)
        {
            Course course = DB.Courses.Get(id);

            if (course != null)
            {
                DB.Courses.Delete(id);
            }

            return RedirectToAction("List");
        }
    }
}
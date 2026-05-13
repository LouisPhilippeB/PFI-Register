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
    public class StudentsController : Controller
    {
        const string IllegalAccessUrl = "/Accounts/Login?message=Tentative d'accès illégal!&success=false";

        private void InitSessionVariables()
        {
            if (Session["CurrentStudentId"] == null) Session["CurrentStudentId"] = 0;
            if (Session["StudentSearch"] == null) Session["StudentSearch"] = false;
            if (Session["StudentSearchString"] == null) Session["StudentSearchString"] = "";
            if (Session["SelectedStudentYear"] == null) Session["SelectedStudentYear"] = 0;
            Session["StudentsYearsList"] = DB.Students.ToList().Select(s => s.Year).Distinct().ToList();
        }

        public ActionResult List()
        {
            InitSessionVariables();
            return View();
        }

        public ActionResult Search()
        {
            InitSessionVariables();
            Session["StudentSearch"] = !(bool)Session["StudentSearch"];
            return RedirectToAction("List");
        }

        public ActionResult SetSearchString(string searchString)
        {
            InitSessionVariables();
            Session["StudentSearchString"] = searchString == null ? "" : searchString.ToLower();
            return null;
        }

        public ActionResult SetSelectedYear(int year = 0)
        {
            InitSessionVariables();
            Session["SelectedStudentYear"] = year;
            return null;
        }

        public ActionResult GetStudents(bool forceRefresh = false)
        {
            try
            {
                InitSessionVariables();

                if (DB.Students.HasChanged || DB.Registrations.HasChanged || forceRefresh)
                {
                    IEnumerable<Student> students = DB.Students.ToList();
                    bool search = (bool)Session["StudentSearch"];
                    string searchString = (string)Session["StudentSearchString"];
                    int selectedYear = (int)Session["SelectedStudentYear"];

                    if (search)
                    {
                        if (searchString != "")
                        {
                            students = students.Where(s => s.Caption.ToLower().Contains(searchString));
                        }

                        if (selectedYear != 0)
                        {
                            students = students.Where(s => s.Year == selectedYear);
                        }
                    }

                    Session["StudentsYearsList"] = students.Select(s => s.Year).Distinct().OrderByDescending(y => y).ToList();

                    return PartialView(students.OrderByDescending(s => s.Year).ThenBy(s => s.LastName).ThenBy(s => s.FirstName));
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
            Student student = DB.Students.Get(id);

            if (student == null)
            {
                return RedirectToAction("List");
            }

            Session["CurrentStudentId"] = id;
            return View(student);
        }

        public ActionResult GetStudentDetails(bool forceRefresh = false)
        {
            try
            {
                InitSessionVariables();

                int id = (int)Session["CurrentStudentId"];
                Student student = DB.Students.Get(id);

                if (student == null)
                {
                    return null;
                }

                if (DB.Students.HasChanged || forceRefresh)
                {
                    return PartialView(student);
                }

                return null;
            }
            catch (Exception ex)
            {
                return Content("Erreur interne " + ex.Message, "text/html");
            }
        }

        public ActionResult GetStudentRegistrations(bool forceRefresh = false)
        {
            try
            {
                InitSessionVariables();

                int id = (int)Session["CurrentStudentId"];
                Student student = DB.Students.Get(id);

                if (student == null)
                {
                    return null;
                }

                if (DB.Students.HasChanged || DB.Courses.HasChanged || DB.Registrations.HasChanged || DB.Allocations.HasChanged || forceRefresh)
                {
                    return PartialView(student);
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
            Student student = new Student();
            return View(student);
        }

        [HttpPost]
        [UserAccess(Access.Write)]
        public ActionResult Create(Student student)
        {
            if (student.IsValid())
            {
                int id = DB.Students.Add(student);
                return RedirectToAction("Details", new { id });
            }

            return Redirect(IllegalAccessUrl);
        }

        [UserAccess(Access.Write)]
        public ActionResult Edit(int id)
        {
            InitSessionVariables();

            Student student = DB.Students.Get(id);

            if (student == null)
            {
                return RedirectToAction("List");
            }

            Session["CurrentStudentId"] = student.Id;
            Session["CurrentStudentCode"] = student.Code;
            ViewBag.Registrations = student.NextSessionCoursesToSelectList;
            ViewBag.Courses = DB.Courses.NextSessionToSelectList;

            return View(student);
        }

        [HttpPost]
        [UserAccess(Access.Write)]
        public ActionResult Edit(Student student, List<int> selectedCoursesId)
        {
            if (student.IsValid())
            {
                student.Id = (int)Session["CurrentStudentId"];
                student.Code = (string)Session["CurrentStudentCode"];
                DB.Students.Update(student, selectedCoursesId);
                return RedirectToAction("Details", new { id = student.Id });
            }

            return Redirect(IllegalAccessUrl);
        }

        [UserAccess(Access.Write)]
        public ActionResult Delete(int id)
        {
            Student student = DB.Students.Get(id);

            if (student != null)
            {
                DB.Students.Delete(id);
            }

            return RedirectToAction("List");
        }
    }
}
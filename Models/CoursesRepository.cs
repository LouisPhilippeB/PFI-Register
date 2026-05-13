using DAL;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Models
{
    public class CoursesRepository : Repository<Course>
    {
        public SelectList ToSelectList => SelectListUtilities<Course>.Convert(ToList().OrderBy(c => c.Session).ThenBy(c => c.Code), "Caption");

        public SelectList NextSessionToSelectList
        {
            get
            {
                List<Course> courses = ToList()
                    .Where(c => NextSession.ValidSessions.Contains(c.Session))
                    .OrderBy(c => c.Session)
                    .ThenBy(c => c.Code)
                    .ToList();

                return SelectListUtilities<Course>.Convert(courses, "Caption");
            }
        }

        public SelectList NextSessionAvailableToSelectList(int teacherId)
        {
            List<Course> courses = ToList()
                .Where(c => NextSession.ValidSessions.Contains(c.Session))
                .Where(c => c.NextSessionAllocations.Count == 0 || c.NextSessionAllocations.Where(a => a.TeacherId == teacherId).FirstOrDefault() != null)
                .OrderBy(c => c.Session)
                .ThenBy(c => c.Code)
                .ToList();

            return SelectListUtilities<Course>.Convert(courses, "Caption");
        }

        public override int Add(Course course)
        {
            return base.Add(course);
        }

        public bool Update(Course course, List<int> selectedStudentsId)
        {
            try
            {
                BeginTransaction();
                bool result = base.Update(course);
                course.UpdateRegistrations(selectedStudentsId);
                EndTransaction();
                return result;
            }
            catch
            {
                EndTransaction();
                return false;
            }
        }

        public override bool Delete(int courseId)
        {
            try
            {
                Course course = Get(courseId);

                if (course != null)
                {
                    BeginTransaction();
                    course.DeleteAllRegistrations();
                    course.DeleteAllAllocations();
                    bool result = base.Delete(courseId);
                    EndTransaction();
                    return result;
                }

                return false;
            }
            catch
            {
                EndTransaction();
                return false;
            }
        }
    }
}
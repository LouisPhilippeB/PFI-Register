using DAL;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Models
{
    public class Course : Record
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public int Session { get; set; }

        [JsonIgnore]
        public string Caption => "[" + Session + "] " + Code + " " + Title;

        [JsonIgnore]
        public List<Registration> Registrations => DB.Registrations.ToList().Where(r => r.CourseId == Id).ToList();

        [JsonIgnore]
        public List<Registration> NextSessionRegistrations => DB.Registrations.ToList().Where(r => r.CourseId == Id && r.IsNextSession).ToList();

        [JsonIgnore]
        public List<Allocation> Allocations => DB.Allocations.ToList().Where(a => a.CourseId == Id).ToList();

        [JsonIgnore]
        public List<Allocation> NextSessionAllocations => DB.Allocations.ToList().Where(a => a.CourseId == Id && a.IsNextSession).ToList();

        [JsonIgnore]
        public List<Student> Students
        {
            get
            {
                List<Student> students = new List<Student>();

                foreach (Registration registration in Registrations.OrderByDescending(r => r.Student.Year).ThenBy(r => r.Student.LastName))
                {
                    if (registration.Student != null)
                        students.Add(registration.Student);
                }

                return students;
            }
        }

        [JsonIgnore]
        public List<Student> NextSessionStudents
        {
            get
            {
                List<Student> students = new List<Student>();

                foreach (Registration registration in NextSessionRegistrations.OrderByDescending(r => r.Student.Year).ThenBy(r => r.Student.LastName))
                {
                    if (registration.Student != null)
                        students.Add(registration.Student);
                }

                return students;
            }
        }

        [JsonIgnore]
        public SelectList StudentsSelectList => SelectListUtilities<Student>.Convert(Students, "Caption");

        [JsonIgnore]
        public SelectList NextSessionStudentsToSelectList => SelectListUtilities<Student>.Convert(NextSessionStudents, "Caption");

        [JsonIgnore]
        public Teacher CurrentSessionTeacher
        {
            get
            {
                Allocation allocation = Allocations
                    .Where(a => a.Year == NextSession.Year && a.Teacher != null)
                    .OrderByDescending(a => a.Id)
                    .FirstOrDefault();

                if (allocation != null)
                    return allocation.Teacher;

                return null;
            }
        }

        public Teacher TeacherForYear(int year)
        {
            Allocation allocation = Allocations.Where(a => a.Year == year && a.Teacher != null).OrderByDescending(a => a.Id).FirstOrDefault();

            if (allocation != null)
                return allocation.Teacher;

            return null;
        }

        public void DeleteAllRegistrations()
        {
            foreach (Registration registration in Registrations.Copy())
            {
                DB.Registrations.Delete(registration.Id);
            }
        }

        public void DeleteAllAllocations()
        {
            foreach (Allocation allocation in Allocations.Copy())
            {
                DB.Allocations.Delete(allocation.Id);
            }
        }

        public void DeleteNextSessionRegistrations()
        {
            foreach (Registration registration in NextSessionRegistrations.Copy())
            {
                DB.Registrations.Delete(registration.Id);
            }
        }

        public void UpdateRegistrations(List<int> selectedStudentsId)
        {
            DeleteNextSessionRegistrations();

            if (selectedStudentsId != null)
            {
                foreach (int studentId in selectedStudentsId)
                {
                    if (studentId > 0)
                    {
                        DB.Registrations.Add(new Registration { StudentId = studentId, CourseId = Id });
                    }
                }
            }
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Code) &&
                   !string.IsNullOrWhiteSpace(Title) &&
                   Session >= 1 &&
                   Session <= 6;
        }
    }
}
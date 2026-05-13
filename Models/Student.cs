using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Models
{
    public class Student : Record
    {
        public Student()
        {
            BirthDate = DateTime.Now;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        [JsonIgnore]
        public string FullName => LastName + " " + FirstName;

        [JsonIgnore]
        public string Caption => Code + " " + LastName + " " + FirstName;

        [JsonIgnore]
        public int Year
        {
            get
            {
                int year = 0;
                if (!string.IsNullOrEmpty(Code) && Code.Length >= 4)
                    int.TryParse(Code.Substring(0, 4), out year);
                return year;
            }
        }

        [JsonIgnore]
        public List<Registration> Registrations => DB.Registrations.ToList().Where(r => r.StudentId == Id).ToList();

        [JsonIgnore]
        public List<Registration> NextSessionRegistrations => DB.Registrations.ToList().Where(r => r.StudentId == Id && r.IsNextSession).ToList();

        [JsonIgnore]
        public List<Course> Courses
        {
            get
            {
                List<Course> courses = new List<Course>();
                foreach (Registration registration in Registrations.OrderBy(r => r.Course.Code))
                {
                    if (registration.Course != null)
                        courses.Add(registration.Course);
                }
                return courses;
            }
        }

        [JsonIgnore]
        public List<Course> NextSessionCourses
        {
            get
            {
                List<Course> courses = new List<Course>();
                foreach (Registration registration in NextSessionRegistrations.OrderBy(r => r.Course.Code))
                {
                    if (registration.Course != null)
                        courses.Add(registration.Course);
                }
                return courses;
            }
        }

        [JsonIgnore]
        public SelectList CoursesSelectList => SelectListUtilities<Course>.Convert(Courses, "Caption");

        [JsonIgnore]
        public SelectList NextSessionCoursesToSelectList => SelectListUtilities<Course>.Convert(NextSessionCourses, "Caption");

        public void DeleteAllRegistrations()
        {
            foreach (Registration registration in Registrations.Copy())
            {
                DB.Registrations.Delete(registration.Id);
            }
        }

        public void DeleteNextSessionRegistrations()
        {
            foreach (Registration registration in NextSessionRegistrations.Copy())
            {
                DB.Registrations.Delete(registration.Id);
            }
        }

        public void UpdateRegistrations(List<int> selectedCoursesId)
        {
            DeleteNextSessionRegistrations();

            if (selectedCoursesId != null)
            {
                foreach (int courseId in selectedCoursesId)
                {
                    if (courseId > 0)
                    {
                        DB.Registrations.Add(new Registration { StudentId = Id, CourseId = courseId });
                    }
                }
            }
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Phone);
        }
    }
}
using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Models
{
    public class Teacher : Record
    {
        public Teacher()
        {
            StartDate = DateTime.Now;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        const string Avatars_Folder = @"/App_Assets/teachers/";
        const string Default_Avatar = @"no_avatar.png";

        [ImageAsset(Avatars_Folder, Default_Avatar)]
        public string Avatar { get; set; } = Avatars_Folder + Default_Avatar;

        [JsonIgnore]
        public string FullName => LastName + " " + FirstName;

        [JsonIgnore]
        public string Caption => Code + " " + LastName + " " + FirstName;

        [JsonIgnore]
        public List<Allocation> Allocations => DB.Allocations.ToList().Where(a => a.TeacherId == Id).ToList();

        [JsonIgnore]
        public List<Allocation> NextSessionAllocations => DB.Allocations.ToList().Where(a => a.TeacherId == Id && a.IsNextSession).ToList();

        [JsonIgnore]
        public List<Course> Courses
        {
            get
            {
                List<Course> courses = new List<Course>();
                foreach (Allocation allocation in Allocations.OrderBy(a => a.Course.Session).ThenBy(a => a.Course.Code))
                {
                    if (allocation.Course != null)
                        courses.Add(allocation.Course);
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
                foreach (Allocation allocation in NextSessionAllocations.OrderBy(a => a.Course.Session).ThenBy(a => a.Course.Code))
                {
                    if (allocation.Course != null)
                        courses.Add(allocation.Course);
                }
                return courses;
            }
        }

        [JsonIgnore]
        public SelectList CoursesSelectList => SelectListUtilities<Course>.Convert(Courses, "Caption");

        [JsonIgnore]
        public SelectList NextSessionCoursesToSelectList => SelectListUtilities<Course>.Convert(NextSessionCourses, "Caption");

        public void DeleteAllAllocations()
        {
            foreach (Allocation allocation in Allocations.Copy())
            {
                DB.Allocations.Delete(allocation.Id);
            }
        }

        public void DeleteNextSessionAllocations()
        {
            foreach (Allocation allocation in NextSessionAllocations.Copy())
            {
                DB.Allocations.Delete(allocation.Id);
            }
        }

        public void UpdateAllocations(List<int> selectedCoursesId)
        {
            DeleteNextSessionAllocations();

            if (selectedCoursesId != null)
            {
                foreach (int courseId in selectedCoursesId)
                {
                    if (courseId > 0)
                    {
                        DB.Allocations.Add(new Allocation { TeacherId = Id, CourseId = courseId });
                    }
                }
            }
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(Phone);
        }
    }
}
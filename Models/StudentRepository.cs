using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Models
{
    public class StudentsRepository : Repository<Student>
    {
        public SelectList ToSelectList => SelectListUtilities<Student>.Convert(ToList().OrderByDescending(s => s.Year).ThenBy(s => s.LastName).ThenBy(s => s.FirstName), "Caption");

        private string GenerateCode()
        {
            Random random = new Random();
            string code = "";

            do
            {
                code = NextSession.Year.ToString() + random.Next(100000, 999999).ToString();
            }
            while (ToList().Where(s => s.Code == code).FirstOrDefault() != null);

            return code;
        }

        public override int Add(Student student)
        {
            if (string.IsNullOrWhiteSpace(student.Code))
            {
                student.Code = GenerateCode();
            }

            return base.Add(student);
        }

        public bool Update(Student student, List<int> selectedCoursesId)
        {
            try
            {
                BeginTransaction();
                bool result = base.Update(student);
                student.UpdateRegistrations(selectedCoursesId);
                EndTransaction();
                return result;
            }
            catch
            {
                EndTransaction();
                return false;
            }
        }

        public override bool Delete(int studentId)
        {
            try
            {
                Student student = Get(studentId);

                if (student != null)
                {
                    BeginTransaction();
                    student.DeleteAllRegistrations();
                    bool result = base.Delete(studentId);
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
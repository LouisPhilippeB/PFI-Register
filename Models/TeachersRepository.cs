using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Models
{
    public class TeachersRepository : Repository<Teacher>
    {
        public SelectList ToSelectList => SelectListUtilities<Teacher>.Convert(ToList().OrderBy(t => t.LastName).ThenBy(t => t.FirstName), "Caption");

        private string GenerateCode()
        {
            Random random = new Random();
            string code = "";

            do
            {
                code = "CLG-420-" + random.Next(10000, 99999).ToString();
            }
            while (ToList().Where(t => t.Code == code).FirstOrDefault() != null);

            return code;
        }

        public override int Add(Teacher teacher)
        {
            if (string.IsNullOrWhiteSpace(teacher.Code))
            {
                teacher.Code = GenerateCode();
            }

            return base.Add(teacher);
        }

        public bool Update(Teacher teacher, List<int> selectedCoursesId)
        {
            try
            {
                BeginTransaction();
                bool result = base.Update(teacher);
                teacher.UpdateAllocations(selectedCoursesId);
                EndTransaction();
                return result;
            }
            catch
            {
                EndTransaction();
                return false;
            }
        }

        public override bool Delete(int teacherId)
        {
            try
            {
                Teacher teacher = Get(teacherId);

                if (teacher != null)
                {
                    BeginTransaction();
                    teacher.DeleteAllAllocations();
                    bool result = base.Delete(teacherId);
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
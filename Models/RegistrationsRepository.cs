using DAL;
using System.Linq;

namespace Models
{
    public class RegistrationsRepository : Repository<Registration>
    {
        public void DeleteByStudentId(int studentId)
        {
            foreach (Registration registration in ToList().Where(r => r.StudentId == studentId).ToList())
            {
                Delete(registration.Id);
            }
        }

        public void DeleteByCourseId(int courseId)
        {
            foreach (Registration registration in ToList().Where(r => r.CourseId == courseId).ToList())
            {
                Delete(registration.Id);
            }
        }
    }
}
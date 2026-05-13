using DAL;
using System.Linq;

namespace Models
{
    public class AllocationsRepository : Repository<Allocation>
    {
        public void DeleteByTeacherId(int teacherId)
        {
            foreach (Allocation allocation in ToList().Where(a => a.TeacherId == teacherId).ToList())
            {
                Delete(allocation.Id);
            }
        }

        public void DeleteByCourseId(int courseId)
        {
            foreach (Allocation allocation in ToList().Where(a => a.CourseId == courseId).ToList())
            {
                Delete(allocation.Id);
            }
        }
    }
}
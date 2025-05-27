using Desktop_Nhom13.DAL;
using Desktop_Nhom13.Models;
using System.Collections.Generic;

namespace Desktop_Nhom13.BLL
{
    public class EnrollmentBLL
    {
        private readonly EnrollmentDAL _enrollmentDAL = new EnrollmentDAL();

        public List<EnrolledStudent> GetEnrolledStudents(int courseID)
        {
            return _enrollmentDAL.GetEnrolledStudents(courseID);
        }
    }
}

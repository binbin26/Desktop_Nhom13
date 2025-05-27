using Desktop_Nhom13.DAL;
using Desktop_Nhom13.Models;
using Desktop_Nhom13.Models.Courses;
using Desktop_Nhom13.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Desktop_Nhom13.BLL
{
    public class CourseBLL
    {
        private CourseDAL _courseDAL;

        public CourseBLL()
        {
            _courseDAL = new CourseDAL();
        }

        public CourseBLL(CourseDAL courseDAL)
        {
            _courseDAL = courseDAL;
        }

        public List<Course> GetAvailableCourses()
        {
            try
            {
                var courses = _courseDAL.GetAllCourses().Where(c => c.EndDate > DateTime.Now).ToList();
                Logger.LogInfo($"Lấy danh sách khóa học còn hoạt động: {courses.Count} khóa học.");
                return courses;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi lấy khóa học còn hoạt động: {ex.Message}\n{ex.StackTrace}");
                return new List<Course>();
            }
        }

        public string RegisterStudentToCourse(int studentId, int courseId)
        {
            try
            {
                Logger.LogInfo($"Đăng ký sinh viên {studentId} vào khóa học {courseId}.");
                return _courseDAL.RegisterCourse(studentId, courseId);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi đăng ký sinh viên vào khóa học: {ex.Message}\n{ex.StackTrace}");
                return "Error";
            }
        }

        public string EnrollStudent(int studentID, int courseID)
        {
            try
            {
                Logger.LogInfo($"Ghi danh sinh viên {studentID} vào khóa học {courseID}.");
                if (!_courseDAL.UserExistsWithRole(studentID, "Student"))
                {
                    Logger.LogInfo($"Người dùng {studentID} không có vai trò 'Student'.");
                    return "NotAStudent";
                }

                return _courseDAL.EnrollStudent(studentID, courseID);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi ghi danh sinh viên: {ex.Message}\n{ex.StackTrace}");
                return "Error";
            }
        }

        public List<Course> GetCoursesByTeacher(int teacherID)
        {
            try
            {
                var result = _courseDAL.GetCoursesByTeacher(teacherID);
                Logger.LogInfo($"Lấy danh sách khóa học của giảng viên {teacherID}: {result.Count} khóa học.");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi lấy khóa học theo giảng viên: {ex.Message}\n{ex.StackTrace}");
                return new List<Course>();
            }
        }

        public Course GetCourseByID(int courseID)
        {
            try
            {
                Logger.LogInfo($"Lấy thông tin khóa học ID: {courseID}");
                return _courseDAL.GetCourseByID(courseID);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi lấy thông tin khóa học: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        public bool AddCourse(Course course)
        {
            try
            {
                if (course == null)
                {
                    Logger.LogInfo("Không thể thêm khóa học: Đối tượng null.");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(course.CourseCode))
                {
                    Logger.LogInfo("Không thể thêm khóa học: Mã khóa học trống.");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(course.CourseName))
                {
                    Logger.LogInfo("Không thể thêm khóa học: Tên khóa học trống.");
                    return false;
                }
                if (course.TeacherID <= 0)
                {
                    Logger.LogInfo("Không thể thêm khóa học: TeacherID không hợp lệ.");
                    return false;
                }
                if (!_courseDAL.UserExistsWithRole(course.TeacherID, "Teacher"))
                {
                    Logger.LogInfo($"Người dùng {course.TeacherID} không có vai trò 'Teacher'.");
                    return false;
                }
                if (course.StartDate >= course.EndDate)
                {
                    Logger.LogInfo("Không thể thêm khóa học: Ngày bắt đầu >= ngày kết thúc.");
                    return false;
                }

                bool result = _courseDAL.AddCourse(course);
                Logger.LogInfo($"Đã thêm khóa học mới: {course.CourseName} | Mã: {course.CourseCode}");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi thêm khóa học: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        public List<EnrolledStudent> GetEnrolledStudents(int courseId)
        {
            try
            {
                var list = _courseDAL.GetEnrolledStudents(courseId);
                Logger.LogInfo($"Lấy danh sách sinh viên đã ghi danh vào khóa học {courseId}: {list.Count} sinh viên.");
                return list;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi lấy danh sách ghi danh: {ex.Message}\n{ex.StackTrace}");
                return new List<EnrolledStudent>();
            }
        }

        public List<Course> GetCoursesByStudent(int userId)
        {
            try
            {
                var courses = _courseDAL.GetCoursesByStudent(userId);
                Logger.LogInfo($"Lấy danh sách khóa học của sinh viên {userId}: {courses.Count} khóa học.");
                return courses;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi lấy khóa học của sinh viên: {ex.Message}\n{ex.StackTrace}");
                return new List<Course>();
            }
        }

        public List<CourseGrade> GetGradesByStudent(int studentId)
        {
            try
            {
                var grades = _courseDAL.GetGradesByStudent(studentId);
                Logger.LogInfo($"Lấy điểm của sinh viên {studentId}: {grades.Count} môn học.");
                return grades;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi lấy điểm sinh viên: {ex.Message}\n{ex.StackTrace}");
                return new List<CourseGrade>();
            }
        }

        public bool RemoveStudent(int studentId, int courseId)
        {
            try
            {
                if (studentId <= 0 || courseId <= 0)
                {
                    Logger.LogInfo($"Không thể xóa sinh viên: studentId = {studentId}, courseId = {courseId} không hợp lệ.");
                    return false;
                }

                bool result = _courseDAL.RemoveStudent(studentId, courseId);
                Logger.LogInfo($"Đã xóa sinh viên {studentId} khỏi khóa học {courseId}: {(result ? "Thành công" : "Thất bại")}");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi xóa sinh viên khỏi khóa học: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }
    }
}

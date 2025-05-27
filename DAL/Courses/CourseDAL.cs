﻿using Desktop_Nhom13.Models;
using Desktop_Nhom13.Models.Courses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;


namespace Desktop_Nhom13.DAL
{
    public class CourseDAL
    {
        public List<Course> GetAllCourses()
        {
            List<Course> courses = new List<Course>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM Courses";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    courses.Add(new Course
                    {
                        CourseID = (int)reader["CourseID"],
                        CourseCode = reader["CourseCode"].ToString(),
                        CourseName = reader["CourseName"].ToString(),
                        TeacherID = (int)reader["TeacherID"],
                        StartDate = (DateTime)reader["StartDate"],
                        EndDate = (DateTime)reader["EndDate"],
                        MaxEnrollment = reader["MaxEnrollment"] != DBNull.Value ? (int)reader["MaxEnrollment"] : 0
                    });
                }
            }
            return courses;
        }
        public string EnrollStudent(int studentID, int courseID)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                // 1. Kiểm tra CourseID có tồn tại không
                string checkCourseQuery = "SELECT COUNT(*) FROM Courses WHERE CourseID = @CourseID";
                SqlCommand checkCourseCmd = new SqlCommand(checkCourseQuery, conn);
                checkCourseCmd.Parameters.AddWithValue("@CourseID", courseID);

                conn.Open();
                int courseExists = (int)checkCourseCmd.ExecuteScalar();
                if (courseExists == 0)
                {
                    return "CourseNotFound";
                }

                // 2. Kiểm tra đã ghi danh chưa
                string checkQuery = "SELECT COUNT(*) FROM CourseEnrollments WHERE StudentID = @StudentID AND CourseID = @CourseID";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@StudentID", studentID);
                checkCmd.Parameters.AddWithValue("@CourseID", courseID);

                int exists = (int)checkCmd.ExecuteScalar();
                if (exists > 0)
                {
                    return "AlreadyEnrolled";
                }

                // 3. Thực hiện insert
                string insertQuery = "INSERT INTO CourseEnrollments (StudentID, CourseID) VALUES (@StudentID, @CourseID)";
                SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@StudentID", studentID);
                insertCmd.Parameters.AddWithValue("@CourseID", courseID);

                int rowsAffected = insertCmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Success" : "Error";
            }
        }
        // Lấy danh sách khóa học do Giảng viên phụ trách
        public List<Course> GetCoursesByTeacher(int teacherID)
        {
            List<Course> courses = new List<Course>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM Courses WHERE TeacherID = @TeacherID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TeacherID", teacherID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    courses.Add(new Course
                    {
                        CourseID = (int)reader["CourseID"],
                        CourseCode = reader["CourseCode"].ToString(),
                        CourseName = reader["CourseName"].ToString(),
                        TeacherID = (int)reader["TeacherID"],
                        StartDate = (DateTime)reader["StartDate"],
                        EndDate = (DateTime)reader["EndDate"],
                        MaxEnrollment = reader["MaxEnrollment"] != DBNull.Value ? (int)reader["MaxEnrollment"] : 0
                    });
                }
            }
            return courses;
        }

        public List<Course> GetCoursesByStudent(int studentId)
        {
            List<Course> courses = new List<Course>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = @"
            SELECT c.*
            FROM Courses c
            JOIN CourseEnrollments ce ON c.CourseID = ce.CourseID
            WHERE ce.StudentID = @StudentID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@StudentID", studentId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    courses.Add(new Course
                    {
                        CourseID = (int)reader["CourseID"],
                        CourseCode = reader["CourseCode"].ToString(),
                        CourseName = reader["CourseName"].ToString(),
                        TeacherID = (int)reader["TeacherID"],
                        StartDate = (DateTime)reader["StartDate"],
                        EndDate = (DateTime)reader["EndDate"],
                        MaxEnrollment = reader["MaxEnrollment"] != DBNull.Value ? (int)reader["MaxEnrollment"] : 0
                    });
                }
            }
            return courses;
        }


        public Course GetCourseByID(int courseID)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM Courses WHERE CourseID = @CourseID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CourseID", courseID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Course
                    {
                        CourseID = (int)reader["CourseID"],
                        CourseCode = reader["CourseCode"].ToString(),
                        CourseName = reader["CourseName"].ToString(),
                        TeacherID = (int)reader["TeacherID"],
                        StartDate = (DateTime)reader["StartDate"],
                        EndDate = (DateTime)reader["EndDate"],
                        MaxEnrollment = reader["MaxEnrollment"] != DBNull.Value ? (int)reader["MaxEnrollment"] : 0
                    };
                }
                return null;
            }
        }

        public bool AddCourse(Course course)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = @"INSERT INTO Courses (CourseCode, CourseName, TeacherID, StartDate, EndDate) 
                               VALUES (@CourseCode, @CourseName, @TeacherID, @StartDate, @EndDate)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CourseCode", course.CourseCode);
                cmd.Parameters.AddWithValue("@CourseName", course.CourseName);
                cmd.Parameters.AddWithValue("@TeacherID", course.TeacherID);
                cmd.Parameters.AddWithValue("@StartDate", course.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", course.EndDate);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        //Đảm bảo StudentID và TeacherID luôn là UserID của tài khoản xác định
        public bool UserExistsWithRole(int userId, string role)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT COUNT(*) FROM Users WHERE UserID = @UserID AND Role = @Role";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@Role", role);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }


        public List<EnrolledStudent> GetEnrolledStudents(int courseId)
        {
            List<EnrolledStudent> students = new List<EnrolledStudent>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = @"
                    SELECT u.UserID as StudentID, u.FullName, u.Email, ce.EnrollmentDate
                    FROM Users u
                    INNER JOIN CourseEnrollments ce ON u.UserID = ce.StudentID
                    WHERE ce.CourseID = @CourseID AND u.Role = 'Student'";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CourseID", courseId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    students.Add(new EnrolledStudent
                    {
                        StudentID = (int)reader["StudentID"],
                        FullName = reader["FullName"].ToString(),
                        Email = reader["Email"].ToString(),
                        EnrollmentDate = (DateTime)reader["EnrollmentDate"]
                    });
                }
            }
            return students;
        }

        public bool RemoveStudent(int studentId, int courseId)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "DELETE FROM CourseEnrollments WHERE StudentID = @StudentID AND CourseID = @CourseID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@StudentID", studentId);
                cmd.Parameters.AddWithValue("@CourseID", courseId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public string RegisterCourse(int studentId, int courseId)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // 1. Kiểm tra đã đăng ký chưa
                string checkExistQuery = "SELECT COUNT(*) FROM CourseEnrollments WHERE StudentID = @StudentID AND CourseID = @CourseID";
                using (SqlCommand checkCmd = new SqlCommand(checkExistQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@StudentID", studentId);
                    checkCmd.Parameters.AddWithValue("@CourseID", courseId);
                    if ((int)checkCmd.ExecuteScalar() > 0)
                        return "Đã đăng ký học phần này rồi!";
                }

                // 2. Kiểm tra số lượng còn trống và hạn đăng ký
                string checkCourseQuery = "SELECT MaxEnrollment, EndDate FROM Courses WHERE CourseID = @CourseID";
                using (SqlCommand courseCmd = new SqlCommand(checkCourseQuery, conn))
                {
                    courseCmd.Parameters.AddWithValue("@CourseID", courseId);
                    using (SqlDataReader reader = courseCmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return "Học phần không tồn tại!";

                        int slotsLeft = reader["MaxEnrollment"] == DBNull.Value ? 0 : Convert.ToInt32(reader["MaxEnrollment"]);
                        DateTime endDate = reader["EndDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["EndDate"]);

                        if (slotsLeft <= 0)
                            return "Học phần đã hết chỗ!";
                        if (endDate < DateTime.Now)
                            return "Đã hết hạn đăng ký học phần này!";
                    }
                }

                // 3. Tiến hành đăng ký
                string insertQuery = "INSERT INTO CourseEnrollments (StudentID, CourseID) VALUES (@StudentID, @CourseID)";
                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@StudentID", studentId);
                    insertCmd.Parameters.AddWithValue("@CourseID", courseId);
                    insertCmd.ExecuteNonQuery();
                }

                return "Đăng ký thành công!";
            }
        }

        //Lấy điểm của sinh viên theo khóa học
        public List<CourseGrade> GetGradesByStudent(int studentId)
        {
            List<CourseGrade> grades = new List<CourseGrade>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = @"
        SELECT 
            c.CourseName,
            AVG(CAST(ss.Score AS FLOAT)) AS AverageScore,
            ISNULL(u.FullName, 'Chưa có') AS GradedBy
        FROM StudentSubmissions ss
        INNER JOIN Assignments a ON ss.AssignmentID = a.AssignmentID
        INNER JOIN Courses c ON a.CourseID = c.CourseID
        LEFT JOIN Users u ON a.CreatedBy = u.UserID
        WHERE ss.StudentID = @StudentID
        GROUP BY c.CourseName, u.FullName";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@StudentID", studentId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    grades.Add(new CourseGrade
                    {
                        CourseName = reader["CourseName"].ToString(),
                        Score = reader["AverageScore"] == DBNull.Value ? (float?)null : Convert.ToSingle(reader["AverageScore"]),
                        GradedBy = reader["GradedBy"].ToString()
                    });
                }
            }
            return grades;
        }
    }
}

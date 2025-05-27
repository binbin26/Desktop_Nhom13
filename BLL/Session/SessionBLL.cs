using Desktop_Nhom13.DAL;
using Desktop_Nhom13.Models.Courses.Sessions;
using Desktop_Nhom13.Utilities;
using System;
using System.Collections.Generic;

namespace Desktop_Nhom13.BLL
{
    public class SessionBLL
    {
        private SessionDAL _sessionDAL = new SessionDAL();

        public void InsertDocument(int courseId, int teacherId, int sessionId, string title, string filePath, string documentType)
        {
            try
            {
                _sessionDAL.InsertDocument(courseId, teacherId, sessionId, title, filePath, documentType);
                Logger.LogInfo($"InsertDocument thành công cho SessionID={sessionId}, Title={title}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"InsertDocument thất bại: {ex.Message}");
                throw new Exception("Lỗi khi lưu tài liệu đính kèm.");
            }
        }

        public void UpdateSessionTitle(int sessionId, string newTitle)
        {
            try
            {
                _sessionDAL.UpdateSessionTitle(sessionId, newTitle);
                Logger.LogInfo($"UpdateSessionTitle thành công: SessionID={sessionId}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"UpdateSessionTitle thất bại: {ex.Message}");
                throw new Exception("Lỗi khi cập nhật tiêu đề buổi học.");
            }
        }

        public void DeleteSession(int sessionId)
        {
            try
            {
                _sessionDAL.DeleteSession(sessionId);
                Logger.LogInfo($"DeleteSession thành công: SessionID={sessionId}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"DeleteSession thất bại: {ex.Message}");
                throw new Exception("Lỗi khi xóa buổi học.");
            }
        }
        private readonly SessionDAL SessionDAL;

        public SessionBLL()
        {
            _sessionDAL = new SessionDAL();
        }

        public List<SessionData> GetAllSessions()
        {
            try
            {
                return SessionDAL.GetAllSessions();
            }
            catch (Exception ex)
            {
                Logger.LogError("Lỗi khi lấy tất cả buổi học: " + ex.Message);
                throw new Exception("Đã xảy ra lỗi khi lấy danh sách buổi học.");
            }
        }

        public static List<SessionData> GetSessions(int courseId)
        {
            try
            {
                return SessionDAL.GetSessionsByCourseID(courseId);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi lấy danh sách buổi học theo CourseID={courseId}: {ex.Message}");
                throw new Exception("Đã xảy ra lỗi khi lấy danh sách buổi học theo khóa học.");
            }
        }

        public static SessionData AddSession(int courseId, string title)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                {
                    throw new ArgumentException("Tiêu đề không được để trống.");
                }

                if (title.Length > 50)
                {
                    throw new ArgumentException("Tiêu đề quá dài! Tối đa 50 ký tự.");
                }

                int sessionId = SessionDAL.AddSession(courseId, title);

                Logger.LogInfo($"Thêm buổi học thành công: CourseID={courseId}, Title='{title}', SessionID={sessionId}");

                return new SessionData
                {
                    SessionID = sessionId,
                    CourseID = courseId,
                    Title = title
                };
            }
            catch (ArgumentException argEx)
            {
                Logger.LogError("Lỗi dữ liệu đầu vào khi thêm buổi học: " + argEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi thêm buổi học: CourseID={courseId}, Title='{title}' - {ex.Message}");
                throw new Exception("Đã xảy ra lỗi khi thêm buổi học.");
            }
        }
    }

}

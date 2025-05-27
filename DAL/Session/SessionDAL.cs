using Desktop_Nhom13.DAL;
using Desktop_Nhom13.Models.Courses.Sessions;
using Desktop_Nhom13.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace Desktop_Nhom13.DAL
{
    public class SessionDAL
    {
        private readonly string sessionFolderPath;

        public SessionDAL()
        {
            sessionFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sessions");

            if (!Directory.Exists(sessionFolderPath))
            {
                Directory.CreateDirectory(sessionFolderPath);
            }
        }
        public static List<SessionData> GetSessionsByCourseID(int courseId)
        {
            var sessions = new List<SessionData>();
            string query = "SELECT SessionID, Title FROM Sessions WHERE CourseID = @CourseID ORDER BY CreatedAt ASC";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@CourseID", courseId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sessions.Add(new SessionData
                        {
                            SessionID = reader.GetInt32(0),
                            Title = reader.GetString(1)
                        });
                    }
                }
            }

            return sessions;
        }

        public static int AddSession(int courseId, string title)
        {
            string query = "INSERT INTO Sessions (CourseID, Title, CreatedAt) OUTPUT INSERTED.SessionID VALUES (@CourseID, @Title, GETDATE())";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@CourseID", courseId);
                cmd.Parameters.AddWithValue("@Title", title);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public List<SessionData> GetAllSessions()
        {
            var sessions = new List<SessionData>();

            var files = Directory.GetFiles(sessionFolderPath, "*.txt");
            foreach (var file in files)
            {
                var session = LoadSessionFromFile(file);
                if (session != null)
                {
                    sessions.Add(session);
                }
            }

            return sessions;
        }

        public SessionData LoadSessionFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            var lines = File.ReadAllLines(filePath);
            var session = new SessionData();

            int i = 0;
            if (lines.Length > 0 && lines[0].StartsWith("SessionTitle: "))
                session.Title = lines[0].Substring("SessionTitle: ".Length).Trim();
            while (i < lines.Length && lines[i].Trim() != "AttachedFiles:") i++;
            i++;
            while (i < lines.Length && !lines[i].StartsWith("Assignments:"))
            {
                var line = lines[i].Trim();
                if (line.StartsWith("- "))
                {
                    var parts = line.Substring(2).Split('|');
                    if (parts.Length >= 2)
                    {
                        session.AttachedFiles.Add(new FileItem
                        {
                            FileName = parts[0],
                            FilePath = parts[1]
                        });
                    }
                }
                i++;
            }
            if (i < lines.Length && lines[i].Trim() == "Assignments:")
            {
                i++;
                while (i < lines.Length)
                {
                    var line = lines[i].Trim();
                    if (line.StartsWith("- "))
                    {
                        var parts = line.Substring(2).Split('|');
                        if (parts.Length >= 2)
                        {
                            session.Assignments.Add(new AssignmentData
                            {
                                Title = parts[0],
                                AssignmentType = parts[1],
                                FilePath = parts.Length > 2 ? parts[2] : null
                            });
                        }
                    }
                    i++;
                }
            }

            return session;
        }

        public void InsertDocument(int courseId, int teacherId, int sessionId, string title, string filePath, string documentType)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO CourseDocuments (CourseID, Title, FilePath, UploadDate, UploadedBy, DocumentType, SessionID) " +
                                   "VALUES (@CourseID, @Title, @FilePath, GETDATE(), @UploadedBy, @DocumentType, @SessionID)";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CourseID", courseId);
                        cmd.Parameters.AddWithValue("@Title", title);
                        cmd.Parameters.AddWithValue("@FilePath", filePath);
                        cmd.Parameters.AddWithValue("@UploadedBy", teacherId);
                        cmd.Parameters.AddWithValue("@DocumentType", documentType);
                        cmd.Parameters.AddWithValue("@SessionID", sessionId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"InsertDocument Error: {ex}");
                throw;
            }
        }

        public void UpdateSessionTitle(int sessionId, string newTitle)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE Sessions SET Title = @Title WHERE SessionID = @ID";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Title", newTitle);
                        cmd.Parameters.AddWithValue("@ID", sessionId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"UpdateSessionTitle Error: {ex}");
                throw;
            }
        }

        public void DeleteAssignment(int assignmentId, SqlConnection conn)
        {
            var commands = new[]
            {
                "DELETE FROM StudentAnswers WHERE AssignmentID = @ID",
                "DELETE FROM Questions WHERE AssignmentID = @ID",
                "DELETE FROM AssignmentMC WHERE AssignmentID = @ID",
                "DELETE FROM StudentSubmissions WHERE AssignmentID = @ID",
                "DELETE FROM AssignmentFiles WHERE AssignmentID = @ID",
                "DELETE FROM Assignments WHERE AssignmentID = @ID"
            };

            foreach (string query in commands)
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", assignmentId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteSession(int sessionId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    List<int> assignmentIds = new List<int>();
                    using (var cmd = new SqlCommand("SELECT AssignmentID FROM Assignments WHERE SessionID = @SID", conn))
                    {
                        cmd.Parameters.AddWithValue("@SID", sessionId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                assignmentIds.Add(reader.GetInt32(0));
                        }
                    }

                    foreach (int assignmentId in assignmentIds)
                    {
                        DeleteAssignment(assignmentId, conn);
                    }

                    using (var cmd = new SqlCommand("DELETE FROM CourseDocuments WHERE SessionID = @SID", conn))
                    {
                        cmd.Parameters.AddWithValue("@SID", sessionId);
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new SqlCommand("DELETE FROM Sessions WHERE SessionID = @SID", conn))
                    {
                        cmd.Parameters.AddWithValue("@SID", sessionId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"DeleteSession Error: {ex}");
                throw;
            }
        }
    }
}

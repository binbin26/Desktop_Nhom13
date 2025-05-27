﻿using ClosedXML.Excel;
using Desktop_Nhom13.DAL;
using Desktop_Nhom13.Models.Assignments;
using Desktop_Nhom13.Models.Courses;
using Desktop_Nhom13.Utilities;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office.CustomXsn;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

public class AssignmentBLL
{
    private readonly AssignmentDAL _assignmentDAL;
    private static AssignmentBLL _instance;

    public AssignmentBLL(AssignmentDAL assignmentDAL)
    {
        _assignmentDAL = assignmentDAL;
    }
    public AssignmentBLL() : this(new AssignmentDAL())
    {

    }
    public static AssignmentBLL Instance
    {
        get
        {
            if (_instance == null)
                _instance = new AssignmentBLL();
            return _instance;
        }
    }
    public static bool UploadEssayAssignment(int teacherId, int courseId, int sessionId, string sourceFilePath, out string message)
    {
        try
        {
            string fileName = Path.GetFileName(sourceFilePath);
            string destPath = Path.Combine(Application.StartupPath, "Assignments", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(destPath));
            File.Copy(sourceFilePath, destPath, true);

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string title = Path.GetFileNameWithoutExtension(fileName);
                int assignmentId = AssignmentDAL.InsertAssignment(conn, courseId, title, sessionId, teacherId);

                string relativePath = Path.Combine("Assignments", fileName);
                AssignmentDAL.InsertAssignmentFile(conn, assignmentId, courseId, fileName, relativePath);
            }

            message = "Tạo bài tập tự luận thành công.";
            return true;
        }
        catch (IOException ex)
        {
            message = "Lỗi file: " + ex.Message;
            Logger.LogError("Lỗi khi sao chép file bài tập tự luận: " + ex.Message);
            return false;
        }
        catch (SqlException ex)
        {
            message = "Lỗi cơ sở dữ liệu: " + ex.Message;
            Logger.LogError("Lỗi SQL khi tạo bài tập tự luận: " + ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            message = "Lỗi không xác định: " + ex.Message;
            Logger.LogError("Lỗi không xác định khi tạo bài tập tự luận: " + ex.Message);
            return false;
        }
    }
    public static bool CreateMultipleChoiceAssignment(
        int teacherId,
        int courseId,
        int sessionId,
        List<Question> questions,
        int duration,
        float passScore,
        int maxAttempts,
        out string message)
    {
        try
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string title = "Bài tập trắc nghiệm";
                DateTime dueDate = DateTime.Now.AddMinutes(duration);

                // 1. Thêm Assignment
                int assignmentId = AssignmentDAL.InsertAssignment(conn, courseId, sessionId, title, teacherId, dueDate);

                // 2. Thêm thông tin AssignmentMC
                AssignmentDAL.InsertAssignmentMC(conn, assignmentId, questions.Count, maxAttempts, passScore, duration);

                // 3. Thêm danh sách câu hỏi
                foreach (var q in questions)
                {
                    AssignmentDAL.InsertQuestion(conn, assignmentId, q);
                }
            }

            message = "Tạo bài tập trắc nghiệm thành công!";
            Logger.LogInfo($"Bài tập trắc nghiệm đã được tạo thành công cho khóa học ID: {courseId}, giáo viên ID: {teacherId}");
            return true;
        }
        catch (SqlException ex)
        {
            message = "Lỗi cơ sở dữ liệu: " + ex.Message;
            return false;
        }
        catch (Exception ex)
        {
            message = "Lỗi không xác định: " + ex.Message;
            return false;
        }
    }
    public string GetAssignmentType(int assignmentId)
    {
        try
        {
            return _assignmentDAL.GetAssignmentType(assignmentId);
        }
        catch (Exception ex)
        {
            Logger.LogError("Lỗi khi lấy loại bài tập: " + ex.Message);
            throw new Exception("Không thể xác định loại bài tập. Vui lòng thử lại.");
        }
    }

    public Result SaveStudentAnswers(int assignmentId, int studentId, Dictionary<int, string> answers)
    {
        try
        {
            _assignmentDAL.SaveStudentAnswers(assignmentId, studentId, answers);

            return new Result { IsSuccess = true };
        }
        catch (Exception ex)
        {
            Logger.LogError("Lỗi khi lưu bài làm Sinh viên: " + ex.Message);
            return new Result
            {
                IsSuccess = false,
                ErrorMessage = $"Lỗi khi lưu đáp án: {ex.Message}"
            };
        }
    }

    public List<Assignments> GetAssignmentsCreatedByTeacher(int teacherId)
    {
        return _assignmentDAL.GetAssignmentsCreatedByTeacher(teacherId);
    }


    public List<Question> LoadQuestions(int assignmentId)
    {
        try
        {
            return _assignmentDAL.GetQuestionsByAssignmentId(assignmentId);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi khi tải danh sách câu hỏi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return new List<Question>();
        }
    }

    public int GetDuration(int assignmentId)
    {
        try
        {
            return _assignmentDAL.GetDuration(assignmentId);
        }
        catch (Exception ex)
        {
            Logger.LogError("Lỗi khi lấy thời gian làm bài: " + ex.Message);
            throw new ApplicationException("Không thể lấy thời gian làm bài. Vui lòng thử lại.");
        }
    }
    //Xử lý file bài tập tự luận cho sinh viên
    public string SaveEssaySubmission(int assignmentId, int studentId, string sourceFilePath)
    {
        if (!File.Exists(sourceFilePath))
            return null;

        string fileName = Path.GetFileName(sourceFilePath);
        string submissionsFolder = Path.Combine(Application.StartupPath, "Submissions");

        if (!Directory.Exists(submissionsFolder))
            Directory.CreateDirectory(submissionsFolder);

        string uniqueFileName = $"{studentId}_{assignmentId}_{fileName}";
        string destinationPath = Path.Combine(submissionsFolder, uniqueFileName);

        try
        {
            File.Copy(sourceFilePath, destinationPath, true);

            string relativePath = Path.Combine("Submissions", uniqueFileName);
            _assignmentDAL.SaveSubmissionToDatabase(assignmentId, studentId, fileName, relativePath, DateTime.Now);
            Logger.LogInfo($"Đã lưu file bài tập tại: {relativePath}");
            return relativePath;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Lỗi khi lưu file nộp bài tự luận:" + ex.Message);
            return null;
        }
    }

    public string GetEssay(int assignmentId)
    {
        try
        {
            return _assignmentDAL.GetEssay(assignmentId);
        }
        catch (Exception ex)
        {
            Logger.LogError("Lỗi khi lấy đường dẫn file đề bài tự luận: " + ex.Message);
            throw new ApplicationException("Không thể lấy đường dẫn file đề bài tự luận. Vui lòng thử lại.");
        }
    }

    public List<CourseDocuments> GetCourseDocuments(int courseId)
    {
        try
        {
            return _assignmentDAL.GetCourseDocuments(courseId);
        }
        catch (SqlException sqlEx)
        {
            Logger.LogError("Lỗi SQL khi lấy tài liệu khóa học: " + sqlEx.Message);
            throw new ApplicationException("Lỗi cơ sở dữ liệu khi truy xuất tài liệu.");
        }
        catch (InvalidOperationException invalidOpEx)
        {
            Logger.LogError("Thao tác không hợp lệ khi lấy tài liệu khóa học: " + invalidOpEx.Message);
            throw new ApplicationException("Đã xảy ra lỗi trong quá trình xử lý yêu cầu.");
        }
        catch (NullReferenceException nullRefEx)
        {
            Logger.LogError("Lỗi null khi lấy tài liệu khóa học: " + nullRefEx.Message);
            throw new ApplicationException("Dữ liệu không hợp lệ hoặc bị thiếu.");
        }
        catch (InvalidCastException castEx)
        {
            Logger.LogError("[Lỗi ép kiểu] " + castEx.Message + "\nStackTrace: " + castEx.StackTrace);
            throw new ApplicationException("Lỗi định dạng dữ liệu. Vui lòng kiểm tra kiểu dữ liệu từ CSDL.");
        }
        catch (IndexOutOfRangeException indexEx)
        {
            Logger.LogError("Lỗi truy cập cột dữ liệu: " + indexEx.Message + "\n" + indexEx.StackTrace);
            throw new ApplicationException("Lỗi khi đọc dữ liệu từ bảng. Có thể tên cột không đúng.");
        }
        catch (Exception ex)
        {
            Logger.LogError("Lỗi không xác định khi lấy tài liệu khóa học: " + ex.Message);
            throw new ApplicationException("Không thể lấy tài liệu khóa học. Vui lòng thử lại.");
        }
    }

    public List<Assignments> GetAssignmentsForStudentWithStatus(string username)
    {
        try
        {
            var assignments = _assignmentDAL.GetAssignmentsForStudentWithStatus(username);

            if (assignments == null)
            {
                Logger.LogError($"Không tìm thấy username: {username} trong hệ thống.");
                throw new ArgumentException($"Không tìm thấy tài khoản sinh viên với tên đăng nhập: {username}");
            }

            return assignments;
        }
        catch (SqlException sqlEx)
        {
            Logger.LogError($"[SQL ERROR] Lỗi truy vấn CSDL cho sinh viên '{username}': {sqlEx}");
            throw new ApplicationException("Đã xảy ra lỗi truy vấn cơ sở dữ liệu. Vui lòng thử lại hoặc liên hệ hỗ trợ kỹ thuật.", sqlEx);
        }
        catch (ArgumentException argEx)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError($"[UNKNOWN ERROR] Lỗi không xác định khi lấy bài tập cho sinh viên '{username}': {ex}");
            throw new ApplicationException("Đã xảy ra lỗi không xác định. Vui lòng thử lại hoặc liên hệ quản trị viên.", ex);
        }
    }

    public bool AddAssignment(Assignments assignment)
    {
        if (assignment == null) return false;
        if (assignment.CourseID <= 0) return false;
        if (string.IsNullOrWhiteSpace(assignment.Title)) return false;
        if (assignment.DueDate <= DateTime.Now) return false;
        if (assignment.MaxScore < 0) return false;
        return _assignmentDAL.AddAssignment(assignment);
    }

    public List<AssignmentMC> GetMultipleChoiceAssignmentIds(int teacherId, int courseId)
    {
        try
        {
            return _assignmentDAL.GetMultipleChoiceAssignmentIds(teacherId, courseId);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Lỗi khi lấy danh sách bài tập trắc nghiệm.", ex);
        }
    }

    public List<ProgressReportDTO> GetCourseProgress(int courseId)
    {
        var data = _assignmentDAL.GetProgressByCourse(courseId);

        foreach (var item in data)
        {
            item.CompletionRate = item.TotalAssignments == 0 ? 0 :
                (double)item.SubmittedAssignments / item.TotalAssignments;

            item.Rating = item.AverageGrade >= 8 ? "Giỏi" :
                          item.AverageGrade >= 6.5 ? "Khá" :
                          item.AverageGrade >= 5 ? "Trung bình" : "Yếu";
        }

        return data;
    }

    public List<QuestionStatsDTO> GetPerformance(int assignmentId)
    {
        return _assignmentDAL.GetQuestionPerformance(assignmentId);
    }
    // Lấy danh sách bài tự luận của một bài tập
    public List<EssaySubmissionDTO> GetEssaySubmissions(int assignmentId, int teacherId)
    {
        try
        {
            return _assignmentDAL.GetEssaySubmissions(assignmentId, teacherId);
        }
        catch (SqlException sqlEx)
        {
            MessageBox.Show("Lỗi truy vấn cơ sở dữ liệu khi lấy bài tự luận.\nChi tiết: " + sqlEx.Message,
                            "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw new ApplicationException("Lỗi truy vấn dữ liệu");
        }
        catch (ArgumentException argEx)
        {
            MessageBox.Show("Không tìm thấy giáo viên tương ứng.\nChi tiết: " + argEx.Message,
                            "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            throw new ApplicationException("Không tìm thấy giáo viên");
        }
    }
    //Cập nhật điểm bài tự luận từ giảng viên
    public bool UpdateSubmissionScore(int assignmentId, int studentId, decimal score, int teacherId)
    {
        try
        {
            Logger.LogInfo($"Đã cập nhật điểm cho sinh viên ID: {studentId} bài tập có ID {assignmentId}");
            return _assignmentDAL.UpdateSubmissionScore(assignmentId, studentId, score, teacherId);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Lỗi khi cập nhật điểm bài nộp.", ex);
        }
    }

    //Chấm điểm tự động bài trắc nghiệm
    public decimal AutoGradeQuiz(int assignmentId, int teacherId)
    {
        try
        {
            Logger.LogInfo($"Đã chấm điểm bài tập có ID: {assignmentId}");
            return _assignmentDAL.AutoGradeQuiz(assignmentId, teacherId);
        }
        catch (Exception ex)
        {
            Logger.LogError("Lỗi khi tự động chấm điểm bài trắc nghiệm" + ex.Message);
            return 0;
        }
    }

    public bool HasExceededMaxAttempts(int assignmentId, int studentId)
    {
        try
        {
            return _assignmentDAL.HasExceededMaxAttempts(assignmentId, studentId);
        }
        catch (Exception ex)
        {
            Logger.LogError("Lỗi vượt quá số lần làm bài tập " + ex.Message);
            throw new ApplicationException("Lỗi khi kiểm tra số lần làm bài", ex);
        }
    }

    //Lấy danh sách bài làm trắc nghiệm của một bài tập
    public List<QuizSubmissionDTO> GetQuizSubmissions(int assignmentId, int teacherId)
    {
        try
        {
            return _assignmentDAL.GetQuizSubmissions(assignmentId, teacherId);
        }
        catch (SqlException sqlEx)
        {
            MessageBox.Show("Lỗi truy vấn cơ sở dữ liệu khi lấy bài tự luận.\nChi tiết: " + sqlEx.Message,
                            "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw new ApplicationException("Lỗi truy vấn dữ liệu");
        }
        catch (ArgumentException argEx)
        {
            MessageBox.Show("Không tìm thấy giáo viên tương ứng.\nChi tiết: " + argEx.Message,
                            "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            throw new ApplicationException("Không tìm thấy giáo viên");
        }
    }


    public void ExportQuestionStatsToExcel(List<QuestionStatsDTO> data, string filePath)
    {
        try
        {
            Logger.LogInfo($"Bắt đầu export thống kê câu hỏi ra file: {filePath}");

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Report");

            ws.Cell(1, 1).Value = "Câu hỏi";
            ws.Cell(1, 2).Value = "Lượt trả lời";
            ws.Cell(1, 3).Value = "Tỉ lệ đúng (%)";
            ws.Cell(1, 4).Value = "Tỉ lệ sai (%)";
            ws.Cell(1, 5).Value = "Độ khó";

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];
                ws.Cell(i + 2, 1).Value = item.Content;
                ws.Cell(i + 2, 2).Value = item.TotalAnswers;
                ws.Cell(i + 2, 3).Value = item.CorrectRate;
                ws.Cell(i + 2, 4).Value = Math.Round(100 - item.CorrectRate, 2);
                ws.Cell(i + 2, 5).Value = item.Difficulty;
            }

            ws.Columns().AdjustToContents();
            wb.SaveAs(filePath);

            Logger.LogInfo($"Export hoàn tất. File đã lưu tại: {filePath}");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Lỗi khi export file Excel: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }

}
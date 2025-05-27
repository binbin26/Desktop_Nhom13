using System;
using System.Collections.Generic;

namespace Desktop_Nhom13.Models.Assignments
{
    public class QuizSubmissionDTO
    {
        public int AssignmentID { get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public DateTime? SubmitDate { get; set; }
        public decimal? Score { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public Dictionary<int, string> Answers { get; set; }
    }

    public class Result
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}

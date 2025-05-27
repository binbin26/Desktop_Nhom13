using System;

namespace Desktop_Nhom13.Models.Assignments
{
    public class AssignmentMC
    {
        public int AssignmentID { get; set; }
        public int TotalAnswers { get; set; }
        public int MaxAttempts { get; set; }
        public decimal PassScore { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Duration { get; set; }
    }
}

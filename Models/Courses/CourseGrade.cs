namespace Desktop_Nhom13.Models.Courses
{
    public class CourseGrade
    {
        public string CourseName { get; set; }
        public float? Score { get; set; }
        public string GradedBy { get; set; }
        public string ScoreText => Score.HasValue ? Score.Value.ToString("0.00") : "Chưa có điểm";
    }
}

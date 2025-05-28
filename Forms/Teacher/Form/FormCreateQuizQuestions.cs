using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using Desktop_Nhom13.Models.Assignments;

namespace Desktop_Nhom13.Forms.Teacher
{
    public partial class FormCreateQuizQuestions : Form
    {
        private int Total, Index = 0;
        private readonly List<Question> Questions = new List<Question>();
        private readonly int TeacherID, CourseID, SessionID, QuestionID;
        public int Duration { get; set; }
        public float PassScore { get; set; } = 5.0f;
        public int MaxAttempts { get; set; } = 1;

        public FormCreateQuizQuestions(int total, int teacherID, int courseId, int sessionId)
        {
            InitializeComponent();
            Total = total;
            TeacherID = teacherID;
            CourseID = courseId;
            SessionID = sessionId;
            LoadNext();
        }

        private void LoadNext()
        {
            if (Index == Total)
            {
                string message;
                bool result = AssignmentBLL.CreateMultipleChoiceAssignment(
                    TeacherID,
                    CourseID,
                    SessionID,
                    Questions,
                    Duration,
                    PassScore,
                    MaxAttempts,
                    out message
                );

                MessageBox.Show(message, result ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                    result ? MessageBoxIcon.Information : MessageBoxIcon.Error);

                this.Close();
                return;
            }

            panelMain.Controls.Clear();
            var questionControl = new UcQuestionCreator();
            questionControl.QuestionSubmitted += (q) =>
            {
                Questions.Add(q);
                Index++;
                LoadNext();
            };
            questionControl.Dock = DockStyle.Fill;
            panelMain.Controls.Add(questionControl);
        }

    }

}
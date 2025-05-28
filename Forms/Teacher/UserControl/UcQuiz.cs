﻿using Desktop_Nhom13.BLL;
using Desktop_Nhom13.DAL;
using Desktop_Nhom13.Models.Assignments;
using Desktop_Nhom13.Models.Courses;
using Desktop_Nhom13.BLL;
using Desktop_Nhom13.Forms.Teacher;
using Desktop_Nhom13.Models.Assignments;
using Desktop_Nhom13.Models.Courses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Desktop_Nhom13.Forms.Teacher.Usercontrol
{
    public partial class UcQuiz : UserControl
    {
        private int TeacherID;
        private AssignmentBLL assignmentBLL = new AssignmentBLL();
        public UcQuiz(int teacherId)
        {
            InitializeComponent();
            TeacherID = teacherId;
            LoadAssignments();
        }

        private void cboAssignments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboAssignments.SelectedItem is Assignments assignment)
            {
                LoadSubmissions(assignment.AssignmentID, TeacherID);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một bài tập.");
            }
        }

        private void LoadSubmissions(int assignmentId, int teacherID)
        {
            try
            {
                var submissions = assignmentBLL.GetQuizSubmissions(assignmentId, teacherID);
                if (submissions == null || submissions.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy bài nộp nào.");
                }
                else
                {
                    foreach (var submission in submissions)
                    {
                        dgvSubmissions.DataSource = submissions;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách bài nộp: " + ex.Message);
            }
        }

        private void dgvSubmissions_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSubmissions.CurrentRow?.DataBoundItem is QuizSubmissionDTO submission)
            {
                nudScore.Value = submission.Score.HasValue
                    ? Math.Min(nudScore.Maximum, submission.Score.Value)
                    : 0;
            }
        }

        private void btnSubmitScore_Click(object sender, EventArgs e)
        {
            if (dgvSubmissions.CurrentRow?.DataBoundItem is QuizSubmissionDTO submission)
            {
                decimal score = nudScore.Value;
                try
                {
                    assignmentBLL.UpdateSubmissionScore(submission.AssignmentID, submission.StudentID, score, TeacherID);
                    MessageBox.Show("Chấm điểm thành công.");
                    LoadSubmissions(submission.AssignmentID, TeacherID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi chấm điểm: " + ex.Message);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (cboAssignments.SelectedValue is int assignmentId)
                LoadSubmissions(assignmentId, TeacherID);
        }

        private Course ShowCourseSelectionDialog(List<Course> courses)
        {
            Form dialog = new Form() { Width = 400, Height = 150, Text = "Chọn môn học" };
            ComboBox cb = new ComboBox() { Left = 20, Top = 20, Width = 340 };
            cb.DataSource = courses;
            cb.DisplayMember = "CourseName";
            cb.ValueMember = "CourseID";

            Button ok = new Button() { Text = "OK", Left = 270, Width = 90, Top = 60, DialogResult = DialogResult.OK };
            dialog.Controls.Add(cb);
            dialog.Controls.Add(ok);
            dialog.AcceptButton = ok;

            return dialog.ShowDialog() == DialogResult.OK ? cb.SelectedItem as Course : null;
        }

        private void LoadAssignments()
        {
            try
            {
                var allAssignments = assignmentBLL.GetAssignmentsCreatedByTeacher(TeacherID);
                if (allAssignments == null || allAssignments.Count == 0)
                {
                    MessageBox.Show("Giáo viên chưa tạo bài tập nào.");
                    return;
                }
                var quizAssignments = new List<Assignments>();
                foreach (var assignment in allAssignments)
                {
                    string type = assignmentBLL.GetAssignmentType(assignment.AssignmentID);
                    if (type == "TracNghiem")
                    {
                        quizAssignments.Add(assignment);
                    }
                }

                if (quizAssignments.Count == 0)
                {
                    MessageBox.Show("Không có bài tập trắc nghiệm nào.");
                    return;
                }

                cboAssignments.DataSource = quizAssignments;
                cboAssignments.DisplayMember = "Title";
                cboAssignments.ValueMember = "AssignmentID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách bài tập: " + ex.Message);
            }
        }

        private void btnStatic_Click(object sender, EventArgs e)
        {
            var courseBLL = new CourseBLL();
            var courses = courseBLL.GetCoursesByTeacher(TeacherID);

            if (courses.Count == 0)
            {
                MessageBox.Show("Không có môn học nào.");
                return;
            }
            var selectedCourse = ShowCourseSelectionDialog(courses);
            if (selectedCourse != null)
            {
                MultipleChoiceProgress progressForm = new MultipleChoiceProgress(selectedCourse.CourseID, TeacherID);
                progressForm.Show();
            }
        }
    }
}

﻿using Desktop_Nhom13.Models.Users;
using Desktop_Nhom13.Models.Assignments;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Desktop_Nhom13.Forms.Student
{
    public partial class ucQuiz : UserControl
    {
        private int assignmentId;
        private int studentId;
        private int currentQuestionIndex = 0;
        private Timer countdownTimer;
        private TimeSpan remainingTime;
        private Dictionary<int, string> studentAnswers = new Dictionary<int, string>();
        private List<Question> questions = new List<Question>();
        private readonly AssignmentBLL _assignmentBLL = new AssignmentBLL();

        public ucQuiz(int assignmentId, int studentId)
        {
            InitializeComponent();
            this.assignmentId = assignmentId;
            this.studentId = studentId;
            remainingTime = TimeSpan.FromMinutes(_assignmentBLL.GetDuration(assignmentId));
            countdownTimer = new Timer();
            countdownTimer.Interval = 1000;
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();
            LoadQuestions();
            LoadQuestionButtons();
        }

        private void LoadQuestions()
        {
            try
            {
                questions = _assignmentBLL.LoadQuestions(assignmentId);
                currentQuestionIndex = 0;
                ShowQuestion();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải câu hỏi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Xác nhận nộp bài?", "Nộp bài", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                SubmitQuiz();
            }
        }


        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentQuestionIndex < questions.Count - 1)
            {
                currentQuestionIndex++;
                ShowQuestion();
            }
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));
            lblTimer.Text = $"{remainingTime:hh\\:mm\\:ss}";

            if (remainingTime.TotalSeconds <= 0)
            {
                countdownTimer.Stop();
                MessageBox.Show("Hết giờ! Bài tập sẽ được tự động nộp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SubmitQuiz();
            }
        }


        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentQuestionIndex > 0)
            {
                currentQuestionIndex--;
                ShowQuestion();
            }
        }

        private void Radio_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton selectedRadio = sender as RadioButton;
            if (selectedRadio == null || !selectedRadio.Checked)
                return;

            string selectedOption = "";
            if (selectedRadio == radioA) selectedOption = "A";
            else if (selectedRadio == radioB) selectedOption = "B";
            else if (selectedRadio == radioC) selectedOption = "C";
            else if (selectedRadio == radioD) selectedOption = "D";
            int currentQuestionId = GetCurrentQuestionId();
            if (studentAnswers.ContainsKey(currentQuestionId))
                studentAnswers[currentQuestionId] = selectedOption;
            else
                studentAnswers.Add(currentQuestionId, selectedOption);
        }

        private void ShowQuestion()
        {
            if (questions.Count == 0) return;

            var question = questions[currentQuestionIndex];
            lblQuestion.Text = question.QuestionText;
            radioA.Text = $"A. {question.OptionA}";
            radioB.Text = $"B. {question.OptionB}";
            radioC.Text = $"C. {question.OptionC}";
            radioD.Text = $"D. {question.OptionD}";

            lblQuestionNumber.Text = $"Câu {currentQuestionIndex + 1}/{questions.Count}";

            if (studentAnswers.TryGetValue(question.QuestionID, out string answer))
            {
                radioA.Checked = answer == "A";
                radioB.Checked = answer == "B";
                radioC.Checked = answer == "C";
                radioD.Checked = answer == "D";
            }
            else
            {
                radioA.Checked = radioB.Checked = radioC.Checked = radioD.Checked = false;
            }

            UpdateNavigationButtons();
        }

        private void UpdateNavigationButtons()
        {
            btnPrevious.Enabled = currentQuestionIndex > 0;
            btnNext.Enabled = currentQuestionIndex < questions.Count - 1;
        }


        private int GetCurrentQuestionId()
        {
            return questions[currentQuestionIndex].QuestionID;
        }

        private void LoadQuestionButtons()
        {
            flpQuestionList.Controls.Clear();

            for (int i = 0; i < questions.Count; i++)
            {
                int questionIndex = i;
                Button btn = new Button
                {
                    Text = (i + 1).ToString(),
                    Width = 40,
                    Height = 40,
                    Margin = new Padding(5),
                    Tag = questionIndex
                };

                btn.Click += (s, e) =>
                {
                    currentQuestionIndex = (int)((Button)s).Tag;
                    ShowQuestion();
                };

                flpQuestionList.Controls.Add(btn);
            }
        }

        private void SubmitQuiz()
        {
            try
            {
                var submissionResult = _assignmentBLL.SaveStudentAnswers(
                    assignmentId,
                    studentId,
                    studentAnswers
                );

                if (!submissionResult.IsSuccess)
                {
                    MessageBox.Show($"Có lỗi khi nộp bài: {submissionResult.ErrorMessage}");
                    return;
                }

                decimal score = _assignmentBLL.AutoGradeQuiz(assignmentId, studentId);

                MessageBox.Show($"Bài đã được nộp.\nĐiểm của bạn là: {score:0.00}/10", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);

                radioA.Enabled = radioB.Enabled = radioC.Enabled = radioD.Enabled = false;
                btnNext.Enabled = btnPrevious.Enabled = btnSubmit.Enabled = false;
                countdownTimer?.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nộp bài: {ex.Message}");
            }
        }
    }
}
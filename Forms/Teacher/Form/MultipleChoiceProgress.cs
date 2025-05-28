using Desktop_Nhom13.BLL;
using Desktop_Nhom13.Models.Assignments;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Desktop_Nhom13.Forms.Teacher
{
    public partial class MultipleChoiceProgress : Form
    {
        private int CourseId;
        private int currentAssignmentId;
        private int TeacherID;
        public MultipleChoiceProgress(int courseId, int teacherID)
        {
            InitializeComponent();
            CourseId = courseId;
            TeacherID = teacherID;
            LoadAssignments();
        }


        private void LoadAssignments()
        {
            try
            {
                var mcAssignment = new AssignmentBLL().GetMultipleChoiceAssignmentIds(TeacherID, CourseId);

                if (mcAssignment == null || mcAssignment.Count == 0)
                {
                    cbAssignments.DataSource = null;
                    MessageBox.Show("Không có bài tập trắc nghiệm nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                cbAssignments.DataSource = mcAssignment;
                cbAssignments.DisplayMember = "AssignmentID";
                cbAssignments.ValueMember = "AssignmentID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải danh sách bài tập trắc nghiệm.\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbAssignments_SelectedIndexChanged(object sender, EventArgs e)
        {
            AssignmentMC selectedAssignment = cbAssignments.SelectedItem as AssignmentMC;
            if (selectedAssignment != null)
            {
                int assignmentId = selectedAssignment.AssignmentID;
                LoadPerformanceData(assignmentId);
            }
        }

        private void dgvPerformance_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                currentAssignmentId = Convert.ToInt32(dgvPerformance.Rows[e.RowIndex].Cells["AssignmentID"].Value);
            }
        }

        private void LoadPerformanceData(int assignmentId)
        {
            var data = new AssignmentBLL().GetPerformance(assignmentId);
            dgvPerformance.DataSource = data;
            DrawChart(data);
        }

        private void DrawChart(List<QuestionStatsDTO> data)
        {
            chartPerformance.Series.Clear();
            var series = new Series("Correct %");
            series.ChartType = SeriesChartType.Column;
            series.Color = Color.SeaGreen;

            foreach (var item in data)
            {
                series.Points.AddXY($"Q{item.QuestionID}", item.CorrectRate);
            }

            chartPerformance.Series.Add(series);
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (cbAssignments.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn một bài tập để xuất báo cáo.");
                return;
            }

            int selectedAssignmentId = (int)cbAssignments.SelectedValue;

            var bll = new AssignmentBLL();
            var data = bll.GetPerformance(selectedAssignmentId);

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Excel Files|*.xlsx";
                saveDialog.Title = "Chọn nơi lưu báo cáo";
                saveDialog.FileName = "PhanTichCauHoi.xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    bll.ExportQuestionStatsToExcel(data, saveDialog.FileName);
                    MessageBox.Show("Xuất Excel thành công!");
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (cbAssignments.SelectedValue != null)
            {
                int assignmentId = (int)cbAssignments.SelectedValue;
                LoadPerformanceData(assignmentId);
            }
        }
    }
}
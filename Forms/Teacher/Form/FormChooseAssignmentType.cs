using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace Desktop_Nhom13.Forms.Teacher
{
    public partial class FormChooseAssignmentType : Form
    {
        private int TeacherID;
        private int CourseID;
        private int SessionID;
        public FormChooseAssignmentType(int teacherID, int courseId, int sessionId)
        {
            InitializeComponent();
            TeacherID = teacherID;
            CourseID = courseId;
            SessionID = sessionId;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (rbMultipleChoice.Checked)
            {
                var setupQuiz = new FormSetupQuiz(TeacherID, CourseID, SessionID);
                setupQuiz.ShowDialog();
            }
            else if (rbEssay.Checked)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string message;
                    bool result = AssignmentBLL.UploadEssayAssignment(TeacherID, CourseID, SessionID, dialog.FileName, out message);
                    MessageBox.Show(message, result ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                        result ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn loại bài tập.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            this.Close();

        }
    }
}
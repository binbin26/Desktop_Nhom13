using Desktop_Nhom13.Forms.Auth;
using Desktop_Nhom13.Forms.Teacher.Usercontrol;
using System;
using System.Windows.Forms;

namespace Desktop_Nhom13.Forms.Teacher
{
    public partial class TeacherForm : Form
    {
        private readonly int TeacherID;

        public TeacherForm(int teacherId)
        {
            InitializeComponent();
            TeacherID = teacherId;
        }

        private void TeacherForm_Load(object sender, EventArgs e)
        {
            btnCourses.PerformClick(); // Load tab mặc định khi mở
        }
        private void btnQuiz_Click(object sender, EventArgs e)
        {
            LoadControl(new UcQuiz(TeacherID));
        }
        private void btnCourses_Click(object sender, EventArgs e)
        {
            LoadControl(new UcCourses(TeacherID));
        }

        private void btnSubmissions_Click(object sender, EventArgs e)
        {
            LoadControl(new UcSubmissions(TeacherID));
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            LoadControl(new UcProfile(TeacherID));
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // TODO: Thực hiện lưu dữ liệu cần thiết tại đây nếu có
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        public void LoadControl(UserControl uc)
        {
            panelContent.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            panelContent.Controls.Add(uc);
        }
    }
}
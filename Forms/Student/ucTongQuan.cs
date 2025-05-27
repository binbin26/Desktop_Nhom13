using Desktop_Nhom13.BLL;
using Desktop_Nhom13.Models.Users;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Desktop_Nhom13.Forms.Student
{
    public partial class ucTongQuan : UserControl
    {

        private string _username;
        UserBLL userBLL = new UserBLL();

        public ucTongQuan(string username)
        {
            _username = username;
            InitializeComponent();
            LoadStudentInfo(_username);
        }


        private void LoadStudentInfo(string username)
        {
            try
            {
                User user = userBLL.GetUserByUsername(username);
                if (user != null)
                {
                    lblFullName.Text = user.FullName;
                    lblEmail.Text = user.Email;
                    lblRole.Text = user.Role;
                    lblCity.Text = user.QueQuan;
                    lblPhone.Text = user.SoDienThoai;

                    if (!string.IsNullOrEmpty(user.AvatarPath) && File.Exists(user.AvatarPath))
                    {
                        if (AvatarPict.Image != null)
                        {
                            AvatarPict.Image.Dispose();
                            AvatarPict.Image = null;
                        }
                        AvatarPict.Image = new Bitmap(user.AvatarPath);
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin người dùng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải thông tin học sinh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPass_Click(object sender, EventArgs e)
        {
            string oldPass = txtOP.Text.Trim() ?? string.Empty;
            string newPass = txtNP.Text.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(oldPass) || string.IsNullOrEmpty(newPass))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success = userBLL.ChangeUserPassword(_username, oldPass, newPass);

            if (success)
            {
                MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtOP.Clear();
                txtNP.Clear();
            }
            else
            {
                MessageBox.Show("Mật khẩu cũ không đúng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AvatarPict_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Chọn ảnh đại diện";
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFile = openFileDialog.FileName;
                string fileExtension = Path.GetExtension(selectedFile);
                string newFileName = _username + fileExtension;
                string saveDirectory = Path.Combine(Application.StartupPath, "Avatars", "Student");

                if (!Directory.Exists(saveDirectory))
                {
                    Directory.CreateDirectory(saveDirectory);
                }

                string savePath = Path.Combine(saveDirectory, newFileName);

                try
                {
                    Directory.CreateDirectory(saveDirectory);
                    if (AvatarPict.Image != null)
                    {
                        AvatarPict.Image.Dispose();
                        AvatarPict.Image = null;
                    }
                    File.Copy(selectedFile, savePath, true);
                    bool updated = userBLL.ChangeUserAvatar(_username, savePath);

                    if (updated)
                    {
                        if (AvatarPict.Image != null)
                        {
                            AvatarPict.Image.Dispose();
                            AvatarPict.Image = null;
                        }
                        AvatarPict.Image = new Bitmap(savePath);
                        MessageBox.Show("Ảnh đại diện đã được cập nhật!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    else
                    {
                        MessageBox.Show("Cập nhật không thành công!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi cập nhật ảnh đại diện: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClosedXML.Excel;
using Desktop_Nhom13.BLL;
using Desktop_Nhom13.DAL;
using static Desktop_Nhom13.BLL.UserBLL;

namespace Desktop_Nhom13.Forms.Teacher
{
    public partial class UcProfile : UserControl
    {
        private int UserID;

        public UcProfile(int userId)
        {
            InitializeComponent();
            UserID = userId;
            LoadProfile();
        }

        private void LoadProfile()
        {
            try
            {
                UserProfile profile = UserBLL.GetProfile(UserID);
                if (profile != null)
                {
                    txtName.Text = profile.FullName;
                    txtEmail.Text = profile.Email;
                    txtPhone.Text = profile.Phone;
                    txtAddress.Text = profile.Address;
                    txtRole.Text = profile.Role;

                    if (!string.IsNullOrEmpty(profile.AvatarPath))
                    {
                        string fullPath = Path.Combine(Application.StartupPath, profile.AvatarPath.Replace("/", "\\"));
                        if (File.Exists(fullPath))
                            picAvatar.Image = Image.FromFile(fullPath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            try
            {
                UserBLL.ChangePassword(UserID, txtOldPassword.Text, txtNewPassword.Text);
                MessageBox.Show("Đổi mật khẩu thành công.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnChooseAvatar_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Ảnh (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string destPath = UserBLL.UpdateAvatar(UserID, dialog.FileName);
                    picAvatar.Image = Image.FromFile(destPath);
                    MessageBox.Show("Cập nhật ảnh đại diện thành công.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

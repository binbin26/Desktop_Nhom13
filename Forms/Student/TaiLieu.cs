using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Desktop_Nhom13.Forms.Student
{
    public partial class TaiLieu : Form
    {
        private int courseId;
        private string filePath;
        public TaiLieu(int courseId)
        {
            InitializeComponent();
            this.courseId = courseId;
            LoadDocuments();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            filePath = btn.Tag.ToString();

            if (File.Exists(filePath))
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    FileName = Path.GetFileName(filePath),
                    Filter = "All files (*.*)|*.*"
                };
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(filePath, saveDialog.FileName, true);
                    MessageBox.Show("Tải tài liệu thành công.");
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy tài liệu.");
            }
        }

        private void LoadDocuments()
        {
            var documents = AssignmentBLL.Instance.GetCourseDocuments(courseId);
            flowPanel.Controls.Clear();
            var grouped = documents
                .Where(d => d.SessionID != 0)
                .GroupBy(d => new { d.SessionID, d.SessionTitle, d.CreatedAt })
                .OrderBy(g => g.Key.CreatedAt);

            foreach (var group in grouped)
            {
                Label lblSession = new Label();
                lblSession.Text = group.Key.SessionTitle ?? "Buổi học không rõ";
                lblSession.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                lblSession.AutoSize = true;
                lblSession.Margin = new Padding(10, 15, 10, 5);
                flowPanel.Controls.Add(lblSession);

                foreach (var doc in group)
                {
                    Panel panel = new Panel();
                    panel.BorderStyle = BorderStyle.FixedSingle;
                    panel.Width = 740;
                    panel.Height = 80;
                    panel.Padding = new Padding(10);
                    panel.Margin = new Padding(10);

                    Label lblTitle = new Label();
                    lblTitle.Text = doc.Title;
                    lblTitle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                    lblTitle.AutoSize = true;
                    lblTitle.Location = new Point(10, 25);

                    Button btnDownload = new Button();
                    btnDownload.Text = "Tải về";
                    btnDownload.Tag = doc.FilePath;
                    btnDownload.Click += btnDownload_Click;
                    btnDownload.Location = new Point(650, 25);

                    panel.Controls.Add(lblTitle);
                    panel.Controls.Add(btnDownload);
                    flowPanel.Controls.Add(panel);
                }
            }
        }
    }

}

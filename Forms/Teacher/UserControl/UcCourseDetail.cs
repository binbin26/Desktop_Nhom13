using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Desktop_Nhom13.BLL;
using Desktop_Nhom13.DAL;
using Desktop_Nhom13.Models.Courses;

namespace Desktop_Nhom13.Forms.Teacher
{
    public partial class UcCourseDetail : UserControl
    {
        private Course currentCourse;

        public UcCourseDetail(Course course)
        {
            InitializeComponent();
            currentCourse = course;
            lblCourseName.Text = course.CourseName;
            LoadSessions();
        }

        private void LoadSessions()
        {
            flowPanelSessions.Controls.Clear();
            var sessions = SessionBLL.GetSessions(currentCourse.CourseID);

            foreach (var session in sessions)
            {
                var sessionItem = new UcSessionItem(currentCourse.TeacherID, currentCourse.CourseID, session.SessionID, session.Title);
                sessionItem.Width = flowPanelSessions.Width - 30;
                flowPanelSessions.Controls.Add(sessionItem);
            }
        }

        private void btnAddSession_Click(object sender, EventArgs e)
        {
            //var form = new FormInputTitle();
            //if (form.ShowDialog() == DialogResult.OK)
            //{
            //    try
            //    {
            //        var newSession = SessionBLL.AddSession(currentCourse.CourseID, form.InputTitle);
            //        var sessionItem = new UcSessionItem(currentCourse.TeacherID, currentCourse.CourseID, newSession.SessionID, newSession.Title);
            //        sessionItem.Width = flowPanelSessions.Width - 30;
            //        flowPanelSessions.Controls.Add(sessionItem);
            //    }
            //    catch (ArgumentException ex)
            //    {
            //        MessageBox.Show(ex.Message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("Đã xảy ra lỗi khi thêm buổi học:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
        }
    }
}
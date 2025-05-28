using System.Windows.Forms;

namespace Desktop_Nhom13.Forms.Teacher.Usercontrol
{
    partial class UcQuiz
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblSelectAssignment;
        private System.Windows.Forms.ComboBox cboAssignments;
        private System.Windows.Forms.DataGridView dgvSubmissions;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.NumericUpDown nudScore;
        private System.Windows.Forms.Button btnSubmitScore;
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblSelectAssignment = new System.Windows.Forms.Label();
            this.cboAssignments = new System.Windows.Forms.ComboBox();
            this.dgvSubmissions = new System.Windows.Forms.DataGridView();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.nudScore = new System.Windows.Forms.NumericUpDown();
            this.btnSubmitScore = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStatic = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubmissions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScore)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSelectAssignment
            // 
            this.lblSelectAssignment.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblSelectAssignment.Location = new System.Drawing.Point(20, 10);
            this.lblSelectAssignment.Name = "lblSelectAssignment";
            this.lblSelectAssignment.Size = new System.Drawing.Size(100, 24);
            this.lblSelectAssignment.TabIndex = 0;
            this.lblSelectAssignment.Text = "Chọn bài tập:";
            // 
            // cboAssignments
            // 
            this.cboAssignments.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAssignments.Location = new System.Drawing.Point(130, 10);
            this.cboAssignments.Name = "cboAssignments";
            this.cboAssignments.Size = new System.Drawing.Size(250, 24);
            this.cboAssignments.TabIndex = 1;
            this.cboAssignments.SelectedIndexChanged += new System.EventHandler(this.cboAssignments_SelectedIndexChanged);
            // 
            // dgvSubmissions
            // 
            this.dgvSubmissions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSubmissions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSubmissions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSubmissions.Location = new System.Drawing.Point(20, 50);
            this.dgvSubmissions.Name = "dgvSubmissions";
            this.dgvSubmissions.ReadOnly = true;
            this.dgvSubmissions.RowHeadersWidth = 51;
            this.dgvSubmissions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSubmissions.Size = new System.Drawing.Size(900, 526);
            this.dgvSubmissions.TabIndex = 0;
            this.dgvSubmissions.SelectionChanged += new System.EventHandler(this.dgvSubmissions_SelectionChanged);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnRefresh.Location = new System.Drawing.Point(20, 623);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(196, 30);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Tải lại";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // nudScore
            // 
            this.nudScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.nudScore.Location = new System.Drawing.Point(569, 627);
            this.nudScore.Name = "nudScore";
            this.nudScore.Size = new System.Drawing.Size(60, 26);
            this.nudScore.TabIndex = 4;
            // 
            // btnSubmitScore
            // 
            this.btnSubmitScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnSubmitScore.Location = new System.Drawing.Point(275, 623);
            this.btnSubmitScore.Name = "btnSubmitScore";
            this.btnSubmitScore.Size = new System.Drawing.Size(250, 30);
            this.btnSubmitScore.TabIndex = 5;
            this.btnSubmitScore.Text = "Chấm điểm";
            this.btnSubmitScore.Click += new System.EventHandler(this.btnSubmitScore_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.label1.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.label1.Location = new System.Drawing.Point(386, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 29);
            this.label1.TabIndex = 6;
            this.label1.Text = "Bài tập trắc nghiệm";
            // 
            // btnStatic
            // 
            this.btnStatic.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnStatic.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.btnStatic.Location = new System.Drawing.Point(767, 10);
            this.btnStatic.Name = "btnStatic";
            this.btnStatic.Size = new System.Drawing.Size(153, 34);
            this.btnStatic.TabIndex = 7;
            this.btnStatic.Text = "Thống kê câu hỏi";
            this.btnStatic.UseVisualStyleBackColor = true;
            this.btnStatic.Click += new System.EventHandler(this.btnStatic_Click);
            // 
            // UcQuiz
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnStatic);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblSelectAssignment);
            this.Controls.Add(this.cboAssignments);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.nudScore);
            this.Controls.Add(this.btnSubmitScore);
            this.Controls.Add(this.dgvSubmissions);
            this.Name = "UcQuiz";
            this.Size = new System.Drawing.Size(952, 681);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubmissions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScore)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private Button btnStatic;
    }
}

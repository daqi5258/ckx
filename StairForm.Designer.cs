namespace ckx
{
    partial class StairForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StairForm));
            this.SubmitButton = new System.Windows.Forms.Button();
            this.ClearButton = new System.Windows.Forms.Button();
            this.InformData = new System.Windows.Forms.DataGridView();
            this.NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FloorWidth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FloorHeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LTW = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LTH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LTN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EXTW = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EXTW2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EXTH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.COVER = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SW = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.InformData)).BeginInit();
            this.SuspendLayout();
            // 
            // SubmitButton
            // 
            this.SubmitButton.Location = new System.Drawing.Point(451, 5);
            this.SubmitButton.Name = "SubmitButton";
            this.SubmitButton.Size = new System.Drawing.Size(75, 23);
            this.SubmitButton.TabIndex = 0;
            this.SubmitButton.Text = "生成";
            this.SubmitButton.UseVisualStyleBackColor = true;
            this.SubmitButton.Click += new System.EventHandler(this.SubmitButton_Click);
            // 
            // ClearButton
            // 
            this.ClearButton.Location = new System.Drawing.Point(370, 5);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(75, 23);
            this.ClearButton.TabIndex = 1;
            this.ClearButton.Text = "清空数据";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // InformData
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.InformData.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.InformData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.InformData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NO,
            this.FloorWidth,
            this.FloorHeight,
            this.LTW,
            this.LTH,
            this.LTN,
            this.EXTW,
            this.EXTW2,
            this.EXTH,
            this.COVER,
            this.SH,
            this.SW});
            this.InformData.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.InformData.Location = new System.Drawing.Point(0, 34);
            this.InformData.Name = "InformData";
            this.InformData.RowTemplate.Height = 23;
            this.InformData.Size = new System.Drawing.Size(1004, 416);
            this.InformData.TabIndex = 2;
            this.InformData.CurrentCellDirtyStateChanged += new System.EventHandler(this.DataGridView_CurrentCellDirtyStateChanged);
            this.InformData.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.DataGridView_DefaultValuesNeeded);
            // 
            // NO
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.NO.DefaultCellStyle = dataGridViewCellStyle2;
            this.NO.HeaderText = "楼层号";
            this.NO.Name = "NO";
            this.NO.Width = 80;
            // 
            // FloorWidth
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.FloorWidth.DefaultCellStyle = dataGridViewCellStyle3;
            this.FloorWidth.HeaderText = "层宽";
            this.FloorWidth.Name = "FloorWidth";
            this.FloorWidth.Width = 80;
            // 
            // FloorHeight
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.FloorHeight.DefaultCellStyle = dataGridViewCellStyle4;
            this.FloorHeight.HeaderText = "层高";
            this.FloorHeight.Name = "FloorHeight";
            this.FloorHeight.Width = 80;
            // 
            // LTW
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.LTW.DefaultCellStyle = dataGridViewCellStyle5;
            this.LTW.HeaderText = "踏步宽";
            this.LTW.Name = "LTW";
            this.LTW.Width = 80;
            // 
            // LTH
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.LTH.DefaultCellStyle = dataGridViewCellStyle6;
            this.LTH.HeaderText = "踏步高";
            this.LTH.Name = "LTH";
            this.LTH.Width = 80;
            // 
            // LTN
            // 
            this.LTN.HeaderText = "踏步数";
            this.LTN.Name = "LTN";
            this.LTN.Width = 80;
            // 
            // EXTW
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.EXTW.DefaultCellStyle = dataGridViewCellStyle7;
            this.EXTW.HeaderText = "左平台";
            this.EXTW.Name = "EXTW";
            this.EXTW.Width = 80;
            // 
            // EXTW2
            // 
            this.EXTW2.FillWeight = 80F;
            this.EXTW2.HeaderText = "右平台";
            this.EXTW2.Name = "EXTW2";
            this.EXTW2.Width = 80;
            // 
            // EXTH
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.EXTH.DefaultCellStyle = dataGridViewCellStyle8;
            this.EXTH.HeaderText = "板厚";
            this.EXTH.Name = "EXTH";
            this.EXTH.Width = 80;
            // 
            // COVER
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.COVER.DefaultCellStyle = dataGridViewCellStyle9;
            this.COVER.HeaderText = "踏步面层";
            this.COVER.Name = "COVER";
            this.COVER.Width = 80;
            // 
            // SH
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.SH.DefaultCellStyle = dataGridViewCellStyle10;
            this.SH.HeaderText = "梯梁高";
            this.SH.Name = "SH";
            this.SH.Width = 80;
            // 
            // SW
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.SW.DefaultCellStyle = dataGridViewCellStyle11;
            this.SW.HeaderText = "梯梁宽";
            this.SW.Name = "SW";
            this.SW.Width = 80;
            // 
            // StairForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 450);
            this.Controls.Add(this.InformData);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.SubmitButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(1020, 488);
            this.MinimumSize = new System.Drawing.Size(1020, 488);
            this.Name = "StairForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "楼梯参数输入";
            ((System.ComponentModel.ISupportInitialize)(this.InformData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button SubmitButton;
        private System.Windows.Forms.Button ClearButton;
        private System.Windows.Forms.DataGridView InformData;
        private System.Windows.Forms.DataGridViewTextBoxColumn NO;
        private System.Windows.Forms.DataGridViewTextBoxColumn FloorWidth;
        private System.Windows.Forms.DataGridViewTextBoxColumn FloorHeight;
        private System.Windows.Forms.DataGridViewTextBoxColumn LTW;
        private System.Windows.Forms.DataGridViewTextBoxColumn LTH;
        private System.Windows.Forms.DataGridViewTextBoxColumn LTN;
        private System.Windows.Forms.DataGridViewTextBoxColumn EXTW;
        private System.Windows.Forms.DataGridViewTextBoxColumn EXTW2;
        private System.Windows.Forms.DataGridViewTextBoxColumn EXTH;
        private System.Windows.Forms.DataGridViewTextBoxColumn COVER;
        private System.Windows.Forms.DataGridViewTextBoxColumn SH;
        private System.Windows.Forms.DataGridViewTextBoxColumn SW;
    }
}
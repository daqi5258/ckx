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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.COVER = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EXTH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EXTW = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EXTW2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EXTH2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.InformData.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.InformData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.InformData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NO,
            this.FloorWidth,
            this.FloorHeight,
            this.LTW,
            this.LTH,
            this.LTN,
            this.COVER,
            this.EXTH,
            this.EXTW,
            this.EXTW2,
            this.EXTH2,
            this.SH,
            this.SW});
            this.InformData.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.InformData.Location = new System.Drawing.Point(0, 34);
            this.InformData.Name = "InformData";
            this.InformData.RowTemplate.Height = 23;
            this.InformData.Size = new System.Drawing.Size(1045, 416);
            this.InformData.TabIndex = 2;
            this.InformData.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_CellValueChanged);
            this.InformData.CurrentCellDirtyStateChanged += new System.EventHandler(this.DataGridView_CurrentCellDirtyStateChanged);
            this.InformData.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.DataGridView_DefaultValuesNeeded);
            // 
            // NO
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.NO.DefaultCellStyle = dataGridViewCellStyle3;
            this.NO.FillWeight = 60F;
            this.NO.HeaderText = "楼层号";
            this.NO.Name = "NO";
            this.NO.Width = 80;
            // 
            // FloorWidth
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.FloorWidth.DefaultCellStyle = dataGridViewCellStyle4;
            this.FloorWidth.FillWeight = 60F;
            this.FloorWidth.HeaderText = "层宽";
            this.FloorWidth.Name = "FloorWidth";
            this.FloorWidth.Width = 60;
            // 
            // FloorHeight
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.FloorHeight.DefaultCellStyle = dataGridViewCellStyle5;
            this.FloorHeight.FillWeight = 60F;
            this.FloorHeight.HeaderText = "层高";
            this.FloorHeight.Name = "FloorHeight";
            this.FloorHeight.Width = 60;
            // 
            // LTW
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.LTW.DefaultCellStyle = dataGridViewCellStyle6;
            this.LTW.FillWeight = 60F;
            this.LTW.HeaderText = "踏步宽";
            this.LTW.Name = "LTW";
            this.LTW.Width = 80;
            // 
            // LTH
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.LTH.DefaultCellStyle = dataGridViewCellStyle7;
            this.LTH.FillWeight = 60F;
            this.LTH.HeaderText = "踏步高";
            this.LTH.Name = "LTH";
            this.LTH.Width = 80;
            // 
            // LTN
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.LTN.DefaultCellStyle = dataGridViewCellStyle8;
            this.LTN.FillWeight = 60F;
            this.LTN.HeaderText = "踏步数";
            this.LTN.Name = "LTN";
            this.LTN.Width = 80;
            // 
            // COVER
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.COVER.DefaultCellStyle = dataGridViewCellStyle9;
            this.COVER.FillWeight = 60F;
            this.COVER.HeaderText = "踏步面层";
            this.COVER.Name = "COVER";
            this.COVER.Width = 80;
            // 
            // EXTH
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.EXTH.DefaultCellStyle = dataGridViewCellStyle10;
            this.EXTH.FillWeight = 60F;
            this.EXTH.HeaderText = "踏步厚";
            this.EXTH.Name = "EXTH";
            this.EXTH.Width = 80;
            // 
            // EXTW
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.EXTW.DefaultCellStyle = dataGridViewCellStyle11;
            this.EXTW.FillWeight = 60F;
            this.EXTW.HeaderText = "左平台";
            this.EXTW.Name = "EXTW";
            this.EXTW.Width = 80;
            // 
            // EXTW2
            // 
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.EXTW2.DefaultCellStyle = dataGridViewCellStyle12;
            this.EXTW2.FillWeight = 60F;
            this.EXTW2.HeaderText = "右平台";
            this.EXTW2.Name = "EXTW2";
            this.EXTW2.Width = 80;
            // 
            // EXTH2
            // 
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.EXTH2.DefaultCellStyle = dataGridViewCellStyle13;
            this.EXTH2.FillWeight = 60F;
            this.EXTH2.HeaderText = "平台厚";
            this.EXTH2.Name = "EXTH2";
            this.EXTH2.Width = 80;
            // 
            // SH
            // 
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.SH.DefaultCellStyle = dataGridViewCellStyle14;
            this.SH.FillWeight = 60F;
            this.SH.HeaderText = "梯梁高";
            this.SH.Name = "SH";
            this.SH.Width = 80;
            // 
            // SW
            // 
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.SW.DefaultCellStyle = dataGridViewCellStyle15;
            this.SW.FillWeight = 60F;
            this.SW.HeaderText = "梯梁宽";
            this.SW.Name = "SW";
            this.SW.Width = 80;
            // 
            // StairForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1045, 450);
            this.Controls.Add(this.InformData);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.SubmitButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(1061, 488);
            this.MinimumSize = new System.Drawing.Size(1061, 488);
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
        private System.Windows.Forms.DataGridViewTextBoxColumn COVER;
        private System.Windows.Forms.DataGridViewTextBoxColumn EXTH;
        private System.Windows.Forms.DataGridViewTextBoxColumn EXTW;
        private System.Windows.Forms.DataGridViewTextBoxColumn EXTW2;
        private System.Windows.Forms.DataGridViewTextBoxColumn EXTH2;
        private System.Windows.Forms.DataGridViewTextBoxColumn SH;
        private System.Windows.Forms.DataGridViewTextBoxColumn SW;
    }
}
namespace ContractSaveToFtp
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.tb_SaveToH3DataField = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_ProcessNumber = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.btn_Process = new System.Windows.Forms.Button();
            this.tb_FilePath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_FilePath = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.tb_TemplateId = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tb_InstanceType = new System.Windows.Forms.TextBox();
            this.cmb_FileType = new System.Windows.Forms.ComboBox();
            this.btn_ProcessedPath = new System.Windows.Forms.Button();
            this.tb_ProcessedPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(44, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "处理的文件类型：";
            // 
            // tb_SaveToH3DataField
            // 
            this.tb_SaveToH3DataField.Enabled = false;
            this.tb_SaveToH3DataField.Location = new System.Drawing.Point(587, 133);
            this.tb_SaveToH3DataField.Name = "tb_SaveToH3DataField";
            this.tb_SaveToH3DataField.Size = new System.Drawing.Size(199, 21);
            this.tb_SaveToH3DataField.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(456, 136);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "保存到H3的字段编码：";
            // 
            // tb_ProcessNumber
            // 
            this.tb_ProcessNumber.Location = new System.Drawing.Point(151, 133);
            this.tb_ProcessNumber.Name = "tb_ProcessNumber";
            this.tb_ProcessNumber.Size = new System.Drawing.Size(89, 21);
            this.tb_ProcessNumber.TabIndex = 6;
            this.tb_ProcessNumber.Text = "5000";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(68, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "处理的数量：";
            // 
            // rtb_Log
            // 
            this.rtb_Log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtb_Log.Location = new System.Drawing.Point(3, 200);
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Size = new System.Drawing.Size(792, 303);
            this.rtb_Log.TabIndex = 7;
            this.rtb_Log.Text = "";
            // 
            // btn_Process
            // 
            this.btn_Process.Location = new System.Drawing.Point(539, 80);
            this.btn_Process.Name = "btn_Process";
            this.btn_Process.Size = new System.Drawing.Size(75, 29);
            this.btn_Process.TabIndex = 8;
            this.btn_Process.Text = "开始处理";
            this.btn_Process.UseVisualStyleBackColor = true;
            this.btn_Process.Click += new System.EventHandler(this.btn_Process_Click);
            // 
            // tb_FilePath
            // 
            this.tb_FilePath.Enabled = false;
            this.tb_FilePath.Location = new System.Drawing.Point(151, 11);
            this.tb_FilePath.Name = "tb_FilePath";
            this.tb_FilePath.Size = new System.Drawing.Size(533, 21);
            this.tb_FilePath.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(56, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "合同文件目录：";
            // 
            // btn_FilePath
            // 
            this.btn_FilePath.Location = new System.Drawing.Point(690, 9);
            this.btn_FilePath.Name = "btn_FilePath";
            this.btn_FilePath.Size = new System.Drawing.Size(34, 23);
            this.btn_FilePath.TabIndex = 11;
            this.btn_FilePath.Text = "...";
            this.btn_FilePath.UseVisualStyleBackColor = true;
            this.btn_FilePath.Click += new System.EventHandler(this.btn_FilePath_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(92, 173);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "模板ID：";
            // 
            // tb_TemplateId
            // 
            this.tb_TemplateId.Enabled = false;
            this.tb_TemplateId.Location = new System.Drawing.Point(151, 170);
            this.tb_TemplateId.Name = "tb_TemplateId";
            this.tb_TemplateId.Size = new System.Drawing.Size(283, 21);
            this.tb_TemplateId.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(246, 136);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 14;
            this.label7.Text = "流程类型：";
            // 
            // tb_InstanceType
            // 
            this.tb_InstanceType.Enabled = false;
            this.tb_InstanceType.Location = new System.Drawing.Point(317, 133);
            this.tb_InstanceType.Name = "tb_InstanceType";
            this.tb_InstanceType.Size = new System.Drawing.Size(133, 21);
            this.tb_InstanceType.TabIndex = 15;
            // 
            // cmb_FileType
            // 
            this.cmb_FileType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_FileType.FormattingEnabled = true;
            this.cmb_FileType.Location = new System.Drawing.Point(151, 85);
            this.cmb_FileType.Name = "cmb_FileType";
            this.cmb_FileType.Size = new System.Drawing.Size(334, 20);
            this.cmb_FileType.TabIndex = 16;
            this.cmb_FileType.SelectedIndexChanged += new System.EventHandler(this.cmb_FileType_SelectedIndexChanged);
            // 
            // btn_ProcessedPath
            // 
            this.btn_ProcessedPath.Location = new System.Drawing.Point(690, 45);
            this.btn_ProcessedPath.Name = "btn_ProcessedPath";
            this.btn_ProcessedPath.Size = new System.Drawing.Size(34, 23);
            this.btn_ProcessedPath.TabIndex = 19;
            this.btn_ProcessedPath.Text = "...";
            this.btn_ProcessedPath.UseVisualStyleBackColor = true;
            this.btn_ProcessedPath.Click += new System.EventHandler(this.btn_ProcessedPath_Click);
            // 
            // tb_ProcessedPath
            // 
            this.tb_ProcessedPath.Enabled = false;
            this.tb_ProcessedPath.Location = new System.Drawing.Point(151, 47);
            this.tb_ProcessedPath.Name = "tb_ProcessedPath";
            this.tb_ProcessedPath.Size = new System.Drawing.Size(533, 21);
            this.tb_ProcessedPath.TabIndex = 18;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 12);
            this.label3.TabIndex = 17;
            this.label3.Text = "已处理完成的目录：";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(798, 506);
            this.Controls.Add(this.btn_ProcessedPath);
            this.Controls.Add(this.tb_ProcessedPath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmb_FileType);
            this.Controls.Add(this.tb_InstanceType);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tb_TemplateId);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btn_FilePath);
            this.Controls.Add(this.tb_FilePath);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btn_Process);
            this.Controls.Add(this.rtb_Log);
            this.Controls.Add(this.tb_ProcessNumber);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tb_SaveToH3DataField);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "合同文件导入到H3 Ftp";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_SaveToH3DataField;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_ProcessNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.Button btn_Process;
        private System.Windows.Forms.TextBox tb_FilePath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btn_FilePath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tb_TemplateId;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tb_InstanceType;
        private System.Windows.Forms.ComboBox cmb_FileType;
        private System.Windows.Forms.Button btn_ProcessedPath;
        private System.Windows.Forms.TextBox tb_ProcessedPath;
        private System.Windows.Forms.Label label3;
    }
}


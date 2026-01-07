namespace ETF_Compare_2._0
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtToday = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCompare = new System.Windows.Forms.Button();
            this.btnBrowseYesterday = new System.Windows.Forms.Button();
            this.btnBrowseToday = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtYesterday = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(418, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "今日";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "程式路徑";
            // 
            // txtToday
            // 
            this.txtToday.Location = new System.Drawing.Point(443, 83);
            this.txtToday.Multiline = true;
            this.txtToday.Name = "txtToday";
            this.txtToday.Size = new System.Drawing.Size(255, 66);
            this.txtToday.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(384, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "程式路徑";
            // 
            // btnCompare
            // 
            this.btnCompare.Location = new System.Drawing.Point(580, 259);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(139, 54);
            this.btnCompare.TabIndex = 6;
            this.btnCompare.Text = "比對";
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click_1);
            // 
            // btnBrowseYesterday
            // 
            this.btnBrowseYesterday.Location = new System.Drawing.Point(303, 155);
            this.btnBrowseYesterday.Name = "btnBrowseYesterday";
            this.btnBrowseYesterday.Size = new System.Drawing.Size(108, 44);
            this.btnBrowseYesterday.TabIndex = 7;
            this.btnBrowseYesterday.Text = "瀏覽路徑位置";
            this.btnBrowseYesterday.UseVisualStyleBackColor = true;
            this.btnBrowseYesterday.Click += new System.EventHandler(this.btnBrowseYesterday_Click_1);
            // 
            // btnBrowseToday
            // 
            this.btnBrowseToday.Location = new System.Drawing.Point(649, 155);
            this.btnBrowseToday.Name = "btnBrowseToday";
            this.btnBrowseToday.Size = new System.Drawing.Size(113, 44);
            this.btnBrowseToday.TabIndex = 8;
            this.btnBrowseToday.Text = "瀏覽路徑位置";
            this.btnBrowseToday.UseVisualStyleBackColor = true;
            this.btnBrowseToday.Click += new System.EventHandler(this.btnBrowseToday_Click_1);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(71, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "昨日";
            // 
            // txtYesterday
            // 
            this.txtYesterday.Location = new System.Drawing.Point(95, 80);
            this.txtYesterday.Multiline = true;
            this.txtYesterday.Name = "txtYesterday";
            this.txtYesterday.Size = new System.Drawing.Size(249, 69);
            this.txtYesterday.TabIndex = 11;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(144, 161);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 33);
            this.button1.TabIndex = 12;
            this.button1.Text = "清除";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(513, 161);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 33);
            this.button2.TabIndex = 13;
            this.button2.Text = "清除";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(580, 334);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(139, 23);
            this.progressBar1.TabIndex = 14;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(524, 340);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(53, 12);
            this.lblStatus.TabIndex = 15;
            this.lblStatus.Text = "準備就緒";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtYesterday);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnBrowseToday);
            this.Controls.Add(this.btnBrowseYesterday);
            this.Controls.Add(this.btnCompare);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtToday);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtToday;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.Button btnBrowseYesterday;
        private System.Windows.Forms.Button btnBrowseToday;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtYesterday;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ProgressBar progressBar1;
        public System.Windows.Forms.Label lblStatus;
    }
}


namespace ZR.WinFormsApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            folderBrowserDialog1 = new FolderBrowserDialog();
            panel1 = new Panel();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            button2 = new Button();
            button3 = new Button();
            button7 = new Button();
            splitContainer1 = new SplitContainer();
            leftBox = new RichTextBox();
            rightBox = new RichTextBox();
            button8 = new Button();
            button9 = new Button();
            button10 = new Button();
            button11 = new Button();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(20, 110);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "选择目录";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(button1);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(117, 175);
            panel1.TabIndex = 3;
            // 
            // button4
            // 
            button4.Location = new Point(195, 33);
            button4.Name = "button4";
            button4.Size = new Size(114, 23);
            button4.TabIndex = 4;
            button4.Text = "总数核对（停止）";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.Location = new Point(192, 76);
            button5.Name = "button5";
            button5.Size = new Size(117, 23);
            button5.TabIndex = 5;
            button5.Text = "导入账单订单号数据";
            button5.TextAlign = ContentAlignment.MiddleLeft;
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button6
            // 
            button6.Location = new Point(195, 122);
            button6.Name = "button6";
            button6.Size = new Size(102, 23);
            button6.TabIndex = 6;
            button6.Text = "导入账单完整数据";
            button6.TextAlign = ContentAlignment.MiddleLeft;
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // button2
            // 
            button2.Location = new Point(192, 164);
            button2.Name = "button2";
            button2.Size = new Size(105, 23);
            button2.TabIndex = 7;
            button2.Text = "生成表Bill2结构";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click_1;
            // 
            // button3
            // 
            button3.Location = new Point(401, 33);
            button3.Name = "button3";
            button3.Size = new Size(104, 23);
            button3.TabIndex = 8;
            button3.Text = "对比总运单量";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click_1;
            // 
            // button7
            // 
            button7.Location = new Point(560, 33);
            button7.Name = "button7";
            button7.Size = new Size(155, 23);
            button7.TabIndex = 9;
            button7.Text = "查看缺失的网点公司信息";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Location = new Point(12, 202);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(leftBox);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(rightBox);
            splitContainer1.Size = new Size(1149, 476);
            splitContainer1.SplitterDistance = 597;
            splitContainer1.TabIndex = 10;
            // 
            // leftBox
            // 
            leftBox.Location = new Point(25, 21);
            leftBox.Name = "leftBox";
            leftBox.Size = new Size(544, 440);
            leftBox.TabIndex = 0;
            leftBox.Text = "";
            // 
            // rightBox
            // 
            rightBox.Location = new Point(2, 21);
            rightBox.Name = "rightBox";
            rightBox.Size = new Size(543, 452);
            rightBox.TabIndex = 0;
            rightBox.Text = "";
            // 
            // button8
            // 
            button8.Location = new Point(752, 33);
            button8.Name = "button8";
            button8.Size = new Size(199, 23);
            button8.TabIndex = 11;
            button8.Text = "给所有客户添加全部的共享店铺";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button9
            // 
            button9.Location = new Point(995, 12);
            button9.Name = "button9";
            button9.Size = new Size(55, 23);
            button9.TabIndex = 12;
            button9.Text = "没计算";
            button9.UseVisualStyleBackColor = true;
            button9.Click += button9_Click;
            // 
            // button10
            // 
            button10.Location = new Point(995, 41);
            button10.Name = "button10";
            button10.Size = new Size(101, 23);
            button10.TabIndex = 13;
            button10.Text = "没计算+没发运";
            button10.UseVisualStyleBackColor = true;
            button10.Click += button10_Click;
            // 
            // button11
            // 
            button11.Location = new Point(995, 70);
            button11.Name = "button11";
            button11.Size = new Size(146, 23);
            button11.TabIndex = 14;
            button11.Text = "没计算+没发运+没店铺";
            button11.UseVisualStyleBackColor = true;
            button11.Click += button11_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1173, 690);
            Controls.Add(button11);
            Controls.Add(button10);
            Controls.Add(button9);
            Controls.Add(button8);
            Controls.Add(button7);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(panel1);
            Controls.Add(splitContainer1);
            Name = "Form1";
            Text = "Form1";
            panel1.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private FolderBrowserDialog folderBrowserDialog1;
        private Panel panel1;
        private Button button4;
        private Button button5;
        private Button button6;
        private Button button2;
        private Button button3;
        private Button button7;
        private SplitContainer splitContainer1;
        private RichTextBox leftBox;
        private Button button8;
        private Button button9;
        private RichTextBox rightBox;
        private Button button10;
        private Button button11;
    }
}

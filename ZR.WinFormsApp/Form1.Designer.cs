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
            folderBrowserDialog1 = new FolderBrowserDialog();
            button7 = new Button();
            splitContainer1 = new SplitContainer();
            leftBox2 = new RichTextBox();
            leftBox = new RichTextBox();
            rightBox2 = new RichTextBox();
            rightBox = new RichTextBox();
            button10 = new Button();
            button11 = new Button();
            button12 = new Button();
            button18 = new Button();
            button1 = new Button();
            panel1 = new Panel();
            button3 = new Button();
            panel3 = new Panel();
            button4 = new Button();
            panel4 = new Panel();
            panel5 = new Panel();
            panel6 = new Panel();
            panel7 = new Panel();
            button6 = new Button();
            button2 = new Button();
            panel2 = new Panel();
            button5 = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            panel3.SuspendLayout();
            panel5.SuspendLayout();
            panel6.SuspendLayout();
            panel7.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // button7
            // 
            button7.Location = new Point(3, 12);
            button7.Name = "button7";
            button7.Size = new Size(155, 23);
            button7.TabIndex = 9;
            button7.Text = "查看缺失的网点公司信息";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Location = new Point(12, 377);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(leftBox2);
            splitContainer1.Panel1.Controls.Add(leftBox);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(rightBox2);
            splitContainer1.Panel2.Controls.Add(rightBox);
            splitContainer1.Size = new Size(1828, 427);
            splitContainer1.SplitterDistance = 916;
            splitContainer1.TabIndex = 10;
            // 
            // leftBox2
            // 
            leftBox2.Location = new Point(366, 21);
            leftBox2.Name = "leftBox2";
            leftBox2.Size = new Size(301, 559);
            leftBox2.TabIndex = 1;
            leftBox2.Text = "";
            // 
            // leftBox
            // 
            leftBox.Location = new Point(25, 21);
            leftBox.Name = "leftBox";
            leftBox.Size = new Size(301, 559);
            leftBox.TabIndex = 0;
            leftBox.Text = "";
            // 
            // rightBox2
            // 
            rightBox2.Location = new Point(375, 21);
            rightBox2.Name = "rightBox2";
            rightBox2.Size = new Size(319, 559);
            rightBox2.TabIndex = 1;
            rightBox2.Text = "";
            // 
            // rightBox
            // 
            rightBox.Location = new Point(24, 21);
            rightBox.Name = "rightBox";
            rightBox.Size = new Size(310, 559);
            rightBox.TabIndex = 0;
            rightBox.Text = "";
            // 
            // button10
            // 
            button10.Location = new Point(3, 41);
            button10.Name = "button10";
            button10.Size = new Size(75, 23);
            button10.TabIndex = 21;
            button10.Text = "差异报告";
            button10.UseVisualStyleBackColor = true;
            button10.Click += button10_Click;
            // 
            // button11
            // 
            button11.Location = new Point(3, 12);
            button11.Name = "button11";
            button11.Size = new Size(157, 23);
            button11.TabIndex = 22;
            button11.Text = "生成表差异报告表Bill10";
            button11.UseVisualStyleBackColor = true;
            button11.Click += button11_Click;
            // 
            // button12
            // 
            button12.Location = new Point(7, 42);
            button12.Name = "button12";
            button12.Size = new Size(130, 23);
            button12.TabIndex = 23;
            button12.Text = "导入2.0账单完整数据";
            button12.TextAlign = ContentAlignment.MiddleLeft;
            button12.UseVisualStyleBackColor = true;
            button12.Click += button12_Click;
            // 
            // button18
            // 
            button18.Location = new Point(7, 13);
            button18.Name = "button18";
            button18.Size = new Size(105, 23);
            button18.TabIndex = 24;
            button18.Text = "生成表Bill3结构";
            button18.UseVisualStyleBackColor = true;
            button18.Click += button18_Click;
            // 
            // button1
            // 
            button1.Location = new Point(0, 3);
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
            panel1.Location = new Point(1020, 303);
            panel1.Name = "panel1";
            panel1.Size = new Size(88, 68);
            panel1.TabIndex = 3;
            // 
            // button3
            // 
            button3.Location = new Point(3, 41);
            button3.Name = "button3";
            button3.Size = new Size(204, 23);
            button3.TabIndex = 25;
            button3.Text = "多个用户使用了同一个店铺账号 ";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // panel3
            // 
            panel3.Controls.Add(button4);
            panel3.Location = new Point(350, 11);
            panel3.Name = "panel3";
            panel3.Size = new Size(153, 50);
            panel3.TabIndex = 27;
            // 
            // button4
            // 
            button4.Location = new Point(6, 8);
            button4.Name = "button4";
            button4.Size = new Size(110, 23);
            button4.TabIndex = 0;
            button4.Text = "查询计算状态";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // panel4
            // 
            panel4.Location = new Point(956, 27);
            panel4.Name = "panel4";
            panel4.Size = new Size(133, 100);
            panel4.TabIndex = 27;
            // 
            // panel5
            // 
            panel5.Controls.Add(button11);
            panel5.Controls.Add(button10);
            panel5.Location = new Point(468, 225);
            panel5.Name = "panel5";
            panel5.Size = new Size(165, 83);
            panel5.TabIndex = 27;
            // 
            // panel6
            // 
            panel6.Controls.Add(button7);
            panel6.Controls.Add(button3);
            panel6.Location = new Point(198, 225);
            panel6.Name = "panel6";
            panel6.Size = new Size(238, 83);
            panel6.TabIndex = 27;
            // 
            // panel7
            // 
            panel7.Controls.Add(button18);
            panel7.Controls.Add(button12);
            panel7.Location = new Point(171, 11);
            panel7.Name = "panel7";
            panel7.Size = new Size(143, 80);
            panel7.TabIndex = 27;
            // 
            // button6
            // 
            button6.Location = new Point(6, 54);
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
            button2.Location = new Point(6, 15);
            button2.Name = "button2";
            button2.Size = new Size(105, 23);
            button2.TabIndex = 7;
            button2.Text = "生成表Bill2结构";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click_1;
            // 
            // panel2
            // 
            panel2.Controls.Add(button5);
            panel2.Controls.Add(button2);
            panel2.Controls.Add(button6);
            panel2.Location = new Point(3, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(133, 153);
            panel2.TabIndex = 26;
            // 
            // button5
            // 
            button5.Location = new Point(6, 101);
            button5.Name = "button5";
            button5.Size = new Size(102, 23);
            button5.TabIndex = 8;
            button5.Text = "bak改为xlsx";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1120, 816);
            Controls.Add(panel3);
            Controls.Add(panel4);
            Controls.Add(panel5);
            Controls.Add(panel6);
            Controls.Add(panel7);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(splitContainer1);
            Name = "Form1";
            Text = "Form1";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel6.ResumeLayout(false);
            panel7.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private FolderBrowserDialog folderBrowserDialog1;
        private Button button7;
        private SplitContainer splitContainer1;
        private RichTextBox leftBox;
        private RichTextBox rightBox;
        private RichTextBox rightBox2;
        private RichTextBox leftBox2;
        private Button button10;
        private Button button11;
        private Button button12;
        private Button button18;
        private Button button1;
        private Panel panel1;
        private Button button3;
        private Panel panel3;
        private Panel panel4;
        private Panel panel5;
        private Panel panel6;
        private Panel panel7;
        private Button button4;
        private Button button6;
        private Button button2;
        private Panel panel2;
        private Button button5;
    }
}

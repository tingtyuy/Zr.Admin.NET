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
            button6 = new Button();
            button2 = new Button();
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
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // button6
            // 
            button6.Location = new Point(451, 31);
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
            button2.Location = new Point(448, 2);
            button2.Name = "button2";
            button2.Size = new Size(105, 23);
            button2.TabIndex = 7;
            button2.Text = "生成表Bill2结构";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click_1;
            // 
            // button7
            // 
            button7.Location = new Point(602, 2);
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
            splitContainer1.Panel1.Controls.Add(leftBox2);
            splitContainer1.Panel1.Controls.Add(leftBox);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(rightBox2);
            splitContainer1.Panel2.Controls.Add(rightBox);
            splitContainer1.Size = new Size(1828, 602);
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
            button10.Location = new Point(812, 31);
            button10.Name = "button10";
            button10.Size = new Size(75, 23);
            button10.TabIndex = 21;
            button10.Text = "差异报告";
            button10.UseVisualStyleBackColor = true;
            button10.Click += button10_Click;
            // 
            // button11
            // 
            button11.Location = new Point(812, 2);
            button11.Name = "button11";
            button11.Size = new Size(157, 23);
            button11.TabIndex = 22;
            button11.Text = "生成表差异报告表Bill10";
            button11.UseVisualStyleBackColor = true;
            button11.Click += button11_Click;
            // 
            // button12
            // 
            button12.Location = new Point(1019, 31);
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
            button18.Location = new Point(1044, 2);
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
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(88, 175);
            panel1.TabIndex = 3;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1852, 816);
            Controls.Add(button18);
            Controls.Add(button12);
            Controls.Add(button11);
            Controls.Add(button10);
            Controls.Add(button7);
            Controls.Add(button2);
            Controls.Add(button6);
            Controls.Add(panel1);
            Controls.Add(splitContainer1);
            Name = "Form1";
            Text = "Form1";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private FolderBrowserDialog folderBrowserDialog1;
        private Button button6;
        private Button button2;
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
    }
}

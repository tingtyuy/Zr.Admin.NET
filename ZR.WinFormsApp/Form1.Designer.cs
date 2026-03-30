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
            splitContainer1 = new SplitContainer();
            leftBox2 = new RichTextBox();
            leftBox = new RichTextBox();
            rightBox2 = new RichTextBox();
            rightBox = new RichTextBox();
            button12 = new Button();
            button18 = new Button();
            panel7 = new Panel();
            button6 = new Button();
            button2 = new Button();
            panel2 = new Panel();
            button5 = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel7.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Location = new Point(15, 444);
            splitContainer1.Margin = new Padding(4);
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
            splitContainer1.Size = new Size(2350, 502);
            splitContainer1.SplitterDistance = 1177;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 10;
            // 
            // leftBox2
            // 
            leftBox2.Location = new Point(471, 25);
            leftBox2.Margin = new Padding(4);
            leftBox2.Name = "leftBox2";
            leftBox2.Size = new Size(386, 657);
            leftBox2.TabIndex = 1;
            leftBox2.Text = "";
            // 
            // leftBox
            // 
            leftBox.Location = new Point(32, 25);
            leftBox.Margin = new Padding(4);
            leftBox.Name = "leftBox";
            leftBox.Size = new Size(386, 657);
            leftBox.TabIndex = 0;
            leftBox.Text = "";
            // 
            // rightBox2
            // 
            rightBox2.Location = new Point(482, 25);
            rightBox2.Margin = new Padding(4);
            rightBox2.Name = "rightBox2";
            rightBox2.Size = new Size(409, 657);
            rightBox2.TabIndex = 1;
            rightBox2.Text = "";
            // 
            // rightBox
            // 
            rightBox.Location = new Point(31, 25);
            rightBox.Margin = new Padding(4);
            rightBox.Name = "rightBox";
            rightBox.Size = new Size(397, 657);
            rightBox.TabIndex = 0;
            rightBox.Text = "";
            // 
            // button12
            // 
            button12.Location = new Point(9, 49);
            button12.Margin = new Padding(4);
            button12.Name = "button12";
            button12.Size = new Size(167, 27);
            button12.TabIndex = 23;
            button12.Text = "导入2.0账单完整数据";
            button12.TextAlign = ContentAlignment.MiddleLeft;
            button12.UseVisualStyleBackColor = true;
            button12.Click += button12_Click;
            // 
            // button18
            // 
            button18.Location = new Point(9, 15);
            button18.Margin = new Padding(4);
            button18.Name = "button18";
            button18.Size = new Size(135, 27);
            button18.TabIndex = 24;
            button18.Text = "生成表Bill3结构";
            button18.UseVisualStyleBackColor = true;
            button18.Click += button18_Click;
            // 
            // panel7
            // 
            panel7.Controls.Add(button18);
            panel7.Controls.Add(button12);
            panel7.Location = new Point(220, 13);
            panel7.Margin = new Padding(4);
            panel7.Name = "panel7";
            panel7.Size = new Size(184, 94);
            panel7.TabIndex = 27;
            // 
            // button6
            // 
            button6.Location = new Point(8, 64);
            button6.Margin = new Padding(4);
            button6.Name = "button6";
            button6.Size = new Size(131, 27);
            button6.TabIndex = 6;
            button6.Text = "导入账单完整数据";
            button6.TextAlign = ContentAlignment.MiddleLeft;
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // button2
            // 
            button2.Location = new Point(8, 18);
            button2.Margin = new Padding(4);
            button2.Name = "button2";
            button2.Size = new Size(135, 27);
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
            panel2.Location = new Point(4, 4);
            panel2.Margin = new Padding(4);
            panel2.Name = "panel2";
            panel2.Size = new Size(171, 180);
            panel2.TabIndex = 26;
            // 
            // button5
            // 
            button5.Location = new Point(8, 119);
            button5.Margin = new Padding(4);
            button5.Name = "button5";
            button5.Size = new Size(131, 27);
            button5.TabIndex = 8;
            button5.Text = "bak改为xlsx";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1440, 960);
            Controls.Add(panel7);
            Controls.Add(panel2);
            Controls.Add(splitContainer1);
            Margin = new Padding(4);
            Name = "Form1";
            Text = "Form1";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel7.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private FolderBrowserDialog folderBrowserDialog1;
        private SplitContainer splitContainer1;
        private RichTextBox leftBox;
        private RichTextBox rightBox;
        private RichTextBox rightBox2;
        private RichTextBox leftBox2;
        private Button button12;
        private Button button18;
        private Panel panel7;
        private Button button6;
        private Button button2;
        private Panel panel2;
        private Button button5;
    }
}

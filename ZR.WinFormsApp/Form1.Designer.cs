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
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            groupBox1 = new GroupBox();
            lbWordInputPath = new Label();
            groupBox2 = new GroupBox();
            button4 = new Button();
            button5 = new Button();
            openFileDialog1 = new OpenFileDialog();
            menuStrip1 = new MenuStrip();
            excelToolStripMenuItem = new ToolStripMenuItem();
            wordToolStripMenuItem = new ToolStripMenuItem();
            imageToolStripMenuItem = new ToolStripMenuItem();
            tempToolStripMenuItem = new ToolStripMenuItem();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(40, 70);
            button1.Margin = new Padding(4);
            button1.Name = "button1";
            button1.Size = new Size(96, 27);
            button1.TabIndex = 0;
            button1.Text = "选择目录";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(16, 101);
            button2.Name = "button2";
            button2.Size = new Size(165, 29);
            button2.TabIndex = 1;
            button2.Text = "Word To Image";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(179, 35);
            button3.Margin = new Padding(4);
            button3.Name = "button3";
            button3.Size = new Size(153, 27);
            button3.TabIndex = 2;
            button3.Text = "Select Word";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lbWordInputPath);
            groupBox1.Controls.Add(button3);
            groupBox1.Controls.Add(button2);
            groupBox1.Location = new Point(715, 160);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(596, 420);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Word";
            // 
            // lbWordInputPath
            // 
            lbWordInputPath.AutoSize = true;
            lbWordInputPath.Location = new Point(28, 42);
            lbWordInputPath.Name = "lbWordInputPath";
            lbWordInputPath.Size = new Size(144, 20);
            lbWordInputPath.TabIndex = 3;
            lbWordInputPath.Text = "请选择WORD路径...";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(button4);
            groupBox2.Location = new Point(715, 652);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(621, 408);
            groupBox2.TabIndex = 4;
            groupBox2.TabStop = false;
            groupBox2.Text = "Image";
            // 
            // button4
            // 
            button4.Location = new Point(29, 39);
            button4.Name = "button4";
            button4.Size = new Size(187, 29);
            button4.TabIndex = 0;
            button4.Text = "Merge Image To 1";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_ClickAsync;
            // 
            // button5
            // 
            button5.Location = new Point(191, 68);
            button5.Name = "button5";
            button5.Size = new Size(94, 29);
            button5.TabIndex = 5;
            button5.Text = "Init";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { excelToolStripMenuItem, wordToolStripMenuItem, imageToolStripMenuItem, tempToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1440, 28);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
            // 
            // excelToolStripMenuItem
            // 
            excelToolStripMenuItem.Name = "excelToolStripMenuItem";
            excelToolStripMenuItem.Size = new Size(60, 24);
            excelToolStripMenuItem.Text = "Excel";
            excelToolStripMenuItem.Click += excelToolStripMenuItem_Click;
            // 
            // wordToolStripMenuItem
            // 
            wordToolStripMenuItem.Name = "wordToolStripMenuItem";
            wordToolStripMenuItem.Size = new Size(64, 24);
            wordToolStripMenuItem.Text = "Word";
            // 
            // imageToolStripMenuItem
            // 
            imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            imageToolStripMenuItem.Size = new Size(68, 24);
            imageToolStripMenuItem.Text = "Image";
            // 
            // tempToolStripMenuItem
            // 
            tempToolStripMenuItem.Name = "tempToolStripMenuItem";
            tempToolStripMenuItem.Size = new Size(65, 24);
            tempToolStripMenuItem.Text = "Temp";
            tempToolStripMenuItem.Click += tempToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1440, 960);
            Controls.Add(button5);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(button1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private FolderBrowserDialog folderBrowserDialog1;
        private Button button1;
        private Button button2;
        private Button button3;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button button4;
        private Label lbWordInputPath;
        private Button button5;
        private OpenFileDialog openFileDialog1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem excelToolStripMenuItem;
        private ToolStripMenuItem wordToolStripMenuItem;
        private ToolStripMenuItem imageToolStripMenuItem;
        private ToolStripMenuItem tempToolStripMenuItem;
    }
}

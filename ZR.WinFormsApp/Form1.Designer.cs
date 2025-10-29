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
            rightBox2 = new RichTextBox();
            rightBox = new RichTextBox();
            button8 = new Button();
            button9 = new Button();
            button13 = new Button();
            button14 = new Button();
            button15 = new Button();
            button16 = new Button();
            button17 = new Button();
            leftBox2 = new RichTextBox();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
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
            // button4
            // 
            button4.Location = new Point(118, 12);
            button4.Name = "button4";
            button4.Size = new Size(114, 23);
            button4.TabIndex = 4;
            button4.Text = "总数核对（停止）";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.Location = new Point(115, 55);
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
            button6.Location = new Point(118, 101);
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
            button2.Location = new Point(115, 143);
            button2.Name = "button2";
            button2.Size = new Size(105, 23);
            button2.TabIndex = 7;
            button2.Text = "生成表Bill2结构";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click_1;
            // 
            // button3
            // 
            button3.Location = new Point(253, 12);
            button3.Name = "button3";
            button3.Size = new Size(104, 23);
            button3.TabIndex = 8;
            button3.Text = "对比总运单量";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click_1;
            // 
            // button7
            // 
            button7.Location = new Point(253, 101);
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
            // button8
            // 
            button8.Location = new Point(253, 55);
            button8.Name = "button8";
            button8.Size = new Size(199, 23);
            button8.TabIndex = 11;
            button8.Text = "给所有客户添加全部的共享店铺";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button9
            // 
            button9.Location = new Point(500, 3);
            button9.Name = "button9";
            button9.Size = new Size(55, 23);
            button9.TabIndex = 12;
            button9.Text = "没计算";
            button9.UseVisualStyleBackColor = true;
            button9.Click += button9_Click;
            // 
            // button13
            // 
            button13.Location = new Point(587, 3);
            button13.Name = "button13";
            button13.Size = new Size(168, 23);
            button13.TabIndex = 16;
            button13.Text = "有真实店铺 ，没报价关系";
            button13.UseVisualStyleBackColor = true;
            button13.Click += button13_Click;
            // 
            // button14
            // 
            button14.Location = new Point(587, 46);
            button14.Name = "button14";
            button14.Size = new Size(320, 23);
            button14.TabIndex = 17;
            button14.Text = "没有店铺或者是共享店铺 ，没有发运表  ，没报价关系";
            button14.UseVisualStyleBackColor = true;
            button14.Click += button14_Click;
            // 
            // button15
            // 
            button15.Location = new Point(587, 75);
            button15.Name = "button15";
            button15.Size = new Size(480, 23);
            button15.TabIndex = 18;
            button15.Text = "没有店铺或者是共享店铺 ，有发运表  ，没报价关系(没发运表客户 和 没发运表店铺)";
            button15.UseVisualStyleBackColor = true;
            button15.Click += button15_Click;
            // 
            // button16
            // 
            button16.Location = new Point(587, 122);
            button16.Name = "button16";
            button16.Size = new Size(410, 23);
            button16.TabIndex = 19;
            button16.Text = "有共享店铺，有发运表 ， 没报价关系(没发运表客户 和 没计算表店铺)";
            button16.UseVisualStyleBackColor = true;
            button16.Click += button16_Click;
            // 
            // button17
            // 
            button17.Location = new Point(587, 164);
            button17.Name = "button17";
            button17.Size = new Size(393, 23);
            button17.TabIndex = 20;
            button17.Text = "没有店铺或者是共享店铺 ， 有发运表，没报价关系(没发运表店铺)";
            button17.UseVisualStyleBackColor = true;
            button17.Click += button17_Click;
            // 
            // leftBox2
            // 
            leftBox2.Location = new Point(366, 21);
            leftBox2.Name = "leftBox2";
            leftBox2.Size = new Size(301, 559);
            leftBox2.TabIndex = 1;
            leftBox2.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1852, 816);
            Controls.Add(button17);
            Controls.Add(button16);
            Controls.Add(button15);
            Controls.Add(button14);
            Controls.Add(button13);
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
        private Button button13;
        private Button button14;
        private Button button15;
        private Button button16;
        private Button button17;
        private RichTextBox rightBox2;
        private RichTextBox leftBox2;
    }
}

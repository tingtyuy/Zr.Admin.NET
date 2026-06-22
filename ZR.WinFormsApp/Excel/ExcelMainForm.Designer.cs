namespace ZR.WinFormsApp.Excel;

partial class ExcelMainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        label1 = new Label();
        btnOpenExcel = new Button();
        SuspendLayout();

        // label1
        label1.AutoSize = true;
        label1.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        label1.Location = new Point(50, 40);
        label1.Name = "label1";
        label1.Size = new Size(185, 27);
        label1.TabIndex = 0;
        label1.Text = "Excel 工具 (开发中)";

        // btnOpenExcel
        btnOpenExcel.Location = new Point(50, 90);
        btnOpenExcel.Name = "btnOpenExcel";
        btnOpenExcel.Size = new Size(200, 35);
        btnOpenExcel.TabIndex = 1;
        btnOpenExcel.Text = "选择 Excel 文件";
        btnOpenExcel.UseVisualStyleBackColor = true;
        btnOpenExcel.Click += btnOpenExcel_Click;

        // ExcelMainForm
        AutoScaleDimensions = new SizeF(9F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(600, 400);
        Controls.Add(btnOpenExcel);
        Controls.Add(label1);
        Name = "ExcelMainForm";
        Text = "Excel 工具";
        ResumeLayout(false);
        PerformLayout();
    }

    private Label label1;
    private Button btnOpenExcel;
}
using MiniExcelLibs;

namespace ZR.WinFormsApp.Excel;

public partial class ExcelMainForm : Form
{
    public ExcelMainForm()
    {
        InitializeComponent();
    }

    private void btnOpenExcel_Click(object sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog();
        dialog.Filter = "Excel 文件|*.xlsx;*.xls|所有文件|*.*";
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var rows = dialog.OpenFile().Query();
                var count = 0;
                foreach (var _ in rows) count++;
                MessageBox.Show($"文件读取成功！共 {count} 行数据", "完成",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"读取失败：{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

using RasterEdge.Imaging.Basic;
using RasterEdge.XDoc.Word;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using ZR.Infrastructure.Images;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace ZR.WinFormsApp;

public partial class Form1 : Form
{
    private string? _selectedFolder;
    private string? _wordFilePath;

    public Form1()
    {
        InitializeComponent();
    }

    private void btnSelectFolder_Click(object sender, EventArgs e)
    {
        folderBrowserDialog1.Description = "请选择工作目录";
        if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
        {
            _selectedFolder = folderBrowserDialog1.SelectedPath;
            lbSelectedFolder.Text = _selectedFolder;
        }
    }

    private void btnInit_Click(object sender, EventArgs e)
    {
        if (_selectedFolder != null && Directory.Exists(_selectedFolder))
        {
            System.Diagnostics.Process.Start("explorer.exe", _selectedFolder);
        }
        else
        {
            MessageBox.Show("请先选择一个有效目录", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void btnSelectWord_Click(object sender, EventArgs e)
    {
        openFileDialog1.Filter = "Word 文档|*.docx;*.doc|所有文件|*.*";
        if (openFileDialog1.ShowDialog() == DialogResult.OK)
        {
            _wordFilePath = openFileDialog1.FileName;
            lbWordPath.Text = _wordFilePath;
        }
    }

    private void btnWordToImage_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_wordFilePath) || !File.Exists(_wordFilePath))
        {
            MessageBox.Show("请先选择一个有效的 Word 文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        try
        {
            DOCXDocument doc = new DOCXDocument(_wordFilePath);
            var outputDir = Path.Combine(Path.GetDirectoryName(_wordFilePath)!, "output");
            doc.ConvertToImages(ImageType.PNG, outputDir, "page");
            MessageBox.Show($"转换完成！图片保存在：{outputDir}", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"转换失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnMergeImages_ClickAsync(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_selectedFolder) || !Directory.Exists(_selectedFolder))
        {
            MessageBox.Show("请先选择一个有效目录", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var btn = (Button)sender;
        btn.Enabled = false;
        btn.Text = "合并中...";

        try
        {
            string outputPath = Path.Combine(_selectedFolder, "merged.png");
            await ImageMerger.MergeImagesVerticallyAsync(_selectedFolder, outputPath);
            MessageBox.Show($"合并完成！文件保存在：{outputPath}", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"合并失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btn.Enabled = true;
            btn.Text = "纵向合并图片";
        }
    }

    private async void btnImagesToPdf_Click(object sender, EventArgs e)
    {
        if (openFileDialogImages.ShowDialog() != DialogResult.OK)
            return;

        if (openFileDialogImages.FileNames.Length == 0)
            return;

        saveFileDialog1.FileName = "output.pdf";
        if (saveFileDialog1.ShowDialog() != DialogResult.OK)
            return;

        var btn = (Button)sender;
        btn.Enabled = false;
        btn.Text = "转换中...";

        try
        {
            var imageFiles = openFileDialogImages.FileNames
                .Where(f => IsImageFile(f))
                .OrderBy(f => f)
                .ToList();

            if (imageFiles.Count == 0)
            {
                MessageBox.Show("未选择有效的图片文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await Task.Run(() =>
            {
                using var document = new PdfDocument();
                foreach (var imageFile in imageFiles)
                {
                    using var img = XImage.FromFile(imageFile);
                    var page = document.AddPage();
                    page.Width = XUnit.FromPoint(img.PointWidth);
                    page.Height = XUnit.FromPoint(img.PointHeight);
                    using var gfx = XGraphics.FromPdfPage(page);
                    gfx.DrawImage(img, 0, 0);
                }
                document.Save(saveFileDialog1.FileName);
            });

            MessageBox.Show($"PDF 生成完成！共 {imageFiles.Count} 页\n文件保存在：{saveFileDialog1.FileName}",
                "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"转换失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btn.Enabled = true;
            btn.Text = "多图片转 PDF";
        }
    }

    private void excelMenuItem_Click(object sender, EventArgs e)
    {
        var excelForm = new Excel.ExcelMainForm();
        excelForm.ShowDialog();
    }

    private void tempMenuItem_Click(object sender, EventArgs e)
    {
        MessageBox.Show("此功能开发中...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private static bool IsImageFile(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext is ".jpg" or ".jpeg" or ".png" or ".bmp" or ".gif" or ".tiff" or ".webp";
    }
}

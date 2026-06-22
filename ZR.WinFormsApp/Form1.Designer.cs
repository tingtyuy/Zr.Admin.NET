namespace ZR.WinFormsApp;

partial class Form1
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
        folderBrowserDialog1 = new FolderBrowserDialog();
        btnSelectFolder = new Button();
        btnWordToImage = new Button();
        btnSelectWord = new Button();
        gbWord = new GroupBox();
        lbWordPath = new Label();
        gbImage = new GroupBox();
        btnMergeImages = new Button();
        btnImagesToPdf = new Button();
        btnInit = new Button();
        openFileDialog1 = new OpenFileDialog();
        menuStrip1 = new MenuStrip();
        excelMenuItem = new ToolStripMenuItem();
        wordMenuItem = new ToolStripMenuItem();
        imageMenuItem = new ToolStripMenuItem();
        tempMenuItem = new ToolStripMenuItem();
        lbSelectedFolder = new Label();
        openFileDialogImages = new OpenFileDialog();
        saveFileDialog1 = new SaveFileDialog();
        gbWord.SuspendLayout();
        gbImage.SuspendLayout();
        menuStrip1.SuspendLayout();
        SuspendLayout();

        // btnSelectFolder
        btnSelectFolder.Location = new Point(40, 70);
        btnSelectFolder.Margin = new Padding(4);
        btnSelectFolder.Name = "btnSelectFolder";
        btnSelectFolder.Size = new Size(96, 27);
        btnSelectFolder.TabIndex = 0;
        btnSelectFolder.Text = "选择目录";
        btnSelectFolder.UseVisualStyleBackColor = true;
        btnSelectFolder.Click += btnSelectFolder_Click;

        // lbSelectedFolder
        lbSelectedFolder.AutoSize = true;
        lbSelectedFolder.Location = new Point(145, 75);
        lbSelectedFolder.Name = "lbSelectedFolder";
        lbSelectedFolder.Size = new Size(80, 20);
        lbSelectedFolder.TabIndex = 7;
        lbSelectedFolder.Text = "未选择目录";

        // btnInit
        btnInit.Location = new Point(330, 68);
        btnInit.Name = "btnInit";
        btnInit.Size = new Size(94, 29);
        btnInit.TabIndex = 5;
        btnInit.Text = "打开目录";
        btnInit.UseVisualStyleBackColor = true;
        btnInit.Click += btnInit_Click;

        // gbWord
        gbWord.Controls.Add(lbWordPath);
        gbWord.Controls.Add(btnSelectWord);
        gbWord.Controls.Add(btnWordToImage);
        gbWord.Location = new Point(40, 130);
        gbWord.Name = "gbWord";
        gbWord.Size = new Size(580, 200);
        gbWord.TabIndex = 3;
        gbWord.TabStop = false;
        gbWord.Text = "Word 工具";

        // lbWordPath
        lbWordPath.AutoSize = true;
        lbWordPath.Location = new Point(20, 42);
        lbWordPath.Name = "lbWordPath";
        lbWordPath.Size = new Size(128, 20);
        lbWordPath.TabIndex = 3;
        lbWordPath.Text = "请选择 Word 文件...";

        // btnSelectWord
        btnSelectWord.Location = new Point(20, 70);
        btnSelectWord.Margin = new Padding(4);
        btnSelectWord.Name = "btnSelectWord";
        btnSelectWord.Size = new Size(153, 27);
        btnSelectWord.TabIndex = 2;
        btnSelectWord.Text = "选择 Word 文件";
        btnSelectWord.UseVisualStyleBackColor = true;
        btnSelectWord.Click += btnSelectWord_Click;

        // btnWordToImage
        btnWordToImage.Location = new Point(20, 110);
        btnWordToImage.Name = "btnWordToImage";
        btnWordToImage.Size = new Size(165, 29);
        btnWordToImage.TabIndex = 1;
        btnWordToImage.Text = "Word 转图片";
        btnWordToImage.UseVisualStyleBackColor = true;
        btnWordToImage.Click += btnWordToImage_Click;

        // gbImage
        gbImage.Controls.Add(btnMergeImages);
        gbImage.Controls.Add(btnImagesToPdf);
        gbImage.Location = new Point(40, 350);
        gbImage.Name = "gbImage";
        gbImage.Size = new Size(580, 200);
        gbImage.TabIndex = 4;
        gbImage.TabStop = false;
        gbImage.Text = "图片工具";

        // btnMergeImages
        btnMergeImages.Location = new Point(20, 40);
        btnMergeImages.Name = "btnMergeImages";
        btnMergeImages.Size = new Size(200, 29);
        btnMergeImages.TabIndex = 0;
        btnMergeImages.Text = "纵向合并图片";
        btnMergeImages.UseVisualStyleBackColor = true;
        btnMergeImages.Click += btnMergeImages_ClickAsync;

        // btnImagesToPdf
        btnImagesToPdf.Location = new Point(20, 80);
        btnImagesToPdf.Name = "btnImagesToPdf";
        btnImagesToPdf.Size = new Size(200, 29);
        btnImagesToPdf.TabIndex = 1;
        btnImagesToPdf.Text = "多图片转 PDF";
        btnImagesToPdf.UseVisualStyleBackColor = true;
        btnImagesToPdf.Click += btnImagesToPdf_Click;

        // openFileDialog1
        openFileDialog1.FileName = "";

        // openFileDialogImages
        openFileDialogImages.FileName = "";
        openFileDialogImages.Multiselect = true;
        openFileDialogImages.Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif|所有文件|*.*";

        // saveFileDialog1
        saveFileDialog1.Filter = "PDF 文件|*.pdf";

        // menuStrip1
        menuStrip1.ImageScalingSize = new Size(20, 20);
        menuStrip1.Items.AddRange(new ToolStripItem[] { excelMenuItem, wordMenuItem, imageMenuItem, tempMenuItem });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(700, 28);
        menuStrip1.TabIndex = 6;
        menuStrip1.Text = "menuStrip1";

        // excelMenuItem
        excelMenuItem.Name = "excelMenuItem";
        excelMenuItem.Size = new Size(60, 24);
        excelMenuItem.Text = "Excel";
        excelMenuItem.Click += excelMenuItem_Click;

        // wordMenuItem
        wordMenuItem.Name = "wordMenuItem";
        wordMenuItem.Size = new Size(64, 24);
        wordMenuItem.Text = "Word";

        // imageMenuItem
        imageMenuItem.Name = "imageMenuItem";
        imageMenuItem.Size = new Size(68, 24);
        imageMenuItem.Text = "Image";

        // tempMenuItem
        tempMenuItem.Name = "tempMenuItem";
        tempMenuItem.Size = new Size(65, 24);
        tempMenuItem.Text = "Temp";
        tempMenuItem.Click += tempMenuItem_Click;

        // Form1
        AutoScaleDimensions = new SizeF(9F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(700, 600);
        Controls.Add(lbSelectedFolder);
        Controls.Add(btnInit);
        Controls.Add(gbImage);
        Controls.Add(gbWord);
        Controls.Add(btnSelectFolder);
        Controls.Add(menuStrip1);
        MainMenuStrip = menuStrip1;
        Margin = new Padding(4);
        Name = "Form1";
        Text = "文档处理工具箱";
        gbWord.ResumeLayout(false);
        gbWord.PerformLayout();
        gbImage.ResumeLayout(false);
        menuStrip1.ResumeLayout(false);
        menuStrip1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private FolderBrowserDialog folderBrowserDialog1;
    private Button btnSelectFolder;
    private Button btnWordToImage;
    private Button btnSelectWord;
    private GroupBox gbWord;
    private GroupBox gbImage;
    private Button btnMergeImages;
    private Button btnImagesToPdf;
    private Label lbWordPath;
    private Button btnInit;
    private OpenFileDialog openFileDialog1;
    private MenuStrip menuStrip1;
    private ToolStripMenuItem excelMenuItem;
    private ToolStripMenuItem wordMenuItem;
    private ToolStripMenuItem imageMenuItem;
    private ToolStripMenuItem tempMenuItem;
    private Label lbSelectedFolder;
    private OpenFileDialog openFileDialogImages;
    private SaveFileDialog saveFileDialog1;
}

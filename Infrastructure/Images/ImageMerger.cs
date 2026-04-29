using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ZR.Infrastructure.Images
{
    public static class ImageMerger
    {
        /// <summary>
        /// 将目录中的所有图片垂直拼接成一张长图
        /// </summary>
        /// <param name="imageFolder">图片所在文件夹路径</param>
        /// <param name="outputPath">输出文件路径（如 "merged.png"）</param>
        /// <param name="searchPattern">图片搜索模式，默认 "*.jpg|*.png|*.jpeg"</param>
        public static async Task MergeImagesVerticallyAsync(
    string imageFolder,
    string outputPath)
        {
            var imageFiles = Directory.GetFiles(imageFolder, "*.jpg")
                .Concat(Directory.GetFiles(imageFolder, "*.png"))
                .OrderBy(f => f)
                .ToList();

            if (imageFiles.Count == 0) return;

            // 1. 加载所有图片 - 这里不能使用 using
            var images = new List<Image<Rgba32>>();
            foreach (var file in imageFiles)
            {
                var img = await Image.LoadAsync<Rgba32>(file);
                images.Add(img);
            }

            // 2. 计算尺寸
            int totalHeight = images.Sum(img => img.Height);
            int maxWidth = images.Max(img => img.Width);

            // 3. 创建画布并拼接
            using var finalImage = new Image<Rgba32>(maxWidth, totalHeight);

            int currentY = 0;
            foreach (var img in images)
            {
                finalImage.Mutate(ctx => ctx.DrawImage(img, new Point(0, currentY), 1f));
                currentY += img.Height;
                img.Dispose();  // 每张图片用完后立即释放
            }
            string outputDir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            // 4. 保存结果
            await finalImage.SaveAsync(outputPath);

            Console.WriteLine($"拼接完成！共 {images.Count} 张图片");
        }
    }
}

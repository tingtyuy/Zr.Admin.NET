using System;
using System.Collections.Generic;
using System.Text;

namespace ZR.ConsoleApp
{
    public class MusicDemo
    {
        public async static Task Run()
        {

            string sourceDir = @"C:\Users\ms363\Downloads\lxmusic";
            string targetDir = @"C:\Users\ms363\Downloads\lxmusic_out";

            try
            {
                Console.WriteLine("开始处理歌词文件同步...");
                Console.WriteLine($"源目录: {sourceDir}");
                Console.WriteLine($"目标目录: {targetDir}");

                int processedCount = SyncLyricsFiles(sourceDir, targetDir);

                Console.WriteLine($"\n处理完成！共处理了 {processedCount} 个歌词文件。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理过程中出现错误: {ex.Message}");
            }

            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }

        static int SyncLyricsFiles(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(sourceDir))
            {
                throw new DirectoryNotFoundException($"源目录不存在: {sourceDir}");
            }

            if (!Directory.Exists(targetDir))
            {
                throw new DirectoryNotFoundException($"目标目录不存在: {targetDir}");
            }

            // 获取源目录中所有的歌词文件（.lrc格式）
            var lrcFiles = Directory.GetFiles(sourceDir, "*.lrc");
            Console.WriteLine($"在源目录中找到 {lrcFiles.Length} 个歌词文件");

            // 获取目标目录中所有的音乐文件
            var mp3Files = Directory.GetFiles(targetDir, "*(Instrumental).mp3");
            Console.WriteLine($"在目标目录中找到 {mp3Files.Length} 个音乐文件");

            int processedCount = 0;

            foreach (var lrcFile in lrcFiles)
            {
                try
                {
                    string lrcFileName = Path.GetFileName(lrcFile);
                    Console.WriteLine($"\n处理: {lrcFileName}");

                    // 从歌词文件名中提取歌曲名和歌手信息
                    // 格式: "歌曲名 - 歌手.lrc"
                    string songInfo = Path.GetFileNameWithoutExtension(lrcFile);

                    // 查找目标目录中匹配的音乐文件
                    var matchedFiles = FindMatchingMusicFiles(mp3Files, songInfo);

                    if (matchedFiles.Count == 0)
                    {
                        Console.WriteLine($"  警告: 未找到匹配的音乐文件");
                        continue;
                    }

                    // 为每个匹配的音乐文件创建对应的歌词文件
                    foreach (var mp3File in matchedFiles)
                    {
                        string targetLrcName = CreateTargetLrcName(mp3File);
                        string targetLrcPath = Path.Combine(targetDir, targetLrcName);

                        // 复制并重命名歌词文件
                        File.Copy(lrcFile, targetLrcPath, true);
                        Console.WriteLine($"  已创建: {targetLrcName}");
                        processedCount++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  错误处理文件 {Path.GetFileName(lrcFile)}: {ex.Message}");
                }
            }

            return processedCount;
        }

        static List<string> FindMatchingMusicFiles(string[] mp3Files, string songInfo)
        {
            var matchedFiles = new List<string>();

            // 分解歌曲信息：提取歌曲名和歌手（如果有"-"分隔）
            string songName = songInfo;
            string artist = string.Empty;

            if (songInfo.Contains(" - "))
            {
                var parts = songInfo.Split(new[] { " - " }, StringSplitOptions.None);
                if (parts.Length >= 2)
                {
                    songName = parts[0].Trim();
                    artist = parts[1].Trim();
                }
            }

            foreach (var mp3File in mp3Files)
            {
                string mp3FileName = Path.GetFileNameWithoutExtension(mp3File);

                // 检查是否包含歌曲名
                // 注意：目标文件名可能包含前缀编号和下划线
                if (mp3FileName.Contains(songName))
                {
                    // 如果有歌手信息，也检查是否匹配（不区分大小写）
                    if (!string.IsNullOrEmpty(artist) &&
                        mp3FileName.IndexOf(artist, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        matchedFiles.Add(mp3File);
                    }
                    else if (string.IsNullOrEmpty(artist))
                    {
                        // 如果没有明确的歌手信息，只匹配歌曲名
                        matchedFiles.Add(mp3File);
                    }
                }
            }

            return matchedFiles;
        }

        static string CreateTargetLrcName(string mp3FilePath)
        {
            string mp3FileName = Path.GetFileNameWithoutExtension(mp3FilePath);

            // 判断是Vocals版还是Instrumental版
            string suffix = mp3FileName.EndsWith("_(Vocals)") ? "_(Vocals)" : "_(Instrumental)";

            // 创建对应的歌词文件名
            return mp3FileName + ".lrc";
        }
    }
}
